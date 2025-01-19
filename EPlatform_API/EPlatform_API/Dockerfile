# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EPlatform_API.csproj", "EPlatform_API/"]
RUN dotnet restore "EPlatform_API/EPlatform_API.csproj"
WORKDIR /src/EPlatform_API
COPY . ./
RUN dotnet build "EPlatform_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c Release -o /app/out

FROM base AS final
ENV ASPNETCORE_HTTP_PORT=http://localhost:5119
ENV ASPNETCORE_HTTPS_PORT=http://localhost:5120
EXPOSE 5119:8080
EXPOSE 5120:8081
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "EPlatform_API.dll"]





