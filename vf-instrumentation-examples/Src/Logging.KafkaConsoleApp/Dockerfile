#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:2.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Src/Logging.KafkaConsoleApp/KafkaConsoleApp.csproj", "Src/Logging.KafkaConsoleApp/"]
COPY . .
WORKDIR "/src/Src/Logging.KafkaConsoleApp"
RUN dotnet build "KafkaConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaConsoleApp.dll"]