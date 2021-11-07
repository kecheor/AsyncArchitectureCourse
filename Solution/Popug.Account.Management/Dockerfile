#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Popug.Account.Management/Popug.Account.Management.csproj", "Popug.Account.Management/"]
RUN dotnet restore "Popug.Account.Management/Popug.Account.Management.csproj"
COPY . .
WORKDIR "/src/Popug.Account.Management"
RUN dotnet build "Popug.Account.Management.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Popug.Account.Management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Popug.Account.Management.dll"]