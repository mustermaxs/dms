# Use .NET 8.0 ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET 8.0 SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["DMS.Api/DMS.Api.csproj", "DMS.Api/"]
COPY ["DMS.Application/DMS.Application.csproj", "DMS.Application/"]
COPY ["DMS.Domain/DMS.Domain.csproj", "DMS.Domain/"]
COPY ["DMS.Infrastructure/DMS.Infrastructure.csproj", "DMS.Infrastructure/"]
RUN dotnet restore "DMS.Api/DMS.Api.csproj"

# Copy the remaining source code
COPY . .

# Build the application
WORKDIR "/src/DMS.Api"
RUN dotnet build "DMS.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DMS.Api.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DMS.Api.dll"]
