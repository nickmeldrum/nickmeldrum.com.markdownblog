** TL/DR: DO NOT REPEAT YOURSELF. I REPEAT: DO NOT REPEAT YOURSELF. Gaffaw. **

## The Caveat:

Mostly the danger in using SASS (or any other CSS pre-processor) is a feeling that just so long as we do things the "sass" way we will create great CSS without having to understand all those horrible complicated CSS-y type things like cascading rules, box models and even how to structure our style.

My argument is use SASS for the very simple DRY functionality of variables, nesting and operators to assist the cleanliness of your CSS a little. However be very wary of Mixins, Extend and Imports as they can seriously LOWER cohesion and INCREASE coupling if used incorrectly/ too much. That doesn't mean DON'T use SASS. That means, use it carefully.

Oh and before you do anything - go away and understand OOCSS concepts for good CSS and HTML structure first: [https://github.com/stubbornella/oocss/wiki/faq](https://github.com/stubbornella/oocss/wiki/faq "Stubornella's great OOCSS FAQ")


## Variables:

### NOT DRY:


	p {
		color: #DC842F;
	}

	h1 {
		background-color: #DC842F;
	}


### DRY!:


	$brand-primary-color: #DC842F;

	p {
		color: $brand-primary-color;
	}

	h1 {
		background-color: $brand-primary-color;
	}


"Well, search and replace works well for this" you say?
Okay, what about when you want a margin width common to a number of elements/areas? - try search and replacing for the number 10!

**Argument no. 1: variables are a useful addition to css and reduce duplication.**

## Nesting:

Organizing your CSS so that it's selectors are particular to a section of your HTML Dom structure is a *good* thing. Doing this with CSS ends up with duplication of the HTML sections in the selectors.

### NOT DRY:


	nav ul {
	  margin: 0;
	  padding: 0;
	  list-style: none;
	}

	nav li {
	  display: inline-block;
	}


### DRY!:


	nav {
	  ul {
	    margin: 0;
	    padding: 0;
	    list-style: none;
	  }

	  li {
	  	display: inline-block;
	  }
	}


**Argument no. 2: Nesting is a useful addition to css and reduces duplication.**


## Operators:

So we use variables for some numbers now? But in order to get our CSS working correctly we will often need to create the negative of this number. How do we do this?

### NOT DRY:

	$primary-left-margin: 10px;
	$special-panel-width: 300px;
	$child-panel-width: 290px;

	.special-parent-panel: {
		margin-left: $primary-left-margin;
		width: $special-panel-width;
	}

	.panel-within-a-special-parent-panel: {
		margin: 0px;
		width: $child-panel-width;
	}

The child panel width is only being specified as it's the parent width minus the margin. but we've had to hardcode it. This is a refactoring pain point.

### DRY!:

	$primary-left-margin: 10px;
	$special-panel-width: 300px;

	.special-parent-panel: {
		margin-left: $primary-left-margin;
		width: $special-panel-width;
	}

	.panel-within-a-special-parent-panel: {
		margin: 0px;
		width: $special-panel-width - $primary-left-margin;
	}

We didn't have to hard code anything about the child panel, we can go ahead and change the panel width in one place without changing anything else.

**Argument no. 3: Operators are a useful addition to css and reduce duplication.**


## Other features of SASS:

I don't argue for these being particularly an argument for using SASS, but just adding them here for completeness as "nice things to have".

### Modules:

The import tag allows us to split our css into separate files and combine them into 1 file. This is of course possible with a simple combination from minification post-processing libraries so it's just a "nice" syntax.

### Extends:

Allows you to share properties from one selector to another. Use this with caution. Always check whether you can accomplish what you want with the fundamental cascading rules of CSS. There are times however when this is useful.

### Mixins:

Note: Mixins definitely have a down-side: They can create horrific large amounts of CSS and a large amount of confusion and less readability when used badly. USE WITH CAUTION!

However, they do serve an important purpose and should be used. Just use them lightly. Only use Mixins where you feel you absolutely need some modular abstraction that requires parameterisation. Otherwise you should use Extends. (I am actually still struggling for a justifiable use of mixins. Will add one when I can justify it!) 


## Cites

This is basically a more argumentative version of the authoritative guide over at [http://sass-lang.com/guide](http://sass-lang.com/guide "The SASS Guide")
