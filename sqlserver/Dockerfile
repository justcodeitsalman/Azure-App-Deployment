FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

# Environment variables required by SQL Server
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=YourStrong!Passw0rd

# Copy optional initialization scripts
COPY init.sql /docker-entrypoint-initdb.d/

# Expose the SQL Server port
EXPOSE 1433

# SQL Server starts via the base image's entrypoint
