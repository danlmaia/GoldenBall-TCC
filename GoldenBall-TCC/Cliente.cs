using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Cliente
    {
        public int Id { get; set; }

        public double CoordenadaX { get; set; }

        public double CoordenadaY { get; set; }

        public int Demanda { get; set; }

        public double DistanciaDeposito { get; set; }

        public bool Visitado { get; set; }

        public int IdProximoCliente { get; set; }

        public double DistanciaProximoCliente { get; set; }
    }
}
