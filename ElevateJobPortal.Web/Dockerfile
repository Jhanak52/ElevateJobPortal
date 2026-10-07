FROM mcr.microsoft.com/dotnet/aspnet:10.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 5080

ENV ASPNETCORE_URLS=http://+:5080

FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["ElevateJobPortal/ElevateJobPortal.Web/ElevateJobPortal.Web.csproj", "ElevateJobPortal/ElevateJobPortal.Web/"]
RUN dotnet restore "ElevateJobPortal\ElevateJobPortal.Web\ElevateJobPortal.Web.csproj"
COPY . .
WORKDIR "/src/ElevateJobPortal/ElevateJobPortal.Web"
RUN dotnet build "ElevateJobPortal.Web.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ElevateJobPortal.Web.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElevateJobPortal.Web.dll"]
