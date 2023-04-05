using System;
namespace Domain.Entities.ReceiptOfGoods
{
	public class Preparation
	{
		public Preparation()
		{
		}

        public Preparation(string mfrSerialNo, string details)
        {
            MfrSerialNo = mfrSerialNo;
            Details = details;
        }

        public string MfrSerialNo { get; set; }
		public string Details { get; set; }
	}
}

