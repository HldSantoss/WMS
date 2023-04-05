namespace Api.ViewModel
{
    public record InventoryViewModel
    {
        public InventoryViewModel(string distNumber, string itemCode, string itemName, string binCode, double onHandQty, int rtrictType)
        {
            DistNumber = distNumber;
            ItemCode = itemCode;
            ItemName = itemName;
            BinCode = binCode;
            OnHandQty = onHandQty;
            RtrictType = rtrictType;
        }

        public string DistNumber { get; set; }
        public string ItemCode { get; init; }
        public string ItemName { get; init; }
        public string BinCode { get; init; }
        public double OnHandQty { get; init; }
        public int RtrictType { get; init; }
    }
}