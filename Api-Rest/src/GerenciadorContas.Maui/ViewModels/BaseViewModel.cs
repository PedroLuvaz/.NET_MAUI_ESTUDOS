using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GerenciadorContas.Maui.ViewModels
{
    // Esta classe implementa a interface mágica do MVVM
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Este é o método que "avisa" a View sobre a mudança
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Este é um método "helper" que usamos nas nossas propriedades
        // Ele atualiza o valor e chama o OnPropertyChanged automaticamente
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}