using GoldenBall_TCC;
using System.Data;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Dataset> Datasets = new List<Dataset>();
        string pathDataset;
        string pathInstancePr;


        for (int i = 1; i <= 23; i++)
        {
            if (i == 1 || i == 2 || i == 6 || i == 08 || i == 10)
                continue;
            if (i >= 10)
                pathDataset = String.Format("..\\..\\..\\Datasets\\p{0}.txt", i);
            else
                pathDataset = String.Format("..\\..\\..\\Datasets\\p0{0}.txt", i);

            Datasets.Add(Mapper.MapperData(pathDataset));

        }

        for (int i = 1; i <= 10; i++)
        {
            if(i == 10)
                pathInstancePr = String.Format("..\\..\\..\\Datasets\\pr{0}.txt", i);
            else
                pathInstancePr = String.Format("..\\..\\..\\Datasets\\pr0{0}.txt", i);

            Datasets.Add(Mapper.MapperData(pathInstancePr));
        }

        int[] quantEquipes = new int[2] { 4 , 8 };
        int[] quantTemporadas = new int[3] { 2, 4, 10 };
        int[] quantIntra = new int[4] { 10, 20, 50, 100 };
        //int quantidadeEquipes = 8;
        //int quantidadeTemporadas = 10;
        //int quantidadeIntraTreino = 10;
        //int quantidadeInterTreino = 10;

        foreach (int quantidadeEquipes in quantEquipes)
        {
            foreach (int quantidadeTemporadas in quantTemporadas)
            {
                foreach (int quantidadeTreino in quantIntra)
                {
                    foreach (Dataset dataset in Datasets)
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        List<Time> times = new List<Time>();
                        times = Time.GerarTimes(dataset, quantidadeEquipes);
                        //TODO OS COPIAR TIME E PASSAR NA FUNCAO DE PRINTAR A SOLUCAO A VERSÃO ANTIGA E PÓS ALGORITMO;

                        Time solucao = Competicao.Start(times, quantidadeTemporadas, quantidadeTreino, quantidadeTreino, Datasets.IndexOf(dataset));

                        Utils.PrintarSolucao(Datasets.IndexOf(dataset), solucao, stopwatch, quantidadeEquipes, quantidadeTemporadas, quantidadeTreino, quantidadeTreino);
                        stopwatch.Restart();
                    }
                }
            }
        }

        //foreach (Dataset dataset in Datasets)
        //{
        //    var stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    List<Time> times = new List<Time>();
        //    times = Time.GerarTimes(dataset, quantidadeEquipes);

        //    Time solucao = Competicao.Start(times, quantidadeTemporadas, quantidadeIntraTreino, quantidadeInterTreino , Datasets.IndexOf(dataset));

        //    Utils.PrintarSolucao(Datasets.IndexOf(dataset), solucao, stopwatch, quantidadeEquipes, quantidadeTemporadas, quantidadeIntraTreino, quantidadeIntraTreino);
        //    stopwatch.Restart();

        //}


    }

}