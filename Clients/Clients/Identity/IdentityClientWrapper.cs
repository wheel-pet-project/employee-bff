using Grpc.Core;
using Proto.IdentityV1;

namespace Clients.Clients.Identity;

public class IdentityClientWrapper(Proto.IdentityV1.Identity.IdentityClient client)
{
    public async Task<CreateResponse> CreateAccount(CreateRequest request, CancellationToken ct = default)
    {
        return await client.CreateAccountAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<ConfirmEmailResponse> ConfirmEmail(ConfirmEmailRequest request, CancellationToken ct = default)
    {
        return await client.ConfirmEmailAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, CancellationToken ct = default)
    {
        return await client.AuthenticateAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<AuthorizeResponse> Authorize(AuthorizeRequest request, CancellationToken ct = default)
    {
        return await client.AuthorizeAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<RefreshAccessTokenResponse> RefreshAccessToken(
        RefreshAccessTokenRequest request,
        CancellationToken ct = default)
    {
        return await client.RefreshAccessTokenAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<RecoverPasswordResponse> RecoverPassword(
        RecoverPasswordRequest request,
        CancellationToken ct = default)
    {
        return await client.RecoverPasswordAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<UpdatePasswordResponse> UpdatePassword(
        UpdatePasswordRequest request,
        CancellationToken ct = default)
    {
        return await client.UpdatePasswordAsync(request, new CallOptions([], cancellationToken: ct));
    }
}