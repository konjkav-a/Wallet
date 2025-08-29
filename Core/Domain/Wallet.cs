namespace Core.Domain
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public long Version { get; set; }
        public DateTime CreatedAt { get; set; }


        public Wallet(Guid id, string ownerName, string currency, decimal initialBalance = 0m)
        {
            Id = id;
            OwnerName = ownerName;
            Currency = currency;
            Balance = initialBalance;
            Version = 1;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
