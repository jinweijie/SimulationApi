############################################ build api ############################################

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-api
WORKDIR /app

# copy project file first to cache layer
COPY ./*.csproj ./
RUN dotnet restore

# copy and publish app and libraries
COPY ./. ./
RUN dotnet publish -c Release -o out

############################################ runtime api ##########################################

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS runtime-api
WORKDIR /app
COPY --from=build-api /app/out ./
ENTRYPOINT ["dotnet", "SimulationApi.dll"]
