using Api;
using Grpc.Core;

namespace Clients.Clients.Rent;

public class RentClientWrapper(Api.Rent.RentClient client)
{
    public async Task<GetRentByIdResponse> GetRentById(GetRentByIdRequest request, CancellationToken ct = default)
    {
        return await client.GetRentByIdAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetAllCurrentRentsResponse> GetAllCurrentRents(
        GetAllCurrentRentsRequest request,
        CancellationToken ct = default)
    {
        return await client.GetAllCurrentRentsAsync(request, new CallOptions([], cancellationToken: ct));
    }

    public async Task<GetCurrentAmountRentResponse> GetCurrentAmountRent(
        GetCurrentAmountRentRequest request,
        CancellationToken ct = default)
    {
        return await client.GetCurrentAmountRentAsync(request, new CallOptions([], cancellationToken: ct));
    }
}