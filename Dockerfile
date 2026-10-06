FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HackerNews.Api/HackerNews.Api.csproj HackerNews.Api/
RUN dotnet restore HackerNews.Api/HackerNews.Api.csproj

COPY HackerNews.Api/ HackerNews.Api/
RUN dotnet publish HackerNews.Api/HackerNews.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "HackerNews.Api.dll"]
