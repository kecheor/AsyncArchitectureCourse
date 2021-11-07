#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Popug.Accounts.IdentityServer/Popug.Accounts.IdentityServer.csproj", "Popug.Accounts.IdentityServer/"]
RUN dotnet restore "Popug.Accounts.IdentityServer/Popug.Accounts.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Popug.Accounts.IdentityServer"
RUN dotnet build "Popug.Accounts.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Popug.Accounts.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Popug.Accounts.IdentityServer.dll"]