
ARG NETCORE_VERSION=2.2-bionic
FROM mcr.microsoft.com/dotnet/core/sdk:${NETCORE_VERSION} as build

ARG GITHUB_TOKEN

WORKDIR /src

COPY *.sln .
COPY *.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish -o ../dist

FROM mcr.microsoft.com/dotnet/core/runtime:${NETCORE_VERSION} as runtime

WORKDIR /app

COPY --from=build /dist ./

EXPOSE 8080

CMD ["dotnet", "CloudState.CSharpTemplate.dll"]