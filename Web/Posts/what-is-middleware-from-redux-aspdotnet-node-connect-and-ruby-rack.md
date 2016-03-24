## Just a brief sojourn...

It turns out, after a little bit of research that defining what middleware is would be a whole thesis in itself. I'm not going to do this article the justice I wanted to, but still think it's worth a brief peek into the definition and etymology of this word.

## Why my interest?

I just posted about looking at some JavaScript techniques for implementing the decorator pattern, sort of inspired by looking at Dan Abramov's redux middleware implementation.

I then entered into a personal well of confusion not understanding the difference between middleware, pipelines and decorators. So my journey began.

### The professor's intepretation

![Professor Heinz Wolff, nothing to do with middleware.](/media/professor.jpg "Professor Heniz Wolff. Nothing to do with middleware.")

The [Electrical and Computer Engineering Department of University of Toronto](https://www.ece.utoronto.ca/) does a course titled: "Introduction to Middleware" (possibly originally from the [Pôle Universitaire Léonard de Vinci](http://www.devinci.fr/en/).)

From [this slide](http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld002.htm "what is middleware") we get an interesting definition:

> "Generally a higher-level software layer providing a set of standardised interfaces to a collection of distributed, disparate, proprietary, heterogeneous computing resources. Developers write applications that interface to the midleware, rather than to proprietary lower-level interfaces."

And they [give some examples](http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld005.htm "examples of middleware") of this kind of middleware:

 * [ODBC](https://en.wikipedia.org/wiki/Open_Database_Connectivity)/ [JDBC](https://en.wikipedia.org/wiki/Java_Database_Connectivity)
 * [CORBA](https://en.wikipedia.org/wiki/Common_Object_Request_Broker_Architecture)/ [DCOM](https://en.wikipedia.org/wiki/Distributed_Component_Object_Model)

What do all these examples have in common? They are all technologies (software) that create an interface for talking to distributed systems written by multiple different 3rd party proprietary systems.

To me, that makes a lot of sense in terms of a definition. The word "middleware" really seems to fit this kind of system.

### Modern usage

It seems nowadays every framework is implementing "middleware". The new [ASP.Net 5 or "ASP.Net Core"](http://docs.asp.net/en/latest/fundamentals/middleware.html) technology is using the term middleware to describe it's implementation of something that sounds very similar to [Connect for Node.js's middleware](https://github.com/senchalabs/connect).

From the ASP.Net page:

> "Middleware are components that are assembled into an application pipeline to handle requests and responses."

From the Connect for Node.js page:

> "Middleware are added as a "stack" where incoming requests will execute each middleware one-by-one until a middleware does not call next() within it."

And it is my understanding that Connect borrowed this middleware term from [Rack for Ruby](http://rack.github.io/) who seem to the be the originators the the modern usage of the term.

The Rack usage of the term middleware is interesting:

> "Rack provides a minimal interface between webservers that support Ruby and Ruby frameworks"

And if you will forgive me, I will use the image from the ASP.Net page to show the concept, as it's the best diagram I've seen:

![middleware in request response](http://docs.asp.net/en/latest/fundamentals/middleware.html)

Ruby Rack, Connect for Node.js and ASP.Net 5 middleware all basically do the same thing, allow software to deal with a request/ response without having to worry about the web server implementation.

This seems akin to the original description, not unlike ODBC. It's a software layer inbetween a software program's code and a proprietary system. In this case a web server.

### So what?

What interests me about all this is that diagram seems an awful lot like the decorator pattern. I have also heard it


### Dan Abramov and Redux are AWESOME

[connect middleware for node.js]()

[some university course's definition of middleware](http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld002.htm)
http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld005.htm
examples of middleware: ODBC, CORBA, DCOM...

not knocking redux or Dan, he is clearly awesome and I am in love with redux.

this middleware page confused me for a while though and it wasn't until I started working on a decorator pattern that I realised why.

Middleware comes from Ruby Rack and then used by Node connect - both are the pipeline pattern NOT middleware. However Redux middleware is clearly a decorator and here's why:

IT WRAPS the inner function - so you can do stuff before calling AND afterwards.

The pipeline pattern is subtly different - but think of it more as a 1 way pipe. (TODO: really? can't i do stuff after calling next in connect? - needs more research?)

middleware - define it as a decorator - connect middleware has filters and providers - 
This shows the 2 types of middleware: filters and providers. A classic example of filters is logging, whereas the providers are meant to do a specific action, a classic example is the static middleware which will serve a static file on a route that matches a filename. This will, like the validator, stop subsequent functions executing.

from http://redux.js.org/docs/advanced/Middleware.html:

>"...middleware is some code you can put between the framework receiving a request, and the framework generating a response. For example, Express or Koa middleware may add CORS headers, logging, compression, and more. The best feature of middleware is that it’s composable in a chain. You can use multiple independent third-party middleware in a single project."

from wikipedia entry on decorator pattern:

>"...allows behavior to be added to an individual object, either statically or dynamically, without affecting the behavior of other objects from the same class.[1] The decorator pattern is often useful for adhering to the Single Responsibility Principle, as it allows functionality to be divided between classes with unique areas of concern."

and 

>"...This pattern is designed so that multiple decorators can be stacked on top of each other, each time adding a new functionality to the overridden method(s)."

That sounds an awful lot like "decorator" to me - so we have a new cool word for decorator, that's okay with me I guess...

