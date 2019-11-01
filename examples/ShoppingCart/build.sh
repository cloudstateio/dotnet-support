CONFIGURATION=Debug
FRAMEWORK=netcoreapp2.2
ARTIFACT_PATH=bin/$CONFIGURATION/$FRAMEWORK/publish
MAIN_DLL=EventSourced.ShoppingCart.dll

dotnet publish -c $CONFIGURATION -f $FRAMEWORK &&
docker build \
    -t test \
    --build-arg main_dll=$MAIN_DLL \
    --build-arg artifact_path=$ARTIFACT_PATH \
    .
