#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Popug.Billing.Pricing/Popug.Billing.Pricing.csproj", "Popug.Billing.Pricing/"]
RUN dotnet restore "Popug.Billing.Pricing/Popug.Billing.Pricing.csproj"
COPY . .
WORKDIR "/src/Popug.Billing.Pricing"
RUN dotnet build "Popug.Billing.Pricing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Popug.Billing.Pricing.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Popug.Billing.Pricing.dll"]