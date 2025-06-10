using Grpc.Core;
using Proto.DrivingLicenseV1;

namespace Clients.Clients.DrivingLicense;

public class DrivingLicenseClientWrapper(Proto.DrivingLicenseV1.DrivingLicense.DrivingLicenseClient client)
{
    public async Task<GetAllLicensesResponse> GetAll(GetAllLicensesRequest request, CancellationToken ct = default)
    {
        return await client.GetAllLicensesByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetLicenseByIdResponse> GetLicenseById(
        GetLicenseByIdRequest request,
        CancellationToken ct = default)
    {
        return await client.GetLicenseByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<ApproveLicenseResponse> Approve(ApproveLicenseRequest request, CancellationToken ct = default)
    {
        return await client.ApproveLicenseAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<RejectLicenseResponse> Reject(RejectLicenseRequest request, CancellationToken ct = default)
    {
        return await client.RejectLicenseAsync(request, new CallOptions([], cancellationToken: ct));
    }
}