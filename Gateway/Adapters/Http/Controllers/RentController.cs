using Api;
using Clients.Clients.Rent;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using GetAllCurrentRentsResponse = OpenApiContractV1.Models.GetAllCurrentRentsResponse;
using GetCurrentAmountRentResponse = OpenApiContractV1.Models.GetCurrentAmountRentResponse;
using GetRentByIdResponse = OpenApiContractV1.Models.GetRentByIdResponse;
using Rent = OpenApiContractV1.Models.Rent;
using RentStatus = OpenApiContractV1.Models.RentStatus;

namespace Gateway.Adapters.Http.Controllers;

public class RentController(RentClientWrapper clientWrapper) : RentApiController
{
    public override async Task<IActionResult> GetAllCurrentRents(int page, int pageSize)
    {
        var serviceResponse = await clientWrapper.GetAllCurrentRents(new GetAllCurrentRentsRequest
            { Page = page, PageSize = pageSize });

        return Ok(MapToResponse(serviceResponse));

        GetAllCurrentRentsResponse MapToResponse(Api.GetAllCurrentRentsResponse grpcResponse)
        {
            return new GetAllCurrentRentsResponse
            {
                Rents = grpcResponse.Rents
                    .Select(x => new Rent
                    {
                        RentId = Guid.Parse((ReadOnlySpan<char>)x.RentId),
                        VehicleId = Guid.Parse((ReadOnlySpan<char>)x.VehicleId),
                        CustomerId = Guid.Parse((ReadOnlySpan<char>)x.CustomerId),
                        Start = x.Start.ToDateTime()
                    })
                    .ToList()
            };
        }
    }

    public override async Task<IActionResult> GetCurrentAmountRent(Guid rentId)
    {
        var serviceResponse = await clientWrapper.GetCurrentAmountRent(new GetCurrentAmountRentRequest
            { RentId = rentId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetCurrentAmountRentResponse MapToResponse(Api.GetCurrentAmountRentResponse grpcResponse)
        {
            return new GetCurrentAmountRentResponse
            {
                CurrentAmount = grpcResponse.CurrentAmount
            };
        }
    }

    public override async Task<IActionResult> GetRentById(Guid rentId)
    {
        var serviceResponse = await clientWrapper.GetRentById(new GetRentByIdRequest { RentId = rentId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetRentByIdResponse MapToResponse(Api.GetRentByIdResponse grpcResponse)
        {
            return new GetRentByIdResponse
            {
                RentId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.RentId),
                RentStatus = StatusToContract(grpcResponse.Status),
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                BookingId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.RentId),
                CustomerId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.CustomerId),
                Start = grpcResponse.Start.ToDateTime().ToUniversalTime(),
                End = grpcResponse.End == null ? null : grpcResponse.End.ToDateTime().ToUniversalTime(),
                ActualAmount = grpcResponse.ActualAmount
            };
        }
    }

    private RentStatus StatusToContract(Api.RentStatus s)
    {
        return s switch
        {
            Api.RentStatus.Completed => RentStatus.CompletedEnum,
            Api.RentStatus.InProgressUnspecified => RentStatus.InProgressEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }
}