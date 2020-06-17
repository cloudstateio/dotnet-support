# Getting started

## Prerequisites

### .Net Core SDK version
Cloudstate .Net support requires .Net Core sdk $cloudstate.dotnet.version$.

## Creating Entity

You must create an Entity Function with your business logic. @ref:[Here](eventsourced.md) and @ref:[here](crdt.md) you will find good examples of how to do this for EventSourced and CRDT entities respectively.

## Creating a main function

Your main class will be responsible for creating the Cloudstate gRPC server, registering the entities for your placement and starting it:

@@snip [Program.cs]($base$/docs/src/test/eventsourced/Program.cs) { #main }

We will see more details on creating entities in the coming pages.

## Run with Docker

Cloudstate requires you to run your applications via [Docker](https://www.docker.com/) containers. Here we explain how you can package your applications and how you can test them locally.

### Instructions

* Create Dockerfile

```
ARG MAIN_DLL
ARG ARTIFACT_PATH
ARG NETCORE_VERSION=2.2
FROM mcr.microsoft.com/dotnet/core/runtime:$NETCORE_VERSION as runtime

ARG MAIN_DLL
ARG ARTIFACT_PATH

ENV MAIN_DLL=${MAIN_DLL}

RUN env

COPY ${ARTIFACT_PATH}/ /app

EXPOSE 8080

ENTRYPOINT dotnet /app/$MAIN_DLL
```

* Build the docker image

```
# Build the docker image
CONFIGURATION="Debug"
FRAMEWORK="netcoreapp2.2"

dotnet publish -c $CONFIGURATION -f $FRAMEWORK &&
docker build \
    -t nagytech/cloudstate-csharp/shopping-cart \
    --build-arg ARTIFACT_PATH="bin/$CONFIGURATION/$FRAMEWORK/publish" \
    --build-arg MAIN_DLL="EventSourced.ShoppingCart.dll" \
    --no-cache .
```

* Run the CloudState Proxy operator image in dev mode

```
# Run the operator image first
docker run -it --rm \
    --name cloudstate \
    -p 9000:9000 \
    cloudstateio/cloudstate-proxy-dev-mode
```

* Run the recently built docker image and attach it to the 
   network of the CloudState proxy container

```
# Run the user function and attach the network
docker run -it --rm \
    --name shopping-cart \
    --network container:cloudstate \
    nagytech/cloudstate-csharp/shopping-cart
```

* Call the CloudState proxy and list all the services

```
# Example using grpc_cli
> grpc_cli ls localhost:9000 -l

filename: example/shoppingcart/shoppingcart.proto
package: com.example.shoppingcart;
service ShoppingCart {
  rpc AddItem(com.example.shoppingcart.AddLineItem) returns (google.protobuf.Empty) {}
  rpc RemoveItem(com.example.shoppingcart.RemoveLineItem) returns (google.protobuf.Empty) {}
  rpc GetCart(com.example.shoppingcart.GetShoppingCart) returns (com.example.shoppingcart.Cart) {}
}

```

