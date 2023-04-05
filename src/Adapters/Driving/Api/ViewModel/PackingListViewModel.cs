using Domain.Entities;
using System.Text.Json.Serialization;

namespace Api.ViewModel
{
    public class PackingListViewModel
    {
        
        public PackingListViewModel()
        {
        }

        public PackingListViewModel(long docNum, int period, int instance, int series, string handwrtten, string status, string requestStatus, string creator, object remark, int docEntry, string canceled, string @object, object logInst, int userSign, string transfered, DateTime createDate, TimeSpan createTime, DateTime updateDate, TimeSpan updateTime, DateTime? closedDateAt, TimeSpan? closedTimeAt, DateTime? exportedDateAt, DateTime? exportedTimeAt, string carrierId, string method, int branch, List<PackingListItem>? items)
        {
            DocNum = docNum;
            Period = period;
            Instance = instance;
            Series = series;
            Handwrtten = handwrtten;
            Status = status;
            RequestStatus = requestStatus;
            Creator = creator;
            Remark = remark;
            DocEntry = docEntry;
            Canceled = canceled;
            Object = @object;
            LogInst = logInst;
            UserSign = userSign;
            Transfered = transfered;
            CreateDate = createDate;
            CreateTime = createTime;
            UpdateDate = updateDate;
            UpdateTime = updateTime;
            ClosedDateAt = closedDateAt;
            ClosedTimeAt = closedTimeAt;
            ExportedDateAt = exportedDateAt;
            ExportedTimeAt = exportedTimeAt;
            CarrierId = carrierId;
            Method = method;
            Branch = branch;
            Items = items;
        }

        public long DocNum { get; set; }
        public int Period { get; set; }
        public int Instance { get; set; }
        public int Series { get; set; }
        public string Handwrtten { get; set; }
        public string Status { get; set; }
        public string RequestStatus { get; set; }
        public string Creator { get; set; }
        public object Remark { get; set; }
        public int DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int UserSign { get; set; }
        public string Transfered { get; set; }
        public DateTime CreateDate { get; set; }
        public TimeSpan CreateTime { get; set; }
        public DateTime UpdateDate { get; set; }
        public TimeSpan UpdateTime { get; set; }
        public DateTime? ClosedDateAt { get; set; }
        public TimeSpan? ClosedTimeAt { get; set; }
        public DateTime? ExportedDateAt { get; set; }
        public DateTime? ExportedTimeAt { get; set; }
        public string CarrierId { get; set; }
        public string Method { get; set; }
        public int Branch { get; set; }
        public List<PackingListItem>? Items { get; set; }
    }

    public class PackingListItemViewModel
    {
        public PackingListItemViewModel(long lineNum, DateTime createdAt, string receiver, long invoiceEntry, long orderEntry, double weight)
        {
            LineNum = lineNum;
            CreatedAt = createdAt;
            Receiver = receiver;
            InvoiceEntry = invoiceEntry;
            OrderEntry = orderEntry;
            Weight = weight;
        }

        public PackingListItemViewModel()
        {
          
        }

        public long LineNum { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Receiver { get; set; }
        public long InvoiceEntry { get; set; }
        public long OrderEntry { get; set; }
        public double Weight { get; set; }
    }
}