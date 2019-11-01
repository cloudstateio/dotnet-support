CONFIGURATION="Debug"
FRAMEWORK="netcoreapp2.2"

dotnet publish -c $CONFIGURATION -f $FRAMEWORK &&
docker build \
    -t nagytech/cloudstate-csharp/shopping-cart \
    --build-arg ARTIFACT_PATH="bin/$CONFIGURATION/$FRAMEWORK/publish" \
    --build-arg MAIN_DLL="EventSourced.ShoppingCart.dll" \
    --no-cache .