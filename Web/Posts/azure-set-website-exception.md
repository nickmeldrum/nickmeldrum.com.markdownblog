running command: 
  
  Get-AzureWebsite -Name nickmeldrum-staging | Set-AzureWebsite -AppSettings @{ "fdas" = "oh yeaaaaaaaaaah"; }

gets exception:

    Set-AzureWebsite : Cannot validate argument on parameter 'PhpVersion'. The argument is null or empty. Provide an argument that is not null or empty,
    and then try the command again.
    At line:1 char:46
    + Get-AzureWebsite -Name nickmeldrum-staging | Set-AzureWebsite -AppSettings @{ "f ...
    +                                              ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        + CategoryInfo          : InvalidData: (Microsoft.Windo....SiteWithConfig:PSObject) [Set-AzureWebsite], ParameterBindingValidationException
        + FullyQualifiedErrorId : ParameterArgumentValidationError,Microsoft.WindowsAzure.Commands.Websites.SetAzureWebsiteCommand

seems like it's getting upset with turning php off!
get-azurewebsite returns empty string when php is off but i expect set-azurewebsite is expecting "Off" and upset by the empty string!

(test by piping and modifying the result?)

answer is to do the following instead and this just sets the appsettings instead of resetting everything from the object returned from get-azurewebsite!

  $appSettings = (get-azurewebsite -name nickmeldrum).appsettings
  $appSettings.add("SCM_POST_DEPLOYMENT_ACTIONS_PATH", "build\post-deploy-actions")
  set-azurewebsite -name nickmeldrum -appsettings $appSettings

