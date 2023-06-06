FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy and restore solution file
COPY AggregateApp.sln .
COPY AggregateApp/AggregateApp.csproj ./AggregateApp/
COPY Core/Core.csproj ./Core/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/
RUN dotnet restore AggregateApp/AggregateApp.csproj

# Copy entire source code
COPY . .

# Build the application
WORKDIR /app/AggregateApp
ENV ASPNETCORE_HTTPS_PORT=https://+:5001
ENV ASPNETCORE_URLS=http://+:5000
RUN dotnet build AggregateApp.csproj -c Release -o /app/build
# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy built application
COPY --from=build /app/build .

# Start the application
CMD ["dotnet", "AggregateApp.dll"]
