using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poliedro.Chapter.FeatureFlags.Api.Data;
using Poliedro.Chapter.FeatureFlags.Api.Models;
using Poliedro.Chapter.FeatureFlags.Api.Services;

namespace Poliedro.Chapter.FeatureFlags.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IDiscountService _discountService;
    private readonly IPricingService _pricingService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        ApplicationDbContext context,
        IDiscountService discountService,
        IPricingService pricingService,
        ILogger<OrdersController> logger)
    {
        _context = context;
        _discountService = discountService;
        _pricingService = pricingService;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo pedido
    /// Feature Flag: NewDiscountCalculation - controla a estratégia de desconto
    /// Feature Flag: PremiumPricing - controla o cálculo de preço dos produtos
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            return BadRequest(new { message = "Nome do cliente é obrigatório" });
        }

        if (request.Items == null || !request.Items.Any())
        {
            return BadRequest(new { message = "O pedido deve conter pelo menos um item" });
        }

        // Validar produtos
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToListAsync();

        if (products.Count != productIds.Count)
        {
            return BadRequest(new { message = "Um ou mais produtos não foram encontrados ou estão inativos" });
        }

        // Criar pedido
        var order = new Order
        {
            CustomerName = request.CustomerName,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            
            if (product.StockQuantity < item.Quantity)
            {
                return BadRequest(new { 
                    message = $"Estoque insuficiente para o produto '{product.Name}'. Disponível: {product.StockQuantity}" 
                });
            }

            // Calcular preço com base na feature flag de pricing
            var unitPrice = await _pricingService.CalculateProductPrice(product);
            var subtotal = unitPrice * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                Subtotal = subtotal
            });

            totalAmount += subtotal;

            // Atualizar estoque
            product.StockQuantity -= item.Quantity;
        }

        order.TotalAmount = totalAmount;

        // Calcular desconto com base na feature flag
        var (discountAmount, discountStrategy) = await _discountService.CalculateDiscount(totalAmount);
        order.DiscountAmount = discountAmount;
        order.FinalAmount = totalAmount - discountAmount;

        _logger.LogInformation(
            "Order created - Total: {Total}, Discount: {Discount}, Strategy: {Strategy}",
            totalAmount, discountAmount, discountStrategy);

        // Salvar no banco
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Adicionar items com o OrderId
        foreach (var item in orderItems)
        {
            item.OrderId = order.Id;
            _context.OrderItems.Add(item);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = order.Id },
            new OrderResponse(
                order.Id,
                order.CustomerName,
                order.OrderDate,
                order.TotalAmount,
                order.DiscountAmount,
                order.FinalAmount,
                order.Status,
                discountStrategy
            ));
    }

    /// <summary>
    /// Obtém um pedido específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound(new { message = "Pedido não encontrado" });
        }

        // Recalcular a estratégia de desconto para exibição
        var (_, discountStrategy) = await _discountService.CalculateDiscount(order.TotalAmount);

        return Ok(new OrderResponse(
            order.Id,
            order.CustomerName,
            order.OrderDate,
            order.TotalAmount,
            order.DiscountAmount,
            order.FinalAmount,
            order.Status,
            discountStrategy
        ));
    }

    /// <summary>
    /// Lista todos os pedidos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders()
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var response = new List<OrderResponse>();
        foreach (var order in orders)
        {
            var (_, discountStrategy) = await _discountService.CalculateDiscount(order.TotalAmount);
            response.Add(new OrderResponse(
                order.Id,
                order.CustomerName,
                order.OrderDate,
                order.TotalAmount,
                order.DiscountAmount,
                order.FinalAmount,
                order.Status,
                discountStrategy
            ));
        }

        return Ok(response);
    }
}
