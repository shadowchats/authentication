// Shadowchats — Copyright (C) 2025
// Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version. See the LICENSE file for details.
// For full copyright and authorship information, see the COPYRIGHT file.

using System.Diagnostics;
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
        catch
        {
            throw new RpcException(new Status(StatusCode.Internal, $"An internal server error occurred. Please provide the trace ID \"{Activity.Current?.TraceId.ToString()}\" to the support team."));
        }
    }
}