using Grpc.Net.Client;
using Grpc.Core.Interceptors;

using Scailo.Sdk;

using DotNetEnv;

// This reads the .env file and pumps the values 
// into the process environment variables
Env.Load();

// 1. Initialize the connection
// For local testing, ensure your server is running or use a mock address
using var channel = GrpcChannel.ForAddress("http://localhost:21000");

// 2. Create the Service Client
// gRPC generates a 'Client' class for every 'service' defined
var loginServiceClient = new LoginService.LoginServiceClient(channel);

Console.WriteLine("=== Scailo C# SDK: Login/Logout Example ===");

try
{
    // 3. Prepare the Login Request
    // Note: proto 'plain_text_password' becomes 'PlainTextPassword' in C#
    var loginRequest = new UserLoginRequest
    {
        Username = Environment.GetEnvironmentVariable("SCAILO_USERNAME"),
        PlainTextPassword = Environment.GetEnvironmentVariable("SCAILO_PASSWORD"),
        Otp = "",
    };

    Console.WriteLine($"Logging in user: {loginRequest.Username}...");

    // 4. Perform Login
    // Always use the 'Async' version of methods in C#
    var loginResponse = await loginServiceClient.LoginAsEmployeePrimaryAsync(loginRequest);

    Console.WriteLine("Login Successful!");
    Console.WriteLine($"Auth Token: {loginResponse.AuthToken}");
    Console.WriteLine($"Token Expires At: {loginResponse.ExpiresAt}");


    // 5. Create an authenticated channel
    var interceptor = new AuthInterceptor(loginResponse.AuthToken);
    var authenticatedChannel = channel.Intercept(interceptor);

    var secureLoginServiceClient = new LoginService.LoginServiceClient(authenticatedChannel);

    var purchaseOrdersServiceClient = new PurchasesOrdersService.PurchasesOrdersServiceClient(authenticatedChannel);
    var filterResp = await purchaseOrdersServiceClient.FilterAsync(new PurchasesOrdersServiceFilterReq { IsActive = BOOL_FILTER.AnyUnspecified, Count = 5 });

    Console.WriteLine($"\n{filterResp.List} orders found.");


    // 5. Perform Logout
    Console.WriteLine("\nSession finished. Logging out...");
    await secureLoginServiceClient.LogoutAsync(new LogoutRequest());

    Console.WriteLine("Logout successful. Session cleared.");
}
catch (Grpc.Core.RpcException ex)
{
    // This will likely trigger if no server is listening on 50051
    Console.WriteLine($"\n[gRPC Error] Status: {ex.StatusCode}, Detail: {ex.Status.Detail}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n[System Error] {ex.Message}");
}