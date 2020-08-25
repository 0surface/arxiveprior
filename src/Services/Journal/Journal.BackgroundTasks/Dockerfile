#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Journal/Journal.BackgroundTasks/Journal.BackgroundTasks.csproj", "Services/Journal/Journal.BackgroundTasks/"]
RUN dotnet restore "Services/Journal/Journal.BackgroundTasks/Journal.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/Services/Journal/Journal.BackgroundTasks"
RUN dotnet build "Journal.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Journal.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Journal.BackgroundTasks.dll"]