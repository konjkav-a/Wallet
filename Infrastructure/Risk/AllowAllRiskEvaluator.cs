using Core.Services;

namespace Infrastructure.Risk
{
    public class AllowAllRiskEvaluator : IRiskEvaluator
    {

        public bool IsAllowedForOperation(decimal amount)
        {
            if (amount > 5000) return false; // سقف تراکنش
            return true;
        }
    }
}
