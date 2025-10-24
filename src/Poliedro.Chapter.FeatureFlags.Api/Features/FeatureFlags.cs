namespace Poliedro.Chapter.FeatureFlags.Api.Features;

/// <summary>
/// Define as feature flags disponíveis na aplicação
/// </summary>
public static class AppFeatureFlags
{
    /// <summary>
    /// Habilita o novo cálculo de desconto progressivo baseado no valor total da compra
    /// Quando desabilitado: desconto fixo de 5%
    /// Quando habilitado: desconto progressivo (5% até R$1000, 10% até R$5000, 15% acima de R$5000)
    /// </summary>
    public const string NewDiscountCalculation = "NewDiscountCalculation";

    /// <summary>
    /// Habilita o novo sistema de precificação premium
    /// Quando desabilitado: usa preço base do produto
    /// Quando habilitado: aplica margem premium de 20% em produtos eletrônicos
    /// </summary>
    public const string PremiumPricing = "PremiumPricing";

    /// <summary>
    /// Habilita a exibição de produtos inativos no catálogo
    /// Quando desabilitado: mostra apenas produtos ativos
    /// Quando habilitado: mostra todos os produtos (ativos e inativos)
    /// </summary>
    public const string ShowInactiveProducts = "ShowInactiveProducts";

    /// <summary>
    /// Habilita filtros avançados de produtos
    /// Quando desabilitado: busca simples por nome
    /// Quando habilitado: busca por nome, categoria e faixa de preço
    /// </summary>
    public const string AdvancedProductFilters = "AdvancedProductFilters";
}
