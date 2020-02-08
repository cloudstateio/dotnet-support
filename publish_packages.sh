dotnet clean
dotnet build -c Release

VERSION=3.12.0-rc1

pushd src/Protos
dotnet pack -c Release -p:PackageVersion=$VERSION --no-build --output ../../nupkgs
popd

pushd src/Common
dotnet pack -c Release -p:PackageVersion=$VERSION --no-build --output ../../nupkgs
popd

pushd src/Internal
dotnet pack -c Release -p:PackageVersion=$VERSION --no-build --output ../../nupkgs
popd

pushd src/Api
dotnet pack -c Release -p:PackageVersion=$VERSION --no-build --output ../../nupkgs
popd

pushd nupkgs

dotnet nuget push \
    -s https://api.nuget.org/v3/index.json \
    -k $APIKEY \
    CloudState.CSharpSupport.Common.$VERSION.nupkg

dotnet nuget push \
    -s https://api.nuget.org/v3/index.json \
    -k $APIKEY \
    CloudState.CSharpSupport.Internal.$VERSION.nupkg

dotnet nuget push \
    -s https://api.nuget.org/v3/index.json \
    -k $APIKEY \
    CloudState.CSharpSupport.$VERSION.nupkg

dotnet nuget push \
    -s https://api.nuget.org/v3/index.json \
    -k $APIKEY \
    CloudState.CSharpSupport.Protos.$VERSION.nupkg

    
popd