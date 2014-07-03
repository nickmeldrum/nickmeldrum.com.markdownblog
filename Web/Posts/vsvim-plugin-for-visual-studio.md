### TL/DR

Vim  is awesome. I have installed Vim plug-ins on my 3 most important developer tools: Visual Studio, Sublime Text 2 and Chrome. This gives me a fantastically productive and consistent experience while coding. If you use these tools I recommend you try the plugins too.

### Okay, first up: An admission about Visual Studio

I have a love/ hate relationship with Visual Studio. It's the most complete, powerful and extensible IDE there is. It's debugging abilities are second to none and, with [JetBrains Resharper](http://www.jetbrains.com/resharper/ "JetBrains Resharper") installed, it turns into a refactoring powerhouse. Unfortunately (especially with Resharper installed) it is a system hog and can become snail-like, making me want to mash my keyboard with all my shattered productivity dreams.

![Visual Studio](/Media/visual-studio-2010-logo.png)![Resharper](/Media/resharperlogo.jpg)

Because of its upsides, it is still an important tool in my tool belt, but the downsides mean I now use it in tandem with other tools that aren't as slow. My current favourite text editor is [Sublime Text 2](http://www.sublimetext.com/2 "Sublime Text 2"). When I'm doing front-end JavaScript development where all my debugging is happening in Chrome and Resharper's refactoring tools aren't quite as powerful, I'll usually switch over to Sublime. I'm running my unit tests in Jasmine using Karma anyway.

### Get to the point: Isn't using Resharper enough?

When I really started grokking Resharper and the power of all that refactoring goodness I really started appreciating how using the right tools, and learning them well, could improve my coding. I started to get frustrated that a lot of my time while coding was moving down to the arrow keys to navigate around a file, then moving up to the delete or backspace key, then back to the keyboard to start typing. I know it can sound like that's not a big deal, but when you move into the Vim world and don't have to move your hand at all to accomplish this you realise how disruptive it really can be.

I code at different speeds throughout the day, but when I'm really in the flow I feel my hands getting in the way and I wanted that fixed. (I have another reason to always want to stay on the home keys but I'm saving that for a future post.) I wanted to reduce the friction that occurs in our imperfect mechanisms for expressing our ideas in code, ie. brain -&gt; hands -&gt; keyboard -&gt; IDE -&gt; program. I wanted to get ever closer to that slightly cyberpunk ideal of being able to just think a design and have it materialise as a functioning system.

### Or in simpler terms: Less Friction = More Fun

![Friction](/Media/friction.jpg)

### Enter VsVim

I started hearing developers rediscovering and raving about Vim as their new favourite editor. I had vague recollections of people talking about it over 15 years ago when I was coding on Sun Solaris at university using Emacs. I didn't try it then and if there's any advice I could give new junior devs it's to get into Vim as early as possible, because it's pretty hard to break the old non-Vim habits if you come back to it as late as I am.

So I thought I would dive into this Vim world by trying out a plugin to visual studio: [VsVim](http://visualstudiogallery.msdn.microsoft.com/59ca71b3-a4a3-46ca-8fe1-0e90e3f79329 "VsVim").

![VsVim Screenshot](/Media/vsvimscreenshot.png)

It brings the power and precise editing of Vim into Visual Studio. I've found the combination of Resharper and VsVim to be a great way of coding. Jared Parsons has written this as a free extension to Visual Studio and it really is extremely high quality. I really respect him for the [work he's done](https://github.com/jaredpar/VsVim "VsVim on GitHub"). If you want to hear from him about what he did, Scott Hanselman did a great [Hanselminutes podcast](http://hanselminutes.com/364/vsvim-visual-studio-and-vim-with-jared-parsons "VsVim Hanselminutes podcast") with him.

### What is so special about Vim then and why should I bother?

I'm just going to go ahead and quote here, because other people have explained this better than me.

From [https://help.ubuntu.com/community/VimHowto](https://help.ubuntu.com/community/VimHowto "HowTo Vim"):

>"vim has a formidable learning curve, but by getting comfortable with vim and its great features, you will became very efficient at manipulating text. A pre-condition to that, is the programmer (novice) should learn Touch Typing to experience the power.
> 
> One of the most confusing things about vim is that it has four modes.
> 
> *   Insert: To type text
> *   Command: To issue commands. Also called as Normal mode.
> *   Ex: To issue colon commands
> *   Visual To select text visually
> 
> The Insert mode is not default, you must press i to move into insert mode. Type some text in the screen. Press the &lt;Esc&gt; button to get out of insert mode into Command mode. The command mode is used to move about, and to manipulate text, sometimes in interesting ways. The Visual mode is used to select text, press v to enter it and select some text, then you can issue commands that will apply only to the selected area, type&lt;Esc&gt; again to return to Command mode."

From [http://stackoverflow.com/questions/597077/is-learning-vim-worth-the-effort](http://stackoverflow.com/questions/597077/is-learning-vim-worth-the-effort "Is Vim worth the effort"):

> "It's definitely worth learning either vim or emacs. It's also worth learning to touch-type. In both cases the reasons are the same: your thinking is no longer interrupted by the mechanical process of getting your code onto the screen."
> "I prefer vim because tasks are broken up - you're either editing or inserting, not doing both."

### How did I go about learning it?

If you can't touch type then Vim is probably going to be tough going. Go and learn that first. You type all day every day - get efficient at it.

I still feel like a beginner with Vim. I can pass on what those first baby steps were like and what it's like to get to toddler level only. You have to accept that you won't be productive at the beginning. Far from it. I had to pick the days that I would do my Vim practice, because it seriously slowed me down at the beginning.

I started with the basic movement commands and some simple delete/ change/ insert combinations. I was patient with myself, and let myself off when it got too frustrating. I would move the mouse away so I couldn't grab it, which was a nice reminder that I should be learning. If it got too much I would bring the mouse back and turn the plug-in off because it was getting in the way of my day job. The next day I would get back on the horse and try again. Slowly after a few weeks, the muscle memory started to kick in and I began enjoying the experience a whole lot more.

Here are a few tips that really helped me in the beginning:

![Vim cheat sheet](/Media/vimcheatsheet-thumb.gif)

1.  Start with "vimtutor". Download Vim from here: [http://vim.sourceforge.net/download.php](http://vim.sourceforge.net/download.php "Download Vim") and it will give you a shortcut to vimtutor - an excellent interactive tutorial to get you started.
2.  Print out a cheat sheet and tape it under your monitor ([http://www.viemu.com/vi-vim-cheat-sheet.gif](http://www.viemu.com/vi-vim-cheat-sheet.gif "Vim cheat sheet")) - then refer to it!
3.  Put the mouse away - force yourself to find the command when you can.
4.  You will be hitting ESCAPE a lot to get back to command mode. Use AutoHotKey to map the useless CAPSLOCK key to ESCAPE and thank me later. [http://vim.wikia.com/wiki/Map_caps_lock_to_escape_in_Windows](http://vim.wikia.com/wiki/Map_caps_lock_to_escape_in_Windows "Mapping Capslock in Windows")
5.  Accept that you are going to be slow at first and let Google be your friend.
6.  Don't beat yourself up when you have a day when you can't face it. Go back to your old ways for a bit.

### Okay I'm ready: How do I do it?

Vim is too big for me to explain here and I'd probably do a worse job than all the other great tutorials out there, so just do this:

1.  Read about the philosophy: [http://www.viemu.com/a-why-vi-vim.html](http://www.viemu.com/a-why-vi-vim.html "Why Vim")
2.  Do the interactive tutorial: "vimtutor" See above for details.
3.  Get practicing
4.  When you are getting out of beginner mode and starting to enjoy it, crank it up a notch and look at this crazy condensed page full of advanced command combinations: [http://rayninfo.co.uk/vimtips.html](http://rayninfo.co.uk/vimtips.html "Advanced Vim tips") :)

&nbsp;

![it](/Media/its-working.gif)

### Conclusion

I'll just leave my favourite simple tip here. To replace the contents of a function argument list:

1.  Put the cursor inside the brackets.
2.  Then hit V, I then ( and Vim will select everything in the brackets.
3.  Hit C to delete and put you into insert mode. Substitute " for ( when in quotes. Really handy.

So now I have the same Vim command set in Visual Studio, [Chrome (with Vimium: http://nickmeldrum.com/blog/mouseless-browsing-with-vimium)](http://nickmeldrum.com/blog/mouseless-browsing-with-vimium "Mouseless browsing with Vimium") and Sublime Text 2 (using the Vintage plugin.) This gives me a really nice consistent experience across all 3 of the tools.

If nothing else it's an interesting experiment into sharpening my saw and learning new stuff. Of course as well as the productivity gains, it's also a great way of warding off that horrible professional injury - repetitive strain injury.

So go try it today and keep being awesome!