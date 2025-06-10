using Api;
using Clients.Clients.Check;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using CheckForReview = OpenApiContractV1.Models.CheckForReview;
using FixationCategory = OpenApiContractV1.Models.FixationCategory;
using GetAllChecksForConsiderateReviewResponse = OpenApiContractV1.Models.GetAllChecksForConsiderateReviewResponse;
using GetCheckByIdResponse = OpenApiContractV1.Models.GetCheckByIdResponse;

namespace Gateway.Adapters.Http.Controllers;

public class CheckController(CheckClientWrapper clientWrapper) : VehicleCheckApiController
{
    public override async Task<IActionResult> GetAllChecksForConsiderateReview(int? page, int? pageSize)
    {
        var serviceResponse = await clientWrapper.AddDamageFixations(new GetAllChecksForConsiderateReviewRequest
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 10
        });

        return Ok(MapToResponse(serviceResponse));

        GetAllChecksForConsiderateReviewResponse MapToResponse(
            Api.GetAllChecksForConsiderateReviewResponse grpcResponse)
        {
            return new GetAllChecksForConsiderateReviewResponse
            {
                Checks = grpcResponse.Checks
                    .Select(x => new CheckForReview
                    {
                        CheckId = Guid.Parse((ReadOnlySpan<char>)x.CheckId),
                        BookingId = Guid.Parse((ReadOnlySpan<char>)x.BookingId),
                        VehicleId = Guid.Parse((ReadOnlySpan<char>)x.VehicleId),
                        Start = x.Start.ToDateTime().ToUniversalTime(),
                        End = x.End == null ? null : x.End.ToDateTime().ToUniversalTime(),
                    })
                    .ToList()
            };
        }
    }

    public override async Task<IActionResult> GetCheckById(Guid checkId)
    {
        var serviceResponse = await clientWrapper.GetCheckById(new GetCheckByIdRequest
        {
            CheckId = checkId.ToString()
        });

        return Ok(MapToResponse(serviceResponse));

        GetCheckByIdResponse MapToResponse(Api.GetCheckByIdResponse grpcResponse)
        {
            return new GetCheckByIdResponse
            {
                CheckId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.CheckId),
                BookingId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.BookingId),
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                Start = grpcResponse.Start.ToDateTime().ToUniversalTime(),
                End = grpcResponse.End == null ? null : grpcResponse.End.ToDateTime().ToUniversalTime(),
                Fixations = grpcResponse.Fixations.Select(x => new Fixation
                    {
                        S3Url = x.S3Url,
                        Category = ToContract(x.Category),
                        Description = x.Description
                    })
                    .ToList()
            };
        }
    }

    private FixationCategory ToContract(Api.FixationCategory c)
    {
        return c switch
        {
            Api.FixationCategory.MinorDamageUnspecified => FixationCategory.MinorDamageEnum,
            Api.FixationCategory.SignificantDamage => FixationCategory.SignificantDamageEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
}