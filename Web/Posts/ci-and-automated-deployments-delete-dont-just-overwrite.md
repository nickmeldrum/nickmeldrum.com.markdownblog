At the moment I use [Nant](http://nant.sourceforge.net/) to configure/program my builds. Yes, programming in XML is hell. I have learnt my lesson. I use Teamcity to manage my builds. We use this setup to do our build (compilation and unit tests), configure deployments (mostly to set machine/deployment specific config settings) and then actually remotely deploy the applications.

How does our remote deployment work? TeamCity runs a nant script that copies the files to the remote server, including a copy of nant and a "setup" build. It then ([utilising psexec](http://technet.microsoft.com/en-us/sysinternals/bb897553.aspx)[)](http://technet.microsoft.com/en-us/sysinternals/bb897553.aspx)) runs nant remotely to actually run the setup. This will then run any DB upgrade scripts, copy over the new website files including the deployment specific config files, then redeploy our SQLServer reports just in case they've been fiddled with.This is accomplished with a whole bunch of cool Nant Extensions I wrote. This has enabled me to move some of our code into C# instead of huge amounts of ugly XML.

You may have noticed I made the decision to **_overwrite_** the website files. Less chance of a page request failing during an upgrade I reason. We are a very low intensity site, and I can pretty much pick a time when I can see no one is online. Otherwise I would have automated an "Under Maintenance" redirection page for the few minutes we were upgrading.

I figured it should have worked fine just overwriting. If a file isn't used anymore, nothing will be linking to it anyway.

Until today I wasted a whole hour trying to reproduce an error on our live server.

The same code on the staging server worked fine. On my dev server it worked fine. Hmmm, checked version numbers - yup - same codebase. Must be the database then... Restored the live database onto my dev server and still can't reproduce the error. WTF?

Ok, so I do a [Beyond Compare](http://www.scootersoftware.com/) on my debug folder and the live web app. Identical. Except that the live web app has a Global.asax and mine doesn't. Ugh. I delete that and ***bing*** the production server starts working again.

Lesson learnt.

I'm going to have to delete everything before copying an upgrade set over. Or maybe I need to have some sort of intelligent file sync app (like the way [DropBox](https://www.dropbox.com/) knows if somethings been deleted automagically.) I Seriously doubt that's pain free even if it's possible though. I will probably just employ a problem solving version of Occam's razor and just delete everything first. Maybe even stick a "maintenance" redirection page in if I'm feeling really fancy..