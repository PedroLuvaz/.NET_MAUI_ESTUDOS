# Estágio 1: O "Ambiente de Build"
# Usamos a imagem completa do SDK do .NET 9 (baseado na sua pasta 'bin')
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copia o arquivo de projeto primeiro. Isso é um truque de cache.
# Se o .csproj não mudou, o Docker reaproveita o 'dotnet restore'.
COPY GerenciadorContas.Api.csproj .
RUN dotnet restore "GerenciadorContas.Api.csproj"

# Copia todo o resto do código-fonte
COPY . .

# Publica o aplicativo, criando uma versão otimizada para 'Release'
RUN dotnet publish "GerenciadorContas.Api.csproj" -c Release -o /app/publish


# Estágio 2: O "Ambiente de Produção" (Final)
# Agora usamos uma imagem muito menor, que só tem o *necessário* para rodar a API
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

# O .NET 9 (e 8+) expõe a porta 8080 por padrão
EXPOSE 8080

# O comando final para iniciar sua API quando o contêiner rodar
ENTRYPOINT ["dotnet", "GerenciadorContas.Api.dll"]