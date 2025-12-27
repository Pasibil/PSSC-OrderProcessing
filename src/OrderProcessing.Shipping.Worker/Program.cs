using Azure.Messaging.ServiceBus;
using OrderProcessing.Events;
using OrderProcessing.Events.ServiceBus;
using OrderProcessing.Shipping.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Configure Azure Service Bus
var serviceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBus:ConnectionString");
builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));

// Register event infrastructure
builder.Services.AddSingleton<IEventListener, ServiceBusTopicEventListener>();
builder.Services.AddSingleton<IEventHandler, OrderPlacedEventHandler>();

// Register HTTP client for calling Shipping API
builder.Services.AddHttpClient();

// Register hosted service
builder.Services.AddHostedService<EventProcessorWorker>();

var host = builder.Build();
host.Run();
