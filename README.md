<p align="center">
<a href="https://scailo.com" target="_blank">
<img src="https://pub-fbb2435be97c492d8ece0578844483ea.r2.dev/scailo-logo.png" alt="Scailo Logo" width="200">
</a>
</p>

<h1 align="center">Scailo Official C#/.NET SDK</h1>

Welcome to the official C#/.NET SDK for the Scailo API. This library provides high-performance gRPC client bindings, enabling seamless integration between your .NET applications and the Scailo ERP ecosystem.

About Scailo

Scailo is a powerful, modern ERP solution designed to be the foundation for your business needs. It provides a wide range of customizable business applications that cover everything from e-commerce, accounting, and CRM to order management, manufacturing, and human resources. With Scailo, you can streamline operations and unify your business processes on a single, scalable platform.

To learn more about what Scailo can do for your business, visit [scailo.com](https://scailo.com).

Installation

You can install the SDK via the NuGet Package Manager or the .NET CLI:

Using .NET CLI:

```bash
dotnet add package Scailo.Sdk
```

Using Package Manager Console:

```powershell
Install-Package Scailo.Sdk
```

Getting Started & Usage

Interacting with the Scailo API is done through gRPC. The following examples demonstrate how to establish a connection, authenticate, and perform requests.

1. Authentication

First, you must authenticate to obtain an auth_token. This token must be included in the headers (metadata) of all subsequent service requests.

```csharp
using Grpc.Net.Client;
using Scailo.Sdk;

// 1. Setup the channel
using var channel = GrpcChannel.ForAddress("[https://your-scailo-instance.com:443](https://your-scailo-instance.com:443)");

// 2. Create the Login Client
var loginClient = new LoginService.LoginServiceClient(channel);

// 3. Authenticate
var loginResponse = await loginClient.LoginAsEmployeePrimaryAsync(new UserLoginRequest
{
    Username = "your-username",
    PlainTextPassword = "your-password"
});

Console.WriteLine($"Successfully logged in! Auth Token: {loginResponse.AuthToken}");
```


2. Making Authenticated Requests

Once you have the auth_token, you must pass it in the gRPC metadata using the key auth_token.

The following example demonstrates how to fetch the latest active purchase order using the authentication token retrieved in the previous step.

```csharp
using Grpc.Core;
using Grpc.Net.Client;
using Scailo.Sdk;

// ... assume channel and auth_token are obtained ...

var purchaseClient = new PurchasesOrdersService.PurchasesOrdersServiceClient(channel);

// Create the metadata with your auth token
var headers = new Metadata
{
    { "auth_token", loginResponse.AuthToken }
};

// Execute the request with headers
var filterRequest = new PurchasesOrdersServiceFilterReq
{
    IsActive = BoolFilter.True,
    Count = 1,
    SortOrder = SortOrder.Descending
};

var response = await purchaseClient.FilterAsync(filterRequest, headers);

foreach (var order in response.List)
{
    Console.WriteLine($"Order ID: {order.Id}");
}
```

Or, authenticated requests can be made using an Interceptor

```csharp
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Scailo.Sdk;


var loginResponse = await loginClient.LoginAsEmployeePrimaryAsync(new UserLoginRequest
{
    Username = "your-username",
    PlainTextPassword = "your-password"
});

// Create an authenticated channel
var interceptor = new AuthInterceptor(loginResponse.AuthToken);
var authenticatedChannel = channel.Intercept(interceptor);

var purchaseOrdersServiceClient = new PurchasesOrdersService.PurchasesOrdersServiceClient(authenticatedChannel);
var filterResp = await purchaseOrdersServiceClient.FilterAsync(new PurchasesOrdersServiceFilterReq { IsActive = BOOL_FILTER.AnyUnspecified, Count = 5 });

Console.WriteLine($"\n{filterResp.List} orders found.");

```

## Requirements

.NET 8.0 or higher (Optimized for .NET 10.0)

gRPC compatible environment

For more detailed information on what you can build, please see our API Documentation.


## API Use Cases

The Scailo API is extensive and allows you to build powerful integrations. Some common use cases include:

- E-commerce Integration: Sync orders, customer data, and inventory levels between Scailo and platforms like Shopify or WooCommerce.

- Automate Business Processes: Automatically transfer data from a CRM or Warehouse Management System (WMS) directly into the ERP.

- Financial Management: Connect Scailo with accounting systems to automate invoice generation and financial reporting.

- Custom Workflows: Build custom applications and workflows tailored to your specific business logic.

For more detailed information on what you can build, please see our [API](https://scailo.com/api) documentation.