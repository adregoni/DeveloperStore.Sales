using DeveloperStore.Sales.Domain;
using DeveloperStore.Sales.Application.Dtos;

namespace DeveloperStore.Sales.Application;

public static class Mapping
{
    public static SaleResponse ToResponse(this Sale sale)
        => new(
            sale.Id,
            sale.SaleNumber,
            sale.DateUtc,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            sale.IsCancelled,
            sale.TotalAmount,
            sale.Items
                .Select(i => new SaleItemResponse(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.Discount, i.Total, i.IsCancelled))
                .ToList()
        );
}
