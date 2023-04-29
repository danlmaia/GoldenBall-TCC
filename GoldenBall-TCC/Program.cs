using GoldenBall_TCC;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Dataset> Datasets = new List<Dataset>();
        List<Cluster> Clusters = new List<Cluster>();
        string pathDataset;


        for (int i = 1; i < 33; i++)
        {
            if (i >= 10)
                pathDataset = String.Format("..\\..\\..\\Datasets\\p{0}.txt", i);
            else
                pathDataset = String.Format("..\\..\\..\\Datasets\\p0{0}.txt", i);

            Datasets.Add(Mapper.MapperData(pathDataset));

        }

        int quantidadeEquipes = 4;

        Competicao.Start(Datasets, quantidadeEquipes);

    }

}