namespace Domain.Services
{
    public interface IPackingListService
    {
        Task<string?> RemovePackingListAsync(long docEntry);
        Task<string?> ClosePackingListAsync(long docEntry);
        Task SendOrders(long docEntry);
        Task AddItemPackingListAsync(long packingListEntry, string keyNfe, bool resend);
        Task AddItemIntelipostAsync(string serial, bool resend);
        Task PrintDanfe(string ip, int amountOfBoxes, string docEntry, string keyNfe);
        Task RemoveItemPackingListAsync(long packingListEntry, string keyNfe);
        byte[]? GetPdfPackingList(long packingListEntry);
    }
}