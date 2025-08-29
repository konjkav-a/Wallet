using Api.Dtos;
using Api.Helper;
using Core.Services;

namespace Api.Operations
{
    public class TransferHandler : IOperationHandler
    {
        public string OperationType => "transfer";

        public object Handle(OperationRequest req, WalletService service, long? version, HttpContext http)
        {
            if (req.FromWalletId == null || req.ToWalletId == null || req.Amount == null)
                throw new ArgumentException("fromWalletId, toWalletId and amount required for transfer.");

            var (fromAgg, toAgg) = service.Transfer(req.FromWalletId.Value, req.ToWalletId.Value, req.Amount.Value, req.Reference, version, null);

            http.Response.Headers.ETag = ETagHelper.ETagFromVersion(fromAgg.Wallet.Version);

            return new
            {
                fromWallet = new { id = fromAgg.Wallet.Id, balance = fromAgg.Wallet.Balance, version = fromAgg.Wallet.Version },
                toWallet = new { id = toAgg.Wallet.Id, balance = toAgg.Wallet.Balance, version = toAgg.Wallet.Version }
            };
        }
    }

}
