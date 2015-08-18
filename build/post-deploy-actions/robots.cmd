copy %DEPLOYMENT_SOURCE%\Web\robots.disallowall %DEPLOYMENT_TARGET%\robots.txt /Y
echo %deployment_branch% > %DEPLOYMENT_TARGET%\test.txt

