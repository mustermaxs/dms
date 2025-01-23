FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5672
EXPOSE 80
EXPOSE 8082


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OcrService.csproj", "."]
RUN dotnet restore "OcrService.csproj"

RUN apt-get update && \
    apt-get install -y tesseract-ocr libtesseract-dev && \
    rm -rf /var/lib/apt/lists/* 


COPY ["OcrService.csproj", "."]
COPY ["log4net.config", "."]
RUN dotnet restore "OcrService.csproj"
COPY . .
RUN dotnet build "OcrService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OcrService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["log4net.config", "/app/"]
ENTRYPOINT ["dotnet", "OcrService.dll"]
