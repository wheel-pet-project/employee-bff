using Api;
using Grpc.Core;

namespace Clients.Clients.VehicleDocuments;

public class VehicleDocumentsClientWrapper(Api.VehicleDocuments.VehicleDocumentsClient client)
{
    public async Task<AddOsagoResponse> AddOsago(AddOsagoRequest request, CancellationToken ct = default)
    {
        return await client.AddOsagoAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<AddPtsResponse> AddPts(AddPtsRequest request, CancellationToken ct = default)
    {
        return await client.AddPtsAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<AddStsResponse> AddSts(AddStsRequest request, CancellationToken ct = default)
    {
        return await client.AddStsAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetVehicleDocumentsByVehicleIdResponse> GetVehicleDocuments(
        GetVehicleDocumentsByVehicleIdRequest request,
        CancellationToken ct = default)
    {
        return await client.GetVehicleDocumentsByVehicleIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetOsagoByVehicleDocumentsIdResponse> GetOsagoByVehicleDocumentsId(
        GetOsagoByVehicleDocumentsIdRequest request,
        CancellationToken ct = default)
    {
        return await client.GetOsagoByVehicleDocumentsIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetPtsByVehicleDocumentsIdResponse> GetPtsByVehicleDocumentsId(
        GetPtsByVehicleDocumentsIdRequest request,
        CancellationToken ct = default)
    {
        return await client.GetPtsByVehicleDocumentsIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetStsByVehicleDocumentsIdResponse> GetStsByVehicleDocumentsId(
        GetStsByVehicleDocumentsIdRequest request,
        CancellationToken ct = default)
    {
        return await client.GetStsByVehicleDocumentsIdAsync(request, new CallOptions([], cancellationToken: ct));
    }
}