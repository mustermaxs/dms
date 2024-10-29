# Use .NET 8.0 ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# Uncomment the next line if you plan to use HTTPS
# EXPOSE 443

# Use a specific version of .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project files to restore dependencies
COPY ["DMS.Api/DMS.Api.csproj", "DMS.Api/"]
COPY ["DMS.Application/DMS.Application.csproj", "DMS.Application/"]
COPY ["DMS.Domain/DMS.Domain.csproj", "DMS.Domain/"]
COPY ["DMS.Infrastructure/DMS.Infrastructure.csproj", "DMS.Infrastructure/"]
COPY ["DMS.Api/log4net.config", "DMS.Api/"]

# Restore the project dependencies
RUN dotnet restore "DMS.Api/DMS.Api.csproj" --verbosity detailed

# Copy the remaining source code
COPY . .

# Build the application
WORKDIR "/src/DMS.Api"
RUN dotnet build "DMS.Api.csproj" -c Release -o /app/build --verbosity detailed

# Publish the application
FROM build AS publish
RUN dotnet publish "DMS.Api.csproj" -c Release -o /app/publish --verbosity detailed

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["DMS.Api/log4net.config", "/app/"]
ENTRYPOINT ["dotnet", "DMS.Api.dll"]

