<img src="/media/azure_logo.png" alt="Microsoft Azure" title="Microsoft Azure" class="centered"/>

## It's a series of posts

* [Part 1: The background](/blog/scripting-azure-services-with-PowerShell-thebackground-part1 "The background post (Part 1)")
* Part 2: Azure Accounts, Subscriptions and Installs: Setting up your account and environment (this one)

## Next post

* [Part 3: Some implementation detail](/blog/scripting-azure-services-with-PowerShell-part3 "Some implementation detail...")

## The hello world of Azure posts

How to set up Azure PowerShell and connect to your account/subscription/website is not a novel post. However, I found I had to go to numerous sources to learn a few key things about connecting to Azure. Hopefully I can explain them all simply here.

I wanted to use / try out both the Azure PowerShell interface as well as the "Azure xpat cli" which is the nodejs based cross-platform way of connecting to Azure. This article will deal with both.

First off let's understand what the difference between an Account and a Subscription is. Then we will look at the different types of Accounts that you can use with Azure. Then we'll get to some commands you can run to actually install, set up and login to Azure PowerShell and the Azure xpat cli.

## What is an account and what is a subscription?

### The account

The account is how Microsoft Azure authenticates you. There are 2 types: a "Microsoft Account" (tied to an email address) and an "Organizational Account" (an Active Directory account.) I will go into the differences between those below. Just remember an account is something that has a password and allows Azure to know that your requests are coming from you!

#### What's the beef with Microsoft Accounts and Organizational Accounts then?

> Take 2 account types into the shower? Not me, I just login and go!

So there 

A Microsoft account is the new name for a "Live ID" and Microsoft would like this to be your personal login for all of their services from Azure to XBox. Importantly it is not tied to your organization but to you personally so you should be able to keep the same login in perpetuity.

An organizational account is an account in an Azure Active Directory (or in an AD that is federated with or synchronized to Azure AD.) This is used by organizations to manage services administered by individuals and mitigating the risk of having those services tied to just a persons individual Microsoft Account.

Using Azure services (via PowerShell or the xpat cli) with a Microsoft Account requires you to "login" using a management certificate held in a .publishsettings file.
Using Azure services (via PowerShell or the xpat cli) with an Organizational Account you can "login" using a username/password (via a credentials object in PowerShell.)

If you want to use Office 365, you can only use an Orgainzational account for that.

(Side note: when trying to automate Azure PowerShell in an unattended way using the new Azure PowerShell commands and a Microsoft Account seems impossible because the Azure-AddAccount function fires up a "sign-in" modal box unfortunately.)

#### The subscription

This is how Microsoft Azure knows how to bill you for your Azure resource usage. There are a number of ways Microsoft can bill you, but for your personal usage you will typically be interested in 2 different options: 1: An MSDN account. Most Microsoft developers will have been given an MSDN license in order to use Visual Studio at work. These licenses come with a monthly quota of credit you can use to pay for your Azure resource usage. If you don't have a currently valid MSDN license, you will have to set up a "Pay as you go" subscription and give them your bank details.

Here is [Microsoft's description](https://msdn.microsoft.com/en-gb/library/azure/hh531793.aspx#BKMK_AccountSub "The difference between an Azure account and a subscription") of all this (including admin roles which I haven't mentioned.)

If you have an MSDN Subscription purchased for you by work it *must* be tied to your Microsoft account. An MSDN Subscription is always purchased for an individual and infers a whole load of rights to you, see here for more details: http://nakedalm.com/do-you-want-visual-studio-ultimate-for-free-do-you-have-msdn/

Here is an excellent article on setting up an Azure AD from scratch: http://blog.codingoutloud.com/2014/01/24/stupid-azure-trick-2-how-do-i-create-a-new-organizational-account-on-windows-azure-active-directory-without-any-existing-accounts-or-ea/


## setting up the libraries

both command line (node xpat cli) and PowerShell. started trying out both and liked the node command line api surface but a) when in PowerShell found working on objects was much easier and less brittle than working with strings (using grep/awk etc.)

however I am going to look at creating a 100% command line version of these scripts using the node command line in bash and unix command line tools in order to do a comparison of the two.

To install the command line (node xpat cli) you must already have nodejs installed from here: https://nodejs.org/download/ (npm comes preinstalled with node.)
Once you have node just install the node xpat cli using the command:

    npm install azure-cli -g

To install "Azure for PowerShell" the easiest way to install it is using the "Microsoft Web Platform Installer" from here: http://www.microsoft.com/web/downloads/platform.aspx
Once you have the Web PI installed just install Azure for PowerShell by running the Web PI, searching for "Microsoft Azure PowerShell" and clicking add and install.
Note: once it's installed in order to use the PowerShell module in any PowerShell session you have to import it. here is the line I use to do that, but of course beware your path may be different:

    Import-Module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"

(In my PowerShell module: https://github.com/nickmeldrum/ps-cloud/blob/master/azure-base-commands.psm1 I supply a function: set up-AzureApi which will install azure-cli and import the azure module.)

## logging in (using a "Microsoft Account" rather than an "Organizational Account")

TODO: detail the difference in accessing azure using either account types
### What is the difference between a Microsoft account and an organizational account?

Now you have the libraries installed you need to give those libraries the authorisation to operate on your azure account. There are 2 ways of authenticating to Azure, 1 using what they call an "Organizational Account" which will need an Azure AD (Active Directory) set up. I am using what they call a "Microsoft Account" and this uses a management certificate that is held in a .publishsettings file.

For now this (and my future Azure posts) are going to assume you are using a Microsoft Account because that's what I'm using at the moment. However if you are interested [see the bottom of this post for more info on the 2 account types](#account-types)

 * To get your .publishsettings file, type "azure account download" and it will load up a browser window to download it for you (allowing you to sign in to your Microsoft account
 TODO: PowerShell equivalent of account download
 * NOTE: Treat your .publishsettings file like a password - it is sensitive data. Add .publishsettings into your .gitignore and ensure you don't commit it to version control!

Use the following line to "login" the PowerShell module:

    Import-AzurePublishSettingsFile filename.publishsettings

And the following line to "login" the node xpat cli:

    azure account import filename.publishsettings

Note: If you are using an "Organizational Account" (e.g. Azure Ad) you can apparently just type:

    azure login username password

However I cannot confirm as I haven't used this account type yet.

(In my PowerShell module: https://github.com/nickmeldrum/ps-cloud/blob/master/azure-base-commands.psm1 I supply a function: Login-AzureApi which will find a .publishsettings file in the current directory and use it to "login" both the PowerShell module and the node xpat cli.)

## Links

For more details or reference articles, see the following:

 * check out readme at https://www.npmjs.com/package/azure-cli or shansleman's post on it here: http://www.hanselman.com/blog/ManagingTheCloudFromTheCommandLine.aspx
 * actually this one probably has the most details: https://azure.microsoft.com/en-gb/documentation/articles/xplat-cli/
 * wealth of information on managing azure websites: http://microsoftazurewebsitescheatsheet.info/

### Command auto-completion for the xpat cli

 * On Windows PowerShell: http://blogs.msdn.com/b/stuartleeks/archive/2015/08/17/posh-azurecli-command-completion-for-azure-cross-platform-command-line-in-PowerShell.aspx
 * On mac/linux: http://github.com/Azure/azure-xplat-cli/blob/28a6fb69c989f157a9c55d84fb3db893e879ebaf/README.md#configure-auto-complete 

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
  so for now we have to do this from PowerShell client or from the front-end...
  install azure for PowerShell using platform installer then in PowerShell: https://azure.microsoft.com/en-gb/documentation/articles/PowerShell-install-configure/
    import-module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"

   do the azure login using the publishsettings files again:
  Import-AzurePublishSettingsFile filename.publishsettings
couldn't find how to delete from azure PowerShell either - so i guess it's the gui for me here :(

 github connection:
  azure site deployment github nickmeldrum --githubrepository nickmeldrum/nickmeldrum.com.markdownblog --githubusername nickmeldrum
   (Going to ask for password - or you can append --githubpassword mypasswordhere - but DON'T COMMIT THAT TO A PUBLIC REPO!!!)

(gave up and used the gui for the last part - i.e. getting the github hook to work, creating the storage account and setting the config options on the site (TODO: some more config options set - redirect for isntance?)
(TODO: is it in release mode with debug false?)

Get active deployment PowerShell:

  $currentDeployId = (Get-AzureWebsiteDeployment -name nickmeldrum | where {$_.Current -eq $true}).Id
  $currentDeployId = (azure site deployment list nickmeldrum | grep -i active | awk '{print $3}')

  azure site deployment redeploy -q $currentDeployId nickmeldrum

set up Config:

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
create it using PowerShell:
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

Given up with xpat cli - using PowerShell as Troy Hunt has excellent posts on it:
http://www.troyhunt.com/2015/01/automating-web-hosting-creation-in.html
Set-AzureWebsite -Name nickmeldrum-staging -PhpVersion Off

## Delete the site:

$webhookid = ((Invoke-RestMethod -Uri https://api.github.com/repos/nickmeldrum/nickmeldrum.com.markdownblog/hooks -Method Get -Headers $headers) | where { $_.config.url.indexof("nickmeldrum-staging.scm.azurewebsites.net") -gt -1}).id

._

if ($webhookid -ne $null) {
    Invoke-RestMethod -Uri https://api.github.com/repos/nickmeldrum/nickmeldrum.com.markdownblog/hooks/$webhookid -Method Delete -Headers $headers

}

DELETE /repos/:owner/:repo/hooks/:id

How to set up auto deployment from another branch?

info on whats going on with the webhook here: https://github.com/projectkudu/kudu/wiki/Continuous-deployment - it;s Kudu baby!
It's actually Kudu doing this behind the scenes. You can see the settings here:
https://nickmeldrum-staging.scm.azurewebsites.net/api/settings

and you can actually set the "deployment_branch" setting via an appsetting!

issue that led me to the answer:
https://github.com/Azure/azure-PowerShell/issues/413

Story so far:

using the new-azurewebsite method but not passing in creds, seting the appsetting then doing a push seems to work with a deployment...
next step: test if doing node method works azure site create, set appsetting, set deployment, then doing a push works...
see if i can get the creds working in new-azurewebsite
then test if a redploy works with node method - and then if you can get it working in PowerShell method with upgrade-azuredeployment thing...

TODO:
stackoverflow/ msdn about creds not working for new-azurewebsite cmdlet function


Creating a full test with pester
create git repo,
create remote github and push hello world
create azure site

This explains the poor support for websites - i should be using "apps" now!!!

http://tfl09.blogspot.co.uk/2015/04/new-version-of-azure-PowerShell-module.html

"Interestingly – the updated module still supports for Azure web sites (e.g. New-AzureWebsite) despite the GUI (the Azure portal and the Azure preview portal) having dropped these in favour of the newly announced Web Apps functionality that in effect subsumes the older Web Sites feature). The Azure portals no longer show Web sites, whereas the cmdlets still support the feature"

POST IDEA:
understanding accounts, resource groups and authenticating in azure!

started using a "microsoft" account and authenticating using a .publishsettings file - all well and good until I want to use the new webapp azure PowerShell functions: (the ones that are superceding the websites.) These use resource groups and refuse to be authenticated using a .publishsettings file.

So if you are using a microsoft account you have to authenticate with these new functions using "add-azureaccount" - if using an organizational id - all well and good as you can pass in a credentials object. however if you are using a microsoft account this CANNOT be automated (as far as I can work out.) The only way to get an auth token is by using the UI to login (via add-azureaccount without passing in creds.) This obviously means this can't be scripted in an unattended manner, which is a must-have for me.

so onto creating an organisation instead of using a "personal" account.


research:
https://social.technet.microsoft.com/Forums/azure/en-US/e60a3fcb-ea2f-4bc8-a05e-488c98006315/azure-PowerShell-resource-manager-mode-getting-authentication-failures?forum=windowsazuremanagement
http://blog.codingoutloud.com/2014/01/24/stupid-azure-trick-2-how-do-i-create-a-new-organizational-account-on-windows-azure-active-directory-without-any-existing-accounts-or-ea/
http://blog.codebeastie.com/setting-up-an-azure-environment-for-a-small-company/
https://msdn.microsoft.com/en-gb/library/azure/hh531793.aspx
http://blog.aditi.com/cloud/organizational-identity-microsoft-accounts-azure-active-directory-part-1/
delete github repo requires a change to the personal access token: add the scope delete_repo to the token via github.com settings


