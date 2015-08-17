## Standard disclaimer for all azure posts:

Azure changes so fast (the services, the naming, the pricing models, the "perma links" going dead for example) that very soon this article will be out of date. If you notice any out-of-dateness or other issues with this post, then please let me know by commenting below.

## The background:

I set up my Azure website on an MSDN subscription paid for by my previous employer which they then cancelled. Azure then immediately shut down all my services (and therefore my website went down) and didn't even give me access to open up the sites in the portal for me to see how I had set them up in order to recreate them on a new subscription.

Note: after talking to Azure support it turns out *you cannot migrate your services from a cancelled account*.

So I learnt for this reason and many others: the best way to set up your azure account is from a Powershell script so if there's ever a problem you can just re-run the script.

## A side note - here be dragons

Luckily I didn't store any data in these Azure services (or rather I only stored temporary data such as Lucene indexes.) If I had stored data in there that I needed to retrieve at some point in the future then when the account was suspended I would have irrevocably lost access to that data. Of course if you are storing important data in Azure you should have off-site backups but just beware of this potential issue!

## Okay okay, show me the money

Off I went on a voyage of discovery into the land of automating Azure services. I will try to document what I learnt and my scripts here in the hope some of the pain I went through can help someone else in the future.

### The requirement:

My requirement was to have my own website completely set up from scratch with 1 script. Indeed even if it were partially set up I want the script to succeed in setting up the site. I guess that is similar to saying I want the script to be idempotent.

Requirement Breakdown: So how is this website setup?

#### General:
 
 * This needs to be done in the most cost-efficient manner possible (To improve the "WAF" - Wife Acceptance Factor)

#### Deployment and staging/ production

 * The code is in github with a master and release branch. I code and test on master branch then merge into the release branch when I want it to go to the production website
 * I have an Azure website hosting the staging site which is deployed to every time I push my master branch up to github
 * I have an Azure website hosting the production site which is deployed to every time I push my release branch up to github

#### DNS

 * I have my DNS records in DNSimple
 * The staging site doesn't need any DNS records and is accessible from the default azurewebsites.net address
 * The production site has quite a few DNS records requiring both A and CNAME recordsd to point to the production site
 * The IIS setup has a canonical hostname redirect so whichever address you use you will be redirected to the canonical

#### Lucene search and Azure storage

 * The website has a search facility implemented by Lucene which requires indexes to be built and stored in files
 * These files are stored in Azure storage blob containers and both staging and production have their own container

#### The site itself

 * The staging site shows draft posts whereas the production site doesn't
 * Both sites have an admin password for website admin access
 * These settings are managed through appsettings in the web.config

### The implementation:

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
see if i can get the creds working in new-azurewebsite
then test if a redploy works with node method - and then if you can get it working in powershell method with upgrade-azuredeployment thing...

TODO:
stackoverflow/ msdn about creds not working for new-azurewebsite cmdlet function


Creating a full test with pester
create git repo,
create remote github and push hello world
create azure site

This explains the poor support for websites - i should be using "apps" now!!!

http://tfl09.blogspot.co.uk/2015/04/new-version-of-azure-powershell-module.html

"Interestingly – the updated module still supports for Azure web sites (e.g. New-AzureWebsite) despite the GUI (the Azure portal and the Azure preview portal) having dropped these in favour of the newly announced Web Apps functionality that in effect subsumes the older Web Sites feature). The Azure portals no longer show Web sites, whereas the cmdlets still support the feature"

POST IDEA:
understanding accounts, resource groups and authenticating in azure!

started using a "microsoft" account and authenticating using a .publishsettings file - all well and good until I want to use the new webapp azure powershell functions: (the ones that are superceding the websites.) These use resource groups and refuse to be authenticated using a .publishsettings file.

So if you are using a microsoft account you have to authenticate with these new functions using "add-azureaccount" - if using an organizational id - all well and good as you can pass in a credentials object. however if you are using a microsoft account this CANNOT be automated (as far as I can work out.) The only way to get an auth token is by using the UI to login (via add-azureaccount without passing in creds.) This obviously means this can't be scripted in an unattended manner, which is a must-have for me.

so onto creating an organisation instead of using a "personal" account.


research:
https://social.technet.microsoft.com/Forums/azure/en-US/e60a3fcb-ea2f-4bc8-a05e-488c98006315/azure-powershell-resource-manager-mode-getting-authentication-failures?forum=windowsazuremanagement
http://blog.codingoutloud.com/2014/01/24/stupid-azure-trick-2-how-do-i-create-a-new-organizational-account-on-windows-azure-active-directory-without-any-existing-accounts-or-ea/
http://blog.codebeastie.com/setting-up-an-azure-environment-for-a-small-company/
https://msdn.microsoft.com/en-gb/library/azure/hh531793.aspx
http://blog.aditi.com/cloud/organizational-identity-microsoft-accounts-azure-active-directory-part-1/
delete github repo requires a change to the personal access token: add the scope delete_repo to the token via github.com settings


