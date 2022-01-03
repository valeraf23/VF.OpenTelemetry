#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Src/Logging.Service.PrometheusMetrics/PrometheusMetrics.csproj", "Src/Logging.Service.PrometheusMetrics/"]
RUN dotnet restore "Src/Logging.Service.PrometheusMetrics/PrometheusMetrics.csproj"
COPY . .
WORKDIR "/src/Src/Logging.Service.PrometheusMetrics"
RUN dotnet build "PrometheusMetrics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrometheusMetrics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrometheusMetrics.dll"]