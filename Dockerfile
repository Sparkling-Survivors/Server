# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/runtime:7.0

# Set the working directory in the container
WORKDIR /app

# Copy the built .exe and other necessary files from the local machine to the container
COPY Server/bin/Release/net7.0/linux-x64 .

# Expose the port that your application listens on (optional)
EXPOSE 7777

# Set the entry point for the container to run the .exe file
CMD ["dotnet", "Server.dll"]      
   