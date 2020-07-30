#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet tool install -g dotnet-ef --version 3.1.1
WORKDIR /src
COPY ["Services/Journal/Journal.Infrastructure/Journal.Infrastructure.csproj", "Services/Journal/Journal.Infrastructure/"]
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