using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Cluster
    {
        public List<Cliente> Clientes { get; set; }

        public Deposito Deposito { get; set; }

        public Rota Rota { get; set; }

        public int Capacidade { get; set; }

        public Cluster()
        {
            Clientes = new List<Cliente>();
            Deposito = new Deposito();
            Rota = new Rota();
        }

    }

}
