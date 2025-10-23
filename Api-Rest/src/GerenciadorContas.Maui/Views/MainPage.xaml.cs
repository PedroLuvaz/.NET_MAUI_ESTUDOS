using GerenciadorContas.Maui.ViewModels;

namespace GerenciadorContas.Maui.Views
{
    public partial class MainPage : ContentPage
    {
        // Guarda uma referência à ViewModel
        private readonly MainPageViewModel _viewModel;

        // O MAUI vai "injetar" a ViewModel (que vamos registar na Fase 9)
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();

            // Define o "cérebro" (BindingContext) desta página
            // como sendo a ViewModel que recebemos.
            _viewModel = viewModel;
            this.BindingContext = _viewModel;
        }

        // Este método é chamado sempre que a página aparece na tela
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Chama o método da ViewModel para carregar os dados
            await _viewModel.OnCarregarDadosAsync();
        }
    }
}