#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов
COPY ["TransactionConsumer/TransactionConsumer.csproj", "TransactionConsumer/"]
COPY ["TransactionConsumer.Data/TransactionConsumer.Data.csproj", "TransactionConsumer.Data/"]
COPY ["TransactionConsumer.Services/TransactionConsumer.Services.csproj", "TransactionConsumer.Services/"]

# Восстанавливаем зависимости
RUN dotnet restore "TransactionConsumer/TransactionConsumer.csproj"

# Копируем весь исходный код
COPY . .

# Собираем приложение
WORKDIR "/src/TransactionConsumer"
RUN dotnet build "TransactionConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionConsumer.dll"]