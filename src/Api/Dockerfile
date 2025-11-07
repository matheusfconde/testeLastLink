# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os projetos necessários
COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Consumer/Consumer.csproj", "src/Consumer/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

# Restaura apenas o projeto principal (não a solution inteira)
RUN dotnet restore "src/Api/Api.csproj"

# Copia o restante do código
COPY . .

# Publica a aplicação (Api)
WORKDIR /src/src/Api
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Api.dll"]
