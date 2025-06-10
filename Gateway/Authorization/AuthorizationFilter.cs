using System.Security.Claims;
using Clients.Clients.Identity;
using FluentResults;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Proto.IdentityV1;
using Error = OpenApiContractV1.Models.Error;
using Status = Proto.IdentityV1.Status;

namespace Gateway.Authorization;

public class AuthorizationFilter(
    IdentityClientWrapper identityApi,
    params Role[] roles) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var token = GetTokenFromHeader(context);

        if (token is null or { Length: 0 })
        {
            CreateResponseIfTokenNotExist(context);
            return;
        }

        var authorizationResult = await AuthorizeTokenInIdentity(token);
        if (authorizationResult.IsFailed)
        {
            CreateResponseForFailedAuthorizationInIdentity(context,
                authorizationResult.Errors.FirstOrDefault(x => x is IdentityAuthorizationError) as
                    IdentityAuthorizationError);

            return;
        }

        ValidateStatusAndRole(context, authorizationResult.Value);

        AddClaimsToHttpContext(context, authorizationResult.Value);
    }

    private string? GetTokenFromHeader(AuthorizationFilterContext context)
    {
        var headerValue = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();

        var token = headerValue?.Trim().Split()[1]; // removing 'Bearer ' prefix

        return token;
    }

    private async Task<Result<AuthorizeResponse>> AuthorizeTokenInIdentity(string token)
    {
        AuthorizeResponse response;

        try
        {
            response = await identityApi.Authorize(new AuthorizeRequest { Tkn = token });
        }
        catch (RpcException e)
        {
            return Result.Fail(new IdentityAuthorizationError(e.Status.Detail));
        }

        return Result.Ok(response);
    }

    private void ValidateStatusAndRole(AuthorizationFilterContext context, AuthorizeResponse authorizationResponse)
    {
        if (authorizationResponse.Status == Status.Deactivated)
            CreateResponseIfStatusDeactivated(context);

        if (roles.All(x => x != authorizationResponse.Role))
            CreateResponseIfRoleNotAcceptedForThisEndpoint(context);
    }

    private void AddClaimsToHttpContext(AuthorizationFilterContext context, AuthorizeResponse authorizationResult)
    {
        var httpContext = context.HttpContext;

        var claims = new List<Claim>
        {
            new("customer_id", authorizationResult.AccId),
            new("status", authorizationResult.Status.ToString())
        };

        httpContext.User.AddIdentity(new ClaimsIdentity(claims));
    }

    private void CreateResponseIfTokenNotExist(AuthorizationFilterContext context)
    {
        ChangeResponse(context,
            "Access token is required to access this endpoint",
            StatusCodes.Status401Unauthorized);
    }

    private void CreateResponseForFailedAuthorizationInIdentity(
        AuthorizationFilterContext context,
        IdentityAuthorizationError? error)
    {
        var description = "Authorization failed";

        if (error != null) description = error.Message;

        ChangeResponse(
            context,
            description,
            StatusCodes.Status401Unauthorized);
    }

    private void CreateResponseIfStatusDeactivated(AuthorizationFilterContext context)
    {
        ChangeResponse(
            context,
            "Your account has been deactivated",
            StatusCodes.Status401Unauthorized);
    }

    private void CreateResponseIfRoleNotAcceptedForThisEndpoint(AuthorizationFilterContext context)
    {
        ChangeResponse(
            context,
            "You don't have access",
            StatusCodes.Status403Forbidden);
    }

    private void ChangeResponse(
        AuthorizationFilterContext context,
        string description,
        int statusCode)
    {
        var error = new Error
        {
            Description = description,
            StatusCode = statusCode,
        };

        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = statusCode;
        context.Result = new JsonResult(error);
    }

    private class IdentityAuthorizationError(string description)
        : FluentResults.Error(description);
}