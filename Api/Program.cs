using Api.EndPoints;
using Api.Helper;
using Core.Services;
using Infrastructure.Idempotency;
using Infrastructure.Repositories;
using Infrastructure.Risk;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<IWalletRepository, WalletRepository>();
builder.Services.AddSingleton<IRiskEvaluator, AllowAllRiskEvaluator>();
builder.Services.AddSingleton<WalletService>();
builder.Services.AddSingleton<IdempotencyStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapWalletEndpoints();
app.MapOperationEndpoints();

app.Run();
