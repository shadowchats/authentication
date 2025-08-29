using Grpc.Core;
using Grpc.Core.Interceptors;
using Shadowchats.Authentication.Core.Domain.Exceptions;

namespace Shadowchats.Authentication.Presentation.GrpcInterceptors;

public class ExceptionHandlingGrpcInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (InvariantViolationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (AuthenticationFailedException ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Произошла внутренняя ошибка сервера"));
        }
    }

    private static string ExpectedError(string message) => $"{{\"message\":\"{message}\"}}";
    
    private static string UnexpectedError() => $"{{\"message\":\"An internal server error has occurred. Please contact technical support.\",\"trace_id\":}}";
}