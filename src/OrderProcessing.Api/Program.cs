using OrderProcessing.Api.Repositories;
using OrderProcessing.Domain.Repositories;
using OrderProcessing.Domain.Workflows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<IProductsRepository, InMemoryProductsRepository>();
builder.Services.AddScoped<IOrdersRepository, InMemoryOrdersRepository>();

// Register workflow
builder.Services.AddScoped<PlaceOrderWorkflow>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
