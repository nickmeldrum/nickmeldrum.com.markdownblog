# Setting up azure for the 2nd time

## Standard disclaimer for all azure posts:

It changes so fast (the services, the naming, the pricing models, the "perma links" going dead etc etc.) that very soon this article and all azure articles will be out of date. If you notice any out of date ness or other issues with this post - please let me know by commenting below!

## The problem:

(Warning: be aware of the risk of setting up azure services on an MSDN subscription paid for by your company without automating the creation of them. Your employer could cancel the subscription at any time and if they do you won't be able to move those services to another subscription without "re-enabling the msdn subscription" - which of course is impossible. Therefore you have completely lost all your azure services settings.)

(Resolved never to use the GUI (Portal) ever again - do everything through the command line in source controlled scripts for repeatability. It's a lesson oft forgotten, harsh lessons learnt and lesson repeated - perhaps for this kind of thing, the GUI should never be there in the 1st place!

Instead set it all up in a repeatable fashion from code which you control. Here's how:

## Setting up a shared web app

 * npm install azure-cli -g
 * check out readme at https://www.npmjs.com/package/azure-cli or shansleman's post on it here: http://www.hanselman.com/blog/ManagingTheCloudFromTheCommandLine.aspx
 * actually this one probably has the most details: https://azure.microsoft.com/en-gb/documentation/articles/xplat-cli/
 * wealth of information on managing azure websites: http://microsoftazurewebsitescheatsheet.info/

### Logging in using a Microsoft Account

i.e. not using an AD account
(i.e. I think what used to be called a live account - TODO: more descriptions needed here)

 * type "azure account download" to download a publishsettingsfile that has a management certificate with it that will allow you to login
 * NOTE: add .publishsettings into your .gitignore - don't share that certificate!
 * import the .publishsettings file by typing "azure account import filename.publishsettings"
 * you are now effectively logged in as if you had typed "azure login username password" with an AD ("Organizational") account

### Creating the website

 * create the site in the desired data centre (Dublin for me) with this command (nickmeldrum is the name of the web site and -vv = very verbose :)):
  azure site create --location "North Europe" nickmeldrum -vv

TODO:
connect domain name
connect to github and deploy

 domain name: 
  azure site domain add nickmeldrum.com nickmeldrum ???
 then upgrade to shared plan as free plan doesn't support custom domain names:
  azure site scale mode -v --mode shared nickmeldrum
 then we want to delete the local repo that gets set up so we can connect this to github:
  azure site repository delete -v -q nickmeldrum
  but that doesn't work currently - see this issue:
    https://github.com/Azure/azure-xplat-cli/issues/1794
  so for now we have to do this from powershell client or from the front-end...
  install azure for powershell using platform installer then in powershell: https://azure.microsoft.com/en-gb/documentation/articles/powershell-install-configure/
    import-module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"

   do the azure login using the publishsettings files again:
  Import-AzurePublishSettingsFile filename.publishsettings
couldn't find how to delete from azure powershell either - so i guess it's the gui for me here :(

 github connection:
  azure site deployment github nickmeldrum --githubrepository nickmeldrum/nickmeldrum.com.markdownblog --githubusername nickmeldrum
   (Going to ask for password - or you can append --githubpassword mypasswordhere - but DON'T COMMIT THAT TO A PUBLIC REPO!!!)

(gave up and used the gui for the last part - i.e. getting the github hook to work, creating the storage account and setting the config options on the site (TODO: some more config options set - redirect for isntance?)
(TODO: is it in release mode with debug false?)

Get active deployment powershell:

  $currentDeployId = (Get-AzureWebsiteDeployment -name nickmeldrum | where {$_.Current -eq $true}).Id
  $currentDeployId = (azure site deployment list nickmeldrum | grep -i active | awk '{print $3}')

  azure site deployment redeploy -q $currentDeployId nickmeldrum

Setup Config:

azure site appsetting add ShowDrafts=false nickmeldrum

azure site appsetting add deployment_branch=staging nawstest1
azure site appsetting add azureStorageAccountName=nickmeldrum nawstest1
azure site appsetting add azureStorageBlobEndPoint=https://nickmeldrum.blob.core.windows.net/ nawstest1
azure site appsetting add azureStorageKey=kVjV1bHjuK3jcShagvfwNV6lndMjb4h12pLNJgkcbQ2ZYQ/TFpXTWIdfORZLxOS0QdymmNfYVtWPZCDHyQZgSw== nawstest1
azure site appsetting add SCM_BUILD_ARGS=-p:Configuration=Debug nawstest1

## setting up staging site:

This one must be 100% scripted!

  * azure site deployment github --verbose --githubusername nickmeldrum --githubrepository nickmeldrum/nickmeldrum.com.markdownblog nickmeldrum-staging

  okay - that seems to give you a deployment trigger url - go get it so we can give it to github!
 mine looks like this: https://$nickmeldrum-staging:[AUTHKEY]@nickmeldrum-staging.scm.azurewebsites.net/deploy
create it using powershell:
$websiteInfo = Get-AzureWebsite nickmeldrum-staging
$triggerUrl = ("https://" + $websiteInfo.publishingusername + ":" + $x.publishingpassword + "@" + "nickmeldrum-staging" + ".scm.azurewebsites.net/deploy")

then we need to use the github api to create the webhook:
following this documentation: https://developer.github.com/v3/repos/hooks/

so we need to create a POST to:/repos/:owner/:repo/hooks
or in my case: /repos/nickmeldrum/nickmeldrum.com.markdownblog/hooks

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

$headers = @{
    "Accept" = "application/vnd.github.v3+json";
    "Content-Type" = "application/json";
    "Authorization" = ("token " + $githubToken);
}

Invoke-RestMethod -Uri https://api.github.com/repos/nickmeldrum/nickmeldrum.com.markdownblog/hooks -Method Post -ContentType "application/json" -Headers $headers -Body (ConvertTo-Json $body)

Given up with xpat cli - using powershell as Troy Hunt has excellent posts on it:
http://www.troyhunt.com/2015/01/automating-web-hosting-creation-in.html
Set-AzureWebsite -Name nickmeldrum-staging -PhpVersion Off

## Delete the site:

$webhookid = ((Invoke-RestMethod -Uri https://api.github.com/repos/nickmeldrum/nickmeldrum.com.markdownblog/hooks -Method Get -Headers $headers) | where { $_.config.url.indexof("nickmeldrum-staging.scm.azurewebsites.net") -gt -1}).id

._

if ($webhookid -ne $null) {
    Invoke-RestMethod -Uri https://api.github.com/repos/nickmeldrum/nickmeldrum.com.markdownblog/hooks/$webhookid -Method Delete -Headers $headers

}

DELETE /repos/:owner/:repo/hooks/:id

How to setup auto deployment from another branch?

info on whats going on with the webhook here: https://github.com/projectkudu/kudu/wiki/Continuous-deployment - it;s Kudu baby!
It's actually Kudu doing this behind the scenes. You can see the settings here:
https://nickmeldrum-staging.scm.azurewebsites.net/api/settings

and you can actually set the "deployment_branch" setting via an appsetting!

issue that led me to the answer:
https://github.com/Azure/azure-powershell/issues/413



Story so far:

using the new-azurewebsite method but not passing in creds, seting the appsetting then doing a push seems to work with a deployment...
next step: test if doing node method works azure site create, set appsetting, set deployment, then doing a push works...

then test if a redploy works with node method - and then if you can get it working in powershell method with upgrade-azuredeployment thing...


