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
