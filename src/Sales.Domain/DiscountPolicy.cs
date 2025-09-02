namespace DeveloperStore.Sales.Domain;

public interface IDiscountPolicy
{
    decimal Calculate(int quantity, decimal unitPrice);
}

public class DiscountPolicy : IDiscountPolicy
{
    public decimal Calculate(int quantity, decimal unitPrice)
    {
        EnsureQuantityAllowed(quantity);
        if (quantity < 4) return 0m;
        var subtotal = unitPrice * quantity;
        if (quantity >= 4 && quantity < 10) return subtotal * 0.10m;
        return subtotal * 0.20m; // 10-20 inclusive
    }

    public static void EnsureQuantityAllowed(int quantity)
    {
        if (quantity > 20) throw new DomainException("It's not possible to sell above 20 identical items.");
    }
}
