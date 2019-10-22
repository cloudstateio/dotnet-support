dotnet test --logger "trx;LogFileName=TestResults.trx" \
    --results-directory ./BuildReports/UnitTests \
    /p:CollectCoverage=true \
    /p:CoverletOutput=BuildReports/Coverage/ \
    /p:CoverletOutputFormat=cobertura
dotnet test --logger "trx;LogFileName=TestResults.trx" \
    --results-directory ./BuildReports/UnitTests \
    /p:CollectCoverage=true \
    /p:CoverletOutput=BuildReports/Coverage/ \
    /p:CoverletOutputFormat=lcov

# 
# dotnet reportgenerator \
#        "-reports:BuildReports/Coverage/coverage.cobertura.xml" \
#        "-targetdir:BuildReports/Coverage" \
#        "-assemblyfilters:-csharp-support-protocols;-csharp-support-abstractions;-Google.Protobuf" \
#        -reporttypes:HTML;HTMLSummary
# open ./BuildReports/Coverage/index.htm