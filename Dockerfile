FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore HavocMatchmaking
# Build and publish a release
RUN dotnet publish -c Release -o out HavocMatchmaking

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 7777
ENTRYPOINT ["dotnet", "HavocMatchmaking.dll"]
