<img src="/media/azure_logo.png" alt="Microsoft Azure" title="Microsoft Azure" class="centered"/>

## It's a series of posts

* [Part 1: The background](/blog/scripting-azure-services-with-PowerShell-thebackground-part1 "The background post (Part 1)")
* Part 2: Azure Accounts, Subscriptions and Installs: Setting up your account and environment (this one)

## The hello world of Azure posts

<img src="/media/helloworld.png" alt="Hello World" title="Hello World" class="centered"/>

This post is about how I set up my Azure workspace. It's about connecting to my account and using my subscription. It's about Installing the Azure command line packages and hooking it all up so I can just work with zero friction.

I wanted to use / try out both the Azure PowerShell interface as well as the "Azure xpat cli" which is the nodejs based cross-platform way of connecting to Azure. This article will deal with both.

First, let's understand what the difference between an Account and a Subscription is. Then we will look at the different types of Accounts that you can use with Azure. Then we'll get to some commands you can run to actually install, set up and login to Azure PowerShell and the Azure xpat cli.

## Accounts and subscriptions

<img src="/media/subscription.png" alt="Accounts and Subscriptions" title="Accounts and Subscriptions" class="centered"/>

(From page: [Understanding resource access in Azure](https://msdn.microsoft.com/en-us/library/azure/dn584083.aspx "Accounts and Subscriptions (msdn.microsoft.com)"))

### The account

The account is how Microsoft Azure authenticates you. There are 2 types: a "Microsoft Account" (tied to an email address) and an "Organizational Account" (an Active Directory account.) I will go into the differences between those below. Just remember an account is something that has a password and allows Azure to know that your requests are coming from you.

### What's the beef with Microsoft Accounts and Organizational Accounts then?

> Take 2 account types into the shower? Not me, I just login and go!

<img src="/media/wash-and-go.jpg" alt="Just wash and go!" title="Just wash and go!" class="centered"/>

**A Microsoft account** is the new name for a "Live ID" and Microsoft would like this to be your personal login for all of their services from Azure to Xbox. Importantly it is not tied to your organization but to you personally so you should be able to keep the same login in perpetuity.

Using **A Microsoft account** requires you to authenticate using a management certificate held in a .publishsettings file to access Azure PowerShell or the xpat cli.

You can [go here to create a Microsoft account](https://signup.live.com/ "Sign up to a Microsoft Account (live.com)") (against any email address, not just outlook or hotmail as some people think.)

[What is a Microsoft account in their own words](http://windows.microsoft.com/en-GB/windows-live/sign-in-what-is-microsoft-account "What is a Microsoft account (windows.microsoft.com)")

**An organizational account** is an account in an Azure Active Directory (or in an AD that is federated with or synchronized to Azure AD.) This is used by organizations to manage services administered by individuals and mitigating the risk of having those services tied to just a persons individual Microsoft Account instead of the organization.

Using **An organizational account** you can authenticate using a username/password (via a secure credentials object in PowerShell.)

If you want to create **An organizational account**, [here is an excellent article on setting up an Azure AD from scratch](http://blog.codingoutloud.com/2014/01/24/stupid-azure-trick-2-how-do-i-create-a-new-organizational-account-on-windows-azure-active-directory-without-any-existing-accounts-or-ea/ "Creating an organizational account (blog.codingoutloud.com)") and you can have an [Azure AD for free](http://azure.microsoft.com/en-gb/pricing/details/active-directory/ "Active Directory pricing models (azure.microsoft.com)")!

### The little differences between the account types:

<img src="/media/the_little_differences.jpg" alt="The little differences..." title="The little differences..." class="centered"/>

* If you want to use Office 365, you will need **An organizational account**.
* To use the new Azure API, requires you to create resource groups and run the Azure-AddAccount function which pops up a GUI modal dialog box when using **A Microsoft account**. So you will need to us **An organizational account** to create pure unattended scripts for now.

### The subscription

<img src="/media/showmethemoney.jpg" alt="Show me the money!" title="Show me the money!" class="centered"/>

This is how Microsoft Azure knows how to bill you for your Azure resource usage. There are a number of ways Microsoft can bill you, but for your personal usage you will typically be interested in 2 different options:

1. An MSDN account: Most professional Microsoft developers will have been given an MSDN license by their company in order to use Visual Studio at work. These licenses come with a monthly quota of credit you can use to pay for your Azure resource usage. Note than an MSDN Subscription is always purchased for an individual and infers a whole load of rights to you, [see here for more details](http://nakedalm.com/do-you-want-visual-studio-ultimate-for-free-do-you-have-msdn/).
<img src="/media/msdn.png" alt="MSDN" title="MSDN" class="centered"/>
This means that this license is given to you personally and the Azure credits can be used by you for anything outside of the company that purchased the license for you, even your own commercial pursuits. You can set up a whole lot of Azure goodies and never pay a dime. If you don't have a currently valid MSDN license, you will have to go for option 2:

2. Set up a "Pay as you go" subscription and give them your bank details. Simple option - you are going to pay yourself for everything you use. Remember to set up billing alerts and possibly even a spending limit on it. [Set up and manage both on the subscriptions page.](https://account.windowsazure.com/Subscriptions)

started using a "microsoft" account and authenticating using a .publishsettings file - all well and good until I want to use the new webapp azure PowerShell functions: (the ones that are superceding the websites.) These use resource groups and refuse to be authenticated using a .publishsettings file.


2 ways to authenticate ourselves with Azure through PowerShell.

Add-AzureAccount and Import-AzurePublishSettingsFile . the Publishsettings method allows us to use a management certificate which is downloaded and held in a .publishsettings file. The Add-AzureAccount method allows us to use credentials. Far better of course because we don't need to store a certificate in a file. Instead we can use a credentials object and even the PasswordVault to authenticate this way. The problem is that we can only use an Organizational Account to do this in an unattended way. If you are using a Microsoft Account the only way of using Add-AzureAccount is with a pop-up sign-in box. Eurgh.

<img src="/media/eurgh.jpg" alt="vomit" title="you just made me sick up in my mouth" class="centered"/>

https://azure.microsoft.com/en-gb/documentation/articles/powershell-install-configure/

So if you are using a microsoft account you have to authenticate with these new functions using "add-azureaccount" - if using an organizational id - all well and good as you can pass in a credentials object. however if you are using a microsoft account this CANNOT be automated (as far as I can work out.) The only way to get an auth token is by using the UI to login (via add-azureaccount without passing in creds.) This obviously means this can't be scripted in an unattended manner, which is a must-have for me.

so onto creating an organisation instead of using a "personal" account.


research:
https://social.technet.microsoft.com/Forums/azure/en-US/e60a3fcb-ea2f-4bc8-a05e-488c98006315/azure-PowerShell-resource-manager-mode-getting-authentication-failures?forum=windowsazuremanagement
http://blog.codingoutloud.com/2014/01/24/stupid-azure-trick-2-how-do-i-create-a-new-organizational-account-on-windows-azure-active-directory-without-any-existing-accounts-or-ea/
http://blog.codebeastie.com/setting-up-an-azure-environment-for-a-small-company/
https://msdn.microsoft.com/en-gb/library/azure/hh531793.aspx
http://blog.aditi.com/cloud/organizational-identity-microsoft-accounts-azure-active-directory-part-1/
delete github repo requires a change to the personal access token: add the scope delete_repo to the token via github.com settings


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

## Next post

* Part 3: Some implementation detail - Post to come soon...
