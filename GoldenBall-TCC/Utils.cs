using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GoldenBall_TCC
{
    public class Utils
    {
        public static void PrintarClusters(int idDataset ,List<Time> times)
        {
            Console.WriteLine("************ DATASET: " + (idDataset + 1));
            Console.WriteLine("--------------------- ROTAS GERADAS -------------------------");

            foreach (Time time in times)
            {
                Console.WriteLine("Time: " + times.IndexOf(time));
                foreach (Cluster cluster in time.Jogadores)
                {
                    Console.WriteLine("Deposito: " + time.Jogadores.IndexOf(cluster));
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
                    return;
                }
            }
        }

        public static void PrintarSolucao(int idDataset, Time time, Stopwatch stopwatch, int quantidadeEquipe, int quantidadeTemporadas, int quantidadeIntraTreino, int quantidadeInterTreino)
        {
            HashSet<int> itensUnicos = new HashSet<int>();
            Console.WriteLine("--------------------- ROTAS GERADAS -------------------------");

            foreach (Cluster cluster in time.Jogadores)
            {
                Console.WriteLine("Deposito: " + time.Jogadores.IndexOf(cluster));
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
                    itensUnicos.Add(cliente);
                    Console.WriteLine("clientes visitados: " + cliente);
                }
            }

            Console.WriteLine("-------------------------");
            Console.WriteLine("Valor da solução: " + time.Valor);
            Console.WriteLine("quantidade de clientes: " + itensUnicos.Count);
            SalvarSolucao(idDataset, time, stopwatch, quantidadeEquipe, quantidadeTemporadas, quantidadeIntraTreino, quantidadeInterTreino);

        }

        public static void SalvarSolucao(int idDataset,Time time, Stopwatch stopwatch, int quantidadeEquipe, int quantidadeTemporadas, int quantidadeIntraTreino, int quantidadeInterTreino)
        {
            string nomeArquivo = string.Format("C:\\TCC\\Resultados\\Rota inicial pela distancia\\Verificando inter treino\\E{0}T{1}T{2}T{3}\\p0{4}.txt", quantidadeEquipe,
                    quantidadeTemporadas, quantidadeIntraTreino, quantidadeInterTreino, idDataset + 1);
            if(idDataset >= 9)
                nomeArquivo = string.Format("C:\\TCC\\Resultados\\Rota inicial pela distancia\\Verificando inter treino\\E{0}T{1}T{2}T{3}\\p{4}.txt", quantidadeEquipe,
                    quantidadeTemporadas, quantidadeIntraTreino, quantidadeInterTreino, idDataset + 1);


            using (StreamWriter writer = new StreamWriter(nomeArquivo))
            {
                foreach (Cluster cluster in time.Jogadores)
                {
                    for (int i = 0; i < cluster.Rota.Caminho.Count; i++)
                    {
                        writer.Write(cluster.Rota.Caminho[i]);

                        // Adicione um espaço após cada item, exceto para o último item
                        if (i < cluster.Rota.Caminho.Count)
                        {
                            writer.Write(" ");
                        }
                    }
                }
                writer.WriteLine();
                writer.WriteLine("Valor da solucao: " + time.Valor);
                stopwatch.Stop();
                writer.WriteLine("Tempo de execução: " + stopwatch.Elapsed.TotalSeconds);
            }

        }





        public static Tuple<Cliente, double> PegarDistanciaClienteMaisProximo(List<Cliente> clientesVisitados, List<Cliente> clientes, List<Tuple<Cliente, double>> vetorAdjacencia)
        {
            double menor = 99999;

            foreach (var tuple in vetorAdjacencia)
            {
                if (tuple.Item2 == 0)
                    continue;
                if (menor > tuple.Item2)
                    menor = tuple.Item2;
            }
            if (vetorAdjacencia.Count == 1)
                return vetorAdjacencia[0];
            Tuple<Cliente, double> proximoCliente = vetorAdjacencia[0];

            if (clientesVisitados.Contains(proximoCliente.Item1))
            {
                vetorAdjacencia.RemoveAt(0);
                proximoCliente = PegarDistanciaClienteMaisProximo(clientesVisitados, clientes, vetorAdjacencia);
            }

            return proximoCliente;
        }

        public static List<Tuple<Cliente, double>> PegarVetorDistanciasClientes(int cluster, int cliente, int quantidadeClientes, List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral)
        {
            List<Tuple<Cliente, double>> vetorDistancia = new List<Tuple<Cliente, double>>();

            for (int i = 0; i < quantidadeClientes; i++)
            {
                vetorDistancia.Add(matrizAdjacenciaGeral[cluster][cliente][i]);
            }

            return vetorDistancia;
        }

        public static double CalcularDistancia(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)); // Metodo para calcular distancia
        }

        public static List<List<List<Tuple<Cliente, double>>>> GerarMatrizAdjacencia(List<Cluster> clusters)
        {
            List<List<List<Tuple<Cliente, double>>>> cuboAdjacencia = new List<List<List<Tuple<Cliente, double>>>>(); // LOUCURAAAAAA

            Random random = new Random();

            foreach (Cluster cluster in clusters)
            {
                List<List<Tuple<Cliente, double>>> matrizAdjacenciaCluster = new List<List<Tuple<Cliente, double>>>();

                foreach (Cliente clienteAtual in cluster.Clientes)
                {
                    List<Tuple<Cliente, double>> vetorAdjacencia = new List<Tuple<Cliente, double>>();

                    foreach (Cliente proxCliente in cluster.Clientes)
                    {
                        Tuple<Cliente, double> tupla;
                        tupla = Tuple.Create(proxCliente, Utils.CalcularDistancia(clienteAtual.CoordenadaX, proxCliente.CoordenadaX, clienteAtual.CoordenadaY, proxCliente.CoordenadaY));
                        vetorAdjacencia.Add(tupla);
                    }

                    matrizAdjacenciaCluster.Add(vetorAdjacencia);
                }

                cuboAdjacencia.Add(matrizAdjacenciaCluster);
            }

            return cuboAdjacencia;
        }

        public static double[,] GerarMatrizDistancia(Dataset dataset) // Gera a matriz distancia dos depositos para os clientes.
        {
            double[,] dist = new double[dataset.QntDepositos, dataset.QntClientes];
            int aux = 0;
            for (int i = dataset.QntClientes; i <= dataset.QntLocais - 1; i++)
            {
                for (int j = 0; j < dataset.QntClientes; j++)
                    dist[aux, j] = Utils.CalcularDistancia(dataset.CoordenadaX[j], dataset.CoordenadaX[i], dataset.CoordenadaY[j], dataset.CoordenadaY[i]);
                aux++;
            }

            return dist;
        }

        public static double[,] GerarDistClusters(double[,] dist, bool[] clientDisp, int qntCluster, int ClientePorCluster)
        {
            double[,] copiaMatriz = (double[,])dist.Clone(); // Gambiara (Daria pra fazer um método só se eu retornasse uma tupla de matrizes)
            double[,] clusters = new double[qntCluster, ClientePorCluster];
            Tuple<double, int> menor = new(0, 0);

            for (int i = 0; i < ClientePorCluster; i++)
            {
                for (int j = 0; j < qntCluster; j++)
                {
                    menor = Cliente.PegarMenorValor(j, copiaMatriz, clientDisp, qntCluster);

                    if (menor == null)
                    {
                        Cliente.PegarMenorValor(j, copiaMatriz, clientDisp, qntCluster);
                    }
                    clusters[j, i] = menor.Item1;
                }
            }

            return clusters;
        }

    }
}
