using Clients.Clients.VehicleFleet;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using Proto.VehicleFleetV1;
using AddModelReq = OpenApiContractV1.Models.AddModelReq;
using AddModelRes = OpenApiContractV1.Models.AddModelRes;
using GetAllModelsRes = Proto.VehicleFleetV1.GetAllModelsRes;
using GetModelByIdRes = OpenApiContractV1.Models.GetModelByIdRes;
using UpdateModelCategoryReq = OpenApiContractV1.Models.UpdateModelCategoryReq;
using UpdateModelTariffReq = OpenApiContractV1.Models.UpdateModelTariffReq;

namespace Gateway.Adapters.Http.Controllers;

public class ModelController(VehicleFleetClientWrapper clientWrapper) : ModelApiController
{
    public override async Task<IActionResult> AddModel(AddModelReq request)
    {
        var serviceResponse = await clientWrapper.AddModel(new Proto.VehicleFleetV1.AddModelReq
        {
            Brand = request.Brand,
            CarModel = request.CarModel,
            Category = request.Category,
            PricePerMin = request.PricePerMin,
            PricePerHour = request.PricePerHour,
            PricePerDay = request.PricePerDay
        });

        return Ok(MapToResponse(serviceResponse));

        AddModelRes MapToResponse(Proto.VehicleFleetV1.AddModelRes grpcResponse)
        {
            return new AddModelRes
            {
                ModelId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.ModelId)
            };
        }
    }

    public override async Task<IActionResult> GetAllModels(int? page, int? pageSize)
    {
        var serviceResponse = await clientWrapper.GetAllModels(new GetAllModelsReq
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 10
        });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.GetAllModelsRes MapToResponse(GetAllModelsRes grpcResponse)
        {
            var response = new OpenApiContractV1.Models.GetAllModelsRes();

            response.Models.AddRange(grpcResponse.Models.Select(x => new ModelShortView
                    { ModelId = Guid.Parse((ReadOnlySpan<char>)x.ModelId), Brand = x.Brand, CarModel = x.CarModel })
                .ToList());

            return response;
        }
    }

    public override async Task<IActionResult> GetModelById(Guid modelId)
    {
        var serviceResponse = await clientWrapper.GetModelById(new GetModelByIdReq { ModelId = modelId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetModelByIdRes MapToResponse(Proto.VehicleFleetV1.GetModelByIdRes grpcResponse)
        {
            return new GetModelByIdRes
            {
                ModelId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.ModelId),
                Brand = grpcResponse.Brand,
                CarModel = grpcResponse.CarModel,
                Category = grpcResponse.Category,
                PricePerMin = grpcResponse.PricePerMin,
                PricePerHour = grpcResponse.PricePerHour,
                PricePerDay = grpcResponse.PricePerDay
            };
        }
    }

    public override async Task<IActionResult> UpdateModelCategory(Guid modelId, UpdateModelCategoryReq request)
    {
        await clientWrapper.UpdateModelCategory(new Proto.VehicleFleetV1.UpdateModelCategoryReq
        {
            ModelId = modelId.ToString(),
            Category = request.Category
        });

        return Ok();
    }

    public override async Task<IActionResult> UpdateModelTariff(Guid modelId, UpdateModelTariffReq request)
    {
        await clientWrapper.UpdateModelTariff(CreateGrpcRequest());

        return Ok();

        Proto.VehicleFleetV1.UpdateModelTariffReq CreateGrpcRequest()
        {
            var r = new Proto.VehicleFleetV1.UpdateModelTariffReq
            {
                ModelId = modelId.ToString()
            };

            if (request.PricePerMin != null) r.PricePerMin = request.PricePerMin.Value;
            if (request.PricePerHour != null) r.PricePerHour = request.PricePerHour.Value;
            if (request.PricePerDay != null) r.PricePerDay = request.PricePerDay.Value;

            return r;
        }
    }
}