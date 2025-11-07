# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os projetos necessários
COPY ["src/Consumer/Consumer.csproj", "src/Consumer/"]
COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

# Restaura apenas o projeto do Consumer (sem incluir o projeto de teste)
RUN dotnet restore "src/Consumer/Consumer.csproj"

# Copia o restante do código
COPY . .

# Publica o Consumer
WORKDIR /src/src/Consumer
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
