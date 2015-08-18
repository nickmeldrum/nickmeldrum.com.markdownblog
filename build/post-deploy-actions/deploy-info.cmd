del %DEPLOYMENT_TARGET%\deployInfo.json /F /Q

FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-parse HEAD`) do set commit_hash=%%F
FOR /F "tokens=* USEBACKQ" %%F IN (`git log -1 --pretty^=format:"%ae"`) do set commit_email=%%F
FOR /F "tokens=* USEBACKQ" %%F IN (`git log -1 --pretty^=format:"%ad"`) do set commit_date=%%F
FOR /F "tokens=* USEBACKQ" %%F IN (`git log -1 --pretty^=format:"%s"`) do set commit_msg=%%F

echo { >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "branch" : "%deployment_branch%", >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "buildArgs" : "%SCM_BUILD_ARGS%", >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "time" : "%date% %time%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitHash" : "%commit_hash%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitEmail" : "%commit_email%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitDate" : "%commit_date%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitMsg" : "%commit_msg%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo } >> %DEPLOYMENT_TARGET%\deployInfo.json

