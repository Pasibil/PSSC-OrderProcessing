using OrderProcessing.Api.Repositories;
using OrderProcessing.Domain.Repositories;
using OrderProcessing.Domain.Workflows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with NSwag
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Order Processing API";
    config.Version = "v1";
    config.Description = "API pentru procesarea comenzilor - DDD Lab Project";
});

// Register repositories
builder.Services.AddScoped<IProductsRepository, InMemoryProductsRepository>();
builder.Services.AddScoped<IOrdersRepository, InMemoryOrdersRepository>();

// Register workflow
builder.Services.AddScoped<PlaceOrderWorkflow>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
