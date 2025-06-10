using Grpc.Core;

namespace Gateway.Middlewares;

public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.Unavailable)
        {
            logger.LogWarning("Service unavailable: {detail}", e.Status.Detail);

            await ChangeResponse(context, StatusCodes.Status502BadGateway, "Service unavailable");
        }
        catch (RpcException e)
        {
            logger.LogWarning("RpcException: {e}", e);

            var codeTranslator = new RpcCodesToHttpTranslator();
            var statusCode = codeTranslator.Translate(e.StatusCode);

            await ChangeResponse(context, statusCode, e.Status.Detail);
        }
        catch (Exception e)
        {
            logger.LogCritical("Exception: {e}", e);

            await ChangeResponse(context, StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    private async Task ChangeResponse(HttpContext context, int statusCode, string description)
    {
        // todo: add implementation after generation models from openapi 

        // var error = new Error
        // { 
        //     Description = description,
        //     StatusCode = statusCode,
        // };
        //
        // context.Response.Clear();
        // context.Response.ContentType = "application/json";
        // context.Response.StatusCode = statusCode;
        // await context.Response.WriteAsJsonAsync(error);
    }
}

class RpcCodesToHttpTranslator
{
    public int Translate(StatusCode statusCode)
    {
        return statusCode switch
        {
            StatusCode.OK => StatusCodes.Status200OK,
            StatusCode.Cancelled => StatusCodes.Status500InternalServerError,
            StatusCode.Unknown => StatusCodes.Status500InternalServerError,
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
            StatusCode.ResourceExhausted => StatusCodes.Status429TooManyRequests,
            StatusCode.FailedPrecondition => StatusCodes.Status412PreconditionFailed,
            StatusCode.Aborted => StatusCodes.Status500InternalServerError,
            StatusCode.OutOfRange => StatusCodes.Status400BadRequest,
            StatusCode.Unimplemented => StatusCodes.Status501NotImplemented,
            StatusCode.Internal => StatusCodes.Status500InternalServerError,
            StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
            StatusCode.DataLoss => StatusCodes.Status500InternalServerError,
            StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}