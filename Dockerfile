FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Hanum.Pay.csproj", "."]
RUN dotnet nuget add source --username $NUGET_USERNAME --password $NUGET_AUTH_TOKEN --store-password-in-clear-text --name hanum "https://nuget.pkg.github.com/hansei-hanum/index.json"
RUN dotnet restore "./Hanum.Pay.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Hanum.Pay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hanum.Pay.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pay.dll"]