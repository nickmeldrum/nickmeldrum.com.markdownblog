## Why?

I find this a very useful customization to Visual Studio to run project specific tasks from PowerShell while developing. For example I have PowerShell .ps1 scripts in a Tasks or Deployment solution folders that do things like:

*   Refresh nuget packages because I use [nuget without committing packages](http://blog.davidebbo.com/2011/03/using-nuget-without-committing-packages.html "http://blog.davidebbo.com/2011/03/using-nuget-without-committing-packages.html") (Update: [Nuget.org have finally fixed this!](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages "http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages"))
*   Seeding my Database/ [RavenDb Document stores](http://ravendb.net/ "http://ravendb.net/")
*   Testing my Build/ Automated test scripts outside of CI

It's nice to be able to right-click them (or use a shortcut key) to execute them in place just like this:

![](/Media/Default/images/blog/run%20powershell%20context%20menu.png)

## So on to how to set it up:

## 

### Step 1: Adding "run powershell script" as an external tool

1.  In Visual Studio go to the menu: Tools | External Tools
2.  Click the "Add" button
3.  Add the following form values:
    *   Title: "Run Powershell script in output window"
    *   Command: "C:\windows\system32\windowspowershell\v1.0\powershell.exe"
    *   Arguments: " -file "$(ItemPath)"
    *   Initial Directory: "$(ItemDir)"
    *   Tick "Use Output window"
    *   (Close on exit will now be automatically on)

4.  Click the "Apply" button
5.  Click the&nbsp;"Add" button
6.  Add the following form values:
    *   Title: "Run powershell script outside of VS"
    *   Command: "C:\windows\system32\windowspowershell\v1.0\powershell.exe"
    *   Arguments: " -file "$(ItemPath)"
    *   Initial Directory: "$(ItemDir)"
    *   Don't tick "Use Output window"
    *   Tick "Close on exit"

7.  Click the "Ok" button

They should look something like this:

![](/Media/Default/images/blog/powershell%20script%20external%20tools%20dialog.png)

### Step 2: Weird Step, trust me!

Check the index position it is in the external tools list. By default mine are at positions 6 and 7. (I think by default Create GUID is no. 1!)

### Step 3: Hook it up to the context menus

1.  Go to the menu: Tools | Customize | Commands
2.  Click the "Context menu" radio option
3.  Scroll down to "Project and Solution Context Menus | Item" (nightmare long menu, type "Proj" to get roughly to the right place)
4.  Click the "Add Command" button
5.  Select the category: "Tools" and Command: "External Command 7" (or whatever your position is you got from the "Weird Step 2")
6.  Hit the "Ok" button
7.  Then to set up the 2nd command:
8.  Select Category: "Tools" and Command: "External Command 8" (or whatever your position is for the other one)
9.  Hit the "Ok" button again
10.  Move them around till you are happy with their order (I usually put them somewhere below "Open With...")

![](/Media/Default/images/blog/powershell%20customize%20context%20menu.png)

### Step 4: Add your keyboard shortcuts

1.  Go to the menu: Tools | Options
2.  Select the Environment | Keyboard section
3.  Find the Tools.ExternalCommand**N** item in the list (nightmare long list again, type "Tools" to get you roughly there again)
4.  Select your shortcut key for each command: I like &lt;CTRL&gt; &lt;SHIFT&gt; &lt;P&gt; and &lt;CTRL&gt; &lt;SHIFT&gt; &lt;ALT&gt; &lt;P&gt; respectively

![](/Media/Default/images/blog/shortcut%20mapping%20external%20tools.png)

## You are all done, enjoy your PowerShell efficiency!

## 

&nbsp;

What kind of PowerShell scripts do you find useful to run straight from Visual Studio?