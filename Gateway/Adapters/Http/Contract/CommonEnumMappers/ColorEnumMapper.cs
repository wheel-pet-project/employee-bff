using Proto.VehicleFleetV1;

namespace Gateway.Adapters.Http.Contract.CommonEnumMappers;

public class ColorEnumMapper
{
    public Color ToVehicleFleetProto(OpenApiContractV1.Models.Color c)
    {
        return c switch
        {
            OpenApiContractV1.Models.Color.WhiteEnum => Color.WhiteUnspecified,
            OpenApiContractV1.Models.Color.GreyEnum => Color.Grey,
            OpenApiContractV1.Models.Color.BlackEnum => Color.Black,
            OpenApiContractV1.Models.Color.BlueEnum => Color.Blue,
            OpenApiContractV1.Models.Color.RedEnum => Color.Red,
            OpenApiContractV1.Models.Color.YellowEnum => Color.Yellow,
            OpenApiContractV1.Models.Color.OrangeEnum => Color.Orange,
            OpenApiContractV1.Models.Color.GreenEnum => Color.Green,
            OpenApiContractV1.Models.Color.BeigeEnum => Color.Beige,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }

    public Api.Color ToVehicleDocumentsProto(OpenApiContractV1.Models.Color c)
    {
        return c switch
        {
            OpenApiContractV1.Models.Color.WhiteEnum => Api.Color.WhiteUnspecified,
            OpenApiContractV1.Models.Color.GreyEnum => Api.Color.Grey,
            OpenApiContractV1.Models.Color.BlackEnum => Api.Color.Black,
            OpenApiContractV1.Models.Color.BlueEnum => Api.Color.Blue,
            OpenApiContractV1.Models.Color.RedEnum => Api.Color.Red,
            OpenApiContractV1.Models.Color.YellowEnum => Api.Color.Yellow,
            OpenApiContractV1.Models.Color.OrangeEnum => Api.Color.Orange,
            OpenApiContractV1.Models.Color.GreenEnum => Api.Color.Green,
            OpenApiContractV1.Models.Color.BeigeEnum => Api.Color.Beige,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }

    public OpenApiContractV1.Models.Color ToVehicleFleetContract(Color c)
    {
        return c switch
        {
            Color.WhiteUnspecified => OpenApiContractV1.Models.Color.WhiteEnum,
            Color.Grey => OpenApiContractV1.Models.Color.GreyEnum,
            Color.Black => OpenApiContractV1.Models.Color.BlackEnum,
            Color.Blue => OpenApiContractV1.Models.Color.BlueEnum,
            Color.Red => OpenApiContractV1.Models.Color.RedEnum,
            Color.Yellow => OpenApiContractV1.Models.Color.YellowEnum,
            Color.Orange => OpenApiContractV1.Models.Color.OrangeEnum,
            Color.Green => OpenApiContractV1.Models.Color.GreenEnum,
            Color.Beige => OpenApiContractV1.Models.Color.BeigeEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }

    public OpenApiContractV1.Models.Color ToVehicleDocumentsContract(Api.Color c)
    {
        return c switch
        {
            Api.Color.WhiteUnspecified => OpenApiContractV1.Models.Color.WhiteEnum,
            Api.Color.Grey => OpenApiContractV1.Models.Color.GreyEnum,
            Api.Color.Black => OpenApiContractV1.Models.Color.BlackEnum,
            Api.Color.Blue => OpenApiContractV1.Models.Color.BlueEnum,
            Api.Color.Red => OpenApiContractV1.Models.Color.RedEnum,
            Api.Color.Yellow => OpenApiContractV1.Models.Color.YellowEnum,
            Api.Color.Orange => OpenApiContractV1.Models.Color.OrangeEnum,
            Api.Color.Green => OpenApiContractV1.Models.Color.GreenEnum,
            Api.Color.Beige => OpenApiContractV1.Models.Color.BeigeEnum,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
}