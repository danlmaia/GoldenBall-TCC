using GoldenBall_TCC;
using System.Data;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Dataset> Datasets = new List<Dataset>();
        string pathDataset;


        for (int i = 3; i <= 3; i++)
        {
            if (i >= 10)
                pathDataset = String.Format("..\\..\\..\\Datasets\\p{0}.txt", i);
            else
                pathDataset = String.Format("..\\..\\..\\Datasets\\p0{0}.txt", i);

            Datasets.Add(Mapper.MapperData(pathDataset));

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