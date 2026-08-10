FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/OrderService/OrderService.API/OrderService.API.csproj", "src/OrderService/OrderService.API/"]
COPY ["src/OrderService/OrderService.Application/OrderService.Application.csproj", "src/OrderService/OrderService.Application/"]
COPY ["src/OrderService/OrderService.Domain/OrderService.Domain.csproj", "src/OrderService/OrderService.Domain/"]
COPY ["src/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "src/OrderService/OrderService.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.CQRS/BuildingBlocks.CQRS.csproj", "src/BuildingBlocks/BuildingBlocks.CQRS/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "src/BuildingBlocks/BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "src/BuildingBlocks/BuildingBlocks.Messaging/"]
RUN dotnet restore "src/OrderService/OrderService.API/OrderService.API.csproj"
COPY . .
WORKDIR "/src/src/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]