using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Rota
    {
        public List<int> Caminho { get; set; }

        public double Distancia { get; set; }

        public Rota() 
        {
            Caminho = new List<int>();
        }

    }
}
