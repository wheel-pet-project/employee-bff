using Grpc.Core;
using Proto.VehicleFleetV1;

namespace Clients.Clients.VehicleFleet;

public class VehicleFleetClientWrapper(Proto.VehicleFleetV1.VehicleFleet.VehicleFleetClient client)
{
    public async Task<AddVehicleRes> AddVehicle(AddVehicleReq request, CancellationToken ct = default)
    {
        return await client.AddVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetAllVehiclesRes> GetAllVehicles(GetAllVehiclesReq request, CancellationToken ct = default)
    {
        return await client.GetAllVehiclesAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetVehiclesInSquareRes> GetVehiclesInSquare(
        GetVehiclesInSquareReq request,
        CancellationToken ct = default)
    {
        return await client.GetVehiclesInSquareAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetVehicleByIdRes> GetVehicleById(GetVehicleByIdReq request, CancellationToken ct = default)
    {
        return await client.GetVehicleByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetAllModelsRes> GetAllModels(GetAllModelsReq request, CancellationToken ct = default)
    {
        return await client.GetAllModelsAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetModelByIdRes> GetModelById(GetModelByIdReq request, CancellationToken ct = default)
    {
        return await client.GetModelByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<AddModelRes> AddModel(AddModelReq request, CancellationToken ct = default)
    {
        return await client.AddModelAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<DeleteVehicleRes> DeleteVehicle(DeleteVehicleReq request, CancellationToken ct = default)
    {
        return await client.DeleteVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetVehicleDetailsByIdRes> GetVehicleDetailsById(
        GetVehicleDetailsByIdReq request,
        CancellationToken ct = default)
    {
        return await client.GetVehicleDetailsByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<UpdateModelCategoryRes> UpdateModelCategory(
        UpdateModelCategoryReq request,
        CancellationToken ct = default)
    {
        return await client.UpdateModelCategoryAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<UpdateModelTariffRes> UpdateModelTariff(
        UpdateModelTariffReq request,
        CancellationToken ct = default)
    {
        return await client.UpdateModelTariffAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<MarkAsReadiedForReleaseVehicleRes> MarkAsReadiedForRelease(
        MarkAsReadiedForReleaseVehicleReq request,
        CancellationToken ct = default)
    {
        return await client.MarkAsReadiedForReleaseVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<SendToServiceVehicleRes> SendToServiceVehicle(
        SendToServiceVehicleReq request,
        CancellationToken ct = default)
    {
        return await client.SendToServiceVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<ReleaseVehicleRes> ReleaseVehicle(ReleaseVehicleReq request, CancellationToken ct = default)
    {
        return await client.ReleaseVehicleAsync(request, new CallOptions([], cancellationToken: ct));
    }
}