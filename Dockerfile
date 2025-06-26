FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore

RUN dotnet build --no-restore

RUN dotnet test --no-build \
    --logger "trx;LogFileName=test-results.trx" \
    --results-directory /artifacts
