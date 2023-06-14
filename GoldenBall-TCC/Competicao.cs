using Newtonsoft.Json;

namespace GoldenBall_TCC
{
    public static class Competicao
    {
        public static Time Start(List<Time> times, int quantidadeTemporadas, int quantidadeIntraTreino, int quantidadeInterTreino , int idDataset)
        {
            Time solucaoAntiga = new Time();
            double MediaValorSolucoesGeral = 0;
            double MediaValorSolucaoGeralAtual = 0;
            for (int i = 0; i < quantidadeTemporadas; i++)
            {
                List<Tuple<Time, int>> Tabela = new List<Tuple<Time, int>>();


                for (int j = 0; j < 2; j++) // Ciclos
                {
                    foreach (Time time in times)
                        Time.TreinarJogador(time, quantidadeIntraTreino);
                    times = Time.TreinarTime(times, quantidadeIntraTreino, quantidadeInterTreino);
                    //times = Time.TreinarTimeSemVerificar(times, quantidadeIntraTreino, quantidadeInterTreino);

                    times = Time.AtualizarTimes(times);

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
                
                if(i == 0)
                {
                    MediaValorSolucoesGeral = Time.CalcularValorSolucoesGeral(times);
                    solucaoAntiga = times[0];
                }
                else
                {
                    if(i >= 2)
                    {
                        MediaValorSolucaoGeralAtual = Time.CalcularValorSolucoesGeral(times);
                        if (solucaoAntiga.Valor <= times[0].Valor && MediaValorSolucoesGeral <= MediaValorSolucaoGeralAtual) // Verificando se houve melhora na melhor solução.
                            return solucaoAntiga;
                        else
                            solucaoAntiga = times[0];

                        //if (MediaValorSolucoesGeral <= MediaValorSolucaoGeralAtual) // Verificando se houve melhora na média da pontuação de todas soluções.
                        //    return solucaoAntiga;

                        MediaValorSolucoesGeral = MediaValorSolucaoGeralAtual;
                    }
                }
                times = Transferencia(times);
                times = Time.AtualizarTimes(times);
                times = Time.ZerarPontuacao(times);
            }

            return times[0];
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
            Cluster piorJogadorTime1;
            Cluster jogadorSubstituidoTime2;
            Cluster melhorJogadorTime2;
            Cluster jogadorSubstituidoTime1;
            for (int j = 0; j < times.Count - 1; j++)
            {
                piorJogadorTime1 = times[j].Jogadores.Last();
                jogadorSubstituidoTime2 = times[j + 1].Jogadores.First(x => x.Id == piorJogadorTime1.Id);

                times[j].Jogadores[times[j].Jogadores.Count - 1] = jogadorSubstituidoTime2;
                times[j+1].Jogadores[times[j + 1].Jogadores.FindIndex(x => x.Id == piorJogadorTime1.Id)] = piorJogadorTime1;

                melhorJogadorTime2 = times[j+1].Jogadores.First();
                jogadorSubstituidoTime1 = times[j].Jogadores.First(x => x.Id == melhorJogadorTime2.Id);

                times[j + 1].Jogadores[times[j + 1].Jogadores.FindIndex(x => x.Id == jogadorSubstituidoTime1.Id)] = jogadorSubstituidoTime1;
                times[j].Jogadores[times[j].Jogadores.FindIndex(x => x.Id == melhorJogadorTime2.Id)] = melhorJogadorTime2;
            }

            times = Time.OrdenarJogadores(times);

            return times;
        }
    }
}
