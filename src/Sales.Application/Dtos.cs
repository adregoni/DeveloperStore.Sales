namespace DeveloperStore.Sales.Application.Dtos;

public record SaleItemRequest(string ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record CreateSaleRequest(
    string SaleNumber,
    DateTime DateUtc,
    string CustomerId,
    string CustomerName,
    string BranchId,
    string BranchName,
    List<SaleItemRequest> Items
);

public record UpdateSaleRequest(
    DateTime DateUtc,
    string CustomerId,
    string CustomerName,
    string BranchId,
    string BranchName,
    List<SaleItemRequest> Items
);

public record AddItemRequest(string ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record UpdateItemRequest(int Quantity, decimal UnitPrice);

public record SaleItemResponse(Guid Id, string ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Discount, decimal Total, bool IsCancelled);
public record SaleResponse(Guid Id, string SaleNumber, DateTime DateUtc, string CustomerId, string CustomerName, string BranchId, string BranchName, bool IsCancelled, decimal TotalAmount, List<SaleItemResponse> Items);
