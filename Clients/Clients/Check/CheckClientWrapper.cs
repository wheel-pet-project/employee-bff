using Api;
using Grpc.Core;

namespace Clients.Clients.Check;

public class CheckClientWrapper(VehicleCheck.VehicleCheckClient client)
{
    public async Task<GetAllChecksForConsiderateReviewResponse> AddDamageFixations(
        GetAllChecksForConsiderateReviewRequest request,
        CancellationToken ct = default)
    {
        return await client.GetAllChecksForConsiderateReviewAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetCheckByIdResponse> GetCheckById(GetCheckByIdRequest request, CancellationToken ct = default)
    {
        return await client.GetCheckByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }
}