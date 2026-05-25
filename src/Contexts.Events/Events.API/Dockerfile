FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/Contexts.Events/Events.API/Events.API.csproj"
WORKDIR "/src/src/Contexts.Events/Events.API"
RUN dotnet build "Events.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Events.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Events.API.dll"]
