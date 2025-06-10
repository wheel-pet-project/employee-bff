using Clients.Clients.VehicleFleet;
using Gateway.Adapters.Http.Contract.CommonEnumMappers;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using Proto.VehicleFleetV1;
using AddVehicleReq = OpenApiContractV1.Models.AddVehicleReq;
using AddVehicleRes = OpenApiContractV1.Models.AddVehicleRes;
using Color = Proto.VehicleFleetV1.Color;
using GetAllVehiclesRes = OpenApiContractV1.Models.GetAllVehiclesRes;
using GetVehicleByIdRes = OpenApiContractV1.Models.GetVehicleByIdRes;
using GetVehicleDetailsByIdRes = OpenApiContractV1.Models.GetVehicleDetailsByIdRes;
using GetVehiclesInSquareReq = OpenApiContractV1.Models.GetVehiclesInSquareReq;
using GetVehiclesInSquareRes = OpenApiContractV1.Models.GetVehiclesInSquareRes;
using Location = Proto.VehicleFleetV1.Location;
using Status = OpenApiContractV1.Models.Status;

namespace Gateway.Adapters.Http.Controllers;

public class VehicleController(VehicleFleetClientWrapper clientWrapper) : VehicleApiController
{
    public override async Task<IActionResult> AddVehicle(AddVehicleReq request)
    {
        var serviceResponse = await clientWrapper.AddVehicle(CreateGrpcRequest());

        return Ok(MapToResponse(serviceResponse));

        Proto.VehicleFleetV1.AddVehicleReq CreateGrpcRequest()
        {
            var r = new Proto.VehicleFleetV1.AddVehicleReq
            {
                ModelId = request.ModelId.ToString(),
                Color = ToProto(request.Color),
                PlateNumber = request.PlateNumber,
                Vin = request.Vin,
            };

            if (request.Location?.Latitude != null)
                r.Location = new Location
                    { Latitude = request.Location.Latitude, Longitude = request.Location.Longitude };

            return r;
        }

        AddVehicleRes MapToResponse(Proto.VehicleFleetV1.AddVehicleRes grpcResponse)
        {
            return new AddVehicleRes
            {
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId)
            };
        }
    }

    public override async Task<IActionResult> DeleteVehicle(Guid vehicleId)
    {
        await clientWrapper.DeleteVehicle(new DeleteVehicleReq { VehicleId = vehicleId.ToString() });

        return Ok();
    }

    public override async Task<IActionResult> GetAllVehicles(Status filteringStatus, int? page, int? pageSize)
    {
        var serviceResponse = await clientWrapper.GetAllVehicles(new GetAllVehiclesReq
        {
            FilteringStatus = ToProto(filteringStatus),
            Page = page ?? 1,
            PageSize = pageSize ?? 10
        });

        return Ok(MapToResponse(serviceResponse));

        GetAllVehiclesRes MapToResponse(Proto.VehicleFleetV1.GetAllVehiclesRes grpcResponse)
        {
            var response = new GetAllVehiclesRes();

            response.Vehicles.AddRange(grpcResponse.Vehicles.Select(v => new VehicleShortView
                {
                    VehicleId = Guid.Parse((ReadOnlySpan<char>)v.VehicleId),
                    Color = ToContract(v.Color),
                    Brand = v.Brand,
                    CarModel = v.CarModel,
                })
                .ToList());

            return response;
        }
    }

    public override async Task<IActionResult> GetVehicleById(Guid vehicleId)
    {
        var serviceResponse =
            await clientWrapper.GetVehicleById(new GetVehicleByIdReq { VehicleId = vehicleId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetVehicleByIdRes MapToResponse(Proto.VehicleFleetV1.GetVehicleByIdRes grpcResponse)
        {
            return new GetVehicleByIdRes
            {
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                Brand = grpcResponse.Brand,
                CarModel = grpcResponse.CarModel,
                Color = ToContract(grpcResponse.Color),
                PlateNumber = grpcResponse.PlateNumber,
                FuelLevelPercents = grpcResponse.FuelLevelPercents,
                PricePerMin = grpcResponse.PricePerMin,
                PricePerHour = grpcResponse.PricePerHour,
                Location = new OpenApiContractV1.Models.Location
                    { Latitude = grpcResponse.Location.Latitude, Longitude = grpcResponse.Location.Longitude }
            };
        }
    }

    public override async Task<IActionResult> GetVehicleDetailsById(Guid vehicleId)
    {
        var serviceResponse = await clientWrapper.GetVehicleDetailsById(new GetVehicleDetailsByIdReq
            { VehicleId = vehicleId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetVehicleDetailsByIdRes MapToResponse(
            Proto.VehicleFleetV1.GetVehicleDetailsByIdRes grpcResponse)
        {
            return new GetVehicleDetailsByIdRes
            {
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                Status = ToContract(grpcResponse.Status),
                Brand = grpcResponse.Brand,
                CarModel = grpcResponse.CarModel,
                Color = ToContract(grpcResponse.Color),
                PlateNumber = grpcResponse.PlateNumber,
                Vin = grpcResponse.Vin,
                FuelLevelPercents = grpcResponse.FuelLevelPercents,
                PricePerMin = grpcResponse.PricePerMin,
                PricePerHour = grpcResponse.PricePerHour,
                PricePerDay = grpcResponse.PricePerDay,
                Location = new OpenApiContractV1.Models.Location
                    { Latitude = grpcResponse.Location.Latitude, Longitude = grpcResponse.Location.Longitude },
            };
        }
    }

    public override async Task<IActionResult> GetVehiclesInSquare(GetVehiclesInSquareReq request)
    {
        var serviceResponse = await clientWrapper.GetVehiclesInSquare(new Proto.VehicleFleetV1.GetVehiclesInSquareReq
        {
            FilteringStatus = ToProto(request.FilteringStatus),
            LowerRightLocation = new Location
                { Latitude = request.LowerRightLocation.Latitude, Longitude = request.LowerRightLocation.Longitude },
            UpperLeftLocation = new Location
                { Latitude = request.UpperLeftLocation.Latitude, Longitude = request.UpperLeftLocation.Longitude }
        });

        return Ok(MapToResponse(serviceResponse));

        GetVehiclesInSquareRes MapToResponse(
            Proto.VehicleFleetV1.GetVehiclesInSquareRes grpcResponse)
        {
            return new GetVehiclesInSquareRes
            {
                Vehicles = grpcResponse.Vehicles.Select(v => new VehicleInSquareShortView
                    {
                        VehicleId = Guid.Parse((ReadOnlySpan<char>)v.VehicleId),
                        Brand = v.Brand,
                        CarModel = v.CarModel,
                        Color = ToContract(v.Color),
                        Location = new OpenApiContractV1.Models.Location
                            { Latitude = v.Location.Latitude, Longitude = v.Location.Longitude }
                    })
                    .ToList()
            };
        }
    }

    public override async Task<IActionResult> MarkAsReadiedForReleaseVehicle(Guid vehicleId)
    {
        await clientWrapper.MarkAsReadiedForRelease(new MarkAsReadiedForReleaseVehicleReq
            { VehicleId = vehicleId.ToString() });

        return Ok();
    }

    public override async Task<IActionResult> ReleaseVehicle(Guid vehicleId)
    {
        await clientWrapper.ReleaseVehicle(new ReleaseVehicleReq { VehicleId = vehicleId.ToString() });

        return Ok();
    }

    public override async Task<IActionResult> SendToServiceVehicle(Guid vehicleId)
    {
        await clientWrapper.SendToServiceVehicle(new SendToServiceVehicleReq { VehicleId = vehicleId.ToString() });

        return Ok();
    }

    private Color ToProto(OpenApiContractV1.Models.Color c)
    {
        var mapper = new ColorEnumMapper();

        return mapper.ToVehicleFleetProto(c);
    }

    private OpenApiContractV1.Models.Color ToContract(Color c)
    {
        var mapper = new ColorEnumMapper();

        return mapper.ToVehicleFleetContract(c);
    }

    private Status ToContract(Proto.VehicleFleetV1.Status s)
    {
        return s switch
        {
            Proto.VehicleFleetV1.Status.Added => Status.AddedEnum,
            Proto.VehicleFleetV1.Status.NotAdded => Status.NotAddedEnum,
            Proto.VehicleFleetV1.Status.ReadiedForRelease => Status.ReadyForReleaseEnum,
            Proto.VehicleFleetV1.Status.ReleasedUnspecified => Status.ReleasedUnspecifiedEnum,
            Proto.VehicleFleetV1.Status.Occupied => Status.OccupiedEnum,
            Proto.VehicleFleetV1.Status.Serviced => Status.ServicedEnum,
            Proto.VehicleFleetV1.Status.Deleted => Status.DeletedEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }

    private Proto.VehicleFleetV1.Status ToProto(Status s)
    {
        return s switch
        {
            Status.AddingInProgressEnum => Proto.VehicleFleetV1.Status.AddingInProgress,
            Status.AddedEnum => Proto.VehicleFleetV1.Status.Added,
            Status.NotAddedEnum => Proto.VehicleFleetV1.Status.NotAdded,
            Status.ReadyForReleaseEnum => Proto.VehicleFleetV1.Status.ReadiedForRelease,
            Status.ReleasedUnspecifiedEnum => Proto.VehicleFleetV1.Status.ReleasedUnspecified,
            Status.OccupiedEnum => Proto.VehicleFleetV1.Status.Occupied,
            Status.ServicedEnum => Proto.VehicleFleetV1.Status.Serviced,
            Status.DeletedEnum => Proto.VehicleFleetV1.Status.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }
}