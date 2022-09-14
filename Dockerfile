FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# RUN apt-get update &&  apt-get install -y mediainfo

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/NasFileIndexer.Common/NasFileIndexer.Common.csproj", "NasFileIndexer.Common/"]
COPY ["src/NasFileIndexer/NasFileIndexer.csproj", "NasFileIndexer/"]

RUN dotnet restore "NasFileIndexer/NasFileIndexer.csproj"

COPY "src/NasFileIndexer.Common" "NasFileIndexer.Common/"
COPY "src/NasFileIndexer" "NasFileIndexer/"

WORKDIR "/src/NasFileIndexer"
RUN dotnet build "NasFileIndexer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NasFileIndexer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NasFileIndexer.dll"]
