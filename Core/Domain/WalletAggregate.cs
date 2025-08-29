namespace Core.Domain
{
    public class WalletAggregate
    {
        public Wallet Wallet { get; }
        public List<Transaction> Transactions { get; } = new();
        public object SyncRoot { get; } = new object();
        public WalletAggregate(Wallet wallet)
        {
            Wallet = wallet;
        }
    }
}
