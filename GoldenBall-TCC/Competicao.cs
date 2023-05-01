using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public static class Competicao
    {
        public static void Start(List<Dataset> datasets, int QuantidadeEquipes)
        {
            List<Cluster> Clusters = new List<Cluster>();

            foreach (Dataset dataset in datasets)
            {
                #region Gerar clusters
                double[,] dist = GerarMatrizDistancia(dataset);

                int qntDepo = dataset.QntDepositos;
                int clientPorCluster = dataset.QntClientes / dataset.QntDepositos;

                bool[] clienteDisponivelId = new bool[dataset.QntClientes];
                int[,] idClient = GerarIdClientClusters(dist, clienteDisponivelId, qntDepo, clientPorCluster);

                bool[] clienteDisponivelDistancia = new bool[dataset.QntClientes];
                double[,] distanciaClienteParaDeposito = GerarDistClusters(dist, clienteDisponivelDistancia, qntDepo, clientPorCluster);

                Clusters = GerarClusterData(idClient, distanciaClienteParaDeposito, dataset);

                #endregion

                #region Gerar Rota
                List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral = GerarMatrizAdjacencia(Clusters);

                Clusters = SetarDadosDoDepositoNoCluster(Clusters, dataset);

                GerarRotaInicial(Clusters, matrizAdjacenciaGeral);
                #endregion

                Utils.PrintarClusters(Clusters);
            }

        }

        public static List<Cluster> GerarRotaInicial(List<Cluster> clusters, List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral)
        {
            Random random = new Random();

            foreach (Cluster cluster in clusters)
            {
                int demandaAtualRota = 0;

                double distRotaAtual;

                List<Cliente> clientesVisitados = new List<Cliente>();

                Cliente cliente = new Cliente();

                while (clientesVisitados.Count != cluster.Clientes.Count)
                {
                    List<Tuple<Cliente, double>> vetorDistancia = new List<Tuple<Cliente, double>>();

                    int indiceCliente;
                    int indiceCluster;
                    int quantidadeClientes;

                    if (clientesVisitados.Count == 0)
                    {
                        cliente = cluster.Clientes[random.Next(cluster.Clientes.Count)];
                        cliente.Visitado = true;
                        clientesVisitados.Add(cliente);
                        demandaAtualRota += cliente.Demanda;
                        cluster.Rota.Caminho.Add(cliente.Id);
                        cluster.Rota.Distancia += cliente.DistanciaDeposito; // Calcula a distancia do deposito para o primeiro cliente.

                        indiceCluster = clusters.IndexOf(cluster);
                        indiceCliente = cluster.Clientes.IndexOf(cliente);
                        quantidadeClientes = cluster.Clientes.Count;

                        vetorDistancia = PegarVetorDistanciasClientes(indiceCluster, indiceCliente, quantidadeClientes, matrizAdjacenciaGeral);
                        vetorDistancia.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                        vetorDistancia.RemoveAt(0);
                        cliente = vetorDistancia[0].Item1;
                        cluster.Rota.Distancia += vetorDistancia[0].Item2; // Adiciona a distancia do novo cliente.
                    }
                    else
                    {
                        cliente.Visitado = true;
                        clientesVisitados.Add(cliente);
                        demandaAtualRota += cliente.Demanda;
                        cluster.Rota.Caminho.Add(cliente.Id);
                        indiceCluster = clusters.IndexOf(cluster);
                        indiceCliente = cluster.Clientes.IndexOf(cliente);
                        quantidadeClientes = cluster.Clientes.Count;
                        vetorDistancia = PegarVetorDistanciasClientes(indiceCluster, indiceCliente, quantidadeClientes, matrizAdjacenciaGeral);
                        vetorDistancia.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                        vetorDistancia.RemoveAt(0);
                        Cliente antigo = cliente;
                        cliente = vetorDistancia[0].Item1;

                        if (clientesVisitados.Contains(cliente))
                        {
                            PegarDistanciaClienteMaisProximo(clientesVisitados, cluster.Clientes, vetorDistancia);
                            cliente = vetorDistancia[0].Item1;
                        }

                        if (cluster.Capacidade < cliente.Demanda + demandaAtualRota) // Valida antes de ir pro novo cliente se a demanda do novo cliente somada com a demanda total é maior que o limite da rota.
                        {
                            cluster.Rota.Distancia += antigo.DistanciaDeposito; // Ida do cliente antigo ao deposito.
                            cluster.Rota.Caminho.Add(clusters.IndexOf(cluster));
                            demandaAtualRota = 0;
                            cluster.Rota.Distancia += cliente.DistanciaDeposito; // Volta do deposito ao cliente novo.
                        }
                        else
                        {
                            cluster.Rota.Distancia += vetorDistancia[0].Item2; // Adiciona a distancia do novo cliente.
                        }

                    }
                }

                cluster.Rota.Distancia += CalcularDistancia(cluster.Deposito.CoordenadaX, cliente.CoordenadaX, cluster.Deposito.CoordenadaY, cliente.CoordenadaY);
                cluster.Rota.Caminho.Add(clusters.IndexOf(cluster));

            }

            return clusters;
        }

        public static List<Tuple<Cliente, double>> PegarVetorDistanciasClientes(int cluster, int cliente, int quantidadeClientes, List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral)
        {
            List<Tuple<Cliente, double>> vetorDistancia = new List<Tuple<Cliente, double>>();

            for (int i = 0; i < quantidadeClientes - 1; i++)
            {
                vetorDistancia.Add(matrizAdjacenciaGeral[cluster][cliente][i]);
            }

            return vetorDistancia;
        }

        public static Cliente GetClienteById(List<Cliente> clientes, int id)
        {
            foreach (Cliente cliente in clientes)
            {
                if (cliente.Id == id)
                    return cliente;
            }
            return null;
        }

        public static List<Cluster> GerarClusterData(int[,] grupos, double[,] dist, Dataset dataset)
        {
            Random random = new Random();
            int deposito = random.Next(grupos.GetLength(0));

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
                        tupla = Tuple.Create(proxCliente, CalcularDistancia(clienteAtual.CoordenadaX, proxCliente.CoordenadaX, clienteAtual.CoordenadaY, proxCliente.CoordenadaY));
                        vetorAdjacencia.Add(tupla);
                    }

                    matrizAdjacenciaCluster.Add(vetorAdjacencia);
                }

                cuboAdjacencia.Add(matrizAdjacenciaCluster);
            }

            return cuboAdjacencia;
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

        public static int[,] GerarIdClientClusters(double[,] dist, bool[] clientDisp, int qntCluster, int ClientePorCluster)
        {
            double[,] copiaMatriz = (double[,])dist.Clone(); // Gambiara (Daria pra fazer um método só se eu retornasse uma tupla de matrizes)
            int[,] clusters = new int[qntCluster, ClientePorCluster];
            Tuple<double, int> menor = new(0, 0);

            for (int i = 0; i < qntCluster; i++)
            {
                for (int j = 0; j < ClientePorCluster; j++)
                {
                    menor = PegarMenorValor(i, copiaMatriz, clientDisp, qntCluster);

                    if (menor == null)
                    {
                        PegarMenorValor(i, copiaMatriz, clientDisp, qntCluster);
                    }
                    clusters[i, j] = menor.Item2;
                }
            }

            return clusters;
        }

        public static double[,] GerarDistClusters(double[,] dist, bool[] clientDisp, int qntCluster, int ClientePorCluster)
        {
            double[,] copiaMatriz = (double[,])dist.Clone(); // Gambiara (Daria pra fazer um método só se eu retornasse uma tupla de matrizes)
            double[,] clusters = new double[qntCluster, ClientePorCluster];
            Tuple<double, int> menor = new(0, 0);

            for (int i = 0; i < qntCluster; i++)
            {
                for (int j = 0; j < ClientePorCluster; j++)
                {
                    menor = PegarMenorValor(i, copiaMatriz, clientDisp, qntCluster);

                    if (menor == null)
                    {
                        PegarMenorValor(i, copiaMatriz, clientDisp, qntCluster);
                    }
                    clusters[i, j] = menor.Item1;
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

        public static double CalcularDistancia(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)); // Metodo para calcular distancia
        }

        public static double[,] GerarMatrizDistancia(Dataset dataset) // Gera a matriz distancia dos depositos para os clientes.
        {
            double[,] dist = new double[dataset.QntDepositos, dataset.QntClientes];
            int aux = 0;
            for (int i = dataset.QntClientes; i <= dataset.QntLocais - 1; i++)
            {
                for (int j = 0; j < dataset.QntClientes; j++)
                    dist[aux, j] = CalcularDistancia(dataset.CoordenadaX[j], dataset.CoordenadaX[i], dataset.CoordenadaY[j], dataset.CoordenadaY[i]);
                aux++;
            }

            return dist;
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
