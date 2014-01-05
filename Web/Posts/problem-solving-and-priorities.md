So yesterday [I blogged about](http://nickmeldrum.com/blog/archive/2010/02/01/problem-saving-images-as-non-bmp-with-internet-explorer.aspx) a problem we had with users trying to save images off the GIS part of our site. We have a workaround now which will work fine.

I would have loved to have been able to spend the time to get a more complete technical solution to the problem. For instance one thing that came to mind was we could possibly use an HTTP Module to intercept and modify the header for the response from that 3rd party component. We could then allow the image to be cached on the client. I have no idea if that would work - I've used HTTP Module to handle requests, never to intercept responses. So I'm only assuming even that part of it works.

So I guess I've had to grow up. I had to make the business decision to use the less perfect but simpler workaround, which will work for now if not the possible future. You could say I had to institute a [YAGNI](http://en.wikipedia.org/wiki/You_ain%27t_gonna_need_it). From our business point of view the issue is solved. Until people start using WMS I guess, but we are much too busy with other more important priorities to worry about that for now - and that's the correct way to be.

I started my career in developer support at Microsoft PSS. This job was all about digging deep to find out why something wasn't working as expected. Sometimes the customer was not doing something the best way, sometimes Microsoft's internal code wasn't doing something right. Whichever it was we had to get a workaround to the customer as soon as possible, then if appropriate dig even deeper to work out what was wrong. This may eventually find it's way to a KB article or even to a fix in a later patch. It was a fantastic job, honing problem solving skills. A perfect start for a career in development and very satisfying to be able to solve some tough problems.

Nowadays if we have a problem on our production code and I can get an acceptable workaround out quickly that has to be the end of it too often. We can't justify the time to root out the fundamental cause of every problem if it's no longer a problem for us. We always have more pressing business priorities.

Growing up is never fun.

Still, hopefully I'll have some spare time in the evening to play with this HTTP Module idea. No one said I couldn't waste my *own* time!