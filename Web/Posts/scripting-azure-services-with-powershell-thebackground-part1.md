<img src="/media/azure_logo.png" alt="Microsoft Azure" title="Microsoft Azure" class="centered"/>

## It's a series of posts

* Part 1: The background post (this one)
* Part 2: Installing Azure Powershell and the Azure CLI and logging in - Post to come soon...

## Standard disclaimer for all azure posts:

Azure changes so fast (the services, the naming, the pricing models, the "perma links" going dead for example) that very soon this article will be out of date. If you notice any out-of-dateness or other issues with this post, then please let me know by commenting below.

## The background:

I set up my Azure website on an MSDN subscription paid for by my previous employer which they then cancelled. Azure then immediately shut down all my services (and therefore my website went down) and didn't even give me access to open up the sites in the portal for me to see how I had set them up in order to recreate them on a new subscription.

So after talking to Azure support it turns out

> *you cannot migrate your services from a disabled account*

<img src="/media/no-entry.jpg" alt="No Entry!" title="No Entry!" width="200px" class="centered"/>

So I learnt for this reason and many others: the best way to set up your azure account is from a Powershell script so if there's ever a problem you can just re-run the script.

## A side note - here be dragons

Luckily I didn't store any data in these Azure services (or rather I only stored temporary data such as Lucene indexes.) If I had stored data in there that I needed to retrieve at some point in the future then when the account was suspended I would have irrevocably lost access to that data. Of course if you are storing important data in Azure you should have off-site backups but just beware of this potential issue!

## Okay okay, show me the money

Off I went on a voyage of discovery into the land of automating Azure services. I will try to document what I learnt and my scripts here in the hope some of the pain I went through can help someone else in the future.

<img src="/media/script.jpg" alt="Scripting" title="It's a script..." width="400px" class="centered"/>

## The requirement:

My requirement was to have my own website completely set up from scratch with 1 script. Indeed even if the website were partially set up I want the script to succeed in setting up the rest. I guess that is similar to saying I want the script to be idempotent.

> Ensure the deployment details are stored in version control and are executable

## Details details:

### General
 
 * Do it in the most cost-efficient manner possible (Respecting the "WAF" or [Wife Acceptance Factor](https://leanpub.com/RelationshipHacks "See Scott for more details"))

### Deployment and staging/ production

<img src="/media/github-logo.png" alt="Github" title="Github" width="200px" class="centered"/>

 * The code is in [github](https://github.com/nickmeldrum/nickmeldrum.com.markdownblog "markdown blog at github") with 2 branches: master and release. I code on the master branch and consider the release branch the current production version.
 * When the master branch is pushed, it gets automatically built and deployed to a [staging site](http://nickmeldrum-staging.azurewebsites.net/ "the blog's staging site"). This is important to test for issues that may not be exposed using the Azure emulators locally.
 * When I want to release to [production](http://nickmeldrum.com/ "the blog's production site"), I merge master to release and push the release branch. This will trigger a build and deployment to the production azure instance.

### DNS

<img src="/media/dnsimple.png" alt="dnsimple" title="dnsimple" width="200px" class="centered"/>

 * I use [dnsimple](https://dnsimple.com/ "awesome dns") to manage my DNS records.
 * The staging site doesn't need any DNS records and is accessible from the default [azurewebsites.net](http://nickmeldrum-staging.azurewebsites.net/ "the blog's staging site") address
 * The production site has quite a few DNS records requiring both A and CNAME recordsd to point to the production site
 * The IIS setup has a canonical hostname redirect so whichever address you use you will be redirected to the [canonical hostname](http://nickmeldrum.com/ "the blog's production site")

### Lucene search and Azure storage

<img src="/media/lucene.png" alt="lucene" title="lucene" width="200px" class="centered"/>

 * The website has a search facility implemented by Lucene which requires indexes to be built and stored locally in files
 * These files are stored in Azure storage blob containers (because storing files locally on an Azure website is a recipe for pain) and both staging and production have their own container

### The site itself

 * The staging site shows draft posts whereas the production site doesn't
 * Both sites have an admin password for website admin access
 * These settings are managed through appsettings in the web.config

## Set the scene? Done. What next?

In this first post I've talked about what I wanted to do and why. The next posts will start talking about my implementation journey!

## Next post

* Part 2: Installing Azure Powershell and the Azure CLI and logging in - Post to come soon...

