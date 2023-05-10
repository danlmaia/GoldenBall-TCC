using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GoldenBall_TCC
{
    public class Cluster
    {
        public int Id { get; set; }

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

        public static List<Cluster> GerarClusterData(int[,] grupos, double[,] dist, Dataset dataset)
        {

            List<Cluster> Clusters = new List<Cluster>();
            for (int i = 0; i < grupos.GetLength(0); i++)
            {
                int[] grupo = new int[grupos.GetLength(1)];
                double[] distGrupo = new double[grupos.GetLength(1)];
                for (int j = 0; j < grupos.GetLength(1); j++)
                {
                    grupo[j] = grupos[i, j]; // separando o vetor de Id de cliente.
                    distGrupo[j] = dist[i, j]; // separando o vetor de distancia depo-clientes.
                }

                Clusters.Add(GetClienteDataByCluster(grupo, distGrupo, dataset)); // Adiciona na lista de cluster os dados do datasets nos clientes que foram separados no vetor. 
            }

            return Clusters;
        }

        public static Cluster GetClienteDataByCluster(int[] grupo, double[] dist, Dataset dataset)
        {
            Cluster Cluster = new Cluster();

            for (int i = 0; i < grupo.Length; i++)
            {
                Cliente cliente = new Cliente();
                cliente.Id = dataset.Id[grupo[i]];
                cliente.CoordenadaX = dataset.CoordenadaX[grupo[i]];
                cliente.CoordenadaY = dataset.CoordenadaY[grupo[i]];
                cliente.Demanda = dataset.Demanda[grupo[i]];
                cliente.DistanciaDeposito = dist[i]; // Criando e setando os clientes do cluster.

                Cluster.Clientes.Add(cliente);
            }
            Cluster.Capacidade = dataset.CapacidadeDeposito;
            return Cluster;
        }

        // Pegar dados dos depositos (verificar a possibilidade de setar já durante o mapeamento do dataset).
        public static List<Cluster> SetarDadosDoDepositoNoCluster(List<Cluster> clusters, Dataset dataset)
        {
            int i = dataset.QntClientes;
            foreach (Cluster cluster in clusters)
            {
                cluster.Deposito.Id = dataset.Id[i];
                cluster.Deposito.CoordenadaX = dataset.CoordenadaX[i];
                cluster.Deposito.CoordenadaY = dataset.CoordenadaY[i];
                i++;
            }

            return clusters;
        }



    }

}
