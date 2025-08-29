using Core.Domain;
using Core.Services;
using System.Collections.Concurrent;


namespace Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ConcurrentDictionary<Guid, WalletAggregate> _store = new();

        public bool TryAddWallet(WalletAggregate aggregate)
        {
            return _store.TryAdd(aggregate.Wallet.Id, aggregate);
        }

        public WalletAggregate? GetWallet(Guid id)
        {
            return _store.TryGetValue(id, out var agg) ? agg : null;
        }
    }
}
