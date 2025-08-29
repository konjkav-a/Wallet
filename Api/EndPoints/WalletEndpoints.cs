using Api.Dtos;
using Api.Helper;
using Api.Operations;
using Core.Services;
using Infrastructure.Idempotency;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Api.EndPoints
{
    public static class WalletEndpoints
    {
        public static void MapWalletEndpoints(this WebApplication app)
        {
            var wallets = app.MapGroup("/wallets");

            wallets.MapPost("/", static async (WalletService service, IdempotencyStore idemStore, WalletRequest req, HttpContext http) =>
            {
                string? idempotencyKey = http.Request.Headers["Idempotency-Key"];

                // بررسی idempotency
                if (!string.IsNullOrWhiteSpace(idempotencyKey) && idemStore.TryGet(idempotencyKey, out var existing))
                {
                    var cachedBody = System.Text.Json.JsonSerializer.Deserialize<object>(existing.ResponseJson);
                    return Results.Json(cachedBody, statusCode: existing.StatusCode);
                }

                try
                {
                    var agg = service.CreateWallet(req.OwnerName, req.Currency);

                    var body = new
                    {
                        id = agg.Wallet.Id,
                        ownerName = agg.Wallet.OwnerName,
                        currency = agg.Wallet.Currency,
                        balance = agg.Wallet.Balance,
                        version = agg.Wallet.Version,
                        createdAt = agg.Wallet.CreatedAt
                    };

                    // ذخیره در idempotency store
                    if (!string.IsNullOrWhiteSpace(idempotencyKey))
                        idemStore.Save(idempotencyKey, StatusCodes.Status201Created, body);

                    return Results.Created($"/wallets/{agg.Wallet.Id}", body);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new ProblemDetails { Title = "Conflict", Detail = ex.Message });
                }
            });
        

        // GET /wallets/{id}
        wallets.MapGet("/{id:guid}", (WalletService service, Guid id, HttpContext http) =>
            {
                try
                {
                    var agg = service.GetWallet(id);
                    var body = new
                    {
                        id = agg.Wallet.Id,
                        ownerName = agg.Wallet.OwnerName,
                        currency = agg.Wallet.Currency,
                        balance = agg.Wallet.Balance,
                        version = agg.Wallet.Version,
                        createdAt = agg.Wallet.CreatedAt
                    };
                    http.Response.Headers.ETag = ETagHelper.ETagFromVersion(agg.Wallet.Version);
                    return Results.Ok(body);
                }
                catch (Core.Domain.NotFoundException)
                {
                    return Results.NotFound(new ProblemDetails { Title = "Not Found", Detail = "Wallet not found." });
                }
            });
        }

        public static void MapOperationEndpoints(this WebApplication app)
        {
            var ops = app.MapGroup("/operations");

            ops.MapPost("/", (WalletService service, IdempotencyStore idemStore, [FromBody] OperationRequest req, HttpContext http) =>
            {
                string? idempotencyKey = http.Request.Headers["Idempotency-Key"];
                string? ifMatch = http.Request.Headers["If-Match"];
                long? version = ETagHelper.VersionFromIfMatch(ifMatch);

                // check idempotency first
                if (!string.IsNullOrWhiteSpace(idempotencyKey) && idemStore.TryGet(idempotencyKey, out var existing))
                {
                    http.Response.Headers.ETag = $"W/\"{GetVersionFromJson(existing.ResponseJson)}\"";
                    var body = System.Text.Json.JsonSerializer.Deserialize<object>(existing.ResponseJson);
                    return Results.Json(body, statusCode: existing.StatusCode);
                }

                try
                {
                    var handler = new OperationHandlerFactory().GetHandler(req.Type);
              
                    var responseBody = handler.Handle(req, service, version, http);

                    if (!string.IsNullOrWhiteSpace(idempotencyKey))
                        idemStore.Save(idempotencyKey, 200, responseBody);

                    return Results.Json(responseBody, statusCode: 200);
                }
                catch (Exception ex)
                {
                    return ExceptionMapper.Map(ex);
                }
            });
        }


        private static long GetVersionFromJson(string json)
        {
            try
            {
                var root = JsonNode.Parse(json);
                return root?["version"]?.GetValue<long>() ?? root?["wallet"]?["version"]?.GetValue<long>() ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}