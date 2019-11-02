find . -name *.nupkg | xargs rm
dotnet clean
dotnet build -c Release
pushd src/api/bin/Release/netcoreapp2.2/
VERSION=3.11.0-rc0.4
nuget pack -Version $VERSION Cloudstate.CSharpSupport.nuspec
dotnet nuget push \
    -s https://api.nuget.org/v3/index.json \
    -k $API_KEY \
    Cloudstate.CSharpSupport.$VERSION.nupkg
popd