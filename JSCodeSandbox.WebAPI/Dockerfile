FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JSCodeSandbox.WebAPI/JSCodeSandbox.WebAPI.csproj", "JSCodeSandbox.WebAPI/"]
RUN dotnet restore "./JSCodeSandbox.WebAPI/JSCodeSandbox.WebAPI.csproj"
COPY . .
WORKDIR "/src/JSCodeSandbox.WebAPI"
RUN dotnet publish "./JSCodeSandbox.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER root
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
ARG APP_UID=1654
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JSCodeSandbox.WebAPI.dll"]