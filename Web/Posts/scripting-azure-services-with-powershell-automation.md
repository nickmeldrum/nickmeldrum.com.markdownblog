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

## setting up the libraries

TODO: Setting up libraries + logging in in separate blog post?

both command line (node xpat cli) and powershell. started trying out both and liked the node command line api surface but a) when in powershell found working on objects was much easier and less brittle than working with strings (using grep/awk etc.)

however I am going to look at creating a 100% command line version of these scripts using the node command line in bash and unix command line tools in order to do a comparison of the two.

To install the command line (node xpat cli) you must already have nodejs installed from here: https://nodejs.org/download/ (npm comes preinstalled with node.)
Once you have node just install the node xpat cli using the command:

    npm install azure-cli -g

To install "Azure for Powershell" the easiest way to install it is using the "Microsoft Web Platform Installer" from here: http://www.microsoft.com/web/downloads/platform.aspx
Once you have the Web PI installed just install Azure for Powershell by running the Web PI, searching for "Microsoft Azure Powershell" and clicking add and install.
Note: once it's installed in order to use the powershell module in any powershell session you have to import it. here is the line I use to do that, but of course beware your path may be different:

    Import-Module "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"

(In my powershell module: https://github.com/nickmeldrum/ps-cloud/blob/master/azure-base-commands.psm1 I supply a function: Setup-AzureApi which will install azure-cli and import the azure module.)

## logging in (using a "Microsoft Account" rather than an "Organizational Account")

TODO: detail the difference in accessing azure using either account types
### What is the difference between a Microsoft account and an organizational account?

Now you have the libraries installed you need to give those libraries the authorisation to operate on your azure account. There are 2 ways of authenticating to Azure, 1 using what they call an "Organizational Account" which will need an Azure AD (Active Directory) setup. I am using what they call a "Microsoft Account" and this uses a management certificate that is held in a .publishsettings file.

For now this (and my future Azure posts) are going to assume you are using a Microsoft Account because that's what I'm using at the moment. However if you are interested [see the bottom of this post for more info on the 2 account types](#account-types)

 * To get your .publishsettings file, type "azure account download" and it will load up a browser window to download it for you (allowing you to sign in to your Microsoft account
 TODO: powershell equivalent of account download
 * NOTE: Treat your .publishsettings file like a password - it is sensitive data. Add .publishsettings into your .gitignore and ensure you don't commit it to version control!

Use the following line to "login" the powershell module:

    Import-AzurePublishSettingsFile filename.publishsettings

And the following line to "login" the node xpat cli:

    azure account import filename.publishsettings

Note: If you are using an "Organizational Account" (e.g. Azure Ad) you can apparently just type:

    azure login username password

However I cannot confirm as I haven't used this account type yet.

(In my powershell module: https://github.com/nickmeldrum/ps-cloud/blob/master/azure-base-commands.psm1 I supply a function: Login-AzureApi which will find a .publishsettings file in the current directory and use it to "login" both the Powershell module and the node xpat cli.)

<a name="account-types"></a>
### Whats the beef with Microsoft Accounts and Organizational Accounts then?

Take 2 account types into the shower? Not me, I just login and go!

A Microsoft account is the new name for a "Live ID" and Microsoft would like this to be your personal login for all of their services from Azure to XBox. Importantly it is not tied to your organization but to you personally so you should be able to keep the same login in perpetuity.

An organizational account is an account in an Azure Active Directory (or in an AD that is federated with or synchronized to Azure AD.) This is used by organizations to manage services administered by individuals and mitigating the risk of having those services tied to just a persons individual Microsoft Account.

Using Azure services (via Powershell or the xpat cli) with a Microsoft Account requires you to "login" using a management certificate held in a .publishsettings file.
Using Azure services (via Powershell or the xpat cli) with an Organizational Account you can "login" using a username/password (via a credentials object in PowerShell.)

If you want to use Office 365, you can only use an Orgainzational account for that.

(Side note: when trying to automate Azure Powershell in an unattended way using the new Azure Powershell commands and a Microsoft Account seems impossible because the Azure-AddAccount function fires up a "sign-in" modal box unfortunately.)

If you have an MSDN Subscription purchased for you by work it *must* be tied to your Microsoft account. An MSDN Subscription is always purchased for an individual and infers a whole load of rights to you, see here for more details: http://nakedalm.com/do-you-want-visual-studio-ultimate-for-free-do-you-have-msdn/

Here is an excellent article on setting up an Azure AD from scratch: http://blog.codingoutloud.com/2014/01/24/stupid-azure-trick-2-how-do-i-create-a-new-organizational-account-on-windows-azure-active-directory-without-any-existing-accounts-or-ea/


## Links

For more details or reference articles, see the following:

 * check out readme at https://www.npmjs.com/package/azure-cli or shansleman's post on it here: http://www.hanselman.com/blog/ManagingTheCloudFromTheCommandLine.aspx
 * actually this one probably has the most details: https://azure.microsoft.com/en-gb/documentation/articles/xplat-cli/
 * wealth of information on managing azure websites: http://microsoftazurewebsitescheatsheet.info/

### Command auto-completion for the xpat cli

 * On Windows Powershell: http://blogs.msdn.com/b/stuartleeks/archive/2015/08/17/posh-azurecli-command-completion-for-azure-cross-platform-command-line-in-powershell.aspx
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


