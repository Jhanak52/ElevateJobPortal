FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ElevateJobPortal.Web/ElevateJobPortal.Web.csproj ElevateJobPortal.Web/
RUN dotnet restore ElevateJobPortal.Web/ElevateJobPortal.Web.csproj
COPY . .
RUN dotnet publish ElevateJobPortal.Web/ElevateJobPortal.Web.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ElevateJobPortal.Web.dll"]
