IF %deployment_branch% == "master" (
    copy %DEPLOYMENT_SOURCE%\Web\robots.disallowall %DEPLOYMENT_TARGET%\robots.txt /Y
)


