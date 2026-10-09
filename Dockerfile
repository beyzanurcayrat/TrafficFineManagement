# 1. Aşama: derleme (SDK imajı)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Önce sadece proje dosyalarını kopyalayıp paketleri indir (Docker katman önbelleği için)
COPY TrafficFineManagement.Entities/*.csproj TrafficFineManagement.Entities/
COPY TrafficFineManagement.DataAccess/*.csproj TrafficFineManagement.DataAccess/
COPY TrafficFineManagement.Business/*.csproj TrafficFineManagement.Business/
COPY TrafficFineManagement.Web/*.csproj TrafficFineManagement.Web/
RUN dotnet restore TrafficFineManagement.Web/TrafficFineManagement.Web.csproj

# Sonra kaynak kodu kopyalayıp yayınla
COPY . .
RUN dotnet publish TrafficFineManagement.Web/TrafficFineManagement.Web.csproj -c Release -o /app/publish --no-restore

# 2. Aşama: çalıştırma (sadece runtime içeren küçük imaj)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TrafficFineManagement.Web.dll"]