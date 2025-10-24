using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Poliedro.Chapter.FeatureFlags.Api.Features;

namespace Poliedro.Chapter.FeatureFlags.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureFlagsController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeatureFlagsController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    /// <summary>
    /// Lista todas as feature flags e seus status
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetFeatureFlags()
    {
        var flags = new Dictionary<string, object>
        {
            {
                AppFeatureFlags.NewDiscountCalculation,
                new
                {
                    Enabled = await _featureManager.IsEnabledAsync(AppFeatureFlags.NewDiscountCalculation),
                    Description = "Novo cálculo de desconto progressivo (5%, 10%, 15% baseado no valor)",
                    OldBehavior = "Desconto fixo de 5%",
                    NewBehavior = "Desconto progressivo: 5% até R$1000, 10% até R$5000, 15% acima"
                }
            },
            {
                AppFeatureFlags.PremiumPricing,
                new
                {
                    Enabled = await _featureManager.IsEnabledAsync(AppFeatureFlags.PremiumPricing),
                    Description = "Sistema de precificação premium para eletrônicos",
                    OldBehavior = "Preço base do produto",
                    NewBehavior = "Preço base + 20% de margem premium em eletrônicos"
                }
            },
            {
                AppFeatureFlags.ShowInactiveProducts,
                new
                {
                    Enabled = await _featureManager.IsEnabledAsync(AppFeatureFlags.ShowInactiveProducts),
                    Description = "Exibir produtos inativos no catálogo",
                    OldBehavior = "Mostra apenas produtos ativos",
                    NewBehavior = "Mostra todos os produtos (ativos e inativos)"
                }
            },
            {
                AppFeatureFlags.AdvancedProductFilters,
                new
                {
                    Enabled = await _featureManager.IsEnabledAsync(AppFeatureFlags.AdvancedProductFilters),
                    Description = "Filtros avançados de produtos",
                    OldBehavior = "Busca simples por nome",
                    NewBehavior = "Busca por nome, categoria e faixa de preço"
                }
            }
        };

        return Ok(flags);
    }

    /// <summary>
    /// Verifica o status de uma feature flag específica
    /// </summary>
    [HttpGet("{flagName}")]
    public async Task<ActionResult<object>> GetFeatureFlag(string flagName)
    {
        var isEnabled = await _featureManager.IsEnabledAsync(flagName);

        return Ok(new
        {
            Name = flagName,
            Enabled = isEnabled
        });
    }
}
