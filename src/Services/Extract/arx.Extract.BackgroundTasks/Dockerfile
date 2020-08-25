#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["arx.Extract.BackgroundTasks/arx.Extract.BackgroundTasks.csproj", "arx.Extract.BackgroundTasks/"]
RUN dotnet restore "arx.Extract.BackgroundTasks/arx.Extract.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/arx.Extract.BackgroundTasks"
RUN dotnet build "arx.Extract.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "arx.Extract.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "arx.Extract.BackgroundTasks.dll"]