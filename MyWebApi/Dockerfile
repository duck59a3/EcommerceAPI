# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
# Stage base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Stage build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj và restore NuGet
COPY ["MyWebApi/MyWebApi.csproj", "MyWebApi/"]
RUN dotnet restore "MyWebApi/MyWebApi.csproj"

# Copy toàn b? source và build
COPY . .
WORKDIR "/src/MyWebApi"
RUN dotnet build "MyWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyWebApi.dll"]
