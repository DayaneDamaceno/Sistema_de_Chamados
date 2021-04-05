using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaDeChamados
{
    class Program
    {
		struct Chamado
		{
			public int NumChamado;
			public string NomeCliente;
			public string CpfCliente;
			public string DescricaoProblema;
			public DateTime DataAbertura;
			public string Status;
			public DateTime PrevisaoAtendimento;
			public string CpfTecnico;
			public DateTime Atendimento;
			public string Solucao;
		}

		static Chamado[] chamados = new Chamado[100];
		static int qtde = 0;

		static void Main(string[] args)
		{
            ReadArquivo();
            ShowMenuPrincipal();
		}
		/// <summary>
		/// Salva todos os dados sobre os chamados em um arquivo de texto
		/// </summary>
		static void SaveData()
		{
			string conteudo = "";
			for (int n = 0; n < qtde; n++)
				conteudo += $"{chamados[n].NumChamado}|" +
							$"{chamados[n].NomeCliente}|" +
							$"{chamados[n].CpfCliente}|" +
							$"{chamados[n].DescricaoProblema}|" +
							$"{chamados[n].DataAbertura}|" +
							$"{chamados[n].Status}|" +
							$"{chamados[n].PrevisaoAtendimento}|" +
							$"{chamados[n].CpfTecnico}|" +
							$"{chamados[n].Atendimento}|" +
							$"{chamados[n].Solucao}" + Environment.NewLine;

			File.WriteAllText("chamados.txt", conteudo);
		}
		/// <summary>
		/// Lê o arquivo de texto e salva os dados em um array
		/// </summary>
		static void ReadArquivo()
		{
			if (File.Exists("chamados.txt"))
			{

				string[] linhas = File.ReadAllLines("chamados.txt");
				foreach (string linha in linhas)
				{
					string[] dataLine = linha.Split('|');

					chamados[qtde] = new Chamado();

					chamados[qtde].NumChamado = Convert.ToInt32(dataLine[0]);
					chamados[qtde].NomeCliente = dataLine[1];
					chamados[qtde].CpfCliente = dataLine[2];
					chamados[qtde].DescricaoProblema = dataLine[3];
					chamados[qtde].DataAbertura = ReadDateTime(dataLine[4]);
					chamados[qtde].Status = dataLine[5];
					chamados[qtde].PrevisaoAtendimento = ReadDateTime(dataLine[6]);
					chamados[qtde].CpfTecnico = dataLine[7];
					chamados[qtde].Atendimento = ReadDateTime(dataLine[8]);
					chamados[qtde].Solucao = dataLine[9];

					qtde++;
				}
			}
		}
		/// <summary>
		/// Exibe as opções do menu estilizadas
		/// </summary>
		/// <param name="options">Opções a serem listadas</param>
		/// <param name="titleMenu">Titulo do menu</param>
		static void ShowMenuOptions(string[] options, string titleMenu)
		{
			Console.Clear();

			WriteTitle(titleMenu);

			Console.ForegroundColor = ConsoleColor.Yellow;

			for (int i = 0; i < options.Length; i++)
				Console.WriteLine($" {i + 1} - {options[i]}");
			
			Console.ResetColor();
		}
		/// <summary>
		/// Exibe menu principal
		/// </summary>
		static void ShowMenuPrincipal()
		{
            string[] options = { "Criar um chamado", "Atender chamado", "Cancelar um chamado", "Realizar consultas", "Sair" };
            ShowMenuOptions(options, "Sistema de Chamados");
            char opcao = ReadString("\n > Insira uma das opções acima: ")[0];
			do
			{
				switch (opcao)
                {
                    case '1':
                        CreateChamado();
                        break;
                    case '2':
                        AtenderChamado();
                        break;
                    case '3':
                        CancelarChamado();
                        break;
                    case '4':
                        ShowMenuConsulta();
                        break;
                    case '5':
                        Console.WriteLine("Saindo...");
                        Thread.Sleep(2000);
						Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opção Invalida");
                        break;
                }
            } while (opcao != '5');
		}
		/// <summary>
		/// Exibe menu de consultas
		/// </summary>
		static void ShowMenuConsulta()
		{
			string[] options = { "Chamados em aberto", "Chamados em atraso", "Chamados concluidos", "Chamados cancelados", "Voltar" };
			ShowMenuOptions(options, "Consultas");
			char opcao = ReadString("\n > Insira uma das opções acima: ")[0];

			Console.Clear();
			switch (opcao)
			{
				case '1':
					WriteTitle(options[0]);
					ListStatus("Aberto");
					ShowChamado("Aberto");
					break;
				case '2':
					WriteTitle(options[1]);
					ListStatus("Atrasado");
                    ShowChamado("Aberto");
                    break;
				case '3':
					WriteTitle(options[2]);
					ListStatus("Concluido");
					ShowChamado("Concluido");
					break;
				case '4':
					WriteTitle(options[3]);
					ListStatus("Cancelado");
					ShowChamado("Cancelado");
					break;
				case '5':
					ShowMenuPrincipal();
					break;
			}

			char resposta = ReadString(" > Digite S para voltar ao menu de consulta: ").ToUpper()[0];
            if (resposta == 'S')
				ShowMenuConsulta();

		}
		/// <summary>
		/// Criar um novo chamado no sistema
		/// </summary>
		static void CreateChamado()
		{
			Console.Clear();
			WriteTitle("Criar um chamado");

			chamados[qtde] = new Chamado();

			chamados[qtde].NumChamado = qtde;
			chamados[qtde].NomeCliente = ReadString(" > Insira seu nome: ");
			chamados[qtde].CpfCliente = ReadCPF();
			chamados[qtde].DescricaoProblema = ReadString(" > Nos conte o que houve com detalhes para que possamos ajudar da melhor forma: ");
			chamados[qtde].DataAbertura = DateTime.Now;
			chamados[qtde].Status = "Aberto";
			chamados[qtde].PrevisaoAtendimento = CalcPrevisaoAtendimento(chamados[qtde].DataAbertura);

			WriteSuccess($"Chamado criado com sucesso \n Previsão de atendimento: {chamados[qtde].PrevisaoAtendimento:dd/MM/yyyy} ");

			qtde++;
			SaveData();

			// Voltar para menu
			Thread.Sleep(4000);
			ShowMenuPrincipal();

		}
		/// <summary>
		/// Atender há chamados em aberto
		/// </summary>
		static void AtenderChamado()
		{
			char opcao;
			do
			{
				Console.Clear();
				WriteTitle("Atendimento de chamados");
				ListStatus("Aberto");

				int numChamado = ShowChamado("Aberto");

				chamados[numChamado].CpfTecnico = ReadCPF();
				chamados[numChamado].Solucao = ReadString(" > Descreva a solução do problema: ");
				chamados[numChamado].Atendimento = DateTime.Now;

				chamados[numChamado].Status = "Concluido";

				SaveData();
				WriteSuccess("Atendimendo realizado com sucesso");

				Console.Write("Se desejar continuar atendendo digite S: ");
				opcao = Console.ReadLine().ToUpper()[0];

			} while (opcao == 'S');

			Thread.Sleep(2000);
			ShowMenuPrincipal();
		}
		/// <summary>
		/// Cancelar chamados que estão em aberto
		/// </summary>
		static void CancelarChamado()
		{
			Console.Clear();
			WriteTitle("Cancelamento de chamado");
			ListStatus("Aberto");

			int numChamado = ReadInt(" > Insira o Nº do chamado que deseja cancelar: ");
			chamados[numChamado].Status = "Cancelado";

			SaveData();
			WriteSuccess("Cancelamento realizado com sucesso!");

			Thread.Sleep(2000);
			ShowMenuPrincipal();

		}
		/// <summary>
		/// Listagem de todos os chamados de acordo com a situação do chamado
		/// </summary>
		/// <param name="status">Situação dos chamados a serem listados</param>
		static void ListStatus(string status)
		{
			ShowHeaderTable();
			int qtdeItensListagem = 0;

			foreach (var chamado in chamados)
			{
				if (chamado.Status == status)
				{
					qtdeItensListagem++;
					ShowBodyTable(chamado);
				}
				if (status == "Atrasado")
                {
					bool atrasado = DateTime.Now > chamado.PrevisaoAtendimento;
					if (chamado.Status == "Aberto" && atrasado)
					{
						qtdeItensListagem++;
						ShowBodyTable(chamado);
                    }
				}
				
			}
			if (qtdeItensListagem == 0)
            {
				WriteErro("Não foi encontrado nenhum registro");
				Thread.Sleep(2000);
				ShowMenuPrincipal();
            }
		}
		/// <summary>
		/// Exibir o cabeçalho da tabela 
		/// </summary>
		static void ShowHeaderTable()
        {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(String.Format("┌ {0,-10} | {1,-20} | {2,-20} | {3,-25} | {4,-20} ┐",
				"Nº Chamado",
				"Nome do Cliente ",
				"Data de Abertura ",
				"Previsão de Atendimento",
				"Data de atendimento"));
			Console.ResetColor();
		}
		/// <summary>
		/// Exibe um indice no corpo da tabela
		/// </summary>
		/// <param name="chamado">Chamado específico a ser listado</param>
		static void ShowBodyTable(Chamado chamado)
        {
			string atendimentoVerificado = $"{chamado.Atendimento}";
			if (DateTime.Equals(chamado.Atendimento, DateTime.MinValue))
			{
				atendimentoVerificado = " - -------------- - ";
			}
			Console.WriteLine(String.Format("├ {0,-10} ┼ {1,-20} ┼ {2,-20} ┼ {3,-25} ┼ {4,-20} ┤",
				chamado.NumChamado,
				chamado.NomeCliente,
				chamado.DataAbertura.ToString("dd/MM/yyyy"),
				chamado.PrevisaoAtendimento.ToString("dd/MM/yyyy"),
				atendimentoVerificado));
		}
		/// <summary>
		/// Exibe todas as informações sobre um chamado específico 
		/// </summary>
		/// <param name="status">Situação do chamado a ser exibido</param>
		/// <returns>O indice do chamado que foi exibido</returns>
		static int ShowChamado(string status)
		{
			int numChamado;
			do
			{
				numChamado = ReadInt(" > Insira o Nº do chamado para exibir mais detalhes: ");
				if (chamados[numChamado].Status != status)
					WriteErro("Nº do chamado não é correspondente a consulta atual, por favor insira um indice valido.");
					
			} while (chamados[numChamado].Status != status);


			Console.WriteLine($"\n Nº do Chamado - {chamados[numChamado].NumChamado}" +
							$"\n Nome do Cliente - {chamados[numChamado].NomeCliente}" +
							$"\n CPF do Cliente - {FormatCPF(chamados[numChamado].CpfCliente)}" +
							$"\n Status - {chamados[numChamado].Status}" +
							$"\n Descrição do problema - {chamados[numChamado].DescricaoProblema}" +
							$"\n Data de Abertura - {chamados[numChamado].DataAbertura:dd/MM/yyyy}" +
							$"\n Previsão do Atendimento - {chamados[numChamado].PrevisaoAtendimento:dd/MM/yyyy}\n");

			if (string.IsNullOrEmpty(chamados[numChamado].CpfTecnico) == false)
				Console.WriteLine($" CPF do Técnico - {FormatCPF(chamados[numChamado].CpfTecnico)}" +
									$"\n Data do Atendimento - {chamados[numChamado].Atendimento}" +
									$"\n Solução do problema - {chamados[numChamado].Solucao} \n");

			return numChamado;
		}
		/// <summary>
		/// Exibe um erro estilizado em tela 
		/// </summary>
		/// <param name="mensagem">Mensagem ao usuário</param>
		static void WriteErro(string mensagem)
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine($"\n    {mensagem}    \n");
			Console.ResetColor();
		}
		/// <summary>
		/// Exibe uma mensagem estilizada de sucesso
		/// </summary>
		/// <param name="mensagem">Mensagem ao usuário</param>
		static void WriteSuccess(string mensagem)
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine($"\n    {mensagem}    \n");
			Console.ResetColor();
		}
		/// <summary>
		/// Exibe um título estilizado 
		/// </summary>
		/// <param name="title">Título a ser exibido</param>
		static void WriteTitle(string title)
        {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.WriteLine($" {title} \n");
			Console.ResetColor();
		}
		/// <summary>
		/// Lê uma string realizado validações e tratamento de exceções
		/// </summary>
		/// <param name="mensagem">Texto exibido ao usuário para solicitar o texto</param>
		/// <returns>Uma string validada</returns>
		static string ReadString(string mensagem)
		{
			string valor = "";
			do
			{
                try
                {
					Console.Write(mensagem);
					valor = Console.ReadLine().Trim();
					break;
				}
				catch
				{
					WriteErro("Por favor insira um texto valido!");
				}
			}
			while (valor.Length == 0);

			return valor;
		}
		/// <summary>
		/// Lê um int realizado validações e tratamento de exceções
		/// </summary>
		/// <param name="mensagem">Texto exibido ao usuário para solicitar o valor</param>
		/// <returns>Um int validado</returns>
		static int ReadInt(string mensagem)
		{
			int valor;

			do
			{
				try
				{
					Console.Write($"\n {mensagem}");
					valor = Convert.ToInt32(Console.ReadLine());
					break;
				}
				catch
				{
					WriteErro("Por favor insira um valor valido!");
				}
			} while (true);

			return valor;
		}
		/// <summary>
		/// Lê um DateTime realizado validações e tratamento de exceções
		/// </summary>
		/// <param name="date">Data a ser convertida</param>
		/// <returns>Um DateTime validado</returns>
		static DateTime ReadDateTime(string date)
        {
			DateTime dateResult = new DateTime();
			try
			{
				dateResult = Convert.ToDateTime(date);
				
			}
			catch
			{
				WriteErro("Erro ao converter data, por favor insira um valor válido");
				Console.ReadKey();
			}

			return dateResult;
        }
		/// <summary>
		/// Calcula uma previsão de atendimento para que seja dentre dias uteis
		/// </summary>
		/// <param name="dataInicial">Data inicial a ser acrescentada</param>
		/// <returns>Data para previsão do atendimento</returns>
		static DateTime CalcPrevisaoAtendimento(DateTime dataInicial)
        {
			int countDays = 3;

			string dayOfWeek = dataInicial.AddDays(countDays).DayOfWeek.ToString();

			if (dayOfWeek == "Saturday")
				countDays += 2;

			if (dayOfWeek == "Sunday")
				countDays += 1;

			DateTime previsao = dataInicial.AddDays(countDays);

			return previsao;
        }
		/// <summary>
		/// Lê um CPF realizado validações e tratamento de exceções
		/// </summary>
		/// <returns>Um CPF Valido</returns>
		static string ReadCPF()
		{
			string cpf;
			bool valido;

			do
			{
				cpf = ReadString(" > Insira seu CPF: ").Trim();
				valido = ValidaCPF(cpf);
				if (valido == false) WriteErro("Por favor insira um CPF valido!");
			}
			while (valido == false);

			return cpf;
		}
		/// <summary>
		/// Formata um CPF com as pontuações necessarias
		/// </summary>
		/// <param name="cpf">CPF sem formatação</param>
		/// <returns>Um CPF formatado com pontuação correta</returns>
		static string FormatCPF(string cpf)
		{
			string cpfFormatado;

			cpf = cpf.Trim().Replace(".", "").Replace("-", "");

			cpfFormatado = $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";

			return cpfFormatado;
		}
		/// <summary>
		/// Calcula o primeiro digito verificador do CPF
		/// </summary>
		/// <param name="cpf">9 primeiros digitos do CPF</param>
		/// <returns>O primeiro digito verificador</returns>
		static string CalcPrimeiroDigito(string cpf)
        {
			//Contador para o dígito 1
			int[] verificaDigito1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
			int cont = 0, resto;

			//Faz a multiplicação de cada dígito pelo peso referente e soma por eles mesmos
			for (int i = 0; i < 9; i++)
				cont += int.Parse(cpf[i].ToString()) * verificaDigito1[i];

			//Faz a divisão da soma por 11
			resto = cont % 11;

			//Verifica se o dígito for igual a 1 ou a 0, se for, é atribuído o valor 0
			if (resto < 2)
				resto = 0;
			//Faz a subtração de 11 pelo resto
			else
				resto = 11 - resto;

			//Converte o resto para string
			return resto.ToString();
		}
		/// <summary>
		/// Calcula o segundo digito verificador do CPF
		/// </summary>
		/// <param name="cpf">10 primeiros digitos do CPF</param>
		/// <returns>O segundo digito verificador</returns>
		static string CalcSegundoDigito(string cpf)
		{
			//Contador para o dígito 2
			int[] verificaDigito2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
			int cont = 0, resto;

			//Faz a multiplicação de cada dígito pelo peso referente e soma por eles mesmos
			for (int i = 0; i < 10; i++)
				cont += int.Parse(cpf[i].ToString()) * verificaDigito2[i];

			//Faz a divisão da soma por 11
			resto = cont % 11;

			//Verifica se o dígito for igual a 1 ou a 0, se for, é atribuído o valor 0
			if (resto < 2)
				resto = 0;
			//Faz a subtração de 11 pelo resto
			else
				resto = 11 - resto;

			//Converte o resto para string
			return resto.ToString();
		}
		/// <summary>
		/// Valida se o CPF contém digitos repetidos
		/// </summary>
		/// <param name="cpf">CPF a ser validado</param>
		/// <returns>True se tiver digitos repetidos, se não tiver False</returns>
		static bool TemDigitosRepetidos(string cpf)
        {
			string[] numerosInvalidos =
			{
				"00000000000",
				"11111111111",
				"22222222222",
				"33333333333",
				"44444444444",
				"55555555555",
				"66666666666",
				"77777777777",
				"88888888888",
				"99999999999"
			};

			return numerosInvalidos.Contains(cpf);
		}
		/// <summary>
		/// Verifica se um CPF é valido ou não
		/// </summary>
		/// <param name="cpf">CPF a ser validado</param>
		/// <returns>True se o CPF for válido, se não for False</returns>
		static bool ValidaCPF(string cpf)
		{
			string cpfClean, digito;

			cpf = cpf.Trim().Replace(".", "").Replace("-", "");

			if (cpf.Length != 11 || TemDigitosRepetidos(cpf))
				return false;
            
			cpfClean = cpf.Substring(0, 9);

			digito = CalcPrimeiroDigito(cpfClean);

			cpfClean += digito;

			digito += CalcSegundoDigito(cpfClean);
			
			return cpf.EndsWith(digito);
		}
	}
}
