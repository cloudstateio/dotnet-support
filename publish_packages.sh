find . -name *.nupkg | xargs rm
dotnet clean
dotnet build -c Release
cp Cloudstate.CSharpSupport.nuspec src/api/bin/Release/netcoreapp2.2/
pushd src/api/bin/Release/netcoreapp2.2/
# TODO: Fix this versioning issue...
VERSION=3.11.0-rc0.5
nuget pack -Version $VERSION Cloudstate.CSharpSupport.nuspec
dotnet nuget push \
    -s https://api.nuget.org/v3/index.json \
    -k $API_KEY \
    Cloudstate.CSharpSupport.$VERSION.nupkg
popd