using Microsoft.AspNetCore.Mvc;

namespace Api.Helper
{
    public static class ExceptionMapper
    {
        public static IResult Map(Exception ex)
        {
            var pd = ex switch
            {
                Core.Domain.ConcurrencyException cex => new ProblemDetails { Title = "Conflict", Detail = cex.Message, Status = 409 },
                Core.Domain.InsufficientFundsException ifx => new ProblemDetails { Title = "Unprocessable Entity", Detail = ifx.Message, Status = 422 },
                Core.Domain.RiskDeniedException rdx => new ProblemDetails { Title = "Forbidden", Detail = rdx.Message, Status = 403 },
                Core.Domain.NotFoundException nfx => new ProblemDetails { Title = "Not Found", Detail = nfx.Message, Status = 404 },
                ArgumentException aex => new ProblemDetails { Title = "Bad Request", Detail = aex.Message, Status = 400 },
                _ => new ProblemDetails { Title = "Error", Detail = ex.Message, Status = 400 }
            };

            return Results.Json(pd, statusCode: pd.Status ?? 400);
        }
    }

}
