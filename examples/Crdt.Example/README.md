# EventSourced.ShoppingCart

This folder contains sample code for building an event sourced shopping cart
using a C# user function for CloudState.  It is maintained with direct project
references to the other CloudState.CSharpSupport libraries to help with 
development and debugging of the framework.

Since the EventSourced.ShoppingCart project references the other CSharpSupport 
projects directly, so it is not advisable to use this example to build upon.
Please see the examples/Template folder for a more portable version which 
depends on NuGet libraries.  Or, use the CloudState cli (TODO).

# Notes

- Requires a custom protobuf build due to the current version (3.10.1) having 
  several `internal` access modifiers which we need to be public.

# Prerequisites

- Docker
- protoc
- gRPC client (of your choice)

# Instructions

1. Initialize the protobuf submodule
2. Build the docker image

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

3. Run the CloudState Proxy operator image in dev mode
```
# Run the operator image first
docker run -it --rm \
    --name cloudstate \
    -p 9000:9000 \
    cloudstateio/cloudstate-proxy-dev-mode
```

4. Run the recently built docker image and attach it to the 
   network of the CloudState proxy container
```
# Run the user function and attach the network
docker run -it --rm \
    --name shopping-cart \
    --network container:cloudstate \
    nagytech/cloudstate-csharp/shopping-cart
```

5. Call the CloudState proxy and list all the services

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