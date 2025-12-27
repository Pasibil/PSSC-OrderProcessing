# Azure Service Bus Configuration Guide

## Overview
This project uses Azure Service Bus for asynchronous communication between microservices. The OrderProcessing API publishes `OrderPlacedEvent` events to a Service Bus topic, which are then consumed by Invoicing and Shipping Worker Services.

## Architecture

```
┌─────────────────┐
│ OrderProcessing │
│      API        │
│   (Port 5259)   │
└────────┬────────┘
         │ Publishes OrderPlacedEvent
         ▼
   ┌─────────────┐
   │ Azure       │
   │ Service Bus │
   │ Topic:      │
   │ "orders"    │
   └──┬───────┬──┘
      │       │
      │       └──────────────┐
      ▼                      ▼
┌──────────────┐    ┌─────────────────┐
│  Invoicing   │    │    Shipping     │
│    Worker    │    │     Worker      │
│ Subscription │    │  Subscription   │
│  "invoicing  │    │   "shipping     │
│ -subscription│    │  -subscription" │
└──────┬───────┘    └────────┬────────┘
       │                     │
       ▼                     ▼
┌──────────────┐    ┌─────────────────┐
│  Invoicing   │    │    Shipping     │
│     API      │    │      API        │
│ (Port 5260)  │    │  (Port 5261)    │
└──────────────┘    └─────────────────┘
```

## Setup Options

### Option 1: Azure Service Bus (Production - Recommended)

1. **Create Azure Service Bus namespace:**
   ```bash
   az servicebus namespace create --resource-group <your-rg> --name <namespace-name> --location westeurope
   ```

2. **Create the "orders" topic:**
   ```bash
   az servicebus topic create --resource-group <your-rg> --namespace-name <namespace-name> --name orders
   ```

3. **Create subscriptions:**
   ```bash
   # Invoicing subscription
   az servicebus topic subscription create --resource-group <your-rg> --namespace-name <namespace-name> --topic-name orders --name invoicing-subscription

   # Shipping subscription
   az servicebus topic subscription create --resource-group <your-rg> --namespace-name <namespace-name> --topic-name orders --name shipping-subscription
   ```

4. **Get connection string:**
   ```bash
   az servicebus namespace authorization-rule keys list --resource-group <your-rg> --namespace-name <namespace-name> --name RootManageSharedAccessKey --query primaryConnectionString --output tsv
   ```

5. **Update appsettings.json in all three projects:**
   - `src/OrderProcessing.Api/appsettings.json`
   - `src/OrderProcessing.Invoicing.Worker/appsettings.json`
   - `src/OrderProcessing.Shipping.Worker/appsettings.json`

   Replace the connection string:
   ```json
   {
     "ServiceBus": {
       "ConnectionString": "Endpoint=sb://<namespace-name>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<your-key>"
     }
   }
   ```

### Option 2: Azure Portal (Manual Setup)

1. Go to [Azure Portal](https://portal.azure.com)
2. Create a **Service Bus Namespace** (Free or Basic tier is sufficient for development)
3. In the namespace, create a **Topic** named `orders`
4. In the topic, create two **Subscriptions**:
   - `invoicing-subscription`
   - `shipping-subscription`
5. Copy the connection string from **Shared access policies → RootManageSharedAccessKey**
6. Update `appsettings.json` files as shown above

### Option 3: Local Development (Emulator)

Azure Service Bus Emulator is available but requires Docker:

```bash
docker run -d -p 5672:5672 mcr.microsoft.com/azure-messaging/servicebus-emulator:latest
```

Connection string for emulator:
```
Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true
```

## CloudEvents Format

Events are sent using the [CloudEvents](https://cloudevents.io/) specification:

```json
{
  "id": "unique-guid",
  "type": "OrderPlacedEvent",
  "source": "https://www.upt.ro/OrderProcessing",
  "datacontenttype": "application/json",
  "time": "2025-01-XX...",
  "data": {
    "orderId": "guid",
    "customerName": "...",
    "customerEmail": "...",
    "orderLines": [...],
    "totalAmount": 150.00
  }
}
```

## Running the System

1. **Start all services:**
   ```bash
   # Terminal 1 - API
   cd src/OrderProcessing.Api
   dotnet run

   # Terminal 2 - Invoicing Worker
   cd src/OrderProcessing.Invoicing.Worker
   dotnet run

   # Terminal 3 - Shipping Worker
   cd src/OrderProcessing.Shipping.Worker
   dotnet run

   # Terminal 4 - Invoicing API
   cd src/OrderProcessing.Invoicing
   dotnet run

   # Terminal 5 - Shipping API
   cd src/OrderProcessing.Shipping
   dotnet run
   ```

2. **Test the flow:**
   ```bash
   curl -X POST http://localhost:5259/api/orders \
     -H "Content-Type: application/json" \
     -d '{
       "customerName": "John Doe",
       "customerEmail": "john@example.com",
       "orderLines": [
         {"productCode": "WIDGET", "quantity": 2}
       ]
     }'
   ```

3. **Check logs:**
   - API logs: Event published
   - Invoicing Worker logs: Event received and processed
   - Shipping Worker logs: Event received and processed

## Troubleshooting

### Connection Issues
- Verify connection string is correct
- Check Azure Service Bus namespace is active
- Ensure topic and subscriptions exist
- Verify firewall rules allow access

### Events Not Received
- Check worker services are running
- Verify subscription names match exactly
- Check worker logs for errors
- Verify message isn't dead-lettered (check dead-letter queue)

### Build Errors
- Ensure all NuGet packages are restored: `dotnet restore`
- Check project references are correct
- Verify .NET 9.0 SDK is installed

## Cost Considerations

- **Free Tier**: 1 GB storage, limited operations (good for development)
- **Basic Tier**: ~€0.05/million operations (sufficient for course project)
- **Standard Tier**: Includes topics (required for this project)

For course purposes, Basic or Standard tier with minimal usage should cost less than €1/month.

## Further Reading

- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
- [CloudEvents Specification](https://cloudevents.io/)
- [Topic and Subscription Patterns](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions)
