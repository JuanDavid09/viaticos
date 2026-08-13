FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Viaticos.sln Directory.Build.props ./
COPY src/Viaticos.Domain/Viaticos.Domain.csproj src/Viaticos.Domain/
COPY src/Viaticos.Application/Viaticos.Application.csproj src/Viaticos.Application/
COPY src/Viaticos.Infrastructure/Viaticos.Infrastructure.csproj src/Viaticos.Infrastructure/
COPY src/Viaticos.Api/Viaticos.Api.csproj src/Viaticos.Api/

RUN dotnet restore src/Viaticos.Api/Viaticos.Api.csproj

COPY src/ src/
RUN dotnet publish src/Viaticos.Api/Viaticos.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Viaticos.Api.dll"]
