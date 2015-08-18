del %DEPLOYMENT_TARGET%\deployInfo.json /F /Q

git rev-parse HEAD > temp.txt
set /p commit_hash=<temp.txt

git log -1 --pretty=format:"%ae" > temp.txt
set /p commit_email=<temp.txt

git log -1 --pretty=format:"%ad" > temp.txt
set /p commit_date=<temp.txt

git log -1 --pretty=format:"%s" > temp.txt
set /p commit_msg=<temp.txt

echo { >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "branch" : "%deployment_branch%", >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "buildArgs" : "%SCM_BUILD_ARGS%", >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "time" : "%date% %time%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitHash" : "%commit_hash%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitEmail" : "%commit_email%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitDate" : "%commit_date%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo "commitMsg" : "%commit_msg%" >> %DEPLOYMENT_TARGET%\deployInfo.json
echo } >> %DEPLOYMENT_TARGET%\deployInfo.json

