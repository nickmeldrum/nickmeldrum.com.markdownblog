This page is interesting:
http://msdn.microsoft.com/en-us/library/aa479319.aspx
See the "Passing Control" section and the quote: "The built-in customErrors feature uses Redirect and not Transfer for a reason. The rationale of the ASP.NET development team is that Redirect accurately displays the URL of the custom error page."
It's an old page but it shows the fundamental design philosphy of dealing with errors from the ASP.Net development team was, in my opinion, wrong.

The standard behavior for dealing with errors uing the customerrors web.config element with ASP.Net is here:
http://msdn.microsoft.com/en-us/library/h0hfz6fc(v=vs.80).aspx

So if you create a new asp.net application and add the custom errors section like:
	<customErrors mode="On">
		<error redirect="404.aspx" statusCode="404" />
		<error redirect="500.aspx" statusCode="500" />
	</customErrors>

You will end up having an experience like this:

	Browse to website.com/pagethatdoesntexist: Response is 302
	Request to 404.aspx (to honour the 302 redirect): Response is 200.

There are quite a few issues with this experience. Let's look at each one in turn:
	
	
1. Making it harder on the customer:


Check the comments out here:
http://stackoverflow.com/questions/4483849/default-redirect-for-error-404

They say it all really: from a customer perspective, redirecting to a "404.html" page is a terrible idea. The customer should still be able to see the address they are currently at so they can have a chance to change it. What if it's a typo?

Also, from a technical perspective it's not right - an http redirect is there to tell the user agent (fancy talk for what is usually a web browser) "the requested resource resides temporarily under a different URI." *1 What we should be saying is the page ain't there at all!



*1 http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.3.3
