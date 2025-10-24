using Microsoft.FeatureManagement;
using Poliedro.Chapter.FeatureFlags.Api.Features;

namespace Poliedro.Chapter.FeatureFlags.Api.Services;

public class DiscountService : IDiscountService
{
    private readonly IFeatureManager _featureManager;

    public DiscountService(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task<(decimal discountAmount, string strategy)> CalculateDiscount(decimal totalAmount)
    {
        var isNewDiscountEnabled = await _featureManager.IsEnabledAsync(AppFeatureFlags.NewDiscountCalculation);

        if (!isNewDiscountEnabled)
        {
            var discount = totalAmount * 0.05m;
            return (discount, "Desconto Fixo 5%");
        }

        // desconto progressivo
        decimal discountPercentage;
        string strategy;

        if (totalAmount <= 1000m)
        {
            discountPercentage = 0.05m;
            strategy = "Desconto Progressivo 5%";
        }
        else if (totalAmount <= 5000m)
        {
            discountPercentage = 0.10m;
            strategy = "Desconto Progressivo 10%";
        }
        else
        {
            discountPercentage = 0.15m;
            strategy = "Desconto Progressivo 15%";
        }

        var progressiveDiscount = totalAmount * discountPercentage;
        return (progressiveDiscount, strategy);
    }
}
