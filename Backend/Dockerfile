# https://hub.docker.com/_/microsoft-dotnet
# First stage: setup the build environment and restore packages
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Second stage: build the app
FROM build-env AS build
WORKDIR /app
COPY . ./
RUN dotnet build -c Release -o out

# Third stage: publish the app
FROM build AS publish
WORKDIR /app
RUN dotnet publish -c Release -o out

# Final stage: setup the runtime environment and copy the published app
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "TeamProject.dll"]