#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Journal/Journal.Infrastructure/Journal.Infrastructure.csproj", "Services/Journal/Journal.Infrastructure/"]
COPY ["BuildingBlocks/Extensions/Common.Util/Common.Util.csproj", "BuildingBlocks/Extensions/Common.Util/"]
COPY ["Services/Journal/Journal.Domain/Journal.Domain.csproj", "Services/Journal/Journal.Domain/"]
COPY ["BuildingBlocks/HostExtensions/Host.Customization/Host.Customization.csproj", "BuildingBlocks/HostExtensions/Host.Customization/"]
RUN dotnet restore "Services/Journal/Journal.Infrastructure/Journal.Infrastructure.csproj"
COPY . .
WORKDIR "/src/Services/Journal/Journal.Infrastructure"
RUN dotnet build "Journal.Infrastructure.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Journal.Infrastructure.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Journal.Infrastructure.dll"]