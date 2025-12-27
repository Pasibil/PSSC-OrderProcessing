using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Api.Data;
using OrderProcessing.Api.Repositories;
using OrderProcessing.Domain.Repositories;
using OrderProcessing.Domain.Workflows;
using OrderProcessing.Events;
using OrderProcessing.Events.ServiceBus;

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

// Configure DbContext with SQL Server
builder.Services.AddDbContext<OrderProcessingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
// Use InMemory for Products (no database needed)
builder.Services.AddSingleton<IProductsRepository, InMemoryProductsRepository>();
// Use SQL for Orders (persistent storage)
builder.Services.AddScoped<IOrdersRepository, SqlOrdersRepository>();

// Register workflow
builder.Services.AddScoped<PlaceOrderWorkflow>();

// Configure Azure Service Bus
var serviceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBus:ConnectionString");
builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));
builder.Services.AddScoped<IEventSender, ServiceBusTopicEventSender>();

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
