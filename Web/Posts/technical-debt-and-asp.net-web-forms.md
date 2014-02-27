An InfoQ article on [Technical Debt](http://www.infoq.com/news/2010/01/is-technical-debt-technical) quoted Ward Cunningham's original coining of the phrase "technical debt" that I will print out again here.

"Shipping first time code is like going into debt. A little debt speeds development so long as it is paid back promptly with a rewrite&hellip;. The danger occurs when the debt is not repaid. Every minute spent on not-quite-right code counts as interest on that debt. Entire engineering organizations can be brought to a stand-still under the debt load of an unconsolidated implementation."

Reading this was a <span style="color: #ff6600; font-weight: bold;">light bulb moment for me. I (like alot of real world developers I'm sure) live with a huge technical debt in their work lives. Some of this debt comes from our working practices, which mostly come down to us from non-technical managers, but that problem is not what I am thinking about today. The debt I am thinking about is ASP.Net web forms. A perfect example of a whole technology that suffers fundamentally from technical debt.

### So why use ASP.Net Web forms?

We currently use ASP.Net web forms (and the Telerik ASP.Net Ajax webform controls) because of the "RAD" opportunity they present. We can get rich UI on the web while still programming in C#, with just a little sprinkling bit of server side HTML markup. <span style="font-style: italic; font-weight: bold;">Please note the heavy sarcasm in my voice when writing that statement. 
 Sarcasm because this approach on this set of technologies comes with a huge amount of technical debt. It's a broken technology that attempts to hide the fact that the web is stateless and the rich UI code is really JavaScript on top of HTML. So I can get an "out of the box" Telerik grid databinding to my datasource in 2 minutes and try to ignore all the "magic" that web forms page/session state and Teleriks building of the client-side grid is doing.<span style="font-weight: bold;"> And it doesn't work. Then we need to extend and polish this grid, and all of a sudden it gets very very ugly. <span style="font-style: italic; color: #ff0000;">I spend days hacking around with this broken model just to get something simple done. I can't get my front-end code nicely designed because there is no nice separation of the view and the controller with traditional web forms approach.  <span style="font-weight: bold;">And the debt builds&hellip;

### What's the answer?

Last year we talked alot about moving to MVC, every time I mentioned it the arguments that arose boiled down to these 2:

1.  It requires JavaScript programming which is slow and doesn't have the tooling support.
2.  There aren't enough mature UI controls for us to leverage. Give me my pageable, sortable Grid out of the box.
3.  Oh and I guess a 3rd: It's a learning curve we don't have time for.

Well after having a brief look around, I'm hopeful that with our next new project we can actually make the switch. More research is needed, but let me try to refute these 3 arguments here:

1.  There are some great JavaScript dev tools coming out now. Obviously we have the libraries like JQuery but it's the lack of intellisense etc. that we really miss. Enter: [ SharpKit.Net.](http://sharpkit.net/) I can't wait to evaluate this, but it has promise! Visual Studio itself already has great JavaScript debugging and there are plenty of great FireFox add-ins like FireBug as well.
2.  JQuery UI has a nice pageable sortable Grid! [(Cheers Phil Haack!](http://haacked.com/archive/2009/04/14/using-jquery-grid-with-asp.net-mvc.aspx)) Maybe it's time&hellip;
3.  Please re-read Ward Cunningham's sortie on Debt! We waste so much time in our Web forms debt this argument is moot.

### So What Now?

I need to check out [SharpKit.Net](http://sharpkit.net/) properly.  I need to mock up a prototype project to see for myself if this really is the answer or not.  I need to do this before our next project start!