using System.Net;
using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Application.Dtos;
using DeveloperStore.Sales.Application.Mediator;
using DeveloperStore.Sales.Application.Validation;
using DeveloperStore.Sales.Domain;
using DeveloperStore.Sales.Infrastructure;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddValidatorsFromAssemblyContaining<CreateSaleValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddSingleton<IDiscountPolicy, DiscountPolicy>();
builder.Services.AddSingleton<ISaleRepository, InMemorySaleRepository>();
builder.Services.AddLogging();
builder.Services.AddSingleton<IEventPublisher, LoggerEventPublisher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException ex)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await ctx.Response.WriteAsJsonAsync(new
        {
            title = "Validation failed",
            errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        });
    }
    catch (DomainException ex)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
        await ctx.Response.WriteAsJsonAsync(new { title = "Business rule violation", detail = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
        await ctx.Response.WriteAsJsonAsync(new { title = "Not Found", detail = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.Conflict;
        await ctx.Response.WriteAsJsonAsync(new { title = "Conflict", detail = ex.Message });
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new { title = "Unexpected error", detail = ex.Message });
    }
});

app.MapSalesEndpoints();
app.Run();

public static class SalesEndpoints
{
    public static void MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        var sales = app.MapGroup("/sales").WithTags("Sales");

        sales.MapPost("/", async (IMediator mediator, CreateSaleRequest req, CancellationToken ct) =>
        {
            var res = await mediator.Send(new CreateSaleCommand(req), ct);
            return Results.Created($"/sales/{res.Id}", res);
        })
        .WithName("CreateSale")
        .Produces<SaleResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        sales.MapGet("/", async (IMediator mediator, DateTime? fromUtc, DateTime? toUtc, string? customerId, string? branchId, bool? cancelled, int page = 1, int pageSize = 50, CancellationToken ct = default) =>
        {
            var res = await mediator.Send(new ListSalesQuery(fromUtc, toUtc, customerId, branchId, cancelled, page, pageSize), ct);
            return Results.Ok(res);
        })
        .WithName("ListSales")
        .Produces<List<SaleResponse>>();

        sales.MapGet("/{id:guid}", async (IMediator mediator, Guid id, CancellationToken ct) =>
        {
            var res = await mediator.Send(new GetSaleByIdQuery(id), ct);
            return res is null ? Results.NotFound() : Results.Ok(res);
        })
        .WithName("GetSaleById")
        .Produces<SaleResponse>()
        .Produces(StatusCodes.Status404NotFound);

        sales.MapPut("/{id:guid}", async (IMediator mediator, Guid id, UpdateSaleRequest req, CancellationToken ct) =>
        {
            var res = await mediator.Send(new UpdateSaleCommand(id, req), ct);
            return Results.Ok(res);
        })
        .WithName("UpdateSale")
        .Produces<SaleResponse>()
        .Produces(StatusCodes.Status409Conflict);

        sales.MapDelete("/{id:guid}", async (IMediator mediator, Guid id, CancellationToken ct) =>
        {
            await mediator.Send(new CancelSaleCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("CancelSale")
        .Produces(StatusCodes.Status204NoContent);

        sales.MapPost("/{id:guid}/items", async (IMediator mediator, Guid id, AddItemRequest req, CancellationToken ct) =>
        {
            var res = await mediator.Send(new AddItemCommand(id, req), ct);
            return Results.Ok(res);
        })
        .WithName("AddItem")
        .Produces<SaleResponse>();

        sales.MapPut("/{id:guid}/items/{itemId:guid}", async (IMediator mediator, Guid id, Guid itemId, UpdateItemRequest req, CancellationToken ct) =>
        {
            var res = await mediator.Send(new UpdateItemCommand(id, itemId, req), ct);
            return Results.Ok(res);
        })
        .WithName("UpdateItem")
        .Produces<SaleResponse>();

        sales.MapDelete("/{id:guid}/items/{itemId:guid}", async (IMediator mediator, Guid id, Guid itemId, CancellationToken ct) =>
        {
            var res = await mediator.Send(new CancelItemCommand(id, itemId), ct);
            return Results.Ok(res);
        })
        .WithName("CancelItem")
        .Produces<SaleResponse>();
    }
}
