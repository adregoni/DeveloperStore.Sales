using MediatR;
using DeveloperStore.Sales.Application.Dtos;

namespace DeveloperStore.Sales.Application.Mediator;

public record CreateSaleCommand(CreateSaleRequest Request) : IRequest<SaleResponse>;
public record UpdateSaleCommand(Guid Id, UpdateSaleRequest Request) : IRequest<SaleResponse>;
public record CancelSaleCommand(Guid Id) : IRequest<Unit>;
public record GetSaleByIdQuery(Guid Id) : IRequest<SaleResponse?>;
public record ListSalesQuery(DateTime? FromUtc, DateTime? ToUtc, string? CustomerId, string? BranchId, bool? Cancelled, int Page, int PageSize) : IRequest<IReadOnlyList<SaleResponse>>;
public record AddItemCommand(Guid SaleId, AddItemRequest Request) : IRequest<SaleResponse>;
public record UpdateItemCommand(Guid SaleId, Guid ItemId, UpdateItemRequest Request) : IRequest<SaleResponse>;
public record CancelItemCommand(Guid SaleId, Guid ItemId) : IRequest<SaleResponse>;
