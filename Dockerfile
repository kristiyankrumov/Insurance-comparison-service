# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["InsuranceComparisonService/InsuranceComparisonService.csproj", "InsuranceComparisonService/"]
RUN dotnet restore "InsuranceComparisonService/InsuranceComparisonService.csproj"

# Copy remaining project files and build
COPY InsuranceComparisonService/. ./InsuranceComparisonService/
WORKDIR "/src/InsuranceComparisonService"
RUN dotnet build "InsuranceComparisonService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "InsuranceComparisonService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final production stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InsuranceComparisonService.dll"]
