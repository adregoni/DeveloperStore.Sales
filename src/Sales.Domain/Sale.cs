using System.Collections.ObjectModel;

namespace DeveloperStore.Sales.Domain;

public class Sale
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string SaleNumber { get; private set; } = default!;
    public DateTime DateUtc { get; private set; }
    public string CustomerId { get; private set; } = default!;
    public string CustomerName { get; private set; } = default!;
    public string BranchId { get; private set; } = default!;
    public string BranchName { get; private set; } = default!;
    public bool IsCancelled { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => new ReadOnlyCollection<SaleItem>(_items);

    public decimal TotalAmount => _items.Where(i => !i.IsCancelled).Sum(i => i.Total);

    private Sale() { }

    public Sale(string saleNumber, DateTime dateUtc, string customerId, string customerName, string branchId, string branchName)
    {
        SaleNumber = saleNumber;
        DateUtc = dateUtc;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
    }

    public SaleItem AddItem(string productId, string productName, int quantity, decimal unitPrice, IDiscountPolicy discountPolicy)
    {
        EnsureNotCancelled();
        DiscountPolicy.EnsureQuantityAllowed(quantity);
        var discount = discountPolicy.Calculate(quantity, unitPrice);
        var item = new SaleItem(productId, productName, quantity, unitPrice, discount);
        _items.Add(item);
        return item;
    }

    public void UpdateItem(Guid itemId, int quantity, decimal unitPrice, IDiscountPolicy discountPolicy)
    {
        EnsureNotCancelled();
        var item = _items.FirstOrDefault(i => i.Id == itemId) ?? throw new DomainException("Item not found.");
        DiscountPolicy.EnsureQuantityAllowed(quantity);
        var discount = discountPolicy.Calculate(quantity, unitPrice);
        item.Update(quantity, unitPrice, discount);
    }

    public void CancelItem(Guid itemId)
    {
        EnsureNotCancelled();
        var item = _items.FirstOrDefault(i => i.Id == itemId) ?? throw new DomainException("Item not found.");
        if (item.IsCancelled) return;
        item.Cancel();
    }

    public void Cancel()
    {
        if (IsCancelled) return;
        IsCancelled = true;
    }

    private void EnsureNotCancelled()
    {
        if (IsCancelled) throw new DomainException("Sale is cancelled and cannot be modified.");
    }
}
