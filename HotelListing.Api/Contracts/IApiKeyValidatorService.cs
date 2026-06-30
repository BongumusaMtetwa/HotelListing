
namespace HotelListing.Api.Contracts;

public interface IApiKeyValidatorService
{
    Task<bool> IsValidAsync(string apiKey, CancellationToken cancellationToken = default);
}
