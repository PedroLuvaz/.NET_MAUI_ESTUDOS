
# Guia Completo: SQL Server no Docker (WSL) para .NET MAUI

Este guia completo e detalhado irá ensiná-lo a:
1.  Instalar o **WSL 2** (Subsistema do Windows para Linux).
2.  Configurar a **memória do WSL** (etapa crítica para o SQL Server).
3.  Instalar o **Docker Engine** (diretamente no WSL, sem Docker Desktop).
4.  Iniciar um contêiner do **SQL Server**.
5.  Encontrar o **IP correto do WSL** para a conexão.
6.  Configurar seu projeto **.NET MAUI** para se conectar ao banco.
7.  Verificar a conexão com o **SSMS**.
8.  Resolver problemas comuns (Troubleshooting).

**⏱️ Tempo Estimado:** 30-45 minutos

---

## Parte 1: Instalar o WSL 2 (Subsistema do Windows para Linux)

### 🎯 O que vamos fazer?
Instalar um "mini-Linux" dentro do Windows para rodar o Docker.

### 📋 Passo a Passo Detalhado:

#### **Passo 1.1: Abrir o PowerShell como Administrador**

1. **Clique no botão Iniciar** do Windows (ícone do Windows no canto inferior esquerdo da tela)
   
2. **Digite:** `PowerShell` (sem pressionar Enter ainda!)
   
3. **Você verá aparecer:** "Windows PowerShell" nos resultados da busca
   
4. **IMPORTANTE:** NÃO clique diretamente! Olhe para o lado direito onde aparece "Executar como administrador"
   
5. **Clique em:** "Executar como administrador" (ícone de escudo azul)
   
6. **Uma janela vai aparecer perguntando:** "Deseja permitir que este aplicativo faça alterações no seu dispositivo?"
   
7. **Clique em:** "Sim"

📸 **O que você deve ver agora:** Uma janela azul escura com texto branco (PowerShell) com o título "Administrador: Windows PowerShell"

---

#### **Passo 1.2: Instalar o WSL e Ubuntu**

1. **Na janela do PowerShell que você acabou de abrir**, digite (ou copie e cole) o seguinte comando:
   ```powershell
   wsl --install -d Ubuntu
   ```

2. **Pressione a tecla:** `Enter`

3. **O que vai acontecer:** Você verá várias mensagens aparecendo:
   - "Instalando: Plataforma de Máquina Virtual"
   - "Instalando: Subsistema do Windows para Linux"
   - "Instalando: Ubuntu"
   - Pode levar de 5 a 15 minutos dependendo da sua internet

4. **Aguarde até ver a mensagem:** "A operação foi concluída com êxito" ou "Successfully installed"

⚠️ **IMPORTANTE:** NÃO feche a janela do PowerShell ainda!

---

#### **Passo 1.3: Reiniciar o Computador**

1. **Depois que tudo terminar de instalar**, você verá uma mensagem pedindo para reiniciar o computador

2. **Salve todos os seus arquivos abertos** (documentos, navegador, etc.)

3. **Clique no botão Iniciar** do Windows

4. **Clique no ícone de energia** (⚡)

5. **Clique em:** "Reiniciar"

6. **Aguarde o computador reiniciar** (pode levar alguns minutos)

---

#### **Passo 1.4: Configurar o Ubuntu (PRIMEIRA VEZ)**

1. **Após o computador reiniciar**, aguarde alguns segundos

2. **Uma janela PRETA** vai abrir automaticamente com o título "Ubuntu"
   - Se não abrir automaticamente, pressione a tecla Windows, digite "Ubuntu" e clique no aplicativo

3. **Você verá a mensagem:** "Installing, this may take a few minutes..." 
   - **Aguarde!** Pode levar de 1 a 5 minutos

4. **Quando aparecer:** "Enter new UNIX username:" 
   - **Digite um nome de usuário** (use só letras minúsculas, sem espaços)
   - Exemplo: `pedro` ou `usuario`
   - **Pressione Enter**

5. **Quando aparecer:** "New password:"
   - **Digite uma senha** (você NÃO verá os caracteres enquanto digita - é normal!)
   - **ANOTE ESTA SENHA!** Você vai precisar dela várias vezes
   - **Pressione Enter**

6. **Quando aparecer:** "Retype new password:"
   - **Digite a mesma senha novamente**
   - **Pressione Enter**

7. **Pronto!** Você verá algo como:
   ```
   usuario@NOME-DO-PC:~$
   ```
   Este é o terminal do Ubuntu!

📝 **ANOTE EM UM PAPEL:**
- Nome de usuário Linux: ________________
- Senha Linux: ________________

---

#### **Passo 1.5: Verificar se deu tudo certo**

1. **Abra um novo PowerShell** (NÃO precisa ser como administrador desta vez):
   - Pressione a tecla Windows
   - Digite: `PowerShell`
   - Clique em "Windows PowerShell"

2. **Digite o comando:**
   ```powershell
   wsl -l -v
   ```

3. **Pressione Enter**

4. **O que você DEVE ver:**
   ```
   NAME      STATE      VERSION
   Ubuntu    Running    2
   ```

✅ **Se você viu "VERSION 2", parabéns!** O WSL 2 está instalado corretamente!

❌ **Se você viu "VERSION 1"**, digite:
```powershell
wsl --set-version Ubuntu 2
```

**✅ Checkpoint:** Você agora tem o Ubuntu rodando no WSL 2.

---

## Parte 2: Configurar a Memória do WSL (Etapa Crítica)

### 🎯 Por que fazer isso?
O SQL Server precisa de pelo menos 2GB de RAM. Sem essa configuração, o Docker vai iniciar mas vai parar sozinho após 1 minuto!

### 📋 Passo a Passo Detalhado:

#### **Passo 2.1: Abrir sua Pasta de Usuário**

1. **Abra o Explorador de Arquivos:**
   - Pressione as teclas `Windows + E` ao mesmo tempo
   - OU clique no ícone de pasta amarela na barra de tarefas

2. **Na barra de endereço** (a barra branca no topo que mostra o caminho), clique dentro dela

3. **Apague tudo que estiver escrito lá**

4. **Digite exatamente:** `%USERPROFILE%`

5. **Pressione a tecla:** `Enter`

📸 **O que você deve ver:** Uma pasta com seu nome, contendo pastas como "Documentos", "Downloads", "Área de Trabalho", etc.

---

#### **Passo 2.2: Criar o Arquivo .wslconfig**

⚠️ **ATENÇÃO:** Este passo é um pouco chato, mas é MUITO importante!

##### **Parte A: Habilitar a visualização de extensões de arquivo**

1. **No Explorador de Arquivos que você abriu**, clique na aba **"Exibir"** (no topo)

2. **Procure por:** "Extensões de nomes de arquivo"

3. **Marque a caixinha** ao lado de "Extensões de nomes de arquivo"

📸 **O que muda:** Agora você verá ".txt", ".pdf", etc. no final dos nomes dos arquivos

##### **Parte B: Criar o arquivo**

1. **Clique com o botão DIREITO** do mouse em qualquer espaço vazio da pasta

2. **Passe o mouse sobre:** "Novo"

3. **Clique em:** "Documento de Texto"

📸 **O que acontece:** Um arquivo chamado "Novo Documento de Texto.txt" aparece

4. **IMPORTANTE:** Apague COMPLETAMENTE o nome (inclusive o .txt)

5. **Digite o novo nome:** `.wslconfig` (com o ponto no início!)

6. **Pressione Enter**

7. **Uma mensagem vai aparecer:** "Se você alterar a extensão do nome do arquivo, o arquivo pode ficar inutilizável. Deseja alterá-la assim mesmo?"

8. **Clique em:** "Sim"

✅ **Sucesso!** Você deve ver um arquivo chamado `.wslconfig` (sem ícone do bloco de notas)

---

#### **Passo 2.3: Editar o Arquivo .wslconfig**

1. **Clique com o botão DIREITO** no arquivo `.wslconfig` que você acabou de criar

2. **Clique em:** "Abrir com..."

3. **Escolha:** "Bloco de Notas" (ou "Notepad")
   - Se não aparecer na lista, clique em "Mais aplicativos" e procure por "Bloco de Notas"

4. **Uma janela do Bloco de Notas vai abrir** (vazia)

5. **Digite EXATAMENTE isto** (ou copie e cole):
   ```ini
   [wsl2]
   memory=4GB
   ```

📸 **O que você deve ver no Bloco de Notas:**
```
[wsl2]
memory=4GB
```

6. **Salve o arquivo:**
   - Clique em "Arquivo" (no topo)
   - Clique em "Salvar"
   - OU pressione `Ctrl + S`

7. **Feche o Bloco de Notas**

---

#### **Passo 2.4: Reiniciar o WSL**

##### **Parte A: Fechar o Ubuntu**

1. **Se você tem alguma janela do Ubuntu aberta**, feche-a clicando no X

##### **Parte B: Desligar o WSL**

1. **Abra o PowerShell** (não precisa ser como administrador):
   - Pressione a tecla Windows
   - Digite: `PowerShell`
   - Clique em "Windows PowerShell"

2. **Digite o comando:**
   ```powershell
   wsl --shutdown
   ```

3. **Pressione Enter**

4. **Aguarde 8 segundos** (conte mentalmente: 1... 2... 3... até 8)

📸 **O que acontece:** Nada! Isso é normal, o comando não mostra mensagem.

##### **Parte C: Verificar se funcionou**

1. **Abra o Ubuntu novamente:**
   - Pressione a tecla Windows
   - Digite: `Ubuntu`
   - Clique em "Ubuntu"

2. **Aguarde o terminal abrir** (alguns segundos)

3. **Digite o comando:**
   ```bash
   free -h
   ```

4. **Pressione Enter**

5. **Procure pela linha que começa com "Mem:"**

📸 **O que você DEVE ver:** Um número próximo a 4.0G ou 3.9G na coluna "total"

✅ **Se você viu um número perto de 4GB, perfeito!**

**✅ Checkpoint:** O WSL está configurado com memória suficiente para o SQL Server.

---

## Parte 3: Instalar o Docker Engine (Dentro do Ubuntu)

### 🎯 O que vamos fazer?
Instalar o Docker (programa que cria e gerencia contêineres) dentro do Ubuntu.

### 📋 Passo a Passo Detalhado:

#### **Passo 3.1: Abrir o Terminal do Ubuntu**

1. **Pressione a tecla Windows** no teclado

2. **Digite:** `Ubuntu`

3. **Clique em:** "Ubuntu" (ícone laranja e branco)

📸 **O que você deve ver:** Uma janela preta com texto colorido e algo como `usuario@NOME-PC:~$`

---

#### **Passo 3.2: Atualizar a Lista de Pacotes**

💡 **Explicação simples:** Isso é como "atualizar a lista de aplicativos disponíveis" no Ubuntu.

1. **No terminal do Ubuntu**, digite (ou copie e cole):
   ```bash
   sudo apt-get update
   ```

2. **Pressione Enter**

3. **Vai pedir a senha:** Digite a senha do Linux que você criou na Parte 1
   - **IMPORTANTE:** Você NÃO vai ver a senha enquanto digita - é normal!
   - Digite a senha e pressione Enter

4. **Aguarde terminar** (várias linhas de texto vão aparecer, cerca de 10-30 segundos)

📸 **Quando terminar:** Você verá novamente `usuario@NOME-PC:~$`

---

#### **Passo 3.3: Instalar Pacotes Necessários**

1. **Digite o comando:**
   ```bash
   sudo apt-get install ca-certificates curl gnupg
   ```

2. **Pressione Enter**

3. **Vai aparecer:** "Do you want to continue? [Y/n]"

4. **Digite:** `Y` (a letra Y maiúscula)

5. **Pressione Enter**

6. **Aguarde a instalação** (pode levar 1-2 minutos)

---

#### **Passo 3.4: Preparar a Pasta para as Chaves do Docker**

💡 **O que isso faz:** Cria uma pasta segura para guardar as "chaves de segurança" do Docker.

1. **Digite o comando:**
   ```bash
   sudo install -m 0755 -d /etc/apt/keyrings
   ```

2. **Pressione Enter**

3. **Se pedir senha, digite** a senha do Linux novamente

📸 **Nada vai aparecer** - isso é normal!

---

#### **Passo 3.5: Baixar a Chave de Segurança do Docker**

⚠️ **COPIE E COLE ESTE COMANDO** - ele é grande e complicado!

1. **Copie TODO este comando** (clique para selecionar, depois Ctrl+C):
   ```bash
   curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
   ```

2. **No terminal do Ubuntu:**
   - **Clique com o botão DIREITO** do mouse
   - **Clique em:** "Paste" ou "Colar"
   - OU pressione `Ctrl + Shift + V` (não é Ctrl+V comum!)

3. **Pressione Enter**

4. **Aguarde alguns segundos**

---

#### **Passo 3.6: Dar Permissão para o Arquivo de Chave**

1. **Digite o comando:**
   ```bash
   sudo chmod a+r /etc/apt/keyrings/docker.gpg
   ```

2. **Pressione Enter**

---

#### **Passo 3.7: Adicionar o Repositório do Docker**

⚠️ **ATENÇÃO:** Este é o comando MAIS LONGO! Copie e cole com MUITO cuidado!

1. **Copie TODO este bloco** (são 4 linhas):
   ```bash
   echo \
     "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
     $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
     sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
   ```

2. **Cole no terminal do Ubuntu** (botão direito → Paste, ou Ctrl+Shift+V)

3. **Pressione Enter**

4. **Se pedir senha, digite** a senha do Linux

---

#### **Passo 3.8: Atualizar a Lista Novamente**

1. **Digite:**
   ```bash
   sudo apt-get update
   ```

2. **Pressione Enter**

3. **Aguarde terminar**

---

#### **Passo 3.9: INSTALAR O DOCKER! 🎉**

💡 **Este é o passo principal!** Vai demorar um pouco (3-5 minutos).

1. **Copie este comando:**
   ```bash
   sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
   ```

2. **Cole no terminal**

3. **Pressione Enter**

4. **Quando perguntar:** "Do you want to continue? [Y/n]"
   - Digite: `Y`
   - Pressione Enter

5. **AGUARDE A INSTALAÇÃO** (muuuito texto vai aparecer, pode levar 3-5 minutos)

📸 **Quando terminar:** Você verá novamente `usuario@NOME-PC:~$`

---

#### **Passo 3.10: Adicionar Seu Usuário ao Grupo do Docker**

💡 **Por que fazer isso:** Para não precisar digitar `sudo` antes de todo comando Docker.

1. **Digite:**
   ```bash
   sudo usermod -aG docker $USER
   ```

2. **Pressione Enter**

📸 **Nada vai aparecer** - é normal!

---

#### **Passo 3.11: FECHAR E REABRIR O UBUNTU** ⚠️ **MUITO IMPORTANTE!**

1. **Feche a janela do Ubuntu** (clique no X ou digite `exit` e Enter)

2. **Aguarde 5 segundos**

3. **Abra o Ubuntu novamente:**
   - Tecla Windows
   - Digite: `Ubuntu`
   - Clique em "Ubuntu"

---

#### **Passo 3.12: Verificar se o Docker foi Instalado**

1. **No novo terminal do Ubuntu**, digite:
   ```bash
   docker --version
   ```

2. **Pressione Enter**

📸 **O que você DEVE ver:** Algo como "Docker version 24.0.7, build..."

3. **Agora digite:**
   ```bash
   docker compose version
   ```

4. **Pressione Enter**

📸 **O que você DEVE ver:** Algo como "Docker Compose version v2.23.0"

✅ **Se você viu as duas versões, PARABÉNS!** O Docker está instalado!

---

#### **Passo 3.13: Iniciar o Serviço do Docker**

⚠️ **IMPORTANTE:** O Docker não inicia sozinho! Você precisa fazer isso.

1. **Digite:**
   ```bash
   sudo service docker start
   ```

2. **Pressione Enter**

3. **Digite a senha do Linux** (se pedir)

📸 **O que você deve ver:** "* Starting Docker: docker" e depois "[ OK ]"

4. **Para confirmar que está rodando, digite:**
   ```bash
   sudo service docker status
   ```

5. **Pressione Enter**

📸 **O que você DEVE ver:** "Docker is running" (em verde)

6. **Para sair desta tela**, pressione a tecla `Q`

---

#### **Passo 3.14: (OPCIONAL) Fazer o Docker Iniciar Automaticamente**

💡 **Isso é útil para não ter que rodar `sudo service docker start` toda vez.**

1. **Digite este comando:**
   ```bash
   echo 'sudo service docker start > /dev/null 2>&1' >> ~/.bashrc
   ```

2. **Pressione Enter**

✅ **Pronto!** Agora o Docker vai iniciar sozinho quando você abrir o Ubuntu.

**✅ Checkpoint:** O Docker está instalado e rodando no WSL!

---


## Parte 4: Criar e Iniciar o SQL Server com Docker Compose

### 🎯 O que vamos fazer?
Criar um arquivo de configuração e iniciar o SQL Server em um contêiner Docker.

### 📋 Passo a Passo Detalhado:

#### **Passo 4.1: Navegar até a Pasta do Projeto**

1. **No terminal do Ubuntu**, digite este comando:
   ```bash
   cd /mnt/c/Users/"PC - Entrega  Mais"/Desktop/.NET_MAUI_ESTUDOS/mauiAlimentosTeste
   ```

2. **Pressione Enter**

💡 **Explicação:** `/mnt/c/` é como o Ubuntu "vê" o seu drive C: do Windows.

3. **Para confirmar que está na pasta certa, digite:**
   ```bash
   pwd
   ```

4. **Pressione Enter**

📸 **Você deve ver:** `/mnt/c/Users/PC - Entrega  Mais/Desktop/.NET_MAUI_ESTUDOS/mauiAlimentosTeste`

---

#### **Passo 4.2: Criar o Arquivo docker-compose.yml**

💡 **O que é isso:** Um arquivo de configuração que diz ao Docker como criar o SQL Server.

1. **Digite o comando:**
   ```bash
   nano docker-compose.yml
   ```

2. **Pressione Enter**

📸 **O que acontece:** Uma tela preta com linhas brancas abre (editor de texto Nano)

---

#### **Passo 4.3: Escrever a Configuração do SQL Server**

⚠️ **MUITO IMPORTANTE:** Copie e cole TODO o texto abaixo EXATAMENTE como está!

1. **Copie TODO este bloco** (todas as linhas):

```yaml
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
      - MSSQL_SA_PASSWORD=TesteForte123!
      - MSSQL_PID=Developer

    # Mapeia a porta 1433 do WSL para a porta 1433 do contêiner
    ports:
      - "1433:1433"

    # Define um volume para persistir os dados do banco
    volumes:
      - sqlserver-data:/var/opt/mssql

    # Configuração de saúde do contêiner
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "TesteForte123!" -Q "SELECT 1" -C || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s

# Define o volume (local de armazenamento)
volumes:
  sqlserver-data:
    driver: local
```

2. **No editor Nano que está aberto:**
   - **Clique com o botão DIREITO** do mouse
   - **Selecione:** "Paste" ou "Colar"
   - OU pressione `Ctrl + Shift + V`

📸 **O que você deve ver:** TODO o texto YAML colorido na tela

3. **IMPORTANTE - Sobre a Senha:**
   - A senha está definida como: `TesteForte123!`
   - **ANOTE ESTA SENHA** em um papel, você vai precisar dela!
   - Se quiser mudar, mude ONDE APARECER (tem 2 lugares: linha do MSSQL_SA_PASSWORD e linha do healthcheck)

📝 **ANOTE:**
- Senha do SQL Server: `TesteForte123!`

---

#### **Passo 4.4: Salvar o Arquivo**

1. **Pressione:** `Ctrl + O` (letra O, não zero)

2. **Vai aparecer:** "File Name to Write: docker-compose.yml"

3. **Pressione:** `Enter` (para confirmar o nome)

📸 **Você verá:** "[ Wrote 38 lines ]" (ou número parecido)

4. **Agora pressione:** `Ctrl + X` (para sair do Nano)

📸 **Você voltou para:** `usuario@NOME-PC:~/.../$`

---

#### **Passo 4.5: Verificar se o Arquivo foi Criado**

1. **Digite:**
   ```bash
   ls -la
   ```

2. **Pressione Enter**

📸 **Você DEVE ver na lista:** `docker-compose.yml`

✅ **Se você viu o arquivo, ótimo!** Continue.

---

#### **Passo 4.6: INICIAR O SQL SERVER! 🚀**

💡 **ESTE É O MOMENTO!** Vamos criar e iniciar o contêiner do SQL Server.

1. **Certifique-se de que o Docker está rodando:**
   ```bash
   sudo service docker start
   ```

2. **Pressione Enter** (digite a senha se pedir)

3. **Agora, digite o comando mágico:**
   ```bash
   docker compose up -d
   ```

4. **Pressione Enter**

📸 **O que vai acontecer:**
- Você verá: "Pulling sql-server-db..." (baixando a imagem do SQL Server)
- Isso pode demorar 2-5 minutos na primeira vez (são ~1.5GB para baixar)
- Você verá uma barra de progresso
- No final: "Container sql_server_maui  Started"

⚠️ **AGUARDE ATÉ VER "Started"** antes de continuar!

---

#### **Passo 4.7: Verificar se o SQL Server Está Rodando**

1. **Aguarde 30 segundos** (o SQL Server precisa de tempo para inicializar)

2. **Digite:**
   ```bash
   docker compose ps
   ```

3. **Pressione Enter**

📸 **O que você DEVE ver:**
```
NAME                IMAGE                                        STATUS                    
sql_server_maui     mcr.microsoft.com/mssql/server:2022-latest   Up 45 seconds (healthy)
```

✅ **SUCESSO!** Se você vê:
- `STATUS` = "Up" (está rodando)
- `(healthy)` = está saudável e pronto para usar

❌ **PROBLEMA:** Se você vê `Exited` ou `unhealthy`:

1. **Veja os logs para descobrir o erro:**
   ```bash
   docker compose logs sql-server-db
   ```

2. **Procure por mensagens de erro** (geralmente em vermelho)

3. **Erros comuns:**
   - "Password validation failed" = Senha muito fraca, mude para uma mais forte
   - "insufficient memory" = Volte para a Parte 2 e configure mais memória

**✅ Checkpoint:** O SQL Server está rodando no Docker e está saudável (healthy)!

---

## Parte 5: Encontrar o IP do WSL (A Chave da Conexão)

### 🎯 Por que precisamos disso?
O Windows e o WSL/Linux são "computadores diferentes" dentro do seu PC. Para o Windows falar com o SQL Server que está no WSL, precisamos do endereço IP do WSL.

### 📋 Passo a Passo Detalhado:

#### **Passo 5.1: Abrir o PowerShell no Windows**

1. **Pressione a tecla Windows** no teclado

2. **Digite:** `PowerShell`

3. **Clique em:** "Windows PowerShell" (NÃO precisa ser como administrador)

📸 **O que você verá:** Uma janela azul escura (PowerShell)

---

#### **Passo 5.2: Descobrir o IP do WSL**

1. **Digite o comando:**
   ```powershell
   wsl hostname -I
   ```

2. **Pressione Enter**

📸 **O que você verá:** Um número como: `172.21.147.168` ou `172.18.0.1` (pode variar)

3. **COPIE ESTE NÚMERO!** Você vai precisar dele várias vezes.

💡 **Dica:** Selecione o número com o mouse, clique com o botão direito para copiar.

📝 **ANOTE EM UM PAPEL:**
```
IP do WSL: _________________
(exemplo: 172.21.147.168)
```

⚠️ **IMPORTANTE:** Este IP pode mudar quando você reiniciar o computador! Se amanhã não conectar, volte aqui e veja se mudou.

---

#### **Passo 5.3: (Opcional) Testar se a Porta 1433 Está Aberta**

💡 **O que isso faz:** Verifica se o Windows consegue "falar" com o SQL Server.

1. **No PowerShell, digite** (substitua `172.21.147.168` pelo SEU IP):
   ```powershell
   Test-NetConnection -ComputerName 172.21.147.168 -Port 1433
   ```

2. **Pressione Enter**

3. **Aguarde 5-10 segundos**

📸 **O que você DEVE ver:**
```
TcpTestSucceeded : True
```

✅ **Se viu "True"**, perfeito! A conexão está funcionando.

❌ **Se viu "False"**, volte para a Parte 4 e verifique se o Docker está rodando.

**✅ Checkpoint:** Você tem o IP do WSL anotado!

---

## Parte 6: Conectar seu Projeto .NET MAUI ao SQL Server

### 🎯 O que vamos fazer?
Configurar seu aplicativo .NET MAUI para se conectar ao SQL Server que está rodando no Docker.

### 📋 Passo a Passo Detalhado:

#### **Passo 6.1: Abrir o VS Code na Pasta do Projeto**

1. **Pressione a tecla Windows**

2. **Digite:** `VS Code` ou `Visual Studio Code`

3. **Clique em:** "Visual Studio Code"

4. **Quando o VS Code abrir:**
   - Clique em "File" (Arquivo) no menu superior
   - Clique em "Open Folder..." (Abrir Pasta)
   
5. **Navegue até a pasta:**
   ```
   C:\Users\PC - Entrega  Mais\Desktop\.NET_MAUI_ESTUDOS\mauiAlimentosTeste\CadastroAlimentos9
   ```

6. **Clique no botão:** "Selecionar Pasta"

---

#### **Passo 6.2: Abrir o Arquivo AppDbContext.cs**

1. **No painel esquerdo do VS Code**, você verá uma lista de pastas

2. **Clique na pasta:** `Data` (para expandir)

3. **Clique no arquivo:** `AppDbContext.cs`

📸 **O que você verá:** O arquivo abre no editor com código C#

---

#### **Passo 6.3: Encontrar o Método OnConfiguring**

1. **Use a busca do VS Code:**
   - Pressione `Ctrl + F` (abre a caixa de busca)
   - Digite: `OnConfiguring`
   - Pressione Enter

2. **O cursor vai pular** para o método `OnConfiguring`

📸 **Você deve ver algo como:**
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    string connectionString = "Server=localhost,1433;...";
    ...
}
```

---

#### **Passo 6.4: Modificar a Connection String**

⚠️ **ATENÇÃO:** Vamos mudar o `localhost` pelo IP do WSL que você anotou!

1. **Procure pela linha que começa com:** `string connectionString =`

2. **Você vai ver algo como:**
   ```csharp
   string connectionString = "Server=localhost,1433;Database=...";
   ```

3. **Selecione a palavra:** `localhost`

4. **Delete e substitua pelo IP do WSL** que você anotou (exemplo: `172.21.147.168`)

5. **A linha completa DEVE ficar assim** (substitua o IP pelo SEU):
   ```csharp
   string connectionString = "Server=172.21.147.168,1433;Database=CadastroAlimentosDB_Final;User ID=sa;Password=TesteForte123!;TrustServerCertificate=True;Encrypt=True;";
   ```

📝 **Checklist - Verifique se sua connection string tem:**
- ✅ Seu IP do WSL (exemplo: 172.21.147.168)
- ✅ Porta: ,1433
- ✅ Database: CadastroAlimentosDB_Final
- ✅ User ID: sa
- ✅ Password: TesteForte123! (a mesma do docker-compose.yml)
- ✅ TrustServerCertificate=True
- ✅ Encrypt=True

6. **Salve o arquivo:**
   - Pressione `Ctrl + S`
   - OU clique em "File" → "Save"

📸 **Como saber que salvou:** O ponto branco no nome da aba desaparece

---

#### **Passo 6.5: Abrir o Terminal Integrado do VS Code**

1. **No VS Code, no menu superior:**
   - Clique em "Terminal"
   - Clique em "New Terminal" (Novo Terminal)

OU pressione: **Ctrl + '** (Ctrl + aspas simples)

📸 **O que acontece:** Um terminal aparece na parte inferior do VS Code

---

#### **Passo 6.6: Verificar se Está na Pasta Correta**

1. **No terminal que acabou de abrir**, olhe o caminho que aparece

📸 **Você DEVE estar em:**
```
PS C:\Users\PC - Entrega  Mais\Desktop\.NET_MAUI_ESTUDOS\mauiAlimentosTeste\CadastroAlimentos9>
```

❌ **Se NÃO estiver nesta pasta**, digite:
```powershell
cd "C:\Users\PC - Entrega  Mais\Desktop\.NET_MAUI_ESTUDOS\mauiAlimentosTeste\CadastroAlimentos9"
```

---

#### **Passo 6.7: Verificar se o Entity Framework Tools Está Instalado**

1. **Digite:**
   ```powershell
   dotnet ef --version
   ```

2. **Pressione Enter**

✅ **Se aparecer um número de versão** (exemplo: "Entity Framework Core .NET Command-line Tools 9.0.0"):
   - **Ótimo!** Pule para o Passo 6.8

❌ **Se aparecer** "'dotnet-ef' is not recognized":

1. **Digite para instalar:**
   ```powershell
   dotnet tool install --global dotnet-ef
   ```

2. **Pressione Enter**

3. **Aguarde a instalação** (1-2 minutos)

4. **Teste novamente:**
   ```powershell
   dotnet ef --version
   ```

---

#### **Passo 6.8: Criar o Banco de Dados! 🎉**

💡 **ESTE É O MOMENTO:** O Entity Framework vai se conectar ao SQL Server e criar seu banco de dados!

1. **Digite o comando:**
   ```powershell
   dotnet ef database update --framework net9.0-windows10.0.19041.0
   ```

2. **Pressione Enter**

3. **O que vai acontecer:**
   - Você verá: "Build started..."
   - Depois: "Build succeeded."
   - Depois: "Applying migration '20251015135232_InitialCreate'..."
   - E outras migrações sendo aplicadas
   - No final: "Done."

⏱️ **Tempo:** Pode levar 30 segundos a 2 minutos

📸 **SUCESSO - O que você DEVE ver:**
```
Build started...
Build succeeded.
Applying migration '20251015135232_InitialCreate'.
Applying migration '20251017190038_InitialCreate2234'.
Done.
```

✅ **PARABÉNS!** Seu banco de dados foi criado com sucesso!

---

#### **Passo 6.9: O Que Fazer se Der Erro**

❌ **Erro:** "A network-related or instance-specific error occurred"

**Solução:**

1. **Verifique se o IP está correto:**
   - Abra um novo PowerShell
   - Digite: `wsl hostname -I`
   - Compare com o IP no seu `AppDbContext.cs`

2. **Verifique se o Docker está rodando:**
   - Abra o Ubuntu
   - Digite: `docker compose ps`
   - Deve mostrar "Up" e "(healthy)"

3. **Verifique se a senha está correta:**
   - Abra o `AppDbContext.cs`
   - Confira se a senha é: `TesteForte123!`

4. **Tente novamente:**
   ```powershell
   dotnet ef database update --framework net9.0-windows10.0.19041.0
   ```

**✅ Checkpoint:** Seu banco de dados foi criado e seu aplicativo .NET MAUI está conectado ao SQL Server!

---

## Parte 7: Conectar pelo SSMS (Verificação Final)

### 🎯 O que vamos fazer?
Conectar ao SQL Server usando o SQL Server Management Studio (SSMS) para ver visualmente seu banco de dados e tabelas.

### 📋 Passo a Passo Detalhado:

#### **Passo 7.1: Abrir o SQL Server Management Studio**

1. **Pressione a tecla Windows**

2. **Digite:** `SSMS` ou `SQL Server Management Studio`

3. **Clique em:** "Microsoft SQL Server Management Studio"

⏱️ **Aguarde o SSMS abrir** (pode demorar 10-30 segundos)

📸 **O que você verá:** Uma janela com o título "Connect to Server" (Conectar ao Servidor)

---

#### **Passo 7.2: Preencher os Dados de Conexão**

💡 **Você vai precisar:** Do IP do WSL que você anotou na Parte 5

##### **Campo 1: Server type**

1. **Clique na caixa:** "Server type"

2. **Selecione:** "Database Engine"

📸 **Deve ficar:** Server type: `Database Engine`

---

##### **Campo 2: Server name**

⚠️ **IMPORTANTE:** Aqui você vai colocar o IP do WSL!

1. **Clique na caixa:** "Server name"

2. **Digite:** `SEU_IP_DO_WSL,1433`
   
   **Exemplos:**
   - Se seu IP é `172.21.147.168`, digite: `172.21.147.168,1433`
   - Se seu IP é `172.18.0.1`, digite: `172.18.0.1,1433`

⚠️ **ATENÇÃO:** 
- Não esqueça a vírgula antes do 1433!
- Não use `localhost`
- Use o IP REAL que você anotou

---

##### **Campo 3: Authentication**

1. **Clique na caixa:** "Authentication"

2. **Selecione:** "SQL Server Authentication"

📸 **IMPORTANTE:** NÃO use "Windows Authentication"!

---

##### **Campo 4: Login**

1. **Clique na caixa:** "Login"

2. **Digite:** `sa`

💡 **Explicação:** `sa` é o usuário administrador padrão do SQL Server

---

##### **Campo 5: Password**

1. **Clique na caixa:** "Password"

2. **Digite a senha:** `TesteForte123!`
   - Ou a senha que você definiu no docker-compose.yml

3. **(Opcional)** Marque a caixinha: "Remember password" para não ter que digitar toda vez

---

#### **Passo 7.3: Conectar!**

📋 **Antes de clicar, verifique:**
- ✅ Server type: Database Engine
- ✅ Server name: SEU_IP,1433 (exemplo: 172.21.147.168,1433)
- ✅ Authentication: SQL Server Authentication
- ✅ Login: sa
- ✅ Password: TesteForte123!

1. **Clique no botão:** "Connect" (Conectar)

⏱️ **Aguarde** 2-5 segundos enquanto conecta...

---

#### **Passo 7.4: Verificar a Conexão**

📸 **SUCESSO - O que você DEVE ver:**

1. **No painel esquerdo** (Object Explorer), você verá:
   ```
   └─ SEU_IP,1433 (SQL Server 16.0.0 - sa)
      ├─ Databases
      ├─ Security
      ├─ Server Objects
      └─ ...
   ```

2. **Expanda "Databases"** (clique no triângulo/seta ao lado)

3. **Você DEVE ver:**
   - System Databases (com 4 bancos do sistema)
   - **CadastroAlimentosDB_Final** ← SEU BANCO DE DADOS! 🎉

---

#### **Passo 7.5: Explorar Seu Banco de Dados**

1. **Clique no triângulo ao lado de:** `CadastroAlimentosDB_Final`

2. **O banco se expande mostrando:**
   ```
   └─ CadastroAlimentosDB_Final
      ├─ Database Diagrams
      ├─ Tables
      ├─ Views
      ├─ External Resources
      └─ ...
   ```

3. **Expanda:** "Tables" (Tabelas)

4. **Você verá suas tabelas:**
   - `dbo.__EFMigrationsHistory` (controle de migrações)
   - `dbo.Alimentos` (ou outras tabelas do seu projeto)

5. **Para ver os dados de uma tabela:**
   - Clique com o botão **DIREITO** em `dbo.Alimentos`
   - Clique em: "Select Top 1000 Rows"
   - Uma janela se abre mostrando os dados!

📸 **Você está DENTRO do banco de dados!** Pode explorar, criar queries, ver dados!

---

#### **Passo 7.6: (Opcional) Salvar a Conexão**

💡 **Para não ter que digitar tudo isso de novo:**

1. **No Object Explorer**, clique com o **botão DIREITO** em seu servidor:
   `172.21.147.168,1433`

2. **Clique em:** "Register..." (Registrar)

3. **Na janela que abre:**
   - Deixe tudo como está
   - Clique em "Save" (Salvar)

✅ **Agora sempre que abrir o SSMS**, sua conexão estará salva!

---

#### **Passo 7.7: O Que Fazer se NÃO Conectar**

❌ **Erro:** "A network-related or instance-specific error occurred..."

**Checklist de Solução:**

1. **✅ Verifique o IP:**
   - Abra PowerShell
   - Digite: `wsl hostname -I`
   - Confira se é o MESMO que você colocou no SSMS

2. **✅ Verifique se o Docker está rodando:**
   - Abra Ubuntu
   - Digite: `docker compose ps`
   - Deve mostrar "Up" e "(healthy)"
   
   Se NÃO estiver rodando:
   ```bash
   sudo service docker start
   docker compose up -d
   ```

3. **✅ Teste a porta:**
   - Abra PowerShell
   - Digite (com SEU IP):
   ```powershell
   Test-NetConnection -ComputerName 172.21.147.168 -Port 1433
   ```
   - Deve mostrar: `TcpTestSucceeded : True`

4. **✅ Confira a senha:**
   - Abra o arquivo `docker-compose.yml` no Ubuntu
   - Veja qual é a senha em `MSSQL_SA_PASSWORD`
   - Use a MESMA no SSMS

5. **Reinicie o WSL:**
   ```powershell
   wsl --shutdown
   ```
   Aguarde 8 segundos, reabra o Ubuntu, inicie o Docker novamente

---

❌ **Erro:** "Login failed for user 'sa'"

**Solução:**
- A senha está ERRADA
- Confira no `docker-compose.yml`
- A senha padrão deste tutorial é: `TesteForte123!`

---

**✅ Checkpoint:** Você está conectado ao SQL Server via SSMS e pode ver seu banco de dados!

**🎉 PARABÉNS!** Você completou TODAS as etapas! Agora você tem:
- ✅ WSL 2 instalado e configurado
- ✅ Docker rodando no WSL
- ✅ SQL Server em um contêiner Docker
- ✅ Banco de dados criado
- ✅ .NET MAUI conectado ao banco
- ✅ SSMS conectado para gerenciar o banco

**💡 Dica Final:** Salve este tutorial! Você vai precisar dele sempre que reiniciar o computador para:
1. Iniciar o Docker: `sudo service docker start`
2. Iniciar o SQL Server: `docker compose up -d`
3. Ver o IP atual: `wsl hostname -I`

---

## Parte 8: Comandos Úteis do Docker

Aqui estão alguns comandos úteis para gerenciar seu ambiente Docker:

### Gerenciamento Básico

```bash
# Ver contêineres em execução
docker compose ps

# Ver logs do SQL Server
docker compose logs sql-server-db

# Ver logs em tempo real (sair com Ctrl+C)
docker compose logs -f sql-server-db

# Parar o SQL Server (sem remover dados)
docker compose stop

# Iniciar o SQL Server novamente
docker compose start

# Parar e remover o contêiner (dados persistem no volume)
docker compose down

# Parar e remover TUDO (incluindo dados!)
docker compose down -v
```

### Verificação de Status

```bash
# Ver todos os contêineres (incluindo parados)
docker ps -a

# Ver volumes criados
docker volume ls

# Ver uso de recursos
docker stats sql_server_maui

# Executar comandos dentro do contêiner
docker exec -it sql_server_maui /bin/bash
```

### Limpeza

```bash
# Remover contêineres parados
docker container prune

# Remover imagens não utilizadas
docker image prune

# Limpar tudo (cuidado!)
docker system prune -a
```

---

## Parte 9: Troubleshooting (Resolução de Problemas)

### Problema 1: "Cannot connect to the Docker daemon"

**Sintoma:** Ao executar comandos Docker, você vê: `Cannot connect to the Docker daemon at unix:///var/run/docker.sock`

**Solução:**
```bash
# Inicie o serviço Docker
sudo service docker start

# Verifique o status
sudo service docker status
```

### Problema 2: Contêiner inicia mas para após 1 minuto

**Sintoma:** O contêiner `sql_server_maui` aparece como "Exited" quando você executa `docker compose ps`.

**Causas Comuns:**
1. **Memória insuficiente** - Verifique se você configurou o `.wslconfig` corretamente (Parte 2).
2. **Senha fraca** - A senha deve ter pelo menos 8 caracteres com maiúsculas, minúsculas, números e símbolos.

**Solução:**
```bash
# Veja os logs para identificar o erro
docker compose logs sql-server-db

# Procure por mensagens como:
# "ERROR: Unable to set system administrator password"
# "ERROR: Password validation failed"
```

### Problema 3: "A network-related or instance-specific error"

**Sintoma:** Seu aplicativo .NET MAUI ou SSMS não consegue conectar ao SQL Server.

**Checklist de Solução:**

1. **Verifique se o contêiner está rodando:**
   ```bash
   docker compose ps
   ```
   Deve mostrar "running" e "healthy".

2. **Verifique o IP do WSL:**
   ```powershell
   wsl hostname -I
   ```
   Confirme que está usando o IP correto na connection string.

3. **Teste a conectividade da porta:**
   No PowerShell do Windows:
   ```powershell
   Test-NetConnection -ComputerName SEU_IP_DO_WSL -Port 1433
   ```
   Deve mostrar `TcpTestSucceeded : True`.

4. **Verifique o Firewall do Windows:**
   O WSL geralmente não precisa de regras especiais, mas se tiver problemas:
   - Vá em "Firewall do Windows Defender"
   - Clique em "Configurações Avançadas"
   - Crie uma regra de entrada para a porta 1433 (TCP)

5. **Reinicie o WSL:**
   ```powershell
   wsl --shutdown
   ```
   Aguarde 8 segundos e reabra o Ubuntu.

### Problema 4: IP do WSL muda após reiniciar

**Sintoma:** Tudo funcionava ontem, mas hoje não conecta mais.

**Solução:**
Após reiniciar o computador, o IP do WSL pode mudar. Execute novamente:
```powershell
wsl hostname -I
```
Atualize a connection string no seu projeto com o novo IP.

**💡 Solução Permanente (Avançado):**
Você pode configurar um IP estático para o WSL editando o arquivo `.wslconfig`:

```ini
[wsl2]
memory=4GB
networkingMode=mirrored
```

Com `networkingMode=mirrored` (disponível no Windows 11 22H2+), você pode usar `localhost` diretamente!

### Problema 5: "dotnet ef" não é reconhecido

**Sintoma:** Ao tentar executar migrations, você vê: `'dotnet-ef' is not recognized`.

**Solução:**
```powershell
# Instale o Entity Framework Tools globalmente
dotnet tool install --global dotnet-ef

# Ou atualize se já estiver instalado
dotnet tool update --global dotnet-ef

# Verifique a instalação
dotnet ef --version
```

### Problema 6: Volumes com dados antigos

**Sintoma:** Você fez mudanças no banco mas elas não aparecem, ou quer começar do zero.

**Solução:**
```bash
# Para totalmente e remove o contêiner E o volume
docker compose down -v

# Inicie novamente do zero
docker compose up -d

# Não esqueça de refazer as migrations no .NET MAUI
```

### Problema 7: WSL está lento ou não inicia

**Sintoma:** O WSL demora muito para iniciar ou congela.

**Solução:**
```powershell
# Desligue o WSL
wsl --shutdown

# Aguarde 8 segundos

# Reinicie o Ubuntu
wsl

# Se o problema persistir, reinicie o computador
```

---

## Parte 10: Boas Práticas e Dicas

### Segurança

1. **Nunca use a senha `sa` em produção.** Em ambientes reais:
   - Crie usuários específicos com permissões limitadas
   - Use variáveis de ambiente para senhas
   - Considere Azure Key Vault ou similar

2. **Não commite senhas no Git:**
   ```bash
   # Adicione ao .gitignore
   docker-compose.yml
   appsettings.json
   ```

### Performance

1. **Monitore o uso de recursos:**
   ```bash
   docker stats sql_server_maui
   ```

2. **Ajuste a memória do WSL conforme necessário:**
   - 4GB é bom para desenvolvimento
   - Para bancos grandes, considere 6GB ou 8GB
   - Sempre deixe pelo menos 4GB para o Windows

### Backup

**Para fazer backup do seu banco:**

```bash
# Entre no contêiner
docker exec -it sql_server_maui /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "TesteForte123!" -C

# Execute o backup (dentro do SQL)
BACKUP DATABASE CadastroAlimentosDB_Final TO DISK = '/var/opt/mssql/backup/meubackup.bak'
GO
EXIT

# Copie o backup para o Windows
docker cp sql_server_maui:/var/opt/mssql/backup/meubackup.bak ./
```

### Scripts Úteis

**Script para iniciar automaticamente o Docker no WSL:**

Adicione ao final do arquivo `~/.bashrc` no Ubuntu:

```bash
# Auto-start Docker
if ! service docker status > /dev/null 2>&1; then
    sudo service docker start > /dev/null 2>&1
fi
```

**Script PowerShell para obter o IP do WSL automaticamente:**

Salve como `Get-WSL-IP.ps1`:

```powershell
$wslIP = wsl hostname -I
Write-Host "IP do WSL: $wslIP" -ForegroundColor Green
Write-Host "Connection String:" -ForegroundColor Yellow
Write-Host "Server=$wslIP,1433;Database=CadastroAlimentosDB_Final;User ID=sa;Password=SUA_SENHA;TrustServerCertificate=True;Encrypt=True;"
```

---

## Resumo Rápido (Quick Reference)

### Setup Inicial (Faça uma vez)
```powershell
# 1. Instalar WSL
wsl --install -d Ubuntu

# 2. Configurar memória (.wslconfig)
# Crie o arquivo em %USERPROFILE%\.wslconfig com:
# [wsl2]
# memory=4GB

# 3. Reiniciar WSL
wsl --shutdown
```

### No Ubuntu (WSL)
```bash
# 1. Atualizar e instalar Docker
sudo apt-get update
sudo apt-get install ca-certificates curl gnupg
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update
sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# 2. Adicionar usuário ao grupo Docker
sudo usermod -aG docker $USER

# 3. Reiniciar terminal e iniciar Docker
sudo service docker start

# 4. Navegar até projeto e criar docker-compose.yml
cd /mnt/c/Users/"PC - Entrega  Mais"/Desktop/.NET_MAUI_ESTUDOS/mauiAlimentosTeste

# 5. Iniciar SQL Server
docker compose up -d
```

### No Windows (PowerShell)
```powershell
# Obter IP do WSL
wsl hostname -I

# Executar migrations
cd "C:\Users\PC - Entrega  Mais\Desktop\.NET_MAUI_ESTUDOS\mauiAlimentosTeste\CadastroAlimentos9"
dotnet ef database update --framework net9.0-windows10.0.19041.0
```

### Comandos Diários
```bash
# Iniciar SQL Server
docker compose up -d

# Parar SQL Server
docker compose stop

# Ver status
docker compose ps

# Ver logs
docker compose logs sql-server-db
```

---

## Recursos Adicionais

- **Documentação Oficial do Docker:** https://docs.docker.com/
- **Documentação do SQL Server no Docker:** https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker
- **Documentação do WSL:** https://learn.microsoft.com/en-us/windows/wsl/
- **Entity Framework Core:** https://learn.microsoft.com/en-us/ef/core/

---

**🎉 Parabéns!** Você agora tem um ambiente de desenvolvimento profissional com SQL Server rodando no Docker via WSL. Este setup é portátil, reproduzível e segue as melhores práticas da indústria.

Pronto! Seu ambiente está configurado corretamente. Agora você pode conectar ao SQL Server diretamente no Docker via WSL. Este setup é ideal para desenvolvimento local e mantém seu ambiente limpo e isolado.
