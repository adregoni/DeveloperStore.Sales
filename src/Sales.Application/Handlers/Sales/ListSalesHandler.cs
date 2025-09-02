using DeveloperStore.Sales.Application;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class ListSalesHandler : IRequestHandler<ListSalesQuery, IReadOnlyList<SaleResponse>>
{
    private readonly ISaleRepository _repo;
    public ListSalesHandler(ISaleRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SaleResponse>> Handle(ListSalesQuery request, CancellationToken ct)
    {
        var skip = Math.Max(0, (request.Page - 1) * request.PageSize);
        var take = Math.Clamp(request.PageSize, 1, 200);
        var list = await _repo.ListAsync(request.FromUtc, request.ToUtc, request.CustomerId, request.BranchId, request.Cancelled, skip, take, ct);
        return list.Select(s => s.ToResponse()).ToList();
    }
}
