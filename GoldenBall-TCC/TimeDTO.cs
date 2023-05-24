using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class TimeDTO
    {
        public List<Cluster> Jogadores { get; set; }

        public Cluster Capitao { get; set; }

        public Cluster PiorJogador { get; set; }

        public double Valor { get; set; }

        public int Pontuacao { get; set; }
    }
}
