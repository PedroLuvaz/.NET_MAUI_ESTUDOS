using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CadastroAlimentos9.Data;
using CadastroAlimentos9.Models;
using Microsoft.EntityFrameworkCore;

namespace CadastroAlimentos9.ViewModels;

public class AlimentosViewModel : INotifyPropertyChanged
{
    private string _nome = "";
    private double _calorias;

    public ObservableCollection<Alimento> Alimentos { get; set; } = new();

    public string Nome
    {
        get => _nome;
        set => SetProperty(ref _nome, value);
    }

    public double Calorias
    {
        get => _calorias;
        set => SetProperty(ref _calorias, value);
    }

    public ICommand SalvarCommand { get; }
    public ICommand ExcluirCommand { get; }

    public AlimentosViewModel()
    {
        SalvarCommand = new Command(async () => await SalvarAlimento());
        ExcluirCommand = new Command<Alimento>(async (alimento) => await ExcluirAlimento(alimento));
        Task.Run(async () => await CarregarAlimentos());
    }

    private async Task CarregarAlimentos()
    {
        using (var db = new AppDbContext())
        {
            var alimentosDoBanco = await db.Alimentos.ToListAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Alimentos.Clear();
                foreach (var alimento in alimentosDoBanco)
                {
                    Alimentos.Add(alimento);
                }
            });
        }
    }

    private async Task SalvarAlimento()
    {
        if (string.IsNullOrWhiteSpace(Nome) || Calorias <= 0)
            return;

        var novoAlimento = new Alimento { Nome = Nome, Calorias = Calorias };
        using (var db = new AppDbContext())
        {
            await db.Alimentos.AddAsync(novoAlimento);
            await db.SaveChangesAsync();
        }

        Nome = string.Empty;
        Calorias = 0;
        await CarregarAlimentos();
    }

    private async Task ExcluirAlimento(Alimento alimento)
    {
        if (alimento == null) return;
        using (var db = new AppDbContext())
        {
            db.Alimentos.Remove(alimento);
            await db.SaveChangesAsync();
        }
        await CarregarAlimentos();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}