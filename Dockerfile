FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
ARG IMAGE_VER
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY . .
RUN dotnet restore 
RUN dotnet build -c Release --no-restore -o /app/build
RUN dotnet publish -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOCKER_IMAGE_TAG="${IMAGE_VER}"
ENTRYPOINT ["dotnet", "Hrim.Event.Analytics.Api.dll"]
