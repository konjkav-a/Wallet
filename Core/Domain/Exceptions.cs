namespace Core.Domain
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message) : base(message) { }
    }

    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message) { }
    }

    public class RiskDeniedException : Exception
    {
        public RiskDeniedException(string message) : base(message) { }
    }
}
