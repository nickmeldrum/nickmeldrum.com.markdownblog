Now, I'm going to have to admit something: I'm still a Linq beginner. But every day I use it I love it more and more.

## Why is Linq so great?

Today I discovered one of the reasons Linq is great - because it makes it easy to run set based semantics on the object graph.

A great reason to use an RDBMS in your application is if the domain lends itself to set-based logic. However I found myself all to often letting go of that set-based thinking when working in the model because the language didn't lend itself to it. It wasn't impossible before Linq, especially with great libraries like C5, but it was still a pain. Now I can think in sets and write my C# in sets without friction.

## On to my discovery:

(Note I have vastly simplified the domain just to get my Linqy goodness point across)

I have an application that is very set based. One of the things it does is to create a report based on a number of calculations it does on some volumes of substances.

Every time I run a calculation on a substance I may want to add a "Note" to the report. This note will have a link to the substance.

I came to actually showing the Report in the UI. At the bottom I show all the notes. "Ack that looks terrible": the same Note was added 5 times, but with different substances - so looping through them meant that the Note description was printed 5 times along with each substance.

> <span style="font-family: courier new;">Note 1: (Long description) on Substance: x
> Note 1: (Long description) on Substance: y
> Note 1: (Long description) on Substance: z
> Note 1: (Long description) on Substance: z
> Note 1: (Long description) on Substance: z</span>

My first thought was "Dammit my model is wrong. A Note should have a set of children Substances rather than just 1." Now in terms of the "correctness of the model" I'm not actually sure. However this would certainly make the UI code easier, but the model more complex and the calculation code even more complex as instead of creating a new note each time it would have to check if the note was created and add a substance to it.

Whether the model is slightly wrong or not, I decided that to change the model based purely on a UI display issue was wrong. And here was where Linq came in. I used the GroupBy method to group my notes by note and zing: I get a collection of unique Notes with a list of substances within each one.

<div class="csharpcode">
<pre class="alt"><span class="lnum">   1:  </span>NotesRepeater.DataSource = from n <span class="kwrd">in</span> Report.ReportNotes</pre>
<pre><span class="lnum">   2:  </span>    group n by n.Note into g</pre>
<pre class="alt"><span class="lnum">   3:  </span>    select <span class="kwrd">new</span> ReportNotesDTO {</pre>
<pre><span class="lnum">   4:  </span>        Note = g.Key.Name,</pre>
<pre class="alt"><span class="lnum">   5:  </span>        NoteDetails = g</pre>
<pre><span class="lnum">   6:  </span>    };</pre>
</div>
<style type="text/css"><!--
.csharpcode, .csharpcode pre
{
   font-size: small;
   color: black;
   font-family: consolas, "Courier New", courier, monospace;
   background-color: #ffffff;
   /*white-space: pre;*/
}
.csharpcode pre { margin: 0em; }
.csharpcode .rem { color: #008000; }
.csharpcode .kwrd { color: #0000ff; }
.csharpcode .str { color: #006080; }
.csharpcode .op { color: #0000c0; }
.csharpcode .preproc { color: #cc6633; }
.csharpcode .asp { background-color: #ffff00; }
.csharpcode .html { color: #800000; }
.csharpcode .attr { color: #ff0000; }
.csharpcode .alt
{
   background-color: #f4f4f4;
   width: 100%;
   margin: 0em;
}
.csharpcode .lnum { color: #606060; }
--></style>

<!-- <![cdata[    .csharpcode, .csharpcode pre  {   font-size: small;   color: black;   font-family: consolas, "Courier New", courier, monospace;   background-color: #ffffff;   /*white-space: pre;*/  }  .csharpcode pre { margin: 0em; }  .csharpcode .rem { color: #008000; }  .csharpcode .kwrd { color: #0000ff; }  .csharpcode .str { color: #006080; }  .csharpcode .op { color: #0000c0; }  .csharpcode .preproc { color: #cc6633; }  .csharpcode .asp { background-color: #ffff00; }  .csharpcode .html { color: #800000; }  .csharpcode .attr { color: #ff0000; }  .csharpcode .alt   {   background-color: #f4f4f4;   width: 100%;   margin: 0em;  }  .csharpcode .lnum { color: #606060; } -->I can now use a repeater within a repeater to show each Note individually with a list of related Substances below it. (Note I'm using the ItemDataBound event to databind the child collection from the groupby.)

<div class="csharpcode">
<pre class="alt"><span class="lnum">   1:  </span><span class="kwrd">&lt;</span><span class="html">asp:Repeater</span> <span class="attr">ID</span><span class="kwrd">="NotesRepeater"</span> <span class="attr">runat</span><span class="kwrd">="server"</span> <span class="attr">OnItemDataBound</span><span class="kwrd">="NotesRepeater_ItemDataBound"</span><span class="kwrd">&gt;</span></pre>
<pre><span class="lnum">   2:  </span><span class="kwrd">&lt;</span><span class="html">ItemTemplate</span><span class="kwrd">&gt;</span></pre>
<pre class="alt"><span class="lnum">   3:  </span>    <span class="kwrd">&lt;</span><span class="html">p</span><span class="kwrd">&gt;</span></pre>
<pre><span class="lnum">   4:  </span>        <span class="asp">&lt;%</span># DataBinder.Eval(Container.DataItem, <span class="str">"Note"</span>)<span class="asp">%&gt;</span>: used on substances:                          </pre>
<pre class="alt"><span class="lnum">   5:  </span>        <span class="kwrd">&lt;</span><span class="html">asp:Repeater</span> <span class="attr">ID</span><span class="kwrd">="NoteDetailsRepeater"</span> <span class="attr">runat</span><span class="kwrd">="server"</span><span class="kwrd">&gt;</span></pre>
<pre><span class="lnum">   6:  </span>        <span class="kwrd">&lt;</span><span class="html">ItemTemplate</span><span class="kwrd">&gt;</span></pre>
<pre class="alt"><span class="lnum">   7:  </span>            <span class="asp">&lt;%</span># DataBinder.Eval(Container.DataItem, <span class="str">"SubstanceInvolved"</span>)<span class="asp">%&gt;</span><span class="kwrd">&lt;</span><span class="html">br</span> <span class="kwrd">/&gt;</span></pre>
<pre><span class="lnum">   8:  </span>         <span class="kwrd">&lt;/</span><span class="html">ItemTemplate</span><span class="kwrd">&gt;</span></pre>
<pre class="alt"><span class="lnum">   9:  </span>         <span class="kwrd">&lt;/</span><span class="html">asp:Repeater</span><span class="kwrd">&gt;</span></pre>
<pre><span class="lnum">  10:  </span>    <span class="kwrd">&lt;/</span><span class="html">p</span><span class="kwrd">&gt;</span></pre>
<pre class="alt"><span class="lnum">  11:  </span><span class="kwrd">&lt;/</span><span class="html">ItemTemplate</span><span class="kwrd">&gt;</span></pre>
<pre><span class="lnum">  12:  </span><span class="kwrd">&lt;/</span><span class="html">asp:Repeater</span><span class="kwrd">&gt;</span></pre>
</div>
<style type="text/css"><!--
.csharpcode, .csharpcode pre
{
   font-size: small;
   color: black;
   font-family: consolas, "Courier New", courier, monospace;
   background-color: #ffffff;
   /*white-space: pre;*/
}
.csharpcode pre { margin: 0em; }
.csharpcode .rem { color: #008000; }
.csharpcode .kwrd { color: #0000ff; }
.csharpcode .str { color: #006080; }
.csharpcode .op { color: #0000c0; }
.csharpcode .preproc { color: #cc6633; }
.csharpcode .asp { background-color: #ffff00; }
.csharpcode .html { color: #800000; }
.csharpcode .attr { color: #ff0000; }
.csharpcode .alt
{
   background-color: #f4f4f4;
   width: 100%;
   margin: 0em;
}
.csharpcode .lnum { color: #606060; }
--></style>

That was so easy. Thanks Linq!

<pre class="csharpcode"></pre>

<!-- <![cdata[    .csharpcode, .csharpcode pre  {   font-size: small;   color: black;   font-family: consolas, "Courier New", courier, monospace;   background-color: #ffffff;   /*white-space: pre;*/  }  .csharpcode pre { margin: 0em; }  .csharpcode .rem { color: #008000; }  .csharpcode .kwrd { color: #0000ff; }  .csharpcode .str { color: #006080; }  .csharpcode .op { color: #0000c0; }  .csharpcode .preproc { color: #cc6633; }  .csharpcode .asp { background-color: #ffff00; }  .csharpcode .html { color: #800000; }  .csharpcode .attr { color: #ff0000; }  .csharpcode .alt   {   background-color: #f4f4f4;   width: 100%;   margin: 0em;  }  .csharpcode .lnum { color: #606060; } -->