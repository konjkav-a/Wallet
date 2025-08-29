using Core.Domain;
using Core.Services;

namespace Core.Services
{

    public class WalletService
    {
        private readonly IWalletRepository _repository;
        private readonly IRiskEvaluator _riskEvaluator;

        public WalletService(IWalletRepository repository, IRiskEvaluator riskEvaluator)
        {
            _repository = repository;
            _riskEvaluator = riskEvaluator;
        }

        public WalletAggregate CreateWallet(string ownerName, string currency)
        {
            var wallet = new Wallet(Guid.NewGuid(), ownerName, currency);
            var aggregate = new WalletAggregate(wallet);

            var added = _repository.TryAddWallet(aggregate);
            if (!added)
                throw new InvalidOperationException("Failed to add wallet");

            return aggregate;
        }

        public WalletAggregate GetWallet(Guid id)
        {
            var agg = _repository.GetWallet(id);
            return agg ?? throw new NotFoundException($"Wallet {id} not found.");

        }


        public WalletAggregate Credit(Guid walletId, decimal amount, string? reference = null, long? expectedVersion = null)
        {
            if (amount <= 0) throw new ArgumentException("amount must be > 0");

            var agg = GetWallet(walletId);

            lock (agg.SyncRoot)
            {
                CheckVersion(agg, expectedVersion);

                if (!_riskEvaluator.IsAllowedForOperation(amount))
                    throw new RiskDeniedException("Operation denied by risk policy.");

                agg.Wallet.Balance += amount;
                agg.Wallet.Version++;
                agg.Transactions.Add(new Transaction(Guid.NewGuid(), walletId, "Credit", amount, reference));
            }

            return agg;
        }

        // Debit
        public WalletAggregate Debit(Guid walletId, decimal amount, string? reference = null, long? expectedVersion = null)
        {
            if (amount <= 0) throw new ArgumentException("amount must be > 0");

            var agg = GetWallet(walletId);

            lock (agg.SyncRoot)
            {
                CheckVersion(agg, expectedVersion);

                if (!_riskEvaluator.IsAllowedForOperation(amount))
                    throw new RiskDeniedException("Operation denied by risk policy.");

                if (agg.Wallet.Balance < amount)
                    throw new InsufficientFundsException("Insufficient funds.");

                agg.Wallet.Balance -= amount;
                agg.Wallet.Version++;
                agg.Transactions.Add(new Transaction(Guid.NewGuid(), walletId, "Debit", amount, reference));
            }

            return agg;
        }

        // Transfer
        public (WalletAggregate from, WalletAggregate to) Transfer(Guid fromId, Guid toId, decimal amount, string? reference = null, long? expectedFromVersion = null, long? expectedToVersion = null)
        {
            if (fromId == toId) throw new ArgumentException("from and to wallets must differ");
            if (amount <= 0) throw new ArgumentException("amount must be > 0");

            var fromAgg = GetWallet(fromId);
            var toAgg = GetWallet(toId);

            // prevent deadlocks
            var first = fromId.CompareTo(toId) < 0 ? fromAgg : toAgg;
            var second = ReferenceEquals(first, fromAgg) ? toAgg : fromAgg;

            lock (first.SyncRoot)
            {
                lock (second.SyncRoot)
                {
                    // if expected versions provided
                    if (expectedFromVersion.HasValue && expectedFromVersion.Value != fromAgg.Wallet.Version)
                        throw new ConcurrencyException($"Stale version for from wallet. Current={fromAgg.Wallet.Version} Expected={expectedFromVersion.Value}");

                    if (expectedToVersion.HasValue && expectedToVersion.Value != toAgg.Wallet.Version)
                        throw new ConcurrencyException($"Stale version for to wallet. Current={toAgg.Wallet.Version} Expected={expectedToVersion.Value}");

                    if (!_riskEvaluator.IsAllowedForOperation(amount))
                        throw new RiskDeniedException("Operation denied by risk policy.");

                    if (fromAgg.Wallet.Balance < amount)
                        throw new InsufficientFundsException("Insufficient funds for transfer.");

                    fromAgg.Wallet.Balance -= amount;
                    fromAgg.Wallet.Version++;
                    fromAgg.Transactions.Add(new Transaction(Guid.NewGuid(), fromId, "TransferOut", amount, reference));

                    toAgg.Wallet.Balance += amount;
                    toAgg.Wallet.Version++;
                    toAgg.Transactions.Add(new Transaction(Guid.NewGuid(), toId, "TransferIn", amount, reference));
                }
            }

            return (fromAgg, toAgg);
        }

        private void CheckVersion(WalletAggregate agg, long? expectedVersion)
        {
            if (expectedVersion.HasValue && agg.Wallet.Version != expectedVersion.Value)
                throw new ConcurrencyException($"Stale version. Current={agg.Wallet.Version} Expected={expectedVersion.Value}");
        }
    }
}




