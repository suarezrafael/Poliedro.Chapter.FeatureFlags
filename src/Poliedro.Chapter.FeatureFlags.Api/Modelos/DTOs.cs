namespace Poliedro.Chapter.FeatureFlags.Api.Models;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    int StockQuantity,
    bool IsActive
);

public record CreateOrderRequest(
    string CustomerName,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(
    int ProductId,
    int Quantity
);

public record OrderResponse(
    int Id,
    string CustomerName,
    DateTime OrderDate,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    string Status,
    string DiscountStrategy
);

public record ProductSearchRequest(
    string? Name = null,
    string? Category = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
);
