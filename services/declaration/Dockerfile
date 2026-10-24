FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["services/declaration/Declaration.Api/Declaration.Api.csproj", "services/declaration/Declaration.Api/"]
COPY ["services/declaration/Declaration.Application/Declaration.Application.csproj", "services/declaration/Declaration.Application/"]
COPY ["services/declaration/Declaration.Domain/Declaration.Domain.csproj", "services/declaration/Declaration.Domain/"]
COPY ["services/declaration/Declaration.Infrastructure/Declaration.Infrastructure.csproj", "services/declaration/Declaration.Infrastructure/"]
COPY ["services/declaration/Declaration.Contracts/Declaration.Contracts.csproj", "services/declaration/Declaration.Contracts/"]
RUN dotnet restore "services/declaration/Declaration.Api/Declaration.Api.csproj"
COPY . .
WORKDIR "/src/services/declaration/Declaration.Api"
RUN dotnet build "Declaration.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Declaration.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER 1000
ENTRYPOINT ["dotnet", "Declaration.Api.dll"]
