using DeveloperStore.Sales.Application;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class UpdateItemHandler : IRequestHandler<UpdateItemCommand, SaleResponse>
{
    private readonly ISaleRepository _repo;
    private readonly IEventPublisher _events;
    private readonly DeveloperStore.Sales.Domain.IDiscountPolicy _discount;

    public UpdateItemHandler(ISaleRepository repo, IEventPublisher events, DeveloperStore.Sales.Domain.IDiscountPolicy discount)
    {
        _repo = repo;
        _events = events;
        _discount = discount;
    }

    public async Task<SaleResponse> Handle(UpdateItemCommand request, CancellationToken ct)
    {
        var sale = await _repo.GetAsync(request.SaleId, ct) ?? throw new KeyNotFoundException("Sale not found.");
        sale.UpdateItem(request.ItemId, request.Request.Quantity, request.Request.UnitPrice, _discount);
        await _repo.UpdateAsync(sale, ct);
        await _events.PublishAsync("SaleModified", new { sale.Id, request.ItemId }, ct);
        return sale.ToResponse();
    }
}
