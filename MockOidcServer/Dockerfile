FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MockOidcServer/MockOidcServer.csproj", "MockOidcServer/"]
RUN dotnet restore "MockOidcServer/MockOidcServer.csproj"
COPY . .
WORKDIR "/src/MockOidcServer"
RUN dotnet build "./MockOidcServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MockOidcServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS="https://+:8081;http://+:8080"
ENV ASPNETCORE_HTTPS_PORTS=8081
ENTRYPOINT ["dotnet", "MockOidcServer.dll"]
