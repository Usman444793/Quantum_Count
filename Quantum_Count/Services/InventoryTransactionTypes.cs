namespace Quantum_Count.Services;
public static class InventoryTransactionTypes
{
    public const string StockIn = "Stock In";
    public const string StockOut = "Stock Out";
    public const string Adjustment = "Adjustment";
    public const string Return = "Return";
    public const string Damaged = "Damaged";

    public static readonly string[] All =
    {
        StockIn,
        StockOut,
        Adjustment,
        Return,
        Damaged
    };
}