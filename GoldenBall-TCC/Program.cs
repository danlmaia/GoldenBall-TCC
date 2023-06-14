using GoldenBall_TCC;
using Newtonsoft.Json;
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
            if (i == 1 || i == 2 || i == 6 || i == 08 || i == 10 || i == 11)
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

        int[] quantEquipes = new int[1] { 8 };
        int[] quantTemporadas = new int[1] { 4 };
        int[] quantIntra = new int[1] { 100 };

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

                        string copiaTimes = JsonConvert.SerializeObject(times);
                        List<Time> timesOriginais = JsonConvert.DeserializeObject<List<Time>>(copiaTimes);

                        Time solucao = Competicao.Start(times, quantidadeTemporadas, quantidadeTreino, quantidadeTreino, Datasets.IndexOf(dataset));

                        Time SolucaoInicial = Time.GetTimeById(solucao.Id, timesOriginais);

                        Utils.PrintarSolucao(Datasets.IndexOf(dataset), solucao, SolucaoInicial, stopwatch, quantidadeEquipes, quantidadeTemporadas, quantidadeTreino, quantidadeTreino);
                        stopwatch.Restart();
                    }
                }
            }
        }
    }
}