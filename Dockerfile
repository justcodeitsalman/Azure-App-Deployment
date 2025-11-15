FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY UserManagement.sln ./

# Copy main project csproj files
COPY UserManagement.Web/UserManagement.Web.csproj UserManagement.Web/
COPY UserManagement.Data/UserManagement.Data.csproj UserManagement.Data/
COPY UserManagement.Services/UserManagement.Services.csproj UserManagement.Services/

# Copy test project csproj files
COPY UserManagement.Data.Tests/UserManagement.Data.Tests.csproj UserManagement.Data.Tests/
COPY UserManagement.Services.Tests/UserManagement.Services.Tests.csproj UserManagement.Services.Tests/
COPY UserManagement.Web.Tests/UserManagement.Web.Tests.csproj UserManagement.Web.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish UserManagement.Web/UserManagement.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UserManagement.Web.dll"]
