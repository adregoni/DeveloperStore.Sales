using DeveloperStore.Sales.Application;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResponse>
{
    private readonly ISaleRepository _repo;
    private readonly IEventPublisher _events;
    private readonly DeveloperStore.Sales.Domain.IDiscountPolicy _discount;

    public UpdateSaleHandler(ISaleRepository repo, IEventPublisher events, DeveloperStore.Sales.Domain.IDiscountPolicy discount)
    {
        _repo = repo;
        _events = events;
        _discount = discount;
    }

    public async Task<SaleResponse> Handle(UpdateSaleCommand request, CancellationToken ct)
    {
        var sale = await _repo.GetAsync(request.Id, ct) ?? throw new KeyNotFoundException("Sale not found.");
        if (sale.IsCancelled) throw new InvalidOperationException("Cancelled sale cannot be updated.");

        // cancel current active items
        foreach (var item in sale.Items.ToList())
            sale.CancelItem(item.Id);

        foreach (var i in request.Request.Items)
            sale.AddItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, _discount);

        await _repo.UpdateAsync(sale, ct);
        await _events.PublishAsync("SaleModified", new { sale.Id, sale.SaleNumber }, ct);
        return sale.ToResponse();
    }
}
