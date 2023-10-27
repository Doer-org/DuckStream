# https://github.com/dotnet/dotnet-docker/issues/4129
# https://github.com/dotnet/dotnet-docker/blob/57e9112e172fa162018c92c57e1910c020786a3a/samples/releasesapp/Dockerfile.alpine#L17
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY . .
RUN dotnet restore "Api/Api.fsproj" -a $TARGETARCH

RUN dotnet publish "Api/Api.fsproj" -a $TARGETARCH --no-restore -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "Api.dll" ]
