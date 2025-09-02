using DeveloperStore.Sales.Application.Abstractions;
using DeveloperStore.Sales.Domain;

namespace DeveloperStore.Sales.Infrastructure;

public class InMemorySaleRepository : ISaleRepository
{
    private readonly List<Sale> _store = new();
    private readonly object _lock = new();

    public Task AddAsync(Sale sale, CancellationToken ct = default)
    {
        lock (_lock) _store.Add(sale);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var s = _store.FirstOrDefault(x => x.Id == id);
            if (s is not null) _store.Remove(s);
        }
        return Task.CompletedTask;
    }

    public Task<Sale?> GetAsync(Guid id, CancellationToken ct = default)
    {
        lock (_lock) return Task.FromResult(_store.FirstOrDefault(x => x.Id == id));
    }

    public Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken ct = default)
    {
        lock (_lock) return Task.FromResult(_store.FirstOrDefault(x => x.SaleNumber == saleNumber));
    }

    public Task<IReadOnlyList<Sale>> ListAsync(DateTime? fromUtc, DateTime? toUtc, string? customerId, string? branchId, bool? cancelled, int skip, int take, CancellationToken ct = default)
    {
        lock (_lock)
        {
            IEnumerable<Sale> q = _store.AsEnumerable();
            if (fromUtc.HasValue) q = q.Where(s => s.DateUtc >= fromUtc.Value);
            if (toUtc.HasValue) q = q.Where(s => s.DateUtc <= toUtc.Value);
            if (!string.IsNullOrWhiteSpace(customerId)) q = q.Where(s => s.CustomerId == customerId);
            if (!string.IsNullOrWhiteSpace(branchId)) q = q.Where(s => s.BranchId == branchId);
            if (cancelled.HasValue) q = q.Where(s => s.IsCancelled == cancelled.Value);

            return Task.FromResult<IReadOnlyList<Sale>>(q.Skip(skip).Take(take).ToList());
        }
    }

    public Task UpdateAsync(Sale sale, CancellationToken ct = default)
    {
        // in-memory ref; nothing to do
        return Task.CompletedTask;
    }
}
