namespace Api.Dtos
{
    public class OperationRequest
    {
        public string Type { get; set; } = string.Empty;
        public Guid? WalletId { get; set; }
        public Guid? FromWalletId { get; set; }
        public Guid? ToWalletId { get; set; }
        public decimal? Amount { get; set; }
        public string? Reference { get; set; }
    }


}
