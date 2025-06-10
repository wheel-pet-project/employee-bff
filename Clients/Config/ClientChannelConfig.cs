namespace Clients.Config;

public class ClientChannelConfig
{
    public Uri Uri { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
}

public class IdentityChannelConfig : ClientChannelConfig;

public class VehicleDocumentsChannelConfig : ClientChannelConfig;

public class BookingChannelConfig : ClientChannelConfig;

public class VehicleCheckChannelConfig : ClientChannelConfig;

public class DrivingLicenseChannelConfig : ClientChannelConfig;

public class RentChannelConfig : ClientChannelConfig;

public class VehicleFleetChannelConfig : ClientChannelConfig;