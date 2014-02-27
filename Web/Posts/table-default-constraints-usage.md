<p>It's not uncommon to see a MS SqlServer table creation script like this:</p>

   1:  CREATE TABLE [Animal] (
   2:      [Id] smallint IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
   3:      [Name] nvarchar(20) NOT NULL UNIQUE,
   4:      [NumberOfLegs] smallint NOT NULL DEFAULT 4,
   5:      [Genus] smallint NOT NULL FOREIGN KEY REFERENCES [Genus] ([Id])
   6:  )

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
<pre class="csharpcode">ALTER TABLE [Animal]DROP COLUMN [NumberOfLegs];
<p>Whoops, error: "<span style="font-family: courier new; color: #ff0000;">The object 'DF__Animal__NumberOf__6CB8F890' is dependent on column 'NumberOfLegs'."</p>
<p>And dammit I can't easily Drop that default constraint because it's not predictably named. I know the name on this server, but I will be running this script on every server running this application and all the default constraint names will be different!</p>
<p>So instead I have to end up writing the following tortuous script just to remove 1 randomnly named default constraint:</p>

   1:  DECLARE @NumLegsDefaultName nvarchar(255)
   2:  &nbsp;
   3:  SELECT
   4:      @NumLegsDefaultName = cObj.name
   5:  FROM
   6:      sysobjects    [cObj]
   7:      join sysobjects [tObj] ON (cObj.parent_obj = tObj.id)
   8:      join sysconstraints [con] ON (cObj.id    = con.constid)
   9:      join syscolumns [col] ON (tObj.id = col.id and con.colid = col.colid)
  10:  WHERE
  11:      cObj.xtype    = 'D' <span class="rem">-- This means default
  12:      and tObj.name = 'Animal'
  13:      and col.name = 'NumberOfLegs'
  14:  &nbsp;
  15:  IF (ISNULL(@NumLegsDefaultName, '') <> '')
  16:  BEGIN
  17:      EXEC('ALTER TABLE [Animal] DROP CONSTRAINT ' + @NumLegsDefaultName);
  18:  END

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

   1:  CREATE TABLE [Animal](
   2:      [Id] smallint IDENTITY(1,1) NOT NULL,
   3:      [Name] nvarchar(20) NOT NULL,
   4:      [NumberOfLegs] smallint NOT NULL CONSTRAINT [DF_Animal_NumberOfLegs] DEFAULT 4,
   5:      [Genus] smallint NOT NULL,
   6:      CONSTRAINT [PK_Animal] PRIMARY KEY CLUSTERED([Id] ASC),
   7:      CONSTRAINT [UQ_Animal_Name] UNIQUE ([Name]),
   8:      CONSTRAINT [FK_Animal_Genus] FOREIGN KEY ([Genus]) REFERENCES [Genus] ([Id])
   9:  )

&nbsp;
