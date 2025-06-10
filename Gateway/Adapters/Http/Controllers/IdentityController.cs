using Clients.Clients.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using ConfirmEmailRequest = Proto.IdentityV1.ConfirmEmailRequest;

namespace Gateway.Adapters.Http.Controllers;

public class IdentityController(IdentityClientWrapper clientWrapper) : IdentityApiController
{
    public override async Task<IActionResult> Authenticate(AuthenticateRequest request)
    {
        var serviceResponse = await clientWrapper.Authenticate(new Proto.IdentityV1.AuthenticateRequest
            { Email = request.Email, Pass = request.Password });

        return Ok(MapToResponse(serviceResponse));

        AuthenticateResponse MapToResponse(Proto.IdentityV1.AuthenticateResponse grpcResponse)
        {
            return new AuthenticateResponse { AccessToken = grpcResponse.Tkn, RefreshToken = grpcResponse.RefreshTkn };
        }
    }

    public override async Task<IActionResult> ConfirmAccountEmail(Guid accountId, string confirmationToken)
    {
        await clientWrapper.ConfirmEmail(new ConfirmEmailRequest
            { AccId = accountId.ToString(), ConfirmationTkn = confirmationToken });

        return Ok();
    }

    public override async Task<IActionResult> CreateAccount(CreateRequest request)
    {
        var roleMapper = new RoleEnumMapper();

        var response = await clientWrapper.CreateAccount(new Proto.IdentityV1.CreateRequest
        {
            Email = request.Email,
            Pass = request.Password,
            Phone = request.Phone,
            Role = roleMapper.ToProto(request.Role)
        });

        return Ok(MapToResponse(response));

        CreateResponse MapToResponse(Proto.IdentityV1.CreateResponse grpcResponse)
        {
            return new CreateResponse { AccountId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.AccId) };
        }
    }

    public override async Task<IActionResult> RecoverPassword(Guid accountId, RecoverPasswordRequest request)
    {
        // todo: убрать из openapi accountID
        await clientWrapper.RecoverPassword(new Proto.IdentityV1.RecoverPasswordRequest { Email = request.Email });

        return Ok();
    }


    public override async Task<IActionResult> RefreshAccessToken(RefreshAccessTokenRequest request)
    {
        var response = await clientWrapper.RefreshAccessToken(new Proto.IdentityV1.RefreshAccessTokenRequest
            { RefreshTkn = request.RefreshToken });

        return Ok(MapToResponse(response));

        RefreshAccessTokenResponse MapToResponse(Proto.IdentityV1.RefreshAccessTokenResponse grpcResponse)
        {
            return new RefreshAccessTokenResponse
                { AccessToken = grpcResponse.Tkn, RefreshToken = grpcResponse.RefreshTkn };
        }
    }

    public override async Task<IActionResult> UpdatePassword(Guid accountId, UpdatePasswordRequest request)
    {
        // todo: убрать из openapi accountID
        await clientWrapper.UpdatePassword(new Proto.IdentityV1.UpdatePasswordRequest
            { Email = request.Email, ResetTkn = request.ResetToken.ToString(), NewPass = request.NewPassword });

        return Ok();
    }
}

class RoleEnumMapper
{
    public Role ToContract(Proto.IdentityV1.Role r)
    {
        return r switch
        {
            Proto.IdentityV1.Role.CustomerUnspecified => Role.CustomerEnum,
            Proto.IdentityV1.Role.Admin => Role.AdminEnum,
            Proto.IdentityV1.Role.Support => Role.SupportEnum,
            Proto.IdentityV1.Role.Maintenance => Role.MaintenanceEnum,
            Proto.IdentityV1.Role.Hr => Role.HrEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
        };
    }

    public Proto.IdentityV1.Role ToProto(Role r)
    {
        return r switch
        {
            Role.CustomerEnum => Proto.IdentityV1.Role.CustomerUnspecified,
            Role.AdminEnum => Proto.IdentityV1.Role.Admin,
            Role.SupportEnum => Proto.IdentityV1.Role.Support,
            Role.MaintenanceEnum => Proto.IdentityV1.Role.Maintenance,
            Role.HrEnum => Proto.IdentityV1.Role.Hr,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
        };
    }
}