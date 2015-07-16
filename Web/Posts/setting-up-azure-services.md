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

 github connection:
  azure site deployment github nickmeldrum --githubrepository nickmeldrum/nickmeldrum.com.markdownblog --githubusername nickmeldrum
   (Going to ask for password - or you can append --githubpassword mypasswordhere - but DON'T COMMIT THAT TO A PUBLIC REPO!!!)


(gave up and used the gui for the last part - i.e. getting the github hook to work, creating the storage account and setting the config options on the site (TODO: some more config options set - redirect for isntance?)
(TODO: is it in release mode with debug false?)

## setting up staging site:

This one must be 100% scripted!

  * azure site create --location "North Europe" nickmeldrum-staging -vv
  * azure site deployment github --verbose --githubusername nickmeldrum --githubrepository nickmeldrum/nickmeldrum.com.markdownblog nickmeldrum-staging

