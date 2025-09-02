using DeveloperStore.Sales.Application;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class CancelItemHandler : IRequestHandler<CancelItemCommand, SaleResponse>
{
    private readonly ISaleRepository _repo;
    private readonly IEventPublisher _events;

    public CancelItemHandler(ISaleRepository repo, IEventPublisher events)
    {
        _repo = repo;
        _events = events;
    }

    public async Task<SaleResponse> Handle(CancelItemCommand request, CancellationToken ct)
    {
        var sale = await _repo.GetAsync(request.SaleId, ct) ?? throw new KeyNotFoundException("Sale not found.");
        sale.CancelItem(request.ItemId);
        await _repo.UpdateAsync(sale, ct);
        await _events.PublishAsync("ItemCancelled", new { sale.Id, request.ItemId }, ct);
        return sale.ToResponse();
    }
}
