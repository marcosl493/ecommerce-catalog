using Infrastructure;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder
    .Services
    .AddOpenApi()
    .AddSerilog()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();


