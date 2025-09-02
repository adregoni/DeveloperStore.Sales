using DeveloperStore.Sales.Domain;

namespace DeveloperStore.Sales.Application.Abstractions;

public interface ISaleRepository
{
    Task<Sale?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken ct = default);
    Task AddAsync(Sale sale, CancellationToken ct = default);
    Task UpdateAsync(Sale sale, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Sale>> ListAsync(DateTime? fromUtc, DateTime? toUtc, string? customerId, string? branchId, bool? cancelled, int skip, int take, CancellationToken ct = default);
}

public interface IEventPublisher
{
    Task PublishAsync(string eventName, object payload, CancellationToken ct = default);
}
