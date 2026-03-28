FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/VulnerableApi/VulnerableApi.csproj", "src/VulnerableApi/"]
RUN dotnet restore "src/VulnerableApi/VulnerableApi.csproj"
COPY . .
WORKDIR "/src/src/VulnerableApi"
RUN dotnet build "VulnerableApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VulnerableApi.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN useradd --no-create-home --shell /bin/false appuser && chown -R appuser /app
USER appuser
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/swagger/index.html || exit 1
ENTRYPOINT ["dotnet", "VulnerableApi.dll"]
