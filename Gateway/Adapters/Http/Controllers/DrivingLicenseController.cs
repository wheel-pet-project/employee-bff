using Clients.Clients.DrivingLicense;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using Proto.DrivingLicenseV1;
using GetAllLicensesResponse = OpenApiContractV1.Models.GetAllLicensesResponse;
using GetLicenseByIdResponse = OpenApiContractV1.Models.GetLicenseByIdResponse;
using Status = Proto.DrivingLicenseV1.Status;

namespace Gateway.Adapters.Http.Controllers;

public class DrivingLicenseController(DrivingLicenseClientWrapper clientWrapper) : DrivingLicenseApiController
{
    public override async Task<IActionResult> GetAllLicenses(
        DrivingLicenseStatus filteringStatus,
        int? page,
        int? pageSize)
    {
        var serviceResponse = await clientWrapper.GetAll(new GetAllLicensesRequest
            { FilteringStatus = ToProto(filteringStatus), Page = page ?? 1, PageSize = pageSize ?? 10 });

        return Ok(MapToResponse(serviceResponse));

        GetAllLicensesResponse MapToResponse(Proto.DrivingLicenseV1.GetAllLicensesResponse grpcResponse)
        {
            return new GetAllLicensesResponse
            {
                DrivingLicenses = grpcResponse.Licenses.Select(x => new DrivingLicenseShortView
                    {
                        LicenseId = Guid.Parse((ReadOnlySpan<char>)x.Id),
                        CustomerId = Guid.Parse((ReadOnlySpan<char>)x.AccountId),
                        Status = ToContract(x.Status)
                    })
                    .ToList()
            };
        }
    }

    public override async Task<IActionResult> GetLicenseById(Guid licenseId)
    {
        var serviceResponse =
            await clientWrapper.GetLicenseById(new GetLicenseByIdRequest { Id = licenseId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetLicenseByIdResponse MapToResponse(Proto.DrivingLicenseV1.GetLicenseByIdResponse grpcResponse)
        {
            return new GetLicenseByIdResponse
            {
                Id = Guid.Parse((ReadOnlySpan<char>)grpcResponse.Id),
                CustomerId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.AccId),
                Categories = [..grpcResponse.Categories.ToList()],
                Number = grpcResponse.Number,
                Status = ToContract(grpcResponse.Status),
                CityOfBirth = grpcResponse.CityOfBirth,
                CodeOfIssue = grpcResponse.CodeOfIssue,
                DateOfIssue = DateOnly.FromDateTime(grpcResponse.DateOfIssue.ToDateTime()),
                DateOfExpiry = DateOnly.FromDateTime(grpcResponse.DateOfExpiry.ToDateTime()),
                FrontPhotoS3Url = string.IsNullOrWhiteSpace(serviceResponse.FrontPhotoS3Url)
                    ? null!
                    : serviceResponse.FrontPhotoS3Url,
                BackPhotoS3Url = string.IsNullOrWhiteSpace(serviceResponse.BackPhotoS3Url)
                    ? null!
                    : serviceResponse.BackPhotoS3Url,
            };
        }
    }

    public override async Task<IActionResult> ApproveLicense(Guid licenseId)
    {
        await clientWrapper.Approve(new ApproveLicenseRequest { LicenseId = licenseId.ToString() });

        return Ok();
    }

    public override async Task<IActionResult> RejectLicense(Guid licenseId)
    {
        await clientWrapper.Reject(new RejectLicenseRequest { LicenseId = licenseId.ToString() });

        return Ok();
    }

    private DrivingLicenseStatus ToContract(Status s)
    {
        return s switch
        {
            Status.ApprovedUnspecified => DrivingLicenseStatus.ApprovedEnum,
            Status.PendingPhotosAdding => DrivingLicenseStatus.PendingPhotosAddingEnum,
            Status.PendingProcessing => DrivingLicenseStatus.PendingProcessingEnum,
            Status.Rejected => DrivingLicenseStatus.RejectedEnum,
            Status.Expired => DrivingLicenseStatus.ExpiredEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }

    private Status ToProto(DrivingLicenseStatus s)
    {
        return s switch
        {
            DrivingLicenseStatus.ApprovedEnum => Status.ApprovedUnspecified,
            DrivingLicenseStatus.PendingProcessingEnum => Status.PendingProcessing,
            DrivingLicenseStatus.PendingPhotosAddingEnum => Status.PendingPhotosAdding,
            DrivingLicenseStatus.RejectedEnum => Status.Rejected,
            DrivingLicenseStatus.ExpiredEnum => Status.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }
}