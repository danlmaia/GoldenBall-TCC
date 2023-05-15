using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC
{
    public class Time
    {
        public List<Cluster> Jogadores { get; set; }

        public Cluster Capitao { get; set; }

        public double Valor { get; set; }

        public int Pontuacao { get; set; }

        public Time()
        {
            Jogadores = new List<Cluster>();
            Pontuacao = 0;
        }

        public static double GerarPontuacaoEquipe(List<Cluster> jogadores)
        {
            double valor = 0;
            foreach (Cluster jogador in jogadores)
            {
                valor += jogador.Rota.Distancia;
            }
            return valor / jogadores.Count;
        }

        public static Cluster DefinirCapitao(List<Cluster> jogadores)
        {
            Cluster capitao = new Cluster();
            double menor = double.PositiveInfinity;
            foreach (Cluster cluster in jogadores)
            {
                if (menor > cluster.Rota.Distancia)
                    menor = cluster.Rota.Distancia;
            }

            foreach (Cluster cluster in jogadores)
            {
                if (menor == cluster.Rota.Distancia)
                {
                    capitao = cluster;
                }
            }
            return capitao;
        }

        //public static Cluster TreinarJogador(Cluster cluster, List<Cluster> clusters, List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral)
        //{
        //    Random random = new Random();

        //    int demandaAtualRota = 0;

        //    List<Cliente> clientesVisitados = new List<Cliente>();

        //    Cliente cliente = new Cliente();

        //    while (clientesVisitados.Count != cluster.Clientes.Count)
        //    {
        //        List<Tuple<Cliente, double>> vetorDistancia = new List<Tuple<Cliente, double>>();

        //        int indiceCliente;
        //        int indiceCluster;
        //        int quantidadeClientes;

        //        if (clientesVisitados.Count == 0)
        //        {
        //            cliente = cluster.Clientes[random.Next(cluster.Clientes.Count)];
        //            cliente.Visitado = true;
        //            clientesVisitados.Add(cliente);
        //            demandaAtualRota += cliente.Demanda;
        //            cluster.Rota.Caminho.Add(cliente.Id);
        //            cluster.Rota.Distancia += cliente.DistanciaDeposito; // Calcula a distancia do deposito para o primeiro cliente.

        //            indiceCluster = clusters.IndexOf(cluster);
        //            indiceCliente = cluster.Clientes.IndexOf(cliente);
        //            quantidadeClientes = cluster.Clientes.Count;

        //            vetorDistancia = Utils.PegarVetorDistanciasClientes(indiceCluster, indiceCliente, quantidadeClientes, matrizAdjacenciaGeral);
        //            vetorDistancia.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        //            vetorDistancia.RemoveAt(0);
        //            cliente = vetorDistancia[0].Item1;
        //            cluster.Rota.Distancia += vetorDistancia[0].Item2; // Adiciona a distancia do novo cliente.
        //        }
        //        else
        //        {
        //            cliente.Visitado = true;
        //            clientesVisitados.Add(cliente);
        //            demandaAtualRota += cliente.Demanda;
        //            cluster.Rota.Caminho.Add(cliente.Id);
        //            indiceCluster = clusters.IndexOf(cluster);
        //            indiceCliente = cluster.Clientes.IndexOf(cliente);
        //            quantidadeClientes = cluster.Clientes.Count;
        //            vetorDistancia = Utils.PegarVetorDistanciasClientes(indiceCluster, indiceCliente, quantidadeClientes, matrizAdjacenciaGeral);
        //            vetorDistancia.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        //            vetorDistancia.RemoveAt(0);
        //            Cliente antigo = cliente;
        //            cliente = vetorDistancia[0].Item1;

        //            if (clientesVisitados.Contains(cliente))
        //            {
        //                Utils.PegarDistanciaClienteMaisProximo(clientesVisitados, cluster.Clientes, vetorDistancia);
        //                cliente = vetorDistancia[0].Item1;
        //            }

        //            if (cluster.Capacidade < cliente.Demanda + demandaAtualRota) // Valida antes de ir pro novo cliente se a demanda do novo cliente somada com a demanda total é maior que o limite da rota.
        //            {
        //                cluster.Rota.Distancia += antigo.DistanciaDeposito; // Ida do cliente antigo ao deposito.
        //                cluster.Rota.Caminho.Add(clusters.IndexOf(cluster));
        //                demandaAtualRota = 0;
        //                cluster.Rota.Distancia += cliente.DistanciaDeposito; // Volta do deposito ao cliente novo.
        //            }
        //            else
        //            {
        //                cluster.Rota.Distancia += vetorDistancia[0].Item2; // Adiciona a distancia do novo cliente.
        //            }

        //        }
        //    }

        //    cluster.Rota.Distancia += Utils.CalcularDistancia(cluster.Deposito.CoordenadaX, cliente.CoordenadaX, cluster.Deposito.CoordenadaY, cliente.CoordenadaY);
        //    cluster.Rota.Caminho.Add(clusters.IndexOf(cluster));

        //    return cluster;

        //}

        public static List<Time> GerarTimes(Dataset dataset, int QuantidadeEquipes)
        {
            List<Cluster> Clusters = new List<Cluster>();

            List<Time> times = new List<Time>();
            for (int i = 0; i < QuantidadeEquipes; i++)
            {
                #region Gerar clusters
                double[,] dist = Utils.GerarMatrizDistancia(dataset);

                int qntDepo = dataset.QntDepositos;
                int clientPorCluster = dataset.QntClientes / dataset.QntDepositos;

                bool[] clienteDisponivelId = new bool[dataset.QntClientes];
                int[,] idClient = Cliente.GerarIdClientClusters(dist, clienteDisponivelId, qntDepo, clientPorCluster);

                bool[] clienteDisponivelDistancia = new bool[dataset.QntClientes];
                double[,] distanciaClienteParaDeposito = Utils.GerarDistClusters(dist, clienteDisponivelDistancia, qntDepo, clientPorCluster);

                Clusters = Cluster.GerarClusterData(idClient, distanciaClienteParaDeposito, dataset);
                #endregion

                #region Gerar Rota
                List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral = Utils.GerarMatrizAdjacencia(Clusters);

                Clusters = Cluster.SetarDadosDoDepositoNoCluster(Clusters, dataset);

                Clusters = GerarRotaInicial(Clusters, matrizAdjacenciaGeral);
                #endregion
                Time time = new Time();

                foreach (Cluster cluster in Clusters)
                {
                    time.Jogadores.Add(cluster);
                }

                time.Capitao = DefinirCapitao(time.Jogadores);
                time.Valor = GerarPontuacaoEquipe(time.Jogadores);

                times.Add(time);
            }

            return times;
        }

        public static List<Cluster> GerarRotaInicial(List<Cluster> clusters, List<List<List<Tuple<Cliente, double>>>> matrizAdjacenciaGeral)
        {
            Random random = new Random();

            foreach (Cluster cluster in clusters)
            {
                cluster.Id = clusters.IndexOf(cluster);
                int demandaAtualRota = 0;

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
                        cluster.Rota.Caminho.Add(cluster.Deposito.Id);
                        cluster.Rota.Caminho.Add(cliente.Id);
                        cluster.Rota.Distancia += cliente.DistanciaDeposito; // Calcula a distancia do deposito para o primeiro cliente.

                        indiceCluster = clusters.IndexOf(cluster);
                        indiceCliente = cluster.Clientes.IndexOf(cliente);
                        quantidadeClientes = cluster.Clientes.Count;

                        vetorDistancia = Utils.PegarVetorDistanciasClientes(indiceCluster, indiceCliente, quantidadeClientes, matrizAdjacenciaGeral);
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
                        vetorDistancia = Utils.PegarVetorDistanciasClientes(indiceCluster, indiceCliente, quantidadeClientes, matrizAdjacenciaGeral);
                        vetorDistancia.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                        vetorDistancia.RemoveAt(0);
                        Cliente antigo = cliente;
                        cliente = vetorDistancia[0].Item1;

                        if (clientesVisitados.Contains(cliente))
                        {
                            Utils.PegarDistanciaClienteMaisProximo(clientesVisitados, cluster.Clientes, vetorDistancia);
                            cliente = vetorDistancia[0].Item1;
                        }

                        if (cluster.Capacidade < cliente.Demanda + demandaAtualRota) // Valida antes de ir pro novo cliente se a demanda do novo cliente somada com a demanda total é maior que o limite da rota.
                        {
                            cluster.Rota.Distancia += antigo.DistanciaDeposito; // Ida do cliente antigo ao deposito.
                            cluster.Rota.Caminho.Add(cluster.Deposito.Id);
                            demandaAtualRota = 0;
                            cluster.Rota.Distancia += cliente.DistanciaDeposito; // Volta do deposito ao cliente novo.

                            if(clientesVisitados.Count == cluster.Clientes.Count - 1) // Validar
                            {

                            }
                        }
                        else
                        {
                            cluster.Rota.Distancia += vetorDistancia[0].Item2; // Adiciona a distancia do novo cliente.
                        }

                    }
                }

                if(cluster.Rota.Caminho.Last() != clusters.IndexOf(cluster)) // verifica se o ultimo cliente visitado foi o deposito.
                {
                    cluster.Rota.Distancia += Utils.CalcularDistancia(cluster.Deposito.CoordenadaX, cliente.CoordenadaX, cluster.Deposito.CoordenadaY, cliente.CoordenadaY);
                    cluster.Rota.Caminho.Add(cluster.Deposito.Id);
                }


            }

            return clusters;

        }

        public static double TreinarJogador(Time time)
        {
            double distancia = 0;
            Random random = new Random();


            foreach (Cluster cliente in time.Jogadores)
            {
                Cliente c = cliente.Clientes[random.Next(cliente.Clientes.Count)];
                foreach (int id in cliente.Rota.Caminho)
                {
                    if(id == cliente.Deposito.Id)
                    {

                    }
                }
            }


            return distancia;
        }
    }
}
