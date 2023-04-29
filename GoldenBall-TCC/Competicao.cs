using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public static class Competicao
    {
        public static void Start(List<Dataset> datasets, int QuantidadeEquipes)
        {
            List<Cluster> Clusters = new List<Cluster>();

            double[,] dist = GerarMatrizDistancia(datasets[1]);

            int qntDepo = datasets[1].QntDepositos;
            int clientPorCluster = datasets[1].QntClientes / datasets[1].QntDepositos;
            bool[] clienteDisponivelId = new bool[datasets[1].QntClientes];

            int[,] idClient = GerarIdClientClusters(dist, clienteDisponivelId, qntDepo, clientPorCluster);
            bool[] clienteDisponivelDistancia = new bool[datasets[1].QntClientes];

            double[,] distanciaClienteParaDeposito = GerarDistClusters(dist, clienteDisponivelDistancia, qntDepo, clientPorCluster);

            Clusters = GerarClusterData(idClient, distanciaClienteParaDeposito, datasets[1]);

            List<List<List<Tuple<int, double>>>> matrizAdjacenciaGeral = GerarMatrizAdjacencia(Clusters);

            Clusters = SetarDadosDoDepositoNoCluster(Clusters, datasets[1]);

            //Utils.PrintarClusters(Clusters);

            //GerarRotaInicial(Clusters, matrizAdjacenciaGeral);
            Utils.PrintarClusters(Clusters);
        }

        public static List<Cluster> GerarRotaInicial(List<Cluster> clusters, List<List<List<Tuple<int, double>>>> matrizAdjacenciaGeral)
        {
            Random random = new Random();

            foreach (Cluster cluster in clusters)
            {
                int demandaAtualRota;
                double distRotaAtual;

                Cliente primeiroCliente = new Cliente();
                primeiroCliente = cluster.Clientes[random.Next(cluster.Clientes.Count)];

                primeiroCliente.Visitado = true;
                demandaAtualRota = primeiroCliente.Demanda;
                Console.WriteLine("Cliente adicionado: " + primeiroCliente.Id);
                cluster.Rota.Caminho.Add(primeiroCliente.Id);
                cluster.Rota.Distancia = primeiroCliente.DistanciaDeposito;

                List<Cliente> copia = new List<Cliente>(cluster.Clientes);
                Cliente cliente = GetClienteById(copia, primeiroCliente.IdProximoCliente);
                copia.Remove(primeiroCliente);

                int contador = 1;

                while (true)
                {
                    if (copia.Count == 0)
                        break;
                    if (cliente.Visitado)
                    {
                        Cliente clientAnterior = cliente;
                        cliente = GetClienteById(copia, cliente.IdProximoCliente);
                        copia.Remove(clientAnterior);

                    }

                    if (cluster.Capacidade < demandaAtualRota)
                    {
                        cluster.Rota.Distancia += CalcularDistancia(cluster.Deposito.CoordenadaX, cliente.CoordenadaX, cluster.Deposito.CoordenadaY, cliente.CoordenadaY);
                        cluster.Rota.Caminho.Add(cluster.Deposito.Id);
                        demandaAtualRota = 0;
                    }
                    Console.WriteLine("Cliente adicionado: " + cliente.Id);
                    demandaAtualRota = cliente.Demanda;
                    cluster.Rota.Caminho.Add(cliente.Id);
                    cliente.Visitado = true;

                    cluster.Rota.Distancia += cliente.DistanciaProximoCliente;

                    cliente = GetClienteById(copia, cliente.IdProximoCliente);
                    contador++;
                }

                //foreach (Cliente cliente in cluster.Clientes)
                //{

                //    if (cliente.Visitado)
                //        continue;
                //    if (cluster.Capacidade < demandaAtualRota)
                //    {
                //        cluster.Rota.Distancia += CalcularDistancia(cluster.Deposito.CoordenadaX, cliente.CoordenadaX, cluster.Deposito.CoordenadaY, cliente.CoordenadaY);
                //        cluster.Rota.Caminho.Add(cluster.Deposito.Id);
                //        demandaAtualRota = 0;
                //    }
                //    //Console.WriteLine("Cliente adicionado: " + cliente.Id);
                //    demandaAtualRota = cliente.Demanda;
                //    cluster.Rota.Caminho.Add(cliente.Id);

                //    cluster.Rota.Distancia += cliente.DistanciaProximoCliente;
                //}

            }

            return clusters;
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

        public static List<List<List<Tuple<int, double>>>> GerarMatrizAdjacencia(List<Cluster> clusters)
        {

            List<List<List<Tuple<int, double>>>> cuboAdjacencia = new List<List<List<Tuple<int, double>>>>(); // LOUCURAAAAAA

            Random random = new Random();

            foreach (Cluster cluster in clusters)
            {
                List<List<Tuple<int, double>>> matrizAdjacenciaCluster = new List<List<Tuple<int, double>>>();

                foreach (Cliente clienteAtual in cluster.Clientes)
                {
                    List<Tuple<int, double>> vetorAdjacencia = new List<Tuple<int, double>>();

                    foreach (Cliente proxCliente in cluster.Clientes)
                    {
                        Tuple<int, double> tupla;
                        tupla = Tuple.Create(proxCliente.Id, CalcularDistancia(clienteAtual.CoordenadaX, proxCliente.CoordenadaX, clienteAtual.CoordenadaY, proxCliente.CoordenadaY));
                        if (proxCliente.IdProximoCliente == tupla.Item1)
                            continue;
                        vetorAdjacencia.Add(tupla);
                    }

                    clienteAtual.IdProximoCliente = PegarIdClienteMaisProximo(vetorAdjacencia);
                    clienteAtual.DistanciaProximoCliente = PegarDistanciaClienteMaisProximo(vetorAdjacencia);

                    matrizAdjacenciaCluster.Add(vetorAdjacencia);
                }
                cuboAdjacencia.Add(matrizAdjacenciaCluster);
            }

            return cuboAdjacencia;
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

        public static int PegarIdClienteMaisProximo(List<Tuple<int, double>> vetorAdjacencia)
        {
            double menor = 99999;

            foreach (var tuple in vetorAdjacencia)
            {
                if (tuple.Item2 == 0)
                    continue;
                if (menor > tuple.Item2)
                    menor = tuple.Item2;
            }
            vetorAdjacencia.OrderBy(x => x.Item2);
            vetorAdjacencia.RemoveAll(tupla => tupla.Item2 != menor);
            Tuple<int, double> proximoCliente = vetorAdjacencia.First();
            return proximoCliente.Item1;
        }

        public static double PegarDistanciaClienteMaisProximo(List<Tuple<int, double>> vetorAdjacencia)
        {
            double menor = 99999;

            foreach (var tuple in vetorAdjacencia)
            {
                if (tuple.Item2 == 0)
                    continue;
                if (menor > tuple.Item2)
                    menor = tuple.Item2;
            }
            vetorAdjacencia.RemoveAll(tupla => tupla.Item2 != menor);
            Tuple<int, double> proximoCliente = vetorAdjacencia.Last();
            return proximoCliente.Item2;
        }

        // Pega a menor distancia de um cliente de uma linha de uma matriz, quando o valor é pego, o indice do valor fica indisponivel de se pegar em outras matrizes. 
        public static Tuple<double, int> PegarMenorValor(int linha, double[,] vetor, bool[] clientDisp, int qntCluster)
        {
            Tuple<double, int> menor = new Tuple<double, int>(99999, 99999);

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

        public static Dataset Mapper(string path)
        {
            Dataset dataset = new Dataset();

            using StreamReader sr = File.OpenText(path);
            string linha;

            // Lê a primeira linha (informações gerais do arquivo)
            linha = sr.ReadLine();
            string[] infoGerais = linha.Split();
            dataset.QntVeiculos = int.Parse(infoGerais[1]);

            dataset.QntClientes = int.Parse(infoGerais[2]);
            dataset.QntDepositos = int.Parse(infoGerais[3]);
            dataset.QntLocais = dataset.QntClientes + dataset.QntDepositos; // inclui o depósito

            // Lê a info de duração na rota e carga por veiculo

            for (int i = 0; i < dataset.QntDepositos; i++)
            {
                linha = sr.ReadLine();
                string[] depositInfo = linha.Split();
                dataset.CapacidadeDeposito = int.Parse(depositInfo[1]);
            }

            dataset.Id = new int[dataset.QntLocais];
            dataset.CoordenadaX = new double[dataset.QntLocais];
            dataset.CoordenadaY = new double[dataset.QntLocais];
            dataset.TempoServico = new double[dataset.QntClientes];
            dataset.Demanda = new int[dataset.QntClientes];

            linha = sr.ReadLine();
            string[] info = linha.Split();

            for (int i = 0; i < dataset.QntLocais; i++)
            {
                dataset.Id[i] = int.Parse(info[0]) - 1;
                dataset.CoordenadaX[i] = double.Parse(info[1]);
                dataset.CoordenadaY[i] = double.Parse(info[2]);
                //dataset.TempoServico[i] = double.Parse(info[3]);
                if (i < dataset.QntClientes)
                    dataset.Demanda[i] = int.Parse(info[4]);

                linha = sr.ReadLine();
                if (linha != null)
                    info = linha.Split();
            }

            return dataset;
        }
    }
}
