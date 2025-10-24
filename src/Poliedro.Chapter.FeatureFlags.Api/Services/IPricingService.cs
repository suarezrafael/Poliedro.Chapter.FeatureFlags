using Poliedro.Chapter.FeatureFlags.Api.Models;

namespace Poliedro.Chapter.FeatureFlags.Api.Services;

public interface IPricingService
{
    Task<decimal> CalculateProductPrice(Product product);
}
