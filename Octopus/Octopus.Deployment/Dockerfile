FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Octopus.Deployment/Octopus.Deployment.csproj", "Octopus.Deployment/"]
RUN dotnet restore "Octopus.Deployment/Octopus.Deployment.csproj"
COPY . .
WORKDIR "/src/Octopus.Deployment"
RUN dotnet build "Octopus.Deployment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Octopus.Deployment.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Octopus.Deployment.dll"]
