using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Scailo.Sdk;

public class AuthInterceptor : Interceptor
{
    private readonly string _token;

    public AuthInterceptor(string token)
    {
        _token = token;
    }

    // This method intercepts "Simple" (Unary) RPC calls
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var metadata = context.Options.Headers ?? new Metadata();

        // Inject the auth_token header
        metadata.Add("auth_token", _token);

        // Reconstruct the context with the new headers
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            context.Options.WithHeaders(metadata));

        return continuation(request, newContext);
    }
}