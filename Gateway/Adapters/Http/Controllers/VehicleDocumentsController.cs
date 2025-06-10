using Api;
using Clients.Clients.VehicleDocuments;
using Gateway.Adapters.Http.Contract.CommonEnumMappers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using OpenApiContractV1.Controllers;
using OpenApiContractV1.Models;
using AddOsagoRequest = OpenApiContractV1.Models.AddOsagoRequest;
using AddPtsRequest = OpenApiContractV1.Models.AddPtsRequest;
using AddStsRequest = OpenApiContractV1.Models.AddStsRequest;
using Color = Api.Color;
using GetOsagoByVehicleDocumentsIdResponse = Api.GetOsagoByVehicleDocumentsIdResponse;
using GetPtsByVehicleDocumentsIdResponse = Api.GetPtsByVehicleDocumentsIdResponse;
using GetStsByVehicleDocumentsIdResponse = Api.GetStsByVehicleDocumentsIdResponse;
using GetVehicleDocumentsByVehicleIdResponse = OpenApiContractV1.Models.GetVehicleDocumentsByVehicleIdResponse;

namespace Gateway.Adapters.Http.Controllers;

public class VehicleDocumentsController(VehicleDocumentsClientWrapper clientWrapper) : VehicleDocumentsApiController
{
    public override async Task<IActionResult> AddOsago(Guid vehicleDocumentsId, AddOsagoRequest request)
    {
        await clientWrapper.AddOsago(new Api.AddOsagoRequest
        {
            VehicleDocsId = vehicleDocumentsId.ToString(), PhotoBytes = ByteString.CopyFrom(request.PhotoBytes),
            DateOfIssue = Timestamp.FromDateTime(request.DateOfIssue.ToDateTime(new TimeOnly()).ToUniversalTime()),
            DateOfExpiry = Timestamp.FromDateTime(request.DateOfExpiry.ToDateTime(new TimeOnly()).ToUniversalTime())
        });

        return Ok();
    }

    public override async Task<IActionResult> AddPts(Guid vehicleDocumentsId, AddPtsRequest request)
    {
        await clientWrapper.AddPts(new Api.AddPtsRequest
        {
            VehicleDocsId = vehicleDocumentsId.ToString(), Vin = request.Vin,
            Color = ToProto(request.Color),
            YearOfManufacture = Timestamp.FromDateTime(request.YearOfManufacture.ToUniversalTime()),
            FrontPhotoBytes = ByteString.CopyFrom(request.FrontPhotoBytes),
            BackPhotoBytes = ByteString.CopyFrom(request.BackPhotoBytes),
        });

        return Ok();
    }

    public override async Task<IActionResult> AddSts(Guid vehicleDocumentsId, AddStsRequest request)
    {
        var serviceResponse = await clientWrapper.AddSts(new Api.AddStsRequest
        {
            VehicleDocsId = vehicleDocumentsId.ToString(),
            FrontPhotoBytes = ByteString.CopyFrom(request.FrontPhotoBytes),
            BackPhotoBytes = ByteString.CopyFrom(request.BackPhotoBytes)
        });

        return Ok();
    }

    public override async Task<IActionResult> GetOsagoByVehicleDocumentsId(Guid vehicleDocumentsId)
    {
        var serviceResponse = await clientWrapper.GetOsagoByVehicleDocumentsId(new GetOsagoByVehicleDocumentsIdRequest
            { VehicleDocsId = vehicleDocumentsId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.GetOsagoByVehicleDocumentsIdResponse MapToResponse(
            GetOsagoByVehicleDocumentsIdResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.GetOsagoByVehicleDocumentsIdResponse
            {
                PhotoS3Url = grpcResponse.PhotoS3Url,
                DateOfIssue = DateOnly.FromDateTime(grpcResponse.DateOfIssue.ToDateTime()),
                DateOfExpiry = DateOnly.FromDateTime(grpcResponse.DateOfExpiry.ToDateTime()),
                OsagoStatus = ToContract(grpcResponse.ExpiryStatus)
            };
        }
    }

    public override async Task<IActionResult> GetPtsByVehicleDocumentsId(Guid vehicleDocumentsId)
    {
        var serviceResponse = await clientWrapper.GetPtsByVehicleDocumentsId(new GetPtsByVehicleDocumentsIdRequest
            { VehicleDocsId = vehicleDocumentsId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.GetPtsByVehicleDocumentsIdResponse MapToResponse(
            GetPtsByVehicleDocumentsIdResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.GetPtsByVehicleDocumentsIdResponse
            {
                Vin = grpcResponse.Vin,
                Color = ToContract(grpcResponse.Color),
                YearOfManufacture = grpcResponse.YearOfManufacture.ToDateTime().ToUniversalTime(),
                FrontPhotoS3Url = grpcResponse.FrontPhotoS3Url,
                BackPhotoS3Url = grpcResponse.BackPhotoS3Url,
            };
        }
    }

    public override async Task<IActionResult> GetStsByVehicleDocumentsId(Guid vehicleDocumentsId)
    {
        var serviceResponse = await clientWrapper.GetStsByVehicleDocumentsId(new GetStsByVehicleDocumentsIdRequest
            { VehicleDocsId = vehicleDocumentsId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        OpenApiContractV1.Models.GetStsByVehicleDocumentsIdResponse MapToResponse(
            GetStsByVehicleDocumentsIdResponse grpcResponse)
        {
            return new OpenApiContractV1.Models.GetStsByVehicleDocumentsIdResponse
            {
                FrontPhotoS3Url = grpcResponse.FrontPhotoS3Url,
                BackPhotoS3Url = grpcResponse.BackPhotoS3Url
            };
        }
    }

    public override async Task<IActionResult> GetVehicleDocumentsByVehicleId(Guid vehicleId)
    {
        var serviceResponse = await clientWrapper.GetVehicleDocuments(new GetVehicleDocumentsByVehicleIdRequest
            { VehicleId = vehicleId.ToString() });

        return Ok(MapToResponse(serviceResponse));

        GetVehicleDocumentsByVehicleIdResponse MapToResponse(Api.GetVehicleDocumentsByVehicleIdResponse grpcResponse)
        {
            return new GetVehicleDocumentsByVehicleIdResponse
            {
                VehicleDocumentsId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleDocsId),
                VehicleId = Guid.Parse((ReadOnlySpan<char>)grpcResponse.VehicleId),
                Status = ToContract(grpcResponse.Status)
            };
        }
    }

    private Color ToProto(OpenApiContractV1.Models.Color c)
    {
        var mapper = new ColorEnumMapper();

        return mapper.ToVehicleDocumentsProto(c);
    }

    private OpenApiContractV1.Models.Color ToContract(Color c)
    {
        var mapper = new ColorEnumMapper();

        return mapper.ToVehicleDocumentsContract(c);
    }

    private OsagoStatus ToContract(ExpiryStatus s)
    {
        return s switch
        {
            ExpiryStatus.Expired => OsagoStatus.ExpiredEnum,
            ExpiryStatus.NotExpiredUnspecified => OsagoStatus.NotExpiredUnspecifiedEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        };
    }

    private GetVehicleDocumentsByVehicleIdResponseStatus ToContract(
        Api.GetVehicleDocumentsByVehicleIdResponse.Types.Status s)
    {
        return new GetVehicleDocumentsByVehicleIdResponseStatus
        {
            IsPtsAdded = s.IsPtsAdded,
            IsStsAdded = s.IsStsAdded,
            IsOsagoAdded = s.IsStsAdded
        };
    }
}