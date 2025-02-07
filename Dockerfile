FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
WORKDIR /src
COPY ["Hanum.Pay.csproj", "."]
RUN dotnet nuget add source --username $NUGET_USERNAME --password $NUGET_PASSWORD --store-password-in-clear-text --name hanum "https://nuget.pkg.github.com/hansei-hanum/index.json"
RUN dotnet restore "./././Hanum.Pay.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Hanum.Pay.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Hanum.Pay.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Migrations/hanum /app/Migrations/hanum
ENTRYPOINT ["dotnet", "Hanum.Pay.dll"]