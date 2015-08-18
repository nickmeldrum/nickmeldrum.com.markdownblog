del %DEPLOYMENT_TARGET%\deployInfo.json /F /Q
echo { >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "branch" : "%deployment_branch%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "buildArgs" : "%SCM_BUILD_ARGS%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "time" : "%date% %time%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo } >> %DEPLOYMENT_TARGET%\deployInfo.json

