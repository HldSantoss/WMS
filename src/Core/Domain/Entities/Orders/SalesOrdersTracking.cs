using System;
using Domain.ValueObject;

namespace Domain.Entities.Orders
{
    public class SalesOrdersTracking : RangeDays
    {
        public SalesOrdersTracking(string status, int weight, int d0, int d1, int d2, int d3, int dx)
        {
            Status = status;
            Weight = weight;
            D0 = d0;
            D1 = d1;
            D2 = d2;
            D3 = d3;
            DX = dx;
        }

        public SalesOrdersTracking(string status, int d0, int d1, int d2, int d3, int dx)
        {
            Status = status;
            D0 = d0;
            D1 = d1;
            D2 = d2;
            D3 = d3;
            DX = dx;
        }

        public string Status { get; set; }
        public int Weight { get; set; }
    }

    public class SalesOrderDetails
    {
        public string NumAtCard { get; set; }

        public string DocEntry { get; set; }

        public DateTime DocDueDate { get; set; }

        public DateTime DocDate { get; set; }

        public string CardName { get; set; }

        public string PickingGroup { get; set; }

        public string User { get; set; }

        public string Carrier { get; set; }

    }

    public class SalesOrderStore
    {
        public SalesOrderStore(string store, int created, int canTax, int canPick, int picking, int canCheckout, int checkingout, int savePicking, int replenish, int canPacking, int packing)
        {
            Store = store;
            Created = created;
            CanTax = canTax;
            CanPick = canPick;
            Picking = picking;
            CanCheckout = canCheckout;
            Checkingout = checkingout;
            SavePicking = savePicking;
            Replenish = replenish;
            CanPacking = canPacking;
            Packing = packing;
        }

        public string Store { get; set; }
        public int Created { get; set; }
        public int CanTax { get; set; }
        public int CanPick { get; set; }
        public int Picking { get; set; }
        public int CanCheckout { get; set; }
        public int Checkingout { get; set; }
        public int SavePicking { get; set; }
        public int Replenish { get; set; }
        public int CanPacking { get; set; }
        public int Packing { get; set; }
    }

}

