using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StockByItem
    {
        public string odatametadata { get; set; }
        public string SqlText { get; set; }
        public List<StockReal> value { get; set; }
    }

    public class StockReal
    {
        public double OnHand { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
    }
}
