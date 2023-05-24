
namespace GoldenBall_TCC
{
    public static class Mapper
    {
        public static Dataset MapperData(string path)
        {
            Dataset dataset = new Dataset();

            using StreamReader sr = File.OpenText(path);
            string linha;

            // Lê a primeira linha (informações gerais do arquivo)
            linha = sr.ReadLine();
            string[] infoGerais = linha.Split();
            dataset.QntVeiculos = int.Parse(infoGerais[1]);

            dataset.QntClientes = int.Parse(infoGerais[2]);
            dataset.QntDepositos = int.Parse(infoGerais[3]);
            dataset.QntLocais = dataset.QntClientes + dataset.QntDepositos; // inclui o depósito

            // Lê a info de duração na rota e carga por veiculo

            for (int i = 0; i < dataset.QntDepositos; i++)
            {
                linha = sr.ReadLine();
                string[] depositInfo = linha.Split();
                dataset.CapacidadeDeposito = int.Parse(depositInfo[1]);
            }

            dataset.Id = new int[dataset.QntLocais];
            dataset.CoordenadaX = new double[dataset.QntLocais];
            dataset.CoordenadaY = new double[dataset.QntLocais];
            dataset.TempoServico = new double[dataset.QntClientes];
            dataset.Demanda = new int[dataset.QntClientes];

            linha = sr.ReadLine();
            string[] info = linha.Split();

            for (int i = 0; i < dataset.QntLocais; i++)
            {
                dataset.Id[i] = int.Parse(info[0]) - 1;
                dataset.CoordenadaX[i] = double.Parse(info[1]);
                dataset.CoordenadaY[i] = double.Parse(info[2]);
                //dataset.TempoServico[i] = double.Parse(info[3]);
                if (i < dataset.QntClientes)
                    dataset.Demanda[i] = int.Parse(info[4]);

                linha = sr.ReadLine();
                if (linha != null)
                    info = linha.Split();
            }

            return dataset;
        }

        public static TimeDTO TimeToTimeDTO(Time time)
        {
            return new TimeDTO()
            {
                Capitao = time.Capitao,
                Pontuacao = time.Pontuacao,
                PiorJogador = time.PiorJogador,
                Valor = time.Valor,
                Jogadores = time.Jogadores,
            };
        }

        public static Time TimeDTOToTime(TimeDTO timeDTO)
        {
            return new Time()
            {
                Capitao = timeDTO.Capitao,
                Jogadores = timeDTO.Jogadores,
                Valor = timeDTO.Valor,
                PiorJogador = timeDTO.PiorJogador,
                Pontuacao = timeDTO.Pontuacao
            };
        }
    }
}

