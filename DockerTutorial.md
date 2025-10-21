
# Guia Completo: SQL Server no Docker (WSL) para .NET MAUI

Este guia completo irá ensiná-lo a:
1.  Instalar o **WSL 2** (Subsistema do Windows para Linux).
2.  Configurar a **memória do WSL** (etapa crítica para o SQL Server).
3.  Instalar o **Docker Engine** (diretamente no WSL, sem Docker Desktop).
4.  Iniciar um contêiner do **SQL Server**.
5.  Encontrar o **IP correto do WSL** para a conexão.
6.  Configurar seu projeto **.NET MAUI** para se conectar ao banco.
7.  Verificar a conexão com o **SSMS**.

---

## Parte 1: Instalar o WSL 2 (Subsistema do Windows para Linux)

O WSL nos permite rodar um ambiente Linux real (como o Ubuntu) diretamente no Windows. Usaremos ele para hospedar o Docker.

1.  **Abra o PowerShell como Administrador:**
    * Clique no Menu Iniciar.
    * Digite "PowerShell".
    * Clique em "Executar como Administrador".

2.  **Execute o Comando de Instalação:**
    Este comando único baixará e instalará o WSL e a distribuição "Ubuntu" (a mais recomendada).

    ```powershell
    wsl --install -d Ubuntu
    ```

3.  **Reinicie o Computador:**
    Após o término, o Windows pedirá para reiniciar. Faça isso.

4.  **Configure o Ubuntu:**
    * Após reiniciar, o terminal do Ubuntu abrirá automaticamente.
    * Ele pedirá para você criar um **Nome de Usuário** e uma **Senha**.
    * **Importante:** Este é o seu usuário *Linux*. Não tem nada a ver com seu usuário Windows. Anote essa senha, você a usará para comandos `sudo`.

5.  **Verifique a Versão (Opcional):**
    Abra um novo PowerShell e digite `wsl -l -v`. Você deve ver "Ubuntu" com a `VERSION` "2".

---

## Parte 2: Configurar a Memória do WSL (Etapa Crítica)

O SQL Server precisa de **pelo menos 2GB de RAM** para funcionar. Por padrão, o WSL pode não alocar isso, fazendo com que o contêiner do SQL Server falhe silenciosamente 1 minuto após iniciar. Vamos corrigir isso agora.

1.  **Vá para sua Pasta de Usuário no Windows:**
    * Abra o **Explorador de Arquivos**.
    * Na barra de endereço, digite `%USERPROFILE%` e pressione **Enter**. (Isso leva você a `C:\Users\SeuUsuario`).

2.  **Crie o Arquivo `.wslconfig`:**
    * Clique com o botão direito, vá em **Novo** > **Documento de Texto**.
    * Nomeie o arquivo como `.wslconfig` (com o ponto no início).
    * O Windows pode reclamar sobre "mudar a extensão". Confirme. Se ele mantiver o `.txt` no final (ficando `.wslconfig.txt`), você precisa habilitar a "Exibição de extensões de arquivo" no Explorador de Arquivos e renomear para tirar o `.txt`.

3.  **Edite o Arquivo:**
    Abra o `.wslconfig` com o Bloco de Notas ou VS Code e cole o seguinte:

    ```ini
    [wsl2]
    memory=4GB
    ```

4.  **Reinicie o WSL para Aplicar:**
    * Feche seu terminal Ubuntu (se estiver aberto).
    * Abra o **PowerShell** (como usuário normal) e execute o comando para desligar o WSL:

    ```powershell
    wsl --shutdown
    ```

Ao abrir o terminal do Ubuntu novamente, ele já estará com 4GB de RAM alocados.

---

## Parte 3: Instalar o Docker Engine (Dentro do Ubuntu)

Agora vamos instalar o Docker *dentro* do Ubuntu que acabamos de configurar.

1.  **Abra seu Terminal Ubuntu.**

2.  **Atualize os Pacotes:**
    ```bash
    sudo apt-get update
    ```
    (Será solicitada a senha do Linux que você criou na Parte 1).

3.  **Instale os Pacotes de Pré-requisito:**
    ```bash
    sudo apt-get install ca-certificates curl gnupg
    ```

4.  **Adicione a Chave GPG Oficial do Docker:**
    ```bash
    sudo install -m 0755 -d /etc/apt/keyrings
    curl -fsSL [https://download.docker.com/linux/ubuntu/gpg](https://download.docker.com/linux/ubuntu/gpg) | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    sudo chmod a+r /etc/apt/keyrings/docker.gpg
    ```

5.  **Adicione o Repositório do Docker:**
    ```bash
    echo \
      "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] [https://download.docker.com/linux/ubuntu](https://download.docker.com/linux/ubuntu) \
      $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
      sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
    ```

6.  **Instale o Docker Engine:**
    ```bash
    sudo apt-get update
    sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    ```

7.  **Adicione seu Usuário ao Grupo do Docker (Para não usar `sudo`):**
    Isso é importante para não ter que digitar `sudo` em todo comando do Docker.
    ```bash
    sudo usermod -aG docker $USER
    ```

8.  **FECHE E REABRA O TERMINAL UBUNTU.**
    Este passo é essencial para que a mudança de grupo tenha efeito.

9.  **Verifique a Instalação:**
    No novo terminal, digite (sem `sudo`):
    ```bash
    docker --version
    ```
    Você deve ver a versão do Docker ser impressa, confirmando que a instalação foi um sucesso.

---


## Parte 4: Definir e Iniciar o SQL Server com Docker Compose

Em vez de usar um longo comando `docker run`, vamos definir nosso serviço em um arquivo `docker-compose.yml`. Isso torna o ambiente fácil de compartilhar e reproduzir.

1.  **Crie o Arquivo `docker-compose.yml`:**
    
    -   Navegue até a pasta raiz do seu projeto (ex: `/mnt/c/Users/Pedro Andrade/Desktop/.NET_MAUI_ESTUDOS`).
        
    -   Crie um novo arquivo chamado `docker-compose.yml`.
        
    -   Cole o seguinte conteúdo nele:
        
    
    YAML
    
    ```
    # Versão do Docker Compose
    version: '3.8'
    
    # Aqui definimos nossos serviços (contêineres)
    services:
    
      # 1. O Serviço do SQL Server
      sql-server-db:
        image: mcr.microsoft.com/mssql/server:2022-latest
        container_name: sql_server_maui
    
        # Defina as variáveis de ambiente, incluindo sua senha forte
        environment:
          - ACCEPT_EULA=Y
          - MSSQL_SA_PASSWORD=TesteForte123! # Troque pela sua senha
    
        # Mapeia a porta 1433 do WSL para a porta 1433 do contêiner
        ports:
          - "1433:1433"
    
        # Define um volume para persistir os dados do banco
        volumes:
          - sqlserver-data:/var/opt/mssql
    
    # Define o volume (local de armazenamento)
    volumes:
      sqlserver-data:
        driver: local
    
    ```
    
    **Anote a senha (`TesteForte123!`) que você definiu.**
    
2.  **Inicie o Contêiner:**
    
    -   Abra seu **Terminal Ubuntu (WSL)**.
        
    -   Navegue até a pasta onde você salvou o `docker-compose.yml`.
        
    -   Execute o comando para iniciar os serviços em segundo plano:
        
    
    Bash
    
    ```
    docker compose up -d
    
    ```
    
3.  **Verifique se está Rodando:** Espere uns 20 segundos para o servidor iniciar e então execute:
    
    Bash
    
    ```
    docker compose ps
    
    ```
    
    Você deve ver o serviço `sql-server-db` com o `STATUS` "running (healthy)" ou "Up".

## Parte 5: Encontrar o IP do WSL (A Chave da Conexão)

Seu .NET MAUI e seu SSMS rodam no Windows. O Docker/SQL Server roda no WSL/Linux. Eles não são o mesmo "computador", então `localhost` não vai funcionar. Precisamos do IP do WSL.

1.  **Abra um terminal PowerShell (no Windows).**

2.  **Execute o Comando:**
    ```powershell
    wsl hostname -I
    ```

3.  **Anote o IP:**
    Ele mostrará um endereço de IP, por exemplo: `172.21.147.168`. **Este é o IP que usaremos no lugar de `localhost`**.

    *(Nota: Este IP pode mudar se você reiniciar o computador. Se a conexão parar de funcionar amanhã, execute este comando novamente para ver se o IP mudou).*

---

## Parte 6: Conectar seu Projeto .NET MAUI

Agora vamos ao seu código no VS Code.

1.  **Abra o arquivo `Data/AppDbContext.cs`** no seu projeto MAUI.
2.  **Ajuste a `ConnectionString`:**
    Localize o método `OnConfiguring`. Substitua `localhost` pelo IP que você acabou de encontrar.

    ```csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ATENÇÃO: Substitua 'SEU_IP_DO_WSL' pelo IP do Passo 5.
        // Use a senha que você definiu no comando 'docker run'.
        
        string connectionString = "Server=SEU_IP_DO_WSL,1433;Database=CadastroAlimentosDB_Final;User ID=sa;Password=TesteForte123!;TrustServerCertificate=True;";
        
        optionsBuilder.UseSqlServer(connectionString);
    }
    ```

3.  **Crie o Banco de Dados (Entity Framework):**
    * Abra um terminal **PowerShell** (não o WSL).
    * Navegue até a pasta do seu projeto:
        ```powershell
        cd "C:\Caminho\Para\Seu\Projeto\CadastroAlimentos9"
        ```
    * Execute o comando de migração:
        ```powershell
        dotnet ef database update --framework net9.0-windows10.0.19041.0
        ```
    Se tudo estiver correto (IP e senha), o Entity Framework se conectará ao contêiner e criará seu banco de dados.

---

## Parte 7: Conectar pelo SSMS (Verificação Final)

1.  Abra o **SQL Server Management Studio (SSMS)** no Windows.
2.  Preencha os dados da conexão:
    * **Server name:** `tcp:SEU_IP_DO_WSL` (O IP que você encontrou, ex: `172.21.147.168,1433`)
    * **Authentication:** `SQL Server Authentication`
    * **Login:** `sa`
    * **Password:** `TesteForte123!` (A senha que você usou no Docker)

3.  Clique em **Conectar**.

Pronto! Você está conectado ao seu SQL Server que está rodando isoladamente no Docker/WSL. Agora seu ambiente está completo e configurado da forma correta.
