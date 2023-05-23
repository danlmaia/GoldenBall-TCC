namespace GoldenBall_TCC
{
    public static class Competicao
    {
        public static void Start(List<Time> times, int quantidadeEquipes)
        {
            for (int i = 0; i < quantidadeEquipes; i++)
            {
                Time.TreinarJogador(times[i]);
            }
                Time.TreinarTime(times);

            // São selecionados dois clusters de forma aleatoria e dois clientes um em cada cluster também de forma aleátoria,
            // é realizado a troca desses clientes entre os clusters depois é refeita a rota disversas vezes afim de encontrar uma solução melhor
            // porem na maior parte dos cenários roda um stack overflow de estouro de memoria pois o algoritmo roda milhares de vezes e não consegue achar uma solução melhor que a anterior.
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
        // TODO Transferência de Jogadores.
    }
}
