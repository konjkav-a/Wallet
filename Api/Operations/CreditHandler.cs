using Api.Dtos;
using Api.Helper;
using Core.Services;

namespace Api.Operations
{
    public class CreditHandler : IOperationHandler
    {
        public string OperationType => "credit"; //return credit

        public object Handle(OperationRequest req, WalletService service, long? version, HttpContext http)
        {
            if (req.WalletId == null || req.Amount == null)
                throw new ArgumentException("walletId and amount required for credit.");

            var agg = service.Credit(req.WalletId.Value, req.Amount.Value, req.Reference, version);

            http.Response.Headers.ETag = ETagHelper.ETagFromVersion(agg.Wallet.Version);

            return new
            {
                wallet = new { id = agg.Wallet.Id, balance = agg.Wallet.Balance, version = agg.Wallet.Version }
            };
        }
    }

}
