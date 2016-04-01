## Just a brief sojourn...

It turns out, after a little bit of research, that defining what middleware is would be a whole thesis in itself. I'm not going to do this article the justice I wanted to, but still think it's worth a brief peek into the definition and etymology of this word.

## Why my interest?

I [just posted](http://nickmeldrum.com/blog/decorators-in-javascript-using-monkey-patching-closures-prototypes-proxies-and-middleware) about looking at some JavaScript techniques for implementing the decorator pattern, sort of inspired by looking at [Dan Abramov's](https://medium.com/@dan_abramov) [Redux](https://github.com/reactjs/redux) middleware implementation.

I then entered into a personal well of confusion not understanding the difference between middleware, pipelines and decorators. So my journey began.

### The professor's intepretation

![Professor Heinz Wolff, nothing to do with middleware.](/media/professor.jpg "Professor Heinz Wolff. Nothing to do with middleware.")

The [Electrical and Computer Engineering Department of University of Toronto](https://www.ece.utoronto.ca/) does a course titled: "Introduction to Middleware" (possibly originally from the [Pôle Universitaire Léonard de Vinci](http://www.devinci.fr/en/).)

From [this slide](http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld002.htm "what is middleware") we get an interesting definition:

> "Generally a higher-level software layer providing a set of standardised interfaces to a collection of distributed, disparate, proprietary, heterogeneous computing resources. Developers write applications that interface to the midleware, rather than to proprietary lower-level interfaces."

And they [give some examples](http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld005.htm "examples of middleware") of this kind of middleware:

 * [ODBC](https://en.wikipedia.org/wiki/Open_Database_Connectivity)/ [JDBC](https://en.wikipedia.org/wiki/Java_Database_Connectivity)
 * [CORBA](https://en.wikipedia.org/wiki/Common_Object_Request_Broker_Architecture)/ [DCOM](https://en.wikipedia.org/wiki/Distributed_Component_Object_Model)

What do all these examples have in common? They are all technologies (software) that create an interface for talking to distributed systems written by multiple different 3rd party proprietary systems.

To me, that makes a lot of sense in terms of a definition. The word "middleware" really seems to fit this kind of system.

If I was to compare this to a "Gang of Four" design pattern it sounds most like the ["bridge pattern"](https://en.wikipedia.org/wiki/Bridge_pattern)

> "decouple an abstraction from its implementation so that the two can vary independently"

It's not a perfect comparison but the concept is similar. It's an interface to a system.

### Enter Rack for Ruby

[Rack for Ruby](http://rack.github.io/) seem to the be the originators of the modern usage of the term.

The Rack usage of the term middleware is interesting:

> "Rack provides a minimal interface between webservers that support Ruby and Ruby frameworks"

I know almost nothing about Rack but the first thing from this quote is clear: It is similar to the original definition above in that it is an abstraction over _web server_ implementations. It's a bridge.

It is *also* a decorator around an application. [This article](https://igor.io/2013/02/02/http-kernel-middlewares.html) describes it best:

> "the gem ships with a set of general-purpose Rack apps which act as decorators"

> "you can stack these middlewares to extend an app"

Here is an example that shows how people talk about Rack middleware:

![Rack Middleware](/media/rack-middleware.jpg)

But Rack for Ruby is 2 things, a general interface to multiple 3rd party proprietary systems (web servers) *AND* a decorator pattern for composing applications. It seems that the 1st thing was in line with the original definition of the word middleware, but everyone seems to talk about the Rack user applications (the decorators or pipeline apps) as the middleware like in the diagram above.

So it seems like Rack's concept of middleware, at least in the web world, got shifted to "this pipeliney/decorator thing" instead of the general interface to a 3rd party system. In Rack's case that being the code that provides the Request object and calls your code with it.

Note: some people talk about Rack and the other similar technologies as being the "pipeline" pattern like composable unix pipelines. I think this is less precise than the decorator analogy as a pipeline (certainly a unix pipeline) is 1 way. A Rack middleware component deals with the HTTP Request on the way in AND the HTTP response on the way out - decorating the application. I guess this is a bit of a pedantic point though and it's very natural to talk about a request pipeline.

### The newer kids on the block

It seems nowadays every framework is implementing "middleware" like Rack for Ruby did. No surprise, as it's a great concept.

[Connect for Node.js's middleware](https://github.com/senchalabs/connect) is a very popular implementation.

From the Connect documentation:

> "Middleware are added as a "stack" where incoming requests will execute each middleware one-by-one until a middleware does not call next() within it."

The new [ASP.Net 5 or "ASP.Net Core"](http://docs.asp.net/en/latest/fundamentals/middleware.html) technology is using the term middleware to describe it's implementation of something that sounds very similar to Connect and Rack as well.

From the ASP.Net page:

> "Middleware are components that are assembled into an application pipeline to handle requests and responses."

And if you will forgive me, I will use the image from the ASP.Net page to show the concept, as it's the best diagram I've seen:

![middleware in request response](/media/middleware.png)

So Connect and ASP.Net Core middleware are clearly both implementing the decorator pattern in the same way Rack does.

They also give you an interface to a web server. In Connect's example it uses Node's built in http server. It's possible Connect allows other http implementations but I'm not sure. ASP.Net Core allows for [different web server implementations](https://docs.asp.net/en/latest/fundamentals/servers.html) by (at a minimum) requiring 2 interfaces: `IHttpRequestFeature` and `IHttpResponseFeature` that must be supported.

So fundamentally Connect and ASP.Net Core implement the web server abstraction *as well* as the decorator just like Rack does. It seems they refer to the user applications as the Middleware just like Rack does as well.

### So what?

The original usage of the term had no decorator in sight. It was simply some software "in the middle" of your code and a 3rd party system (the bridge concept). This modern usage of the term is now intrinsically tied into the decorator pattern as well as a bridge to a proprietary system.

Now let's look at another library: [Dan Abramov's](https://medium.com/@dan_abramov) [Redux](https://github.com/reactjs/redux).

### Dan Abramov and Redux are AWESOME

Firstly, I love hot loading and I really love Redux and really feel uncomfortable criticising anything about it. Having said that...

The usage of the term middleware in Redux confused me. The [documentation for Redux middleware](http://redux.js.org/docs/advanced/Middleware.html) is a good explanation of the concept but the TL/DR is that it is a very similar decorator pattern to the above implementations (especially unsurprisingly Connect as they are both JavaScript.)

You can define a middleware component around a "dispatch" call, you just have to make sure you call the `next()` function the store gives you so that the middleware components can be chained together. This next() function is important in the JavaScript implementations in order to enable asynchronous invocations.

Here's the kicker though: there is no 3rd party proprietary system that is being bridged or interfaced. This is purely a decorator pattern over his own base `store.dispatch()` function.

### Summary: The evolution of language

Here is my summary of that potted history then:

 1. The original definition of middleware was an interface, essentially the bridge pattern, to 3rd party proprietary systems. Like ODBC.
 2. Rack came along and morphed the definition of middleware into an interface to a 3rd party system *USING* a decorator pattern for composability.
 3. Connect for Node and ASP.Net Core come along and create the exact same concept as Rack. No real change to the definition here.
 4. Redux middleware comes along and it's JUST the decorator pattern around it's own function, NO 3rd party system to get in the middle of.

Middleware is evolving into "another name for the decorator pattern" then it seems. I think that is a small shame as the original term made sense - it was **software in the *middle* (hence middleware) of your code and some other 3rd party system**. The modern usage is just another word for a decorator.

**Why not just call it a decorator or a pipeline?**

