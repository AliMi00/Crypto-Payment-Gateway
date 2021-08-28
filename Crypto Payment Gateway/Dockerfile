#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Crypto Payment Gateway/Crypto Payment Gateway.csproj", "Crypto Payment Gateway/"]
RUN dotnet restore "Crypto Payment Gateway/Crypto Payment Gateway.csproj"
COPY . .
WORKDIR "/src/Crypto Payment Gateway"
RUN dotnet build "Crypto Payment Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crypto Payment Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crypto Payment Gateway.dll"]