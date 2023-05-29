using GoldenBall_TCC;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Dataset> Datasets = new List<Dataset>();
        string pathDataset;


        for (int i = 1; i <= 33; i++)
        {
            if (i >= 10)
                pathDataset = String.Format("..\\..\\..\\Datasets\\p{0}.txt", i);
            else
                pathDataset = String.Format("..\\..\\..\\Datasets\\p0{0}.txt", i);

            Datasets.Add(Mapper.MapperData(pathDataset));

        }

        int quantidadeEquipes = 4;
        int quantidadeTemporadas = 10;
        int quantidadeIntraTreino = 10;
        int quantidadeInterTreino = 10;

        foreach (Dataset dataset in Datasets)
        {
            List<Time> times = new List<Time>();
            times = Time.GerarTimes(dataset, quantidadeEquipes);

            Competicao.Start(times, quantidadeTemporadas, quantidadeIntraTreino, quantidadeInterTreino , Datasets.IndexOf(dataset));
        }


    }

}