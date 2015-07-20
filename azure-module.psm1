# Dependencies:
# ==============
#
# If you haven't already - install the azure for powershell using the Microsoft Web Platform Installer
# This also depends on the azure xpat cli, so make sure node.js and npm are installed
# 
# Ensure $githubToken var exists and contains a valid github auth token and $githubPassword var exists and is your actual password (yes this sucks, currently trying to find a better way of doing this.) I stick this in my powershell profile so it's available globally. Don't stick it in source control!
# Assume your .publishsettings file is in the root dir in order for this script to login to your azure account
# (get your .publishsettings file by typing "azure account download" (this uses the azure xpat cli installed above)
# Remember to add .publishsettings to your .gitignore so it doesn't go in source control!

$azureLocation = "North Europe"

$loggedIn = $false
$headers = @{
    "Accept" = "application/vnd.github.v3+json";
    "Content-Type" = "application/json";
    "Authorization" = ("token " + $githubToken);
}
$ErrorActionPreference = "Stop"

Function Check-VarNotNullOrWhiteSpace {
    param ([string]$var, [string]$msg)

    if ([string]::IsNullOrWhiteSpace($var)) {
        throw $msg
    }
}

Function Check-AzureDependencies {

    if ((get-childitem .\*.publishsettings).length -eq 0) {
        throw "no publishsettings file found - can't login to azure, exiting (Ensure 1 .publishsettings file with 1 subscription is in the directory you are executing in.)"
    }
    if ((get-childitem .\*.publishsettings).gettype().IsArray) {
        throw "more than 1 publishsettings file found - dont' know which one to use, exiting. (Ensure 1 .publishsettings file with 1 subscription is in the directory you are executing in.)"
    }
    Check-VarNotNullOrWhiteSpace $githubUsername "doesn't look like your githubUsername variable has been setup, exiting. (Set up a global var with your username in .)"
    Check-VarNotNullOrWhiteSpace $githubToken "doesn't look like your githubToken variable has been setup, exiting. (Set up a global var with your token in .)"
    Check-VarNotNullOrWhiteSpace $githubPassword "doesn't look like your githubPassword variable has been setup, exiting. (Set up a global var with your password in.)"
    Check-VarNotNullOrWhiteSpace $releaseMode "doesn't look like your release mode has been setup, exiting. (Run Set-ReleaseMode to set this.)"

    echo "azure dependency check (.publishsettings, githubToken, githubPassword and releaseMode) passed!"
}

Function Setup-AzureApi {
    echo "installing azure powershell module and azure xpat cli..."
    Import-Module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"
    npm install azure-cli -g
}

Function Login-AzureApi {
    if ($script:loggedIn -eq $false) {
        echo "Assuming the azure powershell module and the azure xpat cli have already been installed - if not, please run Setup-AzureApi first..."

        echo "logging into both azure powershell and azure xpat cli..."
        $publishSettings = (get-childitem .\*.publishsettings).fullname
        Import-AzurePublishSettingsFile $publishSettings
        azure account import $publishSettings
        $script:loggedIn = $true
    }
}

Function Function-Preflight {
    param ([string]$sitename)
    Check-AzureDependencies
    Check-VarNotNullOrWhiteSpace $sitename "Please pass in a site name"
    Login-AzureApi
}

Function Set-ReleaseMode {
    param ([string]$relMode)

    $script:releaseMode = $relMode

    switch ($relMode) {
        "Production" {
            $script:branchName = "release"
            $script:buildConfiguration = "Release"
            $script:showDrafts = "false"
        }
        "Test" {
            $script:branchName = "master"
            $script:showDrafts = "true"
            $script:buildConfiguration = "Debug"
        }
        default {
            throw "unknown release mode, please set to either `"Production`" or `"Test`""
        }
    }
}

Function Get-ReleaseMode {
    echo "Release mode = $releaseMode"
    echo "branch to deploy = $branchName"
    echo "build config = $buildConfiguration"
    echo "show drafts = $showDrafts"
}

Function Create-AzureSitePS {
    param ([string]$sitename, [string]$githubRepo, [string]$siteAdminPassword)
    Check-VarNotNullOrWhiteSpace $siteAdminPassword "Please pass in a valid site admin password as a string"
    Check-VarNotNullOrWhiteSpace $githubRepo "Please pass in a valid githubRepo as a string"
    Function-Preflight $sitename

    echo "creating site..."

    echo "falling back on asking for creds as passing them in (below in func) isn't working atm..."
    new-azurewebsite -name $sitename -location $azureLocation -github -githubrepository "$githubusername/$githubrepo"

    Set-AzureConfig $sitename $siteAdminPassword

    return
    echo "TODO: The following doesn't seem to work - auth failure against github i think..."
    $secpasswd = ConvertTo-SecureString $githubPassword -AsPlainText -Force
    $githubCreds = New-Object -Typename System.Management.Automation.PSCredential -Argumentlist $githubUsername, $secpasswd
    new-azurewebsite -name $sitename -location $azureLocation -github -githubcredentials $githubCreds -githubrepository "$githubusername/$githubrepo"
}

Function Create-AzureSite {
    param ([string]$sitename, [string]$githubRepo, [string]$siteAdminPassword)
    Check-VarNotNullOrWhiteSpace $siteAdminPassword "Please pass in a valid site admin password as a string"
    Check-VarNotNullOrWhiteSpace $githubRepo "Please pass in a valid githubRepo as a string"
    Function-Preflight $sitename

    echo "creating site..."
    azure site create --location $azureLocation $sitename
    
    Set-AzureConfig $sitename $siteAdminPassword

    echo "setting up deployment..."
    azure site deployment github --githubusername $githubUsername --githubpassword $githubPassword --githubrepository "$githubUsername/$githubRepo" $sitename 

    echo "if you got here without errors then bingo bango - you have a new azure website with the name $sitename and a push deployment webhook from github repo $githubRepo setup"
}

Function Set-AzureConfig {
    param ([string]$sitename, [string]$siteAdminPassword)
    Check-VarNotNullOrWhiteSpace $siteAdminPassword "Please pass in a valid site admin password as a string"
    Function-Preflight $sitename

    echo "setting up site config..."
    azure site set --php-version off $sitename

    # deployment settings
    azure site appsetting add "deployment_branch=$branchName;SCM_BUILD_ARGS=-p:Configuration=$buildConfiguration" $sitename
    # azure storage settings
    azure site appsetting add "azureStorageAccountName=nickmeldrum;azureStorageBlobEndPoint=https://nickmeldrum.blob.core.windows.net/;azureStorageKey=kVjV1bHjuK3jcShagvfwNV6lndMjb4h12pLNJgkcbQ2ZYQ/TFpXTWIdfORZLxOS0QdymmNfYVtWPZCDHyQZgSw==" $sitename
    # app settings
    azure site appsetting add "ShowDrafts=$showDrafts;username-Nick-admin=$siteAdminPassword" $sitename
}

Function Clear-AzureConfig {
    param ([string]$sitename)
    Function-Preflight $sitename

    echo "deleting appsettings..."

    azure site appsetting delete -q deployment_branch $sitename
    azure site appsetting delete -q SCM_BUILD_ARGS $sitename

    azure site appsetting delete -q azureStorageAccountName $sitename
    azure site appsetting delete -q azureStorageBlobEndPoint $sitename
    azure site appsetting delete -q azureStorageKey $sitename

    azure site appsetting delete -q ShowDrafts $sitename
    azure site appsetting delete -q username-Nick-Admin $sitename
}

Function Delete-AzureSite {
    param ([string]$sitename)
    Function-Preflight $sitename

    echo "deleting site..."
    azure site delete -q $sitename

    Delete-GithubWebhook $sitenmae
}

Function Create-GithubWebhook {
    param ([string]$sitename, [string]$githubRepo)
    Check-VarNotNullOrWhiteSpace $githubRepo "Please pass in a valid githubRepo as a string"
    Function-Preflight $sitename

    echo "setting up github webhook..."
    $websiteInfo = Get-AzureWebsite $sitename

    if ($websiteInfo.publishingpassword -eq $null) {
        throw "publishing password from azure not found."
    }
    
    $triggerUrl = ('https://' + $websiteInfo.publishingusername + ":" + $websiteInfo.publishingpassword + "@$sitename.scm.azurewebsites.net/deploy")

    echo "azure trigger url is: $triggerUrl"

    $body = @{
      "name" = "web";
      "active" = "true";
      "events" = @(
        "push"
      );
      "config" = @{
        "url" = "$triggerUrl";
        "content_type" = "form"
      }
    }

    Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks" -Method Post -ContentType "application/json" -Headers $headers -Body (ConvertTo-Json $body)
}

Function Delete-GithubWebhook {
    param ([string]$sitename, [string]$githubRepo)
    Check-VarNotNullOrWhiteSpace $sitename "Please pass in a valid sitename as a string"
    Check-VarNotNullOrWhiteSpace $githubRepo "Please pass in a valid githubRepo as a string"

    echo "deleting webhook if it exists..."
    $webhookid = ((Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks" -Method Get -Headers $headers) | where { $_.config.url.indexof("$sitename.scm.azurewebsites.net") -gt -1}).id

    if ($webhookid -ne $null) {
        Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks/$webhookid" -Method Delete -Headers $headers
    }
}

Function List-GithubWebhooks {
    param ([string]$githubRepo)
    Check-VarNotNullOrWhiteSpace $githubRepo "Please pass in a valid githubRepo as a string"

    Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks" -Method Get -Headers $headers
}

export-modulemember *-*

