using DeveloperStore.Sales.Application.Abstractions;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, Unit>
{
    private readonly ISaleRepository _repo;
    private readonly IEventPublisher _events;

    public CancelSaleHandler(ISaleRepository repo, IEventPublisher events)
    {
        _repo = repo;
        _events = events;
    }

    public async Task<Unit> Handle(CancelSaleCommand request, CancellationToken ct)
    {
        var sale = await _repo.GetAsync(request.Id, ct) ?? throw new KeyNotFoundException("Sale not found.");
        sale.Cancel();
        await _repo.UpdateAsync(sale, ct);
        await _events.PublishAsync("SaleCancelled", new { sale.Id, sale.SaleNumber }, ct);
        return Unit.Value;
    }
}
