FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus
WORKDIR /app
EXPOSE 5115

ENV ASPNETCORE_URLS=http://+:5115

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["src/Adapters/Driving/Api/Api.csproj", "src/Adapters/Driving/Api/"]
RUN dotnet restore "src/Adapters/Driving/Api/Api.csproj"
COPY . .
WORKDIR "/src/src/Adapters/Driving/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
