FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["services/ectn/Ectn.Api/Ectn.Api.csproj", "services/ectn/Ectn.Api/"]
COPY ["services/ectn/Ectn.Application/Ectn.Application.csproj", "services/ectn/Ectn.Application/"]
COPY ["services/ectn/Ectn.Domain/Ectn.Domain.csproj", "services/ectn/Ectn.Domain/"]
COPY ["services/ectn/Ectn.Infrastructure/Ectn.Infrastructure.csproj", "services/ectn/Ectn.Infrastructure/"]
COPY ["services/ectn/Ectn.Contracts/Ectn.Contracts.csproj", "services/ectn/Ectn.Contracts/"]
RUN dotnet restore "services/ectn/Ectn.Api/Ectn.Api.csproj"

COPY . .
WORKDIR "/src/services/ectn/Ectn.Api"
RUN dotnet build "Ectn.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ectn.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Run as non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Ectn.Api.dll"]
