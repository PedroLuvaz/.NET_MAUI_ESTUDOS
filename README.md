# 🍔 Guia: Cadastro de Alimentos com .NET MAUI 9 no VS Code

Este guia consolida o passo a passo para criar um aplicativo CRUD (Criar, Ler, Atualizar, Excluir) completo em .NET MAUI, utilizando o padrão MVVM e Entity Framework Core para acesso ao banco de dados SQL Server.

---

## 🚀 Parte 1: Ambiente e Pré-requisitos

Garanta que você tem tudo instalado para começar:

* **.NET 9 SDK**: Verifique a versão no terminal com o comando `dotnet --version`. A saída deve começar com `9.x.x`.
* **Carga de Trabalho .NET MAUI**: Instale os componentes do MAUI com `dotnet workload install maui`.
* **Visual Studio Code**: É recomendado ter a extensão **C# Dev Kit** para uma melhor experiência de desenvolvimento.
* **Banco de Dados**: **SQL Server Express** e **SQL Server Management Studio (SSMS)**.

---

## 📦 Parte 2: Criação do Projeto e da Solução

Vamos criar uma estrutura organizada com um arquivo de solução (`.sln`) desde o início.

1.  **Crie e acesse a pasta da solução** no seu terminal (PowerShell):
    ```powershell
    mkdir mauiAlimentosFinal
    cd mauiAlimentosFinal
    ```
2.  **Crie o arquivo de solução**:
    ```powershell
    dotnet new sln -n mauiAlimentosFinal
    ```
3.  **Crie o projeto .NET MAUI**:
    ```powershell
    dotnet new maui -n CadastroAlimentos9
    ```
4.  **Adicione o projeto à solução**:
    ```powershell
    dotnet sln add .\CadastroAlimentos9\CadastroAlimentos9.csproj
    ```
5.  **Abra a pasta no VS Code**:
    ```powershell
    code .
    ```

---

## 📁 Parte 3: Estrutura de Pastas MVVM

Dentro da pasta do projeto `CadastroAlimentos9`, crie a seguinte estrutura de pastas para organizar o código no padrão Model-View-ViewModel:

* **Data**: Para o contexto do banco de dados (DbContext).
* **Models**: Para as classes de entidade (nosso `Alimento`).
* **ViewModels**: Para a lógica de apresentação (nosso `AlimentosViewModel`).
* **Views**: Para as páginas XAML da interface (nossa `AlimentosPage`).

---

## 🗄️ Parte 4: Configurando o Banco de Dados com EF Core

1.  **Navegue até a pasta do projeto** no terminal integrado do VS Code (`Ctrl+'`):
    ```powershell
    cd CadastroAlimentos9
    ```
2.  **Instale os pacotes do Entity Framework Core**:
    ```powershell
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```
3.  **Crie o Model** no arquivo `Models/Alimento.cs`:
    ```csharp
    namespace CadastroAlimentos9.Models;
    
    public class Alimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Calorias { get; set; }
    }
    ```
4.  **Crie o DbContext** no arquivo `Data/AppDbContext.cs`:
    ```csharp
    using CadastroAlimentos9.Models;
    using Microsoft.EntityFrameworkCore;
    
    namespace CadastroAlimentos9.Data;
    
    public class AppDbContext : DbContext
    {
        public DbSet<Alimento> Alimentos { get; set; }
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ATENÇÃO: Substitua pelo nome da sua instância SQL Server
            string connectionString = "Server=SEU-SERVIDOR\\SQLEXPRESS;Database=CadastroAlimentosDB_Final;Trusted_Connection=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    ```
5.  **Crie o banco de dados via Migrations**. 
    > **Nota:** É crucial especificar o framework de destino do Windows para que os comandos do EF Core funcionem corretamente em um projeto MAUI. O valor exato (ex: `net9.0-windows10.0.19041.0`) pode ser encontrado no seu arquivo `.csproj`.
    ```powershell
    dotnet ef migrations add InitialCreate --framework net9.0-windows10.0.19041.0
    dotnet ef database update --framework net9.0-windows10.0.19041.0
    ```

---

## 🧠 Parte 5: Construindo o ViewModel

Crie o arquivo `ViewModels/AlimentosViewModel.cs`. Este código contém a lógica da nossa página e já está corrigido para as checagens de nulidade do .NET 9.

```csharp
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
/
-----
```
## 🎨 Parte 6: Construindo a View

Crie o arquivo da página `Views/AlimentosPage.xaml` e adicione o seguinte código de interface:

```xml
<ContentPage xmlns="[http://schemas.microsoft.com/dotnet/2021/maui](http://schemas.microsoft.com/dotnet/2021/maui)"
             xmlns:x="[http://schemas.microsoft.com/winfx/2009/xaml](http://schemas.microsoft.com/winfx/2009/xaml)"
             xmlns:viewModels="clr-namespace:CadastroAlimentos9.ViewModels"
             x:Class="CadastroAlimentos9.Views.AlimentosPage"
             Title="Cadastro de Alimentos">

    <ContentPage.BindingContext>
        <viewModels:AlimentosViewModel />
    </ContentPage.BindingContext>

    <VerticalStackLayout Padding="20" Spacing="10">
        <Label Text="Nome do Alimento:" />
        <Entry Text="{Binding Nome}" Placeholder="Ex: Maçã" />
        <Label Text="Calorias (kcal):" />
        <Entry Text="{Binding Calorias}" Keyboard="Numeric" Placeholder="Ex: 52" />
        <Button Text="Salvar Alimento" Command="{Binding SalvarCommand}" Margin="0,20,0,0" />
        <Line Stroke="LightGray" Margin="0,20" />
        <Label Text="Alimentos Cadastrados" FontSize="Large" FontAttributes="Bold" />

        <CollectionView ItemsSource="{Binding Alimentos}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Grid Padding="10" ColumnDefinitions="*, Auto">
                        <Label Grid.Column="0" VerticalOptions="Center">
                            <Label.FormattedText>
                                <FormattedString>
                                    <Span Text="{Binding Nome}" FontAttributes="Bold" />
                                    <Span Text="{Binding Calorias, StringFormat=' - {0} kcal'}" />
                                </FormattedString>
                            </Label.FormattedText>
                        </Label>
                        <Button Grid.Column="1" Text="Excluir" BackgroundColor="Red"
                                Command="{Binding Source={RelativeSource AncestorType={x:Type viewModels:AlimentosViewModel}}, Path=ExcluirCommand}"
                                CommandParameter="{Binding .}"/>
                    </Grid>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </VerticalStackLayout>
</ContentPage>
```

Crie o arquivo code-behind `Views/AlimentosPage.xaml.cs`:

```csharp
namespace CadastroAlimentos9.Views;

public partial class AlimentosPage : ContentPage
{
	public AlimentosPage()
	{
		InitializeComponent();
	}
}
```

-----

## 🧹 Parte 7: Limpando Arquivos Padrão e Ajustando a Inicialização

  * **Delete `MainPage.xaml` e `MainPage.xaml.cs`**: Estes arquivos do template padrão não são mais necessários.
  * **Ajuste o `AppShell.xaml`** para que ele carregue a `AlimentosPage` como página inicial:
    ```xml
    <Shell
        x:Class="CadastroAlimentos9.AppShell"
        xmlns="[http://schemas.microsoft.com/dotnet/2021/maui](http://schemas.microsoft.com/dotnet/2021/maui)"
        xmlns:x="[http://schemas.microsoft.com/winfx/2009/xaml](http://schemas.microsoft.com/winfx/2009/xaml)"
        xmlns:views="clr-namespace:CadastroAlimentos9.Views"
        Shell.FlyoutBehavior="Disabled"
        Title="CadastroAlimentos9">

        <ShellContent
            Title="Home"
            ContentTemplate="{DataTemplate views:AlimentosPage}"
            Route="MainPage" />

    </Shell>
    ```

-----

## ▶️ Parte 8: Executando o Projeto

1.  **Dica**: Antes da primeira execução (ou se encontrar erros), é uma boa prática limpar o cache. Abra o terminal na pasta do projeto e rode:
    ```powershell
    dotnet clean
    ```
2.  No VS Code, selecione o dispositivo de destino como **"Windows Machine"** na barra de status inferior.
3.  Pressione **F5** para compilar e executar o aplicativo.

<!-- end list -->

```
```
