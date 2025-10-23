using GerenciadorContas.Maui.Models;
using GerenciadorContas.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace GerenciadorContas.Maui.ViewModels
{
    // O ViewModel herda do BaseViewModel (que implementa o INotifyPropertyChanged)
    public class MainPageViewModel : BaseViewModel
    {
        // O "Encanamento" da API
        private readonly IApiService _apiService;

        // --- Propriedades "Bindáveis" (para o XAML) ---
        // Usamos ObservableCollection em vez de List<>
        // porque ela notifica a UI automaticamente quando itens são adicionados/removidos.
        public ObservableCollection<Conta> Contas { get; } = new();

        // Esta é a "propriedade de suporte" (backing field)
        private decimal _totalGasto;
        // Esta é a propriedade pública que o XAML vai "ler"
        public decimal TotalGasto
        {
            get => _totalGasto;
            // O SetProperty vem do nosso BaseViewModel
            set => SetProperty(ref _totalGasto, value);
        }

        private string _nome;
        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value);
        }

        private decimal _valor;
        public decimal Valor
        {
            get => _valor;
            set => SetProperty(ref _valor, value);
        }

        // --- Comandos "Bindáveis" (para os botões) ---
        public ICommand SalvarCommand { get; }
        public ICommand CarregarDadosCommand { get; }

        // --- Construtor ---
        public MainPageViewModel(IApiService apiService)
        {
            _apiService = apiService;

            // "Liga" os Comandos aos seus respetivos métodos
            // O "new Command(...)" é a forma padrão do MAUI criar comandos
            SalvarCommand = new Command(async () => await OnSalvarAsync());
            CarregarDadosCommand = new Command(async () => await OnCarregarDadosAsync());
        }

        // --- Métodos de Lógica ---

        // Método público que a View (MainPage.xaml.cs) vai chamar
        public async Task OnCarregarDadosAsync()
        {
            try
            {
                // 1. Carregar o Somatório
                var somatorioDto = await _apiService.GetSomatorioAsync();
                TotalGasto = somatorioDto.TotalGasto;

                // 2. Carregar o Histórico de Contas
                var todasContas = await _apiService.GetTodasContasAsync();

                Contas.Clear(); // Limpa a lista antiga
                foreach (var conta in todasContas)
                {
                    Contas.Add(conta); // Adiciona os novos itens
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar dados: {ex.Message}");
                // (Numa app real, mostraríamos um pop-up)
            }
        }

        private async Task OnSalvarAsync()
        {
            // Validação simples
            if (string.IsNullOrWhiteSpace(Nome) || Valor <= 0)
            {
                // (Numa app real, mostraríamos um pop-up de erro)
                Debug.WriteLine("Dados inválidos");
                return;
            }

            try
            {
                var novaConta = new ContaInputDto
                {
                    Nome = this.Nome,
                    Valor = this.Valor
                };

                // 1. Envia a nova conta para a API
                await _apiService.AdicionarContaAsync(novaConta);

                // 2. Limpa os campos da tela
                Nome = string.Empty;
                Valor = 0;

                // 3. Atualiza os dados (o somatório e a lista)
                await OnCarregarDadosAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar: {ex.Message}");
            }
        }
    }
}