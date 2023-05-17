namespace GoldenBall_TCC
{
    public static class Competicao
    {
        public static void Start(List<Time> times, int quantidadeTemporadas)
        {
            for (int i = 0; i < quantidadeTemporadas; i++)
            {
                Time.TreinarJogador(times[i]);
            }
                Time.TreinarTime(times);
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
    }
}
