#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 3000
EXPOSE 3001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["BookMartWeb/BookMartWeb.csproj", "BookMartWeb/"]
COPY ["BookMart.DataAccess/BookMart.DataAccess.csproj", "BookMart.DataAccess/"]
COPY ["BookMart.Models/BookMart.Models.csproj", "BookMart.Models/"]
COPY ["BookMart.Utility/BookMart.Utility.csproj", "BookMart.Utility/"]
RUN dotnet restore "./BookMartWeb/BookMartWeb.csproj"
COPY . .
WORKDIR "/src/BookMartWeb"
RUN dotnet build "./BookMartWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./BookMartWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookMartWeb.dll"]