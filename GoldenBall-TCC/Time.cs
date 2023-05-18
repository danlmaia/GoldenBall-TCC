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

        public Cluster PiorJogador { get; set; }

        public double Valor { get; set; }

        public int Pontuacao { get; set; }

        public Time()
        {
            Jogadores = new List<Cluster>();
            Pontuacao = 0;
        }

        public static double GerarValorTime(Time time)
        {
            double valor = 0;
            foreach (Cluster jogador in time.Jogadores)
            {
                valor += jogador.Rota.Distancia;
            }
            return valor / time.Jogadores.Count;
        }

        public static Cluster DefinirPiorJogador(Time time)
        {
            Cluster piorJogador = new Cluster();
            double maior = 0;
            foreach (Cluster cluster in time.Jogadores)
            {
                if (maior < cluster.Rota.Distancia)
                    maior = cluster.Rota.Distancia;
            }

            foreach (Cluster cluster in time.Jogadores)
            {
                if (maior == cluster.Rota.Distancia)
                {
                    piorJogador = cluster;
                }
            }
            return piorJogador;
        }

        public static Cluster DefinirCapitao(Time time)
        {
            Cluster capitao = new Cluster();
            double menor = double.PositiveInfinity;
            foreach (Cluster cluster in time.Jogadores)
            {
                if (menor > cluster.Rota.Distancia)
                    menor = cluster.Rota.Distancia;
            }

            foreach (Cluster cluster in time.Jogadores)
            {
                if (menor == cluster.Rota.Distancia)
                {
                    capitao = cluster;
                }
            }
            return capitao;
        }

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

                time.Capitao = DefinirCapitao(time);
                time.PiorJogador = DefinirPiorJogador(time);
                time.Valor = GerarValorTime(time);

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
                        //cluster.Rota.Caminho.Add(cluster.Deposito.Id);
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
                        }
                        else
                        {
                            cluster.Rota.Distancia += vetorDistancia[0].Item2; // Adiciona a distancia do novo cliente.
                        }

                    }
                }

                if(cluster.Rota.Caminho.Last() != cluster.Deposito.Id) // verifica se o ultimo cliente visitado foi o deposito.
                {
                    cluster.Rota.Distancia += Utils.CalcularDistancia(cluster.Deposito.CoordenadaX, cliente.CoordenadaX, cluster.Deposito.CoordenadaY, cliente.CoordenadaY);
                    cluster.Rota.Caminho.Add(cluster.Deposito.Id);
                }


            }

            return clusters;

        }

        public static Time TreinarJogador(Time time)
        {
            foreach (Cluster cluster in time.Jogadores)
            {
                //Console.WriteLine("ANTES: " + cluster.Rota.Distancia);
                List<int> novaRota = TrocarRotaInternoCluster(cluster, cluster.Rota.Caminho);

                double dist = 0;

                dist = CalcularNovaDistancia(cluster, novaRota);

                cluster.Rota.Distancia = dist;
                //Console.WriteLine("Depois: " + cluster.Rota.Distancia);

            }
            return time;

        }

        public static List<Time> TreinarTime(List<Time> times)
        {
            List<Time> TimesDTO = new List<Time>();

            TimesDTO.AddRange(times);

            Random random = new Random();

            Cluster clusterAntigo1;
            Cluster clusterAntigo2;

            do
            {
                clusterAntigo1 = times[0].Jogadores[random.Next(times[0].Jogadores.Count)];
                clusterAntigo2 = times[0].Jogadores[random.Next(times[0].Jogadores.Count)];
            } while (clusterAntigo1 == clusterAntigo2);

            int idCluster1 = clusterAntigo1.Id;
            int idCluster2 = clusterAntigo2.Id;

            Cliente cliente1 = clusterAntigo1.Clientes[random.Next(clusterAntigo1.Clientes.Count)];
            Cliente cliente2 = clusterAntigo2.Clientes[random.Next(clusterAntigo2.Clientes.Count)];

            int idCliente1 = cliente1.Id;
            int idCliente2 = cliente2.Id;

            foreach (Time time in times)
            {
                Time novoTime = TrocarClienteEntreCluster(time, idCluster1, idCluster2, idCliente1, idCliente2);

                List<int> novaRotaCluster1 = new List<int>();
                List<int> novaRotaCluster2 = new List<int>();

                Cluster novoCluster1 = GetClusterByIdAndTime(idCluster1, novoTime);
                Cluster novoCluster2 = GetClusterByIdAndTime(idCluster2 , novoTime);

                novaRotaCluster1.AddRange(novoCluster1.Rota.Caminho);
                novaRotaCluster1.RemoveAll(x => x == novoCluster1.Deposito.Id);

                novaRotaCluster2.AddRange(novoCluster2.Rota.Caminho);
                novaRotaCluster2.RemoveAll(x => x == novoCluster2.Deposito.Id);

                int indiceClienteCluster1 = novaRotaCluster1.IndexOf(cliente1.Id);
                int indiceClienteCluster2 = novaRotaCluster2.IndexOf(cliente2.Id);

                int temp = novaRotaCluster1[indiceClienteCluster1];
                novaRotaCluster1[indiceClienteCluster1] = novaRotaCluster2[indiceClienteCluster2];
                novaRotaCluster2[indiceClienteCluster2] = temp;

                double novaDistCluster1 = 0;

                Console.WriteLine("Antes: " + novoCluster1.Rota.Distancia);

                novaDistCluster1 = CalcularNovaDistancia(novoCluster1, novaRotaCluster1);

                novoCluster1.Rota.Distancia = novaDistCluster1;

                Console.WriteLine("Depois: " + novoCluster1.Rota.Distancia);

                double novaDistCluster2 = 0;

                Console.WriteLine("Antes: " + novoCluster2.Rota.Distancia);

                novaDistCluster2 = CalcularNovaDistancia(novoCluster2, novaRotaCluster2);

                novoCluster2.Rota.Distancia = novaDistCluster2;

                Console.WriteLine("Depois: " + novoCluster2.Rota.Distancia);

            }
            times.Clear();
            times.AddRange(TimesDTO);
            return times;
        }

        public static double CalcularNovaDistancia(Cluster cluster, List<int> rota)
        {
            double dist = 0;
            int demanda = 0;
            List<int> ListaAux = new List<int>();

            Cliente clienteAtual = new Cliente();
            Cliente proximoCliente = new Cliente();

            foreach (int id in rota)
            {
                if (rota.IndexOf(id) == 0)
                {
                    ListaAux.Add(cluster.Deposito.Id);
                    ListaAux.Add(id);
                    //Console.WriteLine("Distancia antes de tudo: " + dist);

                    clienteAtual = Cluster.GetClienteByIdAndCluster(id, cluster);
                    proximoCliente = Cluster.GetClienteByIdAndCluster(rota[1], cluster);

                    demanda += clienteAtual.Demanda;

                    dist += clienteAtual.DistanciaDeposito;
                    //Console.WriteLine(string.Format("Saida do deposito para o cliente {0} distancia percorrida {1}", clienteAtual.Id, dist));
                    dist += Utils.CalcularDistancia(clienteAtual.CoordenadaX, proximoCliente.CoordenadaX, clienteAtual.CoordenadaY, proximoCliente.CoordenadaY);
                    //Console.WriteLine(string.Format("Saida do cliente {0} para o cliente {1} distancia percorrida {2}", clienteAtual.Id, proximoCliente.Id, dist));

                }
                else
                {
                    clienteAtual = Cluster.GetClienteByIdAndCluster(id, cluster);
                    ListaAux.Add(clienteAtual.Id);
                    int prox = rota.IndexOf(id) + 1;

                    if(prox == rota.Count)
                    {
                        dist += clienteAtual.DistanciaDeposito;
                        //Console.WriteLine(string.Format("Foi do cliente {0} para o deposito finalizando as entregas, distancia percorrida: {1}", clienteAtual.Id, dist));
                        ListaAux.Add(cluster.Deposito.Id);
                        if(dist > cluster.Rota.Distancia)
                        {
                            while(dist > cluster.Rota.Distancia) // TODO Quantidade de treinos
                            {
                                List<int> newRota = TrocarRotaInternoCluster(cluster, rota);
                                dist = CalcularNovaDistancia(cluster, newRota);
                            }
                            return dist;
                        }
                        else
                        {
                            cluster.Rota.Caminho.Clear();
                            cluster.Rota.Caminho.AddRange(ListaAux);
                            return dist;
                        }
                    }
                    proximoCliente = Cluster.GetClienteByIdAndCluster(rota[prox], cluster);

                    demanda += clienteAtual.Demanda;

                    if (demanda + proximoCliente.Demanda > cluster.Capacidade)
                    {
                        //Console.WriteLine(string.Format("Demanda maxima atingida: {0}", demanda + proximoCliente.Demanda));
                        dist += clienteAtual.DistanciaDeposito;
                        //Console.WriteLine(string.Format("Foi do cliente {0} reabastecer no deposito, distancia percorrida: {1}", clienteAtual.Id ,dist));
                        ListaAux.Add(cluster.Deposito.Id);
                        demanda = 0;
                        dist += proximoCliente.DistanciaDeposito;
                        //Console.WriteLine(string.Format("Foi do deposito para o cliente {0}, distancia percorrida: {1}", proximoCliente.Id ,dist));

                    }
                    else
                    {
                        dist += Utils.CalcularDistancia(clienteAtual.CoordenadaX, proximoCliente.CoordenadaX, clienteAtual.CoordenadaY, proximoCliente.CoordenadaY);

                        //Console.WriteLine(string.Format("Saida do cliente {0} para o cliente {1} distancia percorrida {2}", clienteAtual.Id, proximoCliente.Id, dist));
                    }

                }

            }

            return dist;
        }

        public static List<int> TrocarRotaInternoCluster(Cluster cluster, List<int> rota)
        {
            List<int> novaRota = new List<int>();
            novaRota.AddRange(rota);
            novaRota.RemoveAll(x => x == cluster.Deposito.Id);

            Random random = new Random();
            Cliente cliente;
            cliente = cluster.Clientes[random.Next(cluster.Clientes.Count)];
            if(cliente.Id != novaRota.Last())
            {
                int indice = novaRota.IndexOf(cliente.Id);
                int proximoCliente = novaRota.IndexOf(cliente.Id) + 1;

                int temp = novaRota[indice];
                novaRota[indice] = novaRota[proximoCliente];
                novaRota[proximoCliente] = temp;

            }
            else
            {
                int indice = novaRota.IndexOf(cliente.Id);
                int clienteAnterior = novaRota.IndexOf(cliente.Id) - 1;

                int temp = novaRota[indice];
                novaRota[indice] = novaRota[clienteAnterior];
                novaRota[clienteAnterior] = temp;
            }

            return novaRota;
        }

        public static Cluster GetClusterByIdAndTime(int id, Time time)
        {
            foreach (Cluster cluster in time.Jogadores)
            {
                if (cluster.Id == id)
                    return cluster;
            }
        return null;
        }

        public static Time TrocarClienteEntreCluster(Time time, int idCluster1, int idCluster2, int idCliente1, int idCliente2)
        {
            Time novoTime = time;
            Cliente c1 = null;
            Cliente c2 = null;
            Cluster cluster;

            c2 = Cluster.GetClienteByIdAndCluster(idCliente2, novoTime.Jogadores[idCluster2]);
            cluster = GetClusterByIdAndTime(idCluster1, time);
            c2.DistanciaDeposito = Utils.CalcularDistancia(cluster.Deposito.CoordenadaX, c2.CoordenadaX, cluster.Deposito.CoordenadaY, c2.CoordenadaY);
            
            c1 = Cluster.GetClienteByIdAndCluster(idCliente1, novoTime.Jogadores[idCluster1]);
            cluster = GetClusterByIdAndTime(idCluster2, time);
            c1.DistanciaDeposito = Utils.CalcularDistancia(cluster.Deposito.CoordenadaX, c1.CoordenadaX, cluster.Deposito.CoordenadaY, c1.CoordenadaY);

            novoTime.Jogadores[idCluster1].Clientes.Add(c2);
            novoTime.Jogadores[idCluster2].Clientes.Add(c1);
            novoTime.Jogadores[idCluster1].Clientes.Remove(c1);
            novoTime.Jogadores[idCluster2].Clientes.Remove(c2);

            return novoTime;
        }
    }
}
