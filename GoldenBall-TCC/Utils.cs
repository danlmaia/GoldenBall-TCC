using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Utils
    {
        public static List<double[]> ConverterMatrizInListVector(double[,] clusters)
        {
            List<double[]> clustersSeparados = new List<double[]>();

            for (int i = 0; i < clusters.GetLength(0); i++)
            {
                double[] vetor = new double[clusters.GetLength(1)];
                for (int j = 0; j < clusters.GetLength(1); j++)
                {
                    vetor[j] = clusters[i, j];
                }
                clustersSeparados.Add(vetor);
            }

            foreach (double[] vetor in clustersSeparados)
            {
                foreach (double numero in vetor)
                {
                    Console.Write(numero + " ");
                }
                Console.WriteLine();
            }

            return clustersSeparados;
        }

        public static void PrintarClusters(List<Cluster> clusters)
        {
            Console.WriteLine("--------------------- ROTAS GERADAS -------------------------");

            foreach (Cluster cluster in clusters)
            {
                Console.WriteLine("Deposito: " + clusters.IndexOf(cluster));
                foreach (Cliente cliente in cluster.Clientes)
                {
                    Console.WriteLine("Id cliente: " + cliente.Id);
                    //Console.WriteLine("Coordernada X: " + cliente.CoordenadaX);
                    //Console.WriteLine("Coordenada Y: " + cliente.CoordenadaY);
                    //Console.WriteLine("Demanda: " + cliente.Demanda);
                    //Console.WriteLine("Distancia do deposito: " + cliente.DistanciaDeposito);
                    Console.WriteLine("Proximo cliente: " + cliente.IdProximoCliente);
                }
                Console.WriteLine("");

                //Console.WriteLine("Distancia da rota: " + cluster.Rota.Distancia);
                //foreach (int cliente in cluster.Rota.Caminho)
                //{
                //    Console.WriteLine("clientes visitados: " + cliente);
                //}
                //Console.WriteLine("-------------------------");
            }

        }
        public static double CalcularDistancia(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)); // Metodo para calcular distancia
        }

    }
}
