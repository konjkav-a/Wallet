using Api.Dtos;

namespace Api.Operations
{
    public class OperationHandlerFactory
    {
        private readonly Dictionary<string, IOperationHandler> _handlers;

        public OperationHandlerFactory()
        {
            _handlers = new List<IOperationHandler>
        {
            new CreditHandler(),
            new DebitHandler(),
            new TransferHandler()
        }.ToDictionary(h => h.OperationType, StringComparer.OrdinalIgnoreCase);
        }

        //dict["credit"] => creditHandler

        public IOperationHandler GetHandler(string? type)
        {
            if (string.IsNullOrWhiteSpace(type) || !_handlers.ContainsKey(type))
                throw new ArgumentException("Invalid operation type.");

            return _handlers[type];
        }

    }

}
