# Setting up azure for the 2nd time

## Standard disclaimer for all azure posts:

It changes so fast (the services, the naming, the pricing models, the "perma links" going dead etc etc.) that very soon this article and all azure articles will be out of date. If you notice any out of date ness or other issues with this post - please let me know by commenting below!

## The problem:

(Warning: be aware of the risk of setting up azure services on an MSDN subscription paid for by your company without automating the creation of them. Your employer could cancel the subscription at any time and if they do you won't be able to move those services to another subscription without "re-enabling the msdn subscription" - which of course is impossible. Therefore you have completely lost all your azure services settings.)

Instead set it all up in a repeatable fashion from code which you control. Here's how:

## Setting up a shared web app

 * install the azure cli: https://azure.microsoft.com/en-gb/documentation/articles/xplat-cli-install/
 * 

