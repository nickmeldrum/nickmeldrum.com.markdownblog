$ErrorActionPreference = "Stop"

pushd
cd $psscriptroot

if (test-path logs) { rm logs -force -recurse }
mkdir logs\nickmeldrum -force
mkdir logs\nickmeldrum-staging -force

cd $psscriptroot

cd logs\nickmeldrum
azure site log download nickmeldrum
unzip-here diagnostics.zip
del .\diagnostics.zip

cd $psscriptroot

cd logs\nickmeldrum-staging
azure site log download nickmeldrum-staging
unzip-here diagnostics.zip
del .\diagnostics.zip

popd
