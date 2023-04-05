namespace Domain.Enums
{
    public enum RtrictType
    {
        None = 0, 
        AllTransactions = 1, 
        InboundTransactions = 2, 
        OutboundTransactions = 3, 
        AllExceptInventoryTransferAndCountingTransactions = 4
    }
}