<p>It's not uncommon to see a MS SqlServer table creation script like this:</p>
<div class="csharpcode">
<pre class="alt"><span class="lnum">   1:  </span><span class="kwrd">CREATE</span> <span class="kwrd">TABLE</span> [Animal] (</pre>
<pre><span class="lnum">   2:  </span>    [Id] <span class="kwrd">smallint</span> <span class="kwrd">IDENTITY</span>(1,1) <span class="kwrd">NOT</span> <span class="kwrd">NULL</span> <span class="kwrd">PRIMARY</span> <span class="kwrd">KEY</span> <span class="kwrd">CLUSTERED</span>,</pre>
<pre class="alt"><span class="lnum">   3:  </span>    [Name] nvarchar(20) <span class="kwrd">NOT</span> <span class="kwrd">NULL</span> <span class="kwrd">UNIQUE</span>,</pre>
<pre><span class="lnum">   4:  </span>    [NumberOfLegs] <span class="kwrd">smallint</span> <span class="kwrd">NOT</span> <span class="kwrd">NULL</span> <span class="kwrd">DEFAULT</span> 4,</pre>
<pre class="alt"><span class="lnum">   5:  </span>    [Genus] <span class="kwrd">smallint</span> <span class="kwrd">NOT</span> <span class="kwrd">NULL</span> <span class="kwrd">FOREIGN</span> <span class="kwrd">KEY</span> <span class="kwrd">REFERENCES</span> [Genus] ([Id])</pre>
<pre><span class="lnum">   6:  </span>)</pre>
</div>
<style type="text/css"><!--
.csharpcode, .csharpcode pre
{
   font-size: small;
   padding: 0;
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
<p><!--.csharpcode, .csharpcode pre {    padding: 0;    font-size: small;    color: black;    font-family: consolas, "Courier New", courier, monospace;    background-color: #ffffff;    /*white-space: pre;*/ } .csharpcode pre { margin: 0em; } .csharpcode .rem { color: #008000; } .csharpcode .kwrd { color: #0000ff; } .csharpcode .str { color: #006080; } .csharpcode .op { color: #0000c0; } .csharpcode .preproc { color: #cc6633; } .csharpcode .asp { background-color: #ffff00; } .csharpcode .html { color: #800000; } .csharpcode .attr { color: #ff0000; } .csharpcode .alt  {    background-color: #f4f4f4;    width: 100%;    margin: 0em; } .csharpcode .lnum { color: #606060; } -->And in general it looks good. Obviously some thought has gone into it's representation, each column is restrained by constraints that make sense to the domain - we have a nice simple primary key, we have a unique name, a default number of legs and it's species is a foreign key to another table. We shouldn't have too many problems with the integrity of our data here.</p>
<p>But what about when our requirements change (as they always do) and we need to change the schema. I'm stretching the Animal analogy here, but the domain isn't important. It turns out we don't want [NumberOfLegs] against this table anymore. Maybe we want to denormalise the properties of the Animal even more (eg. have an Animal class including insects and store the NumberOfLegs against that.)</p>
<p>So we have to delete the column [NumberOfLegs]. Easy you say, lets just issue:</p>
<pre class="csharpcode"><span class="kwrd">ALTER</span> <span class="kwrd">TABLE</span> [Animal]<span class="kwrd">DROP</span> <span class="kwrd">COLUMN</span> [NumberOfLegs];</pre>
<p>Whoops, error: "<span style="font-family: courier new; color: #ff0000;">The object 'DF__Animal__NumberOf__6CB8F890' is dependent on column 'NumberOfLegs'.</span>"</p>
<p>And dammit I can't easily Drop that default constraint because it's not predictably named. I know the name on this server, but I will be running this script on every server running this application and all the default constraint names will be different!</p>
<p>So instead I have to end up writing the following tortuous script just to remove 1 randomnly named default constraint:</p>
<div class="csharpcode">
<pre class="alt"><span class="lnum">   1:  </span><span class="kwrd">DECLARE</span> @NumLegsDefaultName nvarchar(255)</pre>
<pre><span class="lnum">   2:  </span>&nbsp;</pre>
<pre class="alt"><span class="lnum">   3:  </span><span class="kwrd">SELECT</span></pre>
<pre><span class="lnum">   4:  </span>    @NumLegsDefaultName = cObj.name</pre>
<pre class="alt"><span class="lnum">   5:  </span><span class="kwrd">FROM</span></pre>
<pre><span class="lnum">   6:  </span>    sysobjects    [cObj]</pre>
<pre class="alt"><span class="lnum">   7:  </span>    <span class="kwrd">join</span> sysobjects [tObj] <span class="kwrd">ON</span> (cObj.parent_obj = tObj.id)</pre>
<pre><span class="lnum">   8:  </span>    <span class="kwrd">join</span> sysconstraints [con] <span class="kwrd">ON</span> (cObj.id    = con.constid)</pre>
<pre class="alt"><span class="lnum">   9:  </span>    <span class="kwrd">join</span> syscolumns [col] <span class="kwrd">ON</span> (tObj.id = col.id <span class="kwrd">and</span> con.colid = col.colid)</pre>
<pre><span class="lnum">  10:  </span><span class="kwrd">WHERE</span></pre>
<pre class="alt"><span class="lnum">  11:  </span>    cObj.xtype    = <span class="str">'D'</span> <span class="rem">-- This means default</span></pre>
<pre><span class="lnum">  12:  </span>    <span class="kwrd">and</span> tObj.name = <span class="str">'Animal'</span></pre>
<pre class="alt"><span class="lnum">  13:  </span>    <span class="kwrd">and</span> col.name = <span class="str">'NumberOfLegs'</span></pre>
<pre><span class="lnum">  14:  </span>&nbsp;</pre>
<pre class="alt"><span class="lnum">  15:  </span><span class="kwrd">IF</span> (ISNULL(@NumLegsDefaultName, <span class="str">''</span>) &lt;&gt; <span class="str">''</span>)</pre>
<pre><span class="lnum">  16:  </span><span class="kwrd">BEGIN</span></pre>
<pre class="alt"><span class="lnum">  17:  </span>    <span class="kwrd">EXEC</span>(<span class="str">'ALTER TABLE [Animal] DROP CONSTRAINT '</span> + @NumLegsDefaultName);</pre>
<pre><span class="lnum">  18:  </span>END</pre>
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
<p>Now I'm allowed to drop that column. Ouch.</p>
<p>I've always enforced the requirement to name primary keys, unique keys, foreign keys and indexes because I know they will need to be changed in the future so I want to be able to get a handle for any alter statements. I just forgot about default constraints!</p>
<p>So now I would recommend that table above be written instead like the following with everything named:</p>
<p>(Note: For readability, I would prefer to be able to define my default constraint below like I can separate all my other indexes, keys and constraints. Unfortunately MS SqlServer syntax won't allow it. I think this is as readable as I can get - and notice my strict naming conventions but that's for another even more pedantic post!)</p>
<div class="csharpcode">
<pre class="alt"><span class="lnum">   1:  </span><span class="kwrd">CREATE</span> <span class="kwrd">TABLE</span> [Animal](</pre>
<pre><span class="lnum">   2:  </span>    [Id] <span class="kwrd">smallint</span> <span class="kwrd">IDENTITY</span>(1,1) <span class="kwrd">NOT</span> <span class="kwrd">NULL</span>,</pre>
<pre class="alt"><span class="lnum">   3:  </span>    [Name] nvarchar(20) <span class="kwrd">NOT</span> <span class="kwrd">NULL</span>,</pre>
<pre><span class="lnum">   4:  </span>    [NumberOfLegs] <span class="kwrd">smallint</span> <span class="kwrd">NOT</span> <span class="kwrd">NULL</span> <span class="kwrd">CONSTRAINT</span> [DF_Animal_NumberOfLegs] <span class="kwrd">DEFAULT</span> 4,</pre>
<pre class="alt"><span class="lnum">   5:  </span>    [Genus] <span class="kwrd">smallint</span> <span class="kwrd">NOT</span> <span class="kwrd">NULL</span>,</pre>
<pre><span class="lnum">   6:  </span>    <span class="kwrd">CONSTRAINT</span> [PK_Animal] <span class="kwrd">PRIMARY</span> <span class="kwrd">KEY</span> <span class="kwrd">CLUSTERED</span>([Id] <span class="kwrd">ASC</span>),</pre>
<pre class="alt"><span class="lnum">   7:  </span>    <span class="kwrd">CONSTRAINT</span> [UQ_Animal_Name] <span class="kwrd">UNIQUE</span> ([Name]),</pre>
<pre><span class="lnum">   8:  </span>    <span class="kwrd">CONSTRAINT</span> [FK_Animal_Genus] <span class="kwrd">FOREIGN</span> <span class="kwrd">KEY</span> ([Genus]) <span class="kwrd">REFERENCES</span> [Genus] ([Id])</pre>
<pre class="alt"><span class="lnum">   9:  </span>)</pre>
</div>
<div class="csharpcode">&nbsp;</div>