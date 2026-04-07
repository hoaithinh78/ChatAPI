# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ChatR.sln .
# Copy csproj(s) và restore
COPY ChatR/ChatR.csproj ChatR/
RUN dotnet restore

# Copy source code
COPY ChatR/ ChatR/

# Build Release
RUN dotnet publish ChatR/ChatR.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy từ build stage
COPY --from=build /app/publish .

# Set environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose port
EXPOSE 80
EXPOSE 443

# Run the app
ENTRYPOINT ["dotnet", "ChatR.dll"]