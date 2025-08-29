using Api.Dtos;
using Core.Services;

namespace Api.Operations
{
    public interface IOperationHandler
    {
        string OperationType { get; }
        object Handle(OperationRequest req, WalletService service, long? version, HttpContext http);
    }

}
