FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["VideoService/VideoService.API/VideoService.API.csproj", "VideoService/VideoService.API/"]
COPY ["VideoService/VideoService.Application/VideoService.Application.csproj", "VideoService/VideoService.Application/"]
COPY ["VideoService/VideoService.Domain/VideoService.Domain.csproj", "VideoService/VideoService.Domain/"]
COPY ["VideoService/VideoService.Infrastructure/VideoService.Infrastructure.csproj", "VideoService/VideoService.Infrastructure/"]
COPY ["Shared.Domain/Shared.Domain.csproj", "Shared.Domain/"]
COPY ["Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared.Infrastructure/"]

RUN dotnet restore "VideoService/VideoService.API/VideoService.API.csproj"

COPY . .
WORKDIR "/src/VideoService/VideoService.API"
RUN dotnet build "VideoService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoService.API.dll"]