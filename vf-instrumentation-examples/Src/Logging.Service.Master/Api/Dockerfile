#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Src/Logging.Service.Master/Api/Master.csproj", "Src/Logging.Service.Master/Api/"]
COPY ["Src/Logging.Service.Master/Domain/Domain.csproj", "Src/Logging.Service.Master/Domain/"]
COPY ["Src/Logging.Shared/Shared.csproj", "Src/Logging.Shared/"]
COPY ["Src/Logging.Service.Master/Application/Application.csproj", "Src/Logging.Service.Master/Application/"]
COPY ["Src/Logging.Service.Master/Common/Common.csproj", "Src/Logging.Service.Master/Common/"]
COPY ["Src/Logging.RedisCache/RedisCache.csproj", "Src/Logging.RedisCache/"]
COPY ["Src/Logging.Service.Master/Infrastructure/Infrastrucure.csproj", "Src/Logging.Service.Master/Infrastructure/"]
COPY ["Src/Logging.Service.Master/Persistence/Persistence.csproj", "Src/Logging.Service.Master/Persistence/"]
RUN dotnet restore "Src/Logging.Service.Master/Api/Master.csproj"
COPY . .
WORKDIR "/src/Src/Logging.Service.Master/Api"
RUN dotnet build "Master.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Master.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Master.dll"]