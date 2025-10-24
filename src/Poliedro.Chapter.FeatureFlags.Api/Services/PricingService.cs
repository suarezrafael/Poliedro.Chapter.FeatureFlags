using Microsoft.FeatureManagement;
using Poliedro.Chapter.FeatureFlags.Api.Features;
using Poliedro.Chapter.FeatureFlags.Api.Models;

namespace Poliedro.Chapter.FeatureFlags.Api.Services;

public class PricingService : IPricingService
{
    private readonly IFeatureManager _featureManager;

    public PricingService(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task<decimal> CalculateProductPrice(Product product)
    {
        var isPremiumPricingEnabled = await _featureManager.IsEnabledAsync(AppFeatureFlags.PremiumPricing);

        if (!isPremiumPricingEnabled)
        {
            return product.BasePrice;
        }

        // margem premium em eletrônicos
        if (product.Category == ProductCategories.Electronics)
        {
            return product.BasePrice * 1.20m;
        }

        return product.BasePrice;
    }
}
