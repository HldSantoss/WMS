using System;
namespace Domain.Entities.Orders
{
    public class KeyAccess
    {
        public KeyAccess(List<Value> value)
        {
            this.value = value;
        }

        public KeyAccess()
        {
        }

        public List<Value> value { get; set; }
    }

    public class Value
    {
        public Value(string u_ChaveAcesso)
        {
            U_ChaveAcesso = u_ChaveAcesso;
        }

        public Value()
        {
            
        }

        public string U_ChaveAcesso { get; set; }
    }


}

