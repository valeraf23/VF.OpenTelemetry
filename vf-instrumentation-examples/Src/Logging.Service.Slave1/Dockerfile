#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Src/Logging.Service.Slave1/Slave1.csproj", "Src/Logging.Service.Slave1/"]
COPY ["Src/Logging.RedisCache/RedisCache.csproj", "Src/Logging.RedisCache/"]
COPY . .

WORKDIR "/src/Src/Logging.Service.Slave1"
RUN dotnet build "Slave1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Slave1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Slave1.dll"]