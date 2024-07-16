#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GHotel.API/GHotel.API.csproj", "GHotel.API/"]
COPY ["GHotel.Infrastructure/GHotel.Infrastructure.csproj", "GHotel.Infrastructure/"]
COPY ["GHotel.Persistance/GHotel.Persistance.csproj", "GHotel.Persistance/"]
COPY ["GHotel.Application/GHotel.Application.csproj", "GHotel.Application/"]
COPY ["GHotel.Domain/GHotel.Domain.csproj", "GHotel.Domain/"]
RUN dotnet restore "./GHotel.API/GHotel.API.csproj"
COPY . .
WORKDIR "/src/GHotel.API"
RUN dotnet build "./GHotel.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GHotel.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GHotel.API.dll"]
