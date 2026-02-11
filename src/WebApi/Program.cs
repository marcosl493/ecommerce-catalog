using Application;
using Infrastructure;
using Scalar.AspNetCore;
using WebApi;
using WebApi.Endpoints;
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder
    .Services
    .AddOpenApi()
    .AddSerilog()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapProductEndpoints();

await app.RunAsync();