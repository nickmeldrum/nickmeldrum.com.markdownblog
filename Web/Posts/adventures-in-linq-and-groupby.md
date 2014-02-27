Now, I'm going to have to admit something: I'm still a Linq beginner. But every day I use it I love it more and more.

## Why is Linq so great?

Today I discovered one of the reasons Linq is great - because it makes it easy to run set based semantics on the object graph.

A great reason to use an RDBMS in your application is if the domain lends itself to set-based logic. However I found myself all to often letting go of that set-based thinking when working in the model because the language didn't lend itself to it. It wasn't impossible before Linq, especially with great libraries like C5, but it was still a pain. Now I can think in sets and write my C# in sets without friction.

## On to my discovery:

(Note I have vastly simplified the domain just to get my Linqy goodness point across)

I have an application that is very set based. One of the things it does is to create a report based on a number of calculations it does on some volumes of substances.

Every time I run a calculation on a substance I may want to add a "Note" to the report. This note will have a link to the substance.

I came to actually showing the Report in the UI. At the bottom I show all the notes. "Ack that looks terrible": the same Note was added 5 times, but with different substances - so looping through them meant that the Note description was printed 5 times along with each substance.

> Note 1: (Long description) on Substance: x  
> Note 1: (Long description) on Substance: y  
> Note 1: (Long description) on Substance: z  
> Note 1: (Long description) on Substance: z  
> Note 1: (Long description) on Substance: z  

My first thought was "Dammit my model is wrong. A Note should have a set of children Substances rather than just 1." Now in terms of the "correctness of the model" I'm not actually sure. However this would certainly make the UI code easier, but the model more complex and the calculation code even more complex as instead of creating a new note each time it would have to check if the note was created and add a substance to it.

Whether the model is slightly wrong or not, I decided that to change the model based purely on a UI display issue was wrong. And here was where Linq came in. I used the GroupBy method to group my notes by note and zing: I get a collection of unique Notes with a list of substances within each one.


    NotesRepeater.DataSource = from n in Report.ReportNotes
        group n by n.Note into g
        select new ReportNotesDTO {
            Note = g.Key.Name,
            NoteDetails = g
        };

I can now use a repeater within a repeater to show each Note individually with a list of related Substances below it. (Note I'm using the ItemDataBound event to databind the child collection from the groupby.)


    <asp:Repeater ID="NotesRepeater" runat="server" OnItemDataBound="NotesRepeater_ItemDataBound">
    <ItemTemplate>
    <p>
        <%# DataBinder.Eval(Container.DataItem, "Note")%>: used on substances:                          
        <asp:Repeater ID="NoteDetailsRepeater" runat="server">
        <ItemTemplate>
            <%# DataBinder.Eval(Container.DataItem, "SubstanceInvolved")%><br />
         </ItemTemplate>
         </asp:Repeater>
      </p>
    </ItemTemplate>
    </asp:Repeater>

That was so easy. Thanks Linq!
