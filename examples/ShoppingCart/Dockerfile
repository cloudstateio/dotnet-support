ARG artifact_path
ARG main_dll
ARG NETCORE_VERSION=2.2-bionic

FROM mcr.microsoft.com/dotnet/core/runtime:${NETCORE_VERSION} as runtime

ENV main_dll=$main_dll

WORKDIR /app

COPY $artifact_path .

EXPOSE 8080

CMD ["dotnet", $main_dll]
