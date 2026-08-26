FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update && \
    apt-get install -y --no-install-recommends ffmpeg && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MediaService/MediaService.API/MediaService.API.csproj", "MediaService/MediaService.API/"]
COPY ["MediaService/MediaService.Application/MediaService.Application.csproj", "MediaService/MediaService.Application/"]
COPY ["MediaService/MediaService.Domain/MediaService.Domain.csproj", "MediaService/MediaService.Domain/"]
COPY ["MediaService/MediaService.Infrastructure/MediaService.Infrastructure.csproj", "MediaService/MediaService.Infrastructure/"]
COPY ["Shared.Domain/Shared.Domain.csproj", "Shared.Domain/"]
COPY ["Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared.Infrastructure/"]

RUN dotnet restore "MediaService/MediaService.API/MediaService.API.csproj"

COPY . .
WORKDIR "/src/MediaService/MediaService.API"
RUN dotnet build "MediaService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.API.dll"]