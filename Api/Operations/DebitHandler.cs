using Api.Dtos;
using Api.Helper;
using Core.Services;

namespace Api.Operations
{
    public class DebitHandler : IOperationHandler
    {
        public string OperationType => "debit";

        public object Handle(OperationRequest req, WalletService service, long? version, HttpContext http)
        {
            if (req.WalletId == null || req.Amount == null)
                throw new ArgumentException("walletId and amount required for debit.");

            var agg = service.Debit(req.WalletId.Value, req.Amount.Value, req.Reference, version);

            http.Response.Headers.ETag = ETagHelper.ETagFromVersion(agg.Wallet.Version);

            return new
            {
                wallet = new { id = agg.Wallet.Id, balance = agg.Wallet.Balance, version = agg.Wallet.Version }
            };
        }
    }

}
