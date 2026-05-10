FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "AtlasAPI/AtlasAPI.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Forzamos a que escuche en todas las interfaces (0.0.0.0)
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet $(ls *.dll | head -n 1) --urls http://0.0.0.0:10000"]