-----

# Gerenciador de Contas

Um projeto de exemplo de um gerenciador de contas financeiras. O objetivo principal é demonstrar uma arquitetura moderna de 3 camadas, separando completamente o Frontend do Backend e utilizando Docker para containerizar os serviços de backend.

  * **Backend:** API REST construída com ASP.NET Core e Entity Framework Core.
  * **Frontend:** Aplicação de UI nativa construída com .NET MAUI e o padrão MVVM.
  * **Banco de Dados:** SQL Server.
  * **Containerização:** Docker e Docker Compose.

-----

## Arquitetura do Projeto

Este projeto utiliza uma arquitetura de 3 camadas (ou "desacoplada").

```
[APP .NET MAUI] <--- (HTTP/JSON) ---> [API REST (Docker)] <--- (SQL) ---> [BD SQL SERVER (Docker)]
```

  * **Frontend (`GerenciadorContas.Maui`):**

      * É o "rosto" da aplicação (a interface gráfica).
      * **Não** contém nenhuma lógica de negócios ou conexão com o banco de dados.
      * Usa o padrão **MVVM** (`Views`, `ViewModels`, `Models`).
      * A sua única responsabilidade é exibir dados e fazer requisições HTTP (JSON) para o Backend.

  * **Backend (`GerenciadorContas.Api` + `docker-compose.yml`):**

      * É o "cérebro" da aplicação.
      * O **`docker-compose.yml`** orquestra dois serviços: a API e o Banco de Dados.
      * A **API REST** expõe os endpoints (ex: `/api/contas/somatorio`) para o mundo.
      * A API é a **única** que tem permissão para falar com o Banco de Dados. Isto garante que a lógica de negócios e, mais importante, a **segurança** (strings de conexão, senhas) estejam centralizadas e protegidas no servidor.

-----

## Tecnologias Utilizadas

  * .NET 9 (SDK)
  * ASP.NET Core Web API (para o Backend)
  * .NET MAUI (para o Frontend)
  * Entity Framework Core (para acesso ao banco de dados)
  * SQL Server
  * Docker & Docker Compose
  * Swagger (OpenAPI) (para documentação e teste da API)

-----

## Pré-requisitos

Antes de começar, certifique-se de que tem o seguinte software instalado:

1.  **[.NET 9 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0)** (ou a versão mais recente)
2.  **[Docker Desktop](https://www.docker.com/products/docker-desktop/)**
3.  Um editor de código, como **[VS Code](https://code.visualstudio.com/)** ou Visual Studio 2022.
4.  (Opcional, mas recomendado) **[Azure Data Studio](https://learn.microsoft.com/pt-br/sql/azure-data-studio/download-azure-data-studio)** para visualizar o banco de dados.

-----

## 🚀 Como Executar o Projeto (Do Zero)

Siga estes passos para configurar e executar o projeto completo na sua máquina.

### 1\. Clonar o Repositório

Primeiro, obtenha o código-fonte do GitHub (substitua pela sua URL):

```bash
git clone https://github.com/seu-usuario/gerenciadorcontas.git
cd gerenciadorcontas
```

### 2\. Configurar a Senha do Banco de Dados

Você precisa de definir uma senha segura para o banco de dados em dois locais:

1.  **No Docker Compose (para criar o banco):**

      * Abra o ficheiro `GERENCIADORCONTAS/docker-compose.yml`.
      * Encontre a linha `SA_PASSWORD: "SuaSenhaForte!123"`.
      * Substitua `"SuaSenhaForte!123"` por uma senha complexa à sua escolha.

2.  **Na API (para a API se conectar ao banco):**

      * Abra o ficheiro `GERENCIADORCONTAS/src/GerenciadorContas.Api/appsettings.json`.
      * Encontre a `ConnectionStrings.DefaultConnection`.
      * Substitua o valor de `Password=` pela **exata mesma senha** que você definiu no passo anterior.

### 3\. Iniciar o Backend (API + Banco de Dados)

Com o Docker Desktop em execução, vamos iniciar todo o backend.

1.  Certifique-se de que o seu terminal (PowerShell) está na pasta raiz `GERENCIADORCONTAS`.

2.  Execute o seguinte comando:

    ```powershell
    docker-compose up -d --build
    ```

<!-- end list -->

  * `--build`: Força o Docker a (re)construir a imagem da sua API usando o `Dockerfile`.
  * `-d`: Executa os contentores em segundo plano (detached).

Ao final, você deverá ter dois contentores (`gerenciador-contas-db` e `gerenciador-contas-api`) a serem executados no seu Docker Desktop.

### 4\. Executar a Migração do Banco de Dados (EF Core)

O backend está a ser executado, mas o banco de dados ainda está vazio. Precisamos de criar as nossas tabelas.

1.  Abra um **novo terminal** e navegue até à pasta do projeto da API:

    ```powershell
    cd src/GerenciadorContas.Api
    ```

2.  Execute o comando `dotnet ef database update`. Este comando precisa de se conectar ao banco de dados, que está no Docker, exposto na porta `localhost:1433`.

    *(**Nota:** Usamos `localhost` aqui porque estamos a executar o comando a partir do nosso PC. A API usa `sql-server-db` porque está *dentro* da rede Docker).*

    ```powershell
    # Certifique-se de usar a MESMA senha que você definiu no Passo 2
    dotnet ef database update --connection "Server=localhost,1433;Database=GerenciadorContasDB;User Id=sa;Password=SuaSenhaForte!123;TrustServerCertificate=True"
    ```

Se o comando for bem-sucedido, o seu banco de dados e a tabela `Contas` foram criados.

### 5\. Executar o Frontend (.NET MAUI)

Agora que o backend está 100% operacional, podemos iniciar a interface gráfica.

1.  Abra a pasta do projeto (ou o ficheiro `GerenciadorContas.sln`) no VS Code ou Visual Studio.
2.  Configure o `GerenciadorContas.Maui` como o projeto de inicialização (Startup Project).
3.  Escolha o seu alvo de depuração (ex: **Windows Machine**, **Emulador Android**, etc.).
4.  Pressione **F5** ou "Iniciar Depuração".

A aplicação MAUI irá compilar e abrir. Ela irá conectar-se automaticamente ao `http://localhost:8080` (a sua API no Docker) e começar a funcionar.

-----

## Verificando o Funcionamento

### Testando o Backend (API)

  * Com o backend a ser executado (Passo 3), abra o seu navegador e vá para:
  * **`http://localhost:8080/swagger`**
  * Você verá a interface do Swagger, onde pode testar os endpoints `POST /api/contas` e `GET /api/contas/somatorio` manualmente.

### Testando o Frontend (MAUI)

  * Com a aplicação MAUI aberta (Passo 5):
  * Tente adicionar uma "Conta de Energia" com valor `150.50`.
  * Tente adicionar uma "Conta de Água" com valor `80.00`.
  * O "Total Gasto" deverá ser atualizado para `R$ 230,50` e o histórico deverá exibir os dois itens que você acabou de salvar.

-----

## Estrutura do Projeto

```
GERENCIADORCONTAS/
├── docker-compose.yml       # (A "comanda" que orquestra os contentores)
├── GerenciadorContas.sln    # (A Solução principal do .NET)
└── src/
    ├── GerenciadorContas.Api/   # (O Backend - API REST)
    │   ├── Controllers/         # (Endpoints da API, ex: ContasController)
    │   ├── Data/              # (AppDbContext do EF Core)
    │   ├── Models/            # (Modelos de dados, ex: Conta.cs)
    │   ├── appsettings.json     # (Configurações, incluindo a Connection String)
    │   └── Dockerfile           # (A "receita" para construir a imagem desta API)
    │
    └── GerenciadorContas.Maui/  # (O Frontend - App de UI)
        ├── Models/            # (Modelos de dados, ex: Conta.cs, SomatorioDto.cs)
        ├── Services/          # (IApiService e ApiService - o "encanamento" HTTP)
        ├── ViewModels/        # (O "cérebro" MVVM, ex: MainPageViewModel)
        ├── Views/             # (As telas XAML, ex: MainPage.xaml)
        ├── Platforms/         # (Código específico da plataforma, ex: AndroidManifest)
        └── MauiProgram.cs       # (Ponto de entrada, registo de serviços)
```
