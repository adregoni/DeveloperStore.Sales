using DeveloperStore.Sales.Application;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using DeveloperStore.Sales.Domain;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleResponse>
{
    private readonly ISaleRepository _repo;
    private readonly IEventPublisher _events;
    private readonly IDiscountPolicy _discount;

    public CreateSaleHandler(ISaleRepository repo, IEventPublisher events, IDiscountPolicy discount)
    {
        _repo = repo;
        _events = events;
        _discount = discount;
    }

    public async Task<SaleResponse> Handle(CreateSaleCommand request, CancellationToken ct)
    {
        var r = request.Request;
        if (await _repo.GetByNumberAsync(r.SaleNumber, ct) is not null)
            throw new InvalidOperationException($"Sale with number '{r.SaleNumber}' already exists.");
        var sale = new Sale(r.SaleNumber, r.DateUtc, r.CustomerId, r.CustomerName, r.BranchId, r.BranchName);
        foreach (var i in r.Items)
            sale.AddItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, _discount);
        await _repo.AddAsync(sale, ct);
        await _events.PublishAsync("SaleCreated", new { sale.Id, sale.SaleNumber, sale.TotalAmount }, ct);
        return sale.ToResponse();
    }
}
