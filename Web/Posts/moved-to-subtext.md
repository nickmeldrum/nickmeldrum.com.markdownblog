So I've moved my blog to my own hosted [subtext ](http://subtextproject.com/)domain. 
This was one of the least painful "setting up an OSS project on a shared host" experiences I've had. 
My previous experiences include:

 1. Failing to get [nHibernate](https://www.hibernate.org/343.html) to work 
in a medium trust shared host environment (because of the way the proxy uses reflection breaks standard medium
 trust settings) so moving to [SubSonic](http://www.subsonicproject.com/) for shared host ORM needs... 
(I love subsonic for some stuff, but still recommend nHibernate for my enterprise level stuff I'm afraid.)

 2. Running a few discussion forums based on [yetanotherforum.Net](http://www.yetanotherforum.net/) - also nice and easy to set up and get going with on a shared host to be honest. 

### Lessons along the way

#### Subfolders

From the outset I knew that I wanted the subtext blog to be viewable from a subfolder,
 ie. http://nickmeldrum.com/blog/ and as it's a bit of a pain to get my hosting provider to get 
virtual directories set up assumed this meant that I would need to get the blog engine working 
from a subfolder physically on the server.  So I played about with various different ways of 
actually physically getting it to work while all the files were under a subfolder. None of them 
were working. In the end I RTFM'd a bit more and worked it out!  Subtext works with alot of redirection
 and HTTP Handler settings

So after [reading this page](http://code.google.com/p/subtext/wiki/UrlBlogMapping)
 I worked out that actually the blog can seem to come from any subfolder by setting the "subfolder field" 
in the /hostadmin/ page.
 That way if I wanted I could have a different home page to the blog at [http://nickmeldrum.com/](http://nickmeldrum.com/)
 and just have the blog at [http://nickmeldrum.com/blog/](http://nickmeldrum.com/blog/).

  Note, so you don't have to require the "default.aspx" page -
 ensure you [read this page too](http://subtextproject.com/Configuring-a-Custom-404-Page.ashx).

#### WordPress lockin

Another learning (which I kind of thought would happen) was that it's not easy to get your blog posts out of WordPress.
 This kind of stinks of WordPress - which is a shame, as my experience while using WordPress was very positive.
 However they don't use the standard BlogML for exporting the blog - so you have to use their own system which
 would only ever allow you to import back into another WordPress blog anyway.

Yuck.

Luckily enough I'd only written 4 posts so far, so I did it the old fashioned "recreate them all" style. 
 Nice to see the Tag Cloud works straight away out of the box, and the FKCD editor and technorati tags are so easy to use.
  Thanks to [Phil Haack](http://haacked.com/Default.aspx) for a great and [simple .Net blogging engine](http://subtextproject.com/).
  I'm currently using [twitterfeed.com](http://twitterfeed.com/) to advertise my blog posts.

#### TODO

*   I want a twitter stream on the side of my blog - gotta work out how to get that into this skin.
*   I think a "last 5 blog posts" is a more useful set of links than a month by month archive - will try to get that into the skin too.
*   Oh where's my Gravatar! gotta sort that out too :)