using Newtonsoft.Json;

namespace GoldenBall_TCC
{
    public static class Competicao
    {
        public static void Start(List<Time> times, int quantidadeTemporadas)
        {
            Utils.PrintarClusters(0, times);
            for (int i = 0; i < quantidadeTemporadas; i++)
            {
                List<Tuple<Time, int>> Tabela = new List<Tuple<Time, int>>();

                Time.TreinarJogador(times[i]);
                times = Time.TreinarTime(times);

                for (int j = 0; j < 2; j++)
                {
                    foreach (Time time in times)
                    {
                        foreach (Time oponente in times)
                        {
                            if (time == oponente)
                                continue;
                            else
                                Partida(time, oponente);
                        }
                    }
                }
                times = Time.OrdenarJogadores(times);
                times.Sort((x, y) => y.Pontuacao.CompareTo(x.Pontuacao));
                times = Transferencia(times);

                times = Time.ZerarPontuacao(times);
                times = Time.AtualizarTimes(times);
            }

        }

        public static List<Tuple<Time, int>> GerarTabelaClassificacao(List<Time> times)
        {
            List<Tuple<Time, int>> Tabela = new List<Tuple<Time, int>>();
            foreach (Time time in times)
            {
                Tuple<Time, int> timeTabela = Tuple.Create(time, time.Pontuacao);
                Tabela.Add(timeTabela);
            }
            Tabela.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            return Tabela;
        }

        public static void Partida(Time time1 , Time time2)
        {
            int golsTime1 = 0;
            int golsTime2 = 0;

            for (int i = 0; i < time1.Jogadores.Count; i++)
            {
                if (time1.Jogadores[i].Rota.Distancia < time2.Jogadores[i].Rota.Distancia)
                    golsTime1++;
                else if(time1.Jogadores[i].Rota.Distancia == time2.Jogadores[i].Rota.Distancia)
                {
                    golsTime1++;
                    golsTime2++;
                }
                else
                    golsTime2++;
            }
            if(golsTime1 > golsTime2)
                time1.Pontuacao += 3;
            else if(golsTime1 < golsTime2)
                time2.Pontuacao += 3;
            else
            {
                time1.Pontuacao += 1;
                time2.Pontuacao += 1;
            }
        }

        public static List<Time> Transferencia(List<Time> times)
        {
            Cluster jogador;
            Cluster outroJogador;
            for (int j = 0; j < times.Count - 1; j++)
            {
                jogador = times[j].Jogadores.Last();
                outroJogador = times[j + 1].Jogadores.First(x => x.Id == jogador.Id);

                int index = times[j + 1].Jogadores.IndexOf(outroJogador);

                times[j].Jogadores[times[j].Jogadores.Count - 1] = outroJogador;
                times[j+1].Jogadores[index] = jogador;

                jogador = times[j+1].Jogadores.First();
                outroJogador = times[j].Jogadores.First(x => x.Id == jogador.Id);

                index = times[j].Jogadores.IndexOf(outroJogador);

                times[j + 1].Jogadores[times[j].Jogadores.Count - 1] = outroJogador;
                times[j].Jogadores[index] = jogador;
            }

            times = Time.OrdenarJogadores(times);

            return times;
        }
    }
}
