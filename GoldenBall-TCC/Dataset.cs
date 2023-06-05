using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Dataset
    {
        public Dataset() 
        {
            Depositos = new List<Deposito>();
        }

        public int[]? Id { get; set; }

        public double[]? CoordenadaY { get; set; }

        public double[]? CoordenadaX { get; set; }

        public double[]? TempoServico { get; set; }

        public int[]? Demanda { get; set; }

        public int QntVeiculos { get; set; }

        public int QntClientes { get; set; }

        public int QntDepositos { get; set; }

        public int QntLocais { get; set; }

        public int CapacidadeDeposito { get; set; }

        public int TamanhoDaRota { get; set; }

        public List<Deposito> Depositos { get; set; }
    }
}