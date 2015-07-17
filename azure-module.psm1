$githubUsername = "nickmeldrum"
$githubRepo = "nickmeldrum.com.markdownblog"
$azureLocation = "North Europe"

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

$loggedIn = $false

$headers = @{
    "Accept" = "application/vnd.github.v3+json";
    "Content-Type" = "application/json";
    "Authorization" = ("token " + $githubToken);
}

Function Check-AzureGlobalDependencies {
    if ((get-childitem .\*.publishsettings).length -eq 0) {
        throw "no publishsettings file found - can't login to azure, exiting"
    }
    if ((get-childitem .\*.publishsettings).gettype().IsArray) {
        throw "more than 1 publishsettings file found - dont' know which one to use, exiting"
    }
    if ($githubToken -eq $null) {
        throw "doesn't look like your githubToken variable has been setup, exiting"
    }
    if ($githubPassword -eq $null) {
        throw "doesn't look like your githubPassword variable has been setup, exiting"
    }

    echo "azure dependency check (publishsettings and githubtoken) passed"
}

Function Setup-AzureApi {
    echo "installing azure powershell module and azure xpat cli..."
    Import-Module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"
    npm install azure-cli -g
}

Function Login-AzureApi {
    if ($script:loggedIn -eq $false) {
        echo "Assuming the azure powershell module and the azure xpat cli have already been installed - if not, please run Setup-AzureApi first..."

        Check-AzureGlobalDependencies

        echo "logging into both azure powershell and azure xpat cli..."
        $publishSettings = (get-childitem .\*.publishsettings).fullname
        Import-AzurePublishSettingsFile $publishSettings
        azure account import $publishSettings
        $script:loggedIn = $true
    }
}

Function Create-AzureSiteAndGithubWebhook {
    param ([string]$sitename)

    Login-AzureApi

    echo "creating site..."
    azure site create --location $azureLocation $sitename

    echo "setting up site config..."
    Set-AzureWebsite -Name $sitename -PhpVersion Off

    echo "setting up deployment..."
    azure site deployment github --githubusername $githubUsername --githubpassword $githubPassword --githubrepository $githubRepo $sitename 
    Create-GithubWebhook $sitename

    echo "if you got here without errors then bingo bango - you have a new azure website with the name $sitename and a push deployment webhook from github repo $githubRepo setup"
}

Function Create-GithubWebhook {
    param ([string]$sitename)

    Login-AzureApi

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

Function Delete-AzureSiteAndGithubWebhook {
    param ([string]$sitename)

    Login-AzureApi

    echo "deleting site..."
    azure site delete -q $sitename

    echo "deleting webhook if it exists..."
    $webhookid = ((Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks" -Method Get -Headers $headers) | where { $_.config.url.indexof("$sitename.scm.azurewebsites.net") -gt -1}).id

    if ($webhookid -ne $null) {
        Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks/$webhookid" -Method Delete -Headers $headers
    }
}

Function List-GithubWebhooks {
    Invoke-RestMethod -Uri "https://api.github.com/repos/$githubUsername/$githubRepo/hooks" -Method Get -Headers $headers
}

export-modulemember *-*

