It's not uncommon to see a MS SqlServer table creation script like this:

       CREATE TABLE [Animal] (
        [Id] smallint IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
        [Name] nvarchar(20) NOT NULL UNIQUE,
        [NumberOfLegs] smallint NOT NULL DEFAULT 4,
        [Genus] smallint NOT NULL FOREIGN KEY REFERENCES [Genus] ([Id])
       )
   
And in general it looks good. Obviously some thought has gone into it's representation, each column is restrained 
by constraints that make sense to the domain - we have a nice simple primary key, we have a unique name, a default 
number of legs and it's species is a foreign key to another table. We shouldn't have too many problems with the integrity
of our data here.

But what about when our requirements change (as they always do) and we need to change the schema. I'm stretching the
Animal analogy here, but the domain isn't important. It turns out we don't want [NumberOfLegs] against this table anymore.
Maybe we want to denormalise the properties of the Animal even more (eg. have an Animal class including insects and store 
the NumberOfLegs against that.)

So we have to delete the column [NumberOfLegs]. Easy you say, lets just issue:

    ALTER TABLE [Animal]DROP COLUMN [NumberOfLegs];
    
Whoops, error:
>The object 'DF__Animal__NumberOf__6CB8F890' is dependent on column 'NumberOfLegs'

And dammit I can't easily Drop that default constraint because it's not predictably named. I know the name on this server,
but I will be running this script on every server running this application and all the default constraint names will be different!
So instead I have to end up writing the following tortuous script just to remove 1 randomnly named default constraint:

    DECLARE @NumLegsDefaultName nvarchar(255)

    SELECT
        @NumLegsDefaultName = cObj.name
    FROM
        sysobjects    [cObj]
        join sysobjects [tObj] ON (cObj.parent_obj = tObj.id)
        join sysconstraints [con] ON (cObj.id    = con.constid)
        join syscolumns [col] ON (tObj.id = col.id and con.colid = col.colid)
    WHERE
        cObj.xtype    = 'D' -- This means default
        and tObj.name = 'Animal'
        and col.name = 'NumberOfLegs'

    IF (ISNULL(@NumLegsDefaultName, '') <> '')
    BEGIN
        EXEC('ALTER TABLE [Animal] DROP CONSTRAINT ' + @NumLegsDefaultName);
    END
  
Now I'm allowed to drop that column. Ouch.
I've always enforced the requirement to name primary keys, unique keys, foreign keys and indexes because I know they will
need to be changed in the future so I want to be able to get a handle for any alter statements.

I just forgot about default constraints!

So now I would recommend that table above be written instead like the following with everything named:

(Note: For readability, I would prefer to be able to define my default constraint below like I can separate all my
other indexes, keys and constraints. Unfortunately MS SqlServer syntax won't allow it. I think this is as readable
as I can get - and notice my strict naming conventions but that's for another even more pedantic post!)

    CREATE TABLE [Animal](
        [Id] smallint IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(20) NOT NULL,
        [NumberOfLegs] smallint NOT NULL CONSTRAINT [DF_Animal_NumberOfLegs] DEFAULT 4,
        [Genus] smallint NOT NULL,
        CONSTRAINT [PK_Animal] PRIMARY KEY CLUSTERED([Id] ASC),
        CONSTRAINT [UQ_Animal_Name] UNIQUE ([Name]),
        CONSTRAINT [FK_Animal_Genus] FOREIGN KEY ([Genus]) REFERENCES [Genus] ([Id])
    )

