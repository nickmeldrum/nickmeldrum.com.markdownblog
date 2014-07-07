There's a request I have been hearing a few times recently. It goes something like this:

> "Where is a friendlier editor for my customer/BA/PM to write my Gherkin features? I can't expect them to use Visual Studio!"

<img src="/media/gherkin-building.jpg" style="float: right;padding-left: 20px;" alt="Okay this is a different Gherkin"></img>

I started hearing it so much I thought: "Ah hah! A gap in the very overcrowded open source &lsquo;development/ productivity tools&rsquo; market. I could become famous and write the first .Net Gherkin editor for business users. Now, to think of a cute name..."

**How about 'Cornichon&rdquo; or 'Piccalilli'? Or... 'Merkin&rdquo;?!**

After thinking for a while about how this tool could work, I realised that I was thinking about this part of BDD/SBE/ATDD all wrong. By the way, for the uninitiated,

 * BDD = '[Behaviour Driven Development](http://dannorth.net/introducing-bdd/ "Introducing bdd by Dan North")&rdquo;
 * SBE = '[Specification By Example](http://specificationbyexample.com/key_ideas.html "Specification By Example by Gojko Adzic")&rdquo;
 * and ATDD = '[Acceptance Test Driven Development](http://testobsessed.com/blog/2008/12/08/acceptance-test-driven-development-atdd-an-overview/ "Elisabeth Hendrickson")&rdquo;.

Remember: BDD is about focusing on the business goals and the practice of 'outside-in development&rdquo;. What is the reason we want a tool for the non developer to write the Gherkin features?

> 'To get the customer or the BA to define the application of course &ndash; it&rsquo;s their application!"

A tool could encourage the feature and scenarios to be written by the customer or BA without the technical team involved. Then the developers could go implement the steps and the system to satisfy the steps right?

But [BDD is about the conversation](http://dannorth.net/whats-in-a-story/ "Dan North"). I don&rsquo;t want to have a tool separating the team. Instead I want to use Gherkin as a platform for agreeing the formal language the whole team uses to communicate their understanding of the project.

<img src="/media/2headssharing.jpg" style="float: left;padding-right: 20px;" alt="2 Heads sharing 1 thought?"></img>

So instead, make Gherkin a part of your conversations with your customer. Get a technical team member and a customer together to discuss the scenarios. This discussion is where the real value is. It&rsquo;s where we get to attack the assumptions we are all making when we think about that feature.

Don't get me wrong. I believe tools and automation are key. I want executable specifications here. I want a spec that can break the build. I want to use this methodology to bridge the gap between the specification and the testing.

I also want to reduce the brittleness of my steps and encourage reuse. The idea is once we are well into defining our features, we should have enough step definitions to be able to craft scenarios without any code.

Good luck creating useful reusable steps that bind well to your test code if you get a business user to write them. We need a developer to guide the scenario writing to make the process of step reuse actually work.

* * *

One last point: Another added value of ensuring that a representative from the business as well as the technical side get together to discuss the features and scenarios. The more times they go through this process, the more they will uncover all those "implicit assumptions" about the system that aren't written into a scenario.

Who&rsquo;s making the incorrect assumption here?

<img src="/media/geekandpoke-followingme.jpg" alt="Is there an implicit assumption being made here?"></img>

Implicit assumptions are dangerous if they aren't shared by everyone. Only good communication will uncover these assumptions.

Sometimes you can add a scenario to cover the assumption. This makes it explicit and part of the living documentation. All good. Sometimes it&rsquo;s not possible to do that though. These implicit assumptions should be noted in some "non living" documentation somewhere. Else they will trip someone up in the future.