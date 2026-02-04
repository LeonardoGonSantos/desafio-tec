FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FluxoCaixa.API/FluxoCaixa.API.csproj", "FluxoCaixa.API/"]
COPY ["src/FluxoCaixa.Application/FluxoCaixa.Application.csproj", "FluxoCaixa.Application/"]
COPY ["src/FluxoCaixa.Infrastructure/FluxoCaixa.Infrastructure.csproj", "FluxoCaixa.Infrastructure/"]
COPY ["src/FluxoCaixa.Domain.Lancamentos/FluxoCaixa.Domain.Lancamentos.csproj", "FluxoCaixa.Domain.Lancamentos/"]
COPY ["src/FluxoCaixa.Domain.Consolidado/FluxoCaixa.Domain.Consolidado.csproj", "FluxoCaixa.Domain.Consolidado/"]
COPY ["src/FluxoCaixa.Shared/FluxoCaixa.Shared.csproj", "FluxoCaixa.Shared/"]
RUN dotnet restore "FluxoCaixa.API/FluxoCaixa.API.csproj"
COPY src/ .
WORKDIR "/src/FluxoCaixa.API"
RUN dotnet build "FluxoCaixa.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FluxoCaixa.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FluxoCaixa.API.dll"]
