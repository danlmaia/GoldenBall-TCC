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

        public static int[,] GerarIdClientClusters(double[,] dist, bool[] clientDisp, int qntCluster, int ClientePorCluster)
        {
            double[,] copiaMatriz = (double[,])dist.Clone(); // Gambiara (Daria pra fazer um método só se eu retornasse uma tupla de matrizes)
            int[,] clusters = new int[qntCluster, ClientePorCluster];
            Tuple<double, int> menor = new(0, 0);

            for (int i = 0; i < ClientePorCluster; i++)
            {
                for (int j = 0; j < qntCluster; j++)
                {
                    menor = PegarMenorValor(j, copiaMatriz, clientDisp, qntCluster);

                    if (menor == null)
                    {
                        PegarMenorValor(j, copiaMatriz, clientDisp, qntCluster);
                    }
                    clusters[j, i] = menor.Item2;
                }
            }

            return clusters;
        }

        // Pega a menor distancia de um cliente de uma linha de uma matriz, quando o valor é pego, o indice do valor fica indisponivel de se pegar em outras matrizes. 
        public static Tuple<double, int> PegarMenorValor(int linha, double[,] vetor, bool[] clientDisp, int qntCluster)
        {
            Tuple<double, int> menor = new Tuple<double, int>(double.PositiveInfinity, int.MaxValue);

            for (int i = 0; i < vetor.Length / qntCluster; i++)
            {
                if (vetor[linha, i] == 0)
                    continue;
                if (i == 0)
                    menor = Tuple.Create(vetor[linha, i], i);
                if (i > 0)
                {
                    if (menor.Item1 > vetor[linha, i])
                        menor = Tuple.Create(vetor[linha, i], i);
                }
            }

            if (!clientDisp[menor.Item2])
            {
                clientDisp[menor.Item2] = true;
                for (int i = 0; i < qntCluster; i++)
                    vetor[i, menor.Item2] = 0;
                return menor;
            }
            else
            {
                return null;
            }

        }


    }
}
