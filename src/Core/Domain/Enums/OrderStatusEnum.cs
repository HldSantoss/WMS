namespace Domain.Enums
{
    public enum OrderStatusEnum
    {
        Created,
        CanPick,
        Picking,
        SavePicking,
        CanCheckout,
        Checkingout,
        CanPacking,
        Packing,
        Shipped,
        Replenish
    }
}