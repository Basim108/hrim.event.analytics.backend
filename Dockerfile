FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Hrim.Event.Analytics.Api.csproj", "./"]
RUN dotnet restore "Hrim.Event.Analytics.sln"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Hrim.Event.Analytics.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hrim.Event.Analytics.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hrim.Event.Analytics.Api.dll"]
