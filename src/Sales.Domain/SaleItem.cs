namespace DeveloperStore.Sales.Domain;

public class SaleItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ProductId { get; private set; } = default!;
    public string ProductName { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsCancelled { get; private set; }
    public decimal Total => (UnitPrice * Quantity) - Discount;

    private SaleItem() { }

    public SaleItem(string productId, string productName, int quantity, decimal unitPrice, decimal discount)
    {
        ProductId = productId;
        ProductName = productName;
        Update(quantity, unitPrice, discount);
    }

    public void Update(int quantity, decimal unitPrice, decimal discount)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (quantity > 20) throw new DomainException("Quantity cannot be greater than 20.");
        if (unitPrice <= 0) throw new DomainException("Unit price must be greater than zero.");
        if (IsCancelled) throw new DomainException("Item is cancelled and cannot be modified.");
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
    }

    public void Cancel() => IsCancelled = true;
}
