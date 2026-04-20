FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
# Forzamos la publicación a una carpeta limpia
RUN dotnet publish "AtlasAPI/AtlasAPI.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Esta línea buscará el nombre real del archivo .dll y lo ejecutará
ENTRYPOINT ["sh", "-c", "dotnet $(ls *.dll | head -n 1)"]