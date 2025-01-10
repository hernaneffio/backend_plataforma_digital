FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5063

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infraestructure/Infraestructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Metroli_PDF/Metroli_PDF.csproj", "Metroli_PDF/"]
RUN dotnet restore "Metroli_PDF/Metroli_PDF.csproj"
COPY . .
WORKDIR "/src/Metroli_PDF"
RUN dotnet build "Metroli_PDF.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Metroli_PDF.csproj" -c Release -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_URLS=http://+:5063

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metroli_PDF.dll"]