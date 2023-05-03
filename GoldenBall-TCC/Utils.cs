using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Utils
    {
        public static void PrintarClusters(int idDataset ,List<Cluster> clusters)
        {
            Console.WriteLine("************ DATASET: " + (idDataset + 1));
            Console.WriteLine("--------------------- ROTAS GERADAS -------------------------");


            foreach (Cluster cluster in clusters)
            {
                Console.WriteLine("Deposito: " + clusters.IndexOf(cluster));
                Console.WriteLine("Demanda deposito: " + cluster.Capacidade);
                int demanda = 0;
                foreach (var cliente in cluster.Clientes)
                {
                    demanda += cliente.Demanda;
                }
                Console.WriteLine("Demanda total da rota: " + demanda);
                Console.WriteLine("Distancia da rota: " + Math.Round(cluster.Rota.Distancia, 2));
                foreach (int cliente in cluster.Rota.Caminho)
                {
                    Console.WriteLine("clientes visitados: " + cliente);
                }
                Console.WriteLine("-------------------------");
            }

        }
        public static double CalcularDistancia(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)); // Metodo para calcular distancia
        }

    }
}
