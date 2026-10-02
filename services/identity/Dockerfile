FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY NuGet.config ./
COPY Directory.Build.props Directory.Build.targets ./
COPY services/identity/Identity.Api/Identity.Api.csproj services/identity/Identity.Api/
COPY services/identity/Identity.Application/Identity.Application.csproj services/identity/Identity.Application/
COPY services/identity/Identity.Domain/Identity.Domain.csproj services/identity/Identity.Domain/
COPY services/identity/Identity.Infrastructure/Identity.Infrastructure.csproj services/identity/Identity.Infrastructure/
COPY building-blocks/persistence/BuildingBlocks.Persistence.csproj building-blocks/persistence/
RUN dotnet restore services/identity/Identity.Api/Identity.Api.csproj

COPY . .
RUN dotnet publish services/identity/Identity.Api/Identity.Api.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Identity.Api.dll"]
