﻿FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProjectUCS.Server/ProjectUCS.Server.csproj", "ProjectUCS.Server/"]
RUN dotnet restore "ProjectUCS.Server/ProjectUCS.Server.csproj"
COPY . .
WORKDIR "/src/ProjectUCS.Server"
RUN dotnet build "ProjectUCS.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectUCS.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectUCS.Server.dll"]
