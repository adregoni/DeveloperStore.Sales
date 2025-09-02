using DeveloperStore.Sales.Application;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using MediatR;

namespace DeveloperStore.Sales.Application.Mediator;

public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, SaleResponse?>
{
    private readonly ISaleRepository _repo;
    public GetSaleByIdHandler(ISaleRepository repo) => _repo = repo;

    public async Task<SaleResponse?> Handle(GetSaleByIdQuery request, CancellationToken ct)
    {
        var sale = await _repo.GetAsync(request.Id, ct);
        return sale?.ToResponse();
    }
}
