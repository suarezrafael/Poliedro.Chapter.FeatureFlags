using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Poliedro.Chapter.FeatureFlags.Api.Data;
using Poliedro.Chapter.FeatureFlags.Api.Features;
using Poliedro.Chapter.FeatureFlags.Api.Models;
using Poliedro.Chapter.FeatureFlags.Api.Services;

namespace Poliedro.Chapter.FeatureFlags.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IFeatureManager _featureManager;
    private readonly IPricingService _pricingService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ApplicationDbContext context,
        IFeatureManager featureManager,
        IPricingService pricingService,
        ILogger<ProductsController> logger)
    {
        _context = context;
        _featureManager = featureManager;
        _pricingService = pricingService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os produtos
    /// Feature Flag: ShowInactiveProducts - controla se produtos inativos são exibidos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        var showInactive = await _featureManager.IsEnabledAsync(AppFeatureFlags.ShowInactiveProducts);
        
        _logger.LogInformation("GetProducts - ShowInactiveProducts: {ShowInactive}", showInactive);

        var query = _context.Products.AsQueryable();

        if (!showInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        var products = await query.ToListAsync();

        var response = new List<ProductResponse>();
        foreach (var product in products)
        {
            var price = await _pricingService.CalculateProductPrice(product);
            response.Add(new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                price,
                product.Category,
                product.StockQuantity,
                product.IsActive
            ));
        }

        return Ok(response);
    }

    /// <summary>
    /// Busca produtos por critérios
    /// Feature Flag: AdvancedProductFilters - habilita filtros avançados
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> SearchProducts(
        [FromBody] ProductSearchRequest request)
    {
        var advancedFilters = await _featureManager.IsEnabledAsync(AppFeatureFlags.AdvancedProductFilters);
        var showInactive = await _featureManager.IsEnabledAsync(AppFeatureFlags.ShowInactiveProducts);

        _logger.LogInformation(
            "SearchProducts - AdvancedFilters: {AdvancedFilters}, ShowInactive: {ShowInactive}",
            advancedFilters, showInactive);

        var query = _context.Products.AsQueryable();

        if (!showInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(p => p.Name.Contains(request.Name));
        }

        if (advancedFilters)
        {
            // Filtros avançados habilitados
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                query = query.Where(p => p.Category == request.Category);
            }
        }

        var products = await query.ToListAsync();

        // Apply price filtering after calculating actual prices (to account for PremiumPricing)
        if (advancedFilters && (request.MinPrice.HasValue || request.MaxPrice.HasValue))
        {
            var productsWithPrices = new List<(Product product, decimal price)>();
            foreach (var product in products)
            {
                var price = await _pricingService.CalculateProductPrice(product);
                productsWithPrices.Add((product, price));
            }

            if (request.MinPrice.HasValue)
            {
                productsWithPrices = productsWithPrices.Where(p => p.price >= request.MinPrice.Value).ToList();
            }

            if (request.MaxPrice.HasValue)
            {
                productsWithPrices = productsWithPrices.Where(p => p.price <= request.MaxPrice.Value).ToList();
            }

            products = productsWithPrices.Select(p => p.product).ToList();
        }

        var response = new List<ProductResponse>();
        foreach (var product in products)
        {
            var price = await _pricingService.CalculateProductPrice(product);
            response.Add(new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                price,
                product.Category,
                product.StockQuantity,
                product.IsActive
            ));
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtém um produto específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound(new { message = "Produto não encontrado" });
        }

        var showInactive = await _featureManager.IsEnabledAsync(AppFeatureFlags.ShowInactiveProducts);
        if (!product.IsActive && !showInactive)
        {
            return NotFound(new { message = "Produto não disponível" });
        }

        var price = await _pricingService.CalculateProductPrice(product);

        return Ok(new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            price,
            product.Category,
            product.StockQuantity,
            product.IsActive
        ));
    }
}
