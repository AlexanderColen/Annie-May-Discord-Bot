# https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/core/sdk:3.1
MAINTAINER Alexander Colen

# Create directory for application.
RUN mkdir ./AnnieMay

# Copy files to created directory.
COPY . ./AnnieMay

# Go into directory.
WORKDIR ./AnnieMay

# Build the app
RUN dotnet build AnnieMayDiscordBot.sln