namespace Poliedro.Chapter.FeatureFlags.Api.Services;

public interface IDiscountService
{
    Task<(decimal discountAmount, string strategy)> CalculateDiscount(decimal totalAmount);
}
