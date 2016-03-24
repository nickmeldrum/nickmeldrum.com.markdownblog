## Huh? What's this all about then?

Recently I had the opportunity to study various different ways of implementing the ["wrapper" pattern also known as the "decorator" pattern](https://en.wikipedia.org/wiki/Decorator_pattern) in JavaScript.

I thought it was worth sharing what I learnt along the way about some valuable JavaScript techniques as well as the pros and cons of using these techniques to implement a decorator.

### "Okay okay, not *that* type of decorator..."

![Okay, not that type of decorator](/media/decorator.jpg "Okay, not that type of decorator")

The five different decorator implementations are:

 1. A simple wrapper (wherein we investigate closures)
 2. Monkey patching
 3. Prototypal inheritance
 4. Proxy object (ES6)
 5. For want of a better word: "middleware"

At the end I will take a brief diversion into the current hot topic: "middleware" and whether any of these could really be called a "middleware" implementation or not. Fun times.

I have put some notes about this post in an appendix so as not to disrupt the flow. So if interested in a) why I am using ES6 syntax or b) why I am NOT using classes or c) a list of the source files for you to play with, check towards the end of this post.

## First, the simple component I want to decorate:

    'use strict'

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => suffix = suf,
            printValue: value => console.log(`value is ${value + suffix}`)
        }
    }

    const component = myComponentFactory()
    component.setSuffix('!')
    component.printValue('My Value')

It's a simple component with a `printValue(val)` method which will console log the value with a suffix at the end. The suffix can be set using the `setSuffix(val)` method.

We want to decorate the `printValue(val)` method with a decorator to validate our input and another decorator to lower case the value (in order to show chaining of decorators.) We created the `setSuffix(val)` method in order to show the complications when trying to decorate just 1 method in a component that has other members.

It's worth noting that all my examples here except the last one will work for decorating an isolated function instead of a member function and is a much simpler case.

### How we are going to decorate this component

Below is a diagram of how we are going to decorate our component. We will wrap our original "printValue" method first in a "lower case" decorator, then in a "validate" decorator. That way when a consumer of our decorated component calls "printValue" *first* we will validate their value, then lower case it, then print it out.

(Note, this diagram shows that we could also add behaviour to our wrapper *after* the original call is made "on the way out", however in these examples we are not doing this.

<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="643px" height="248px" version="1.1"><defs/><g transform="translate(0.5,0.5)"><ellipse cx="61" cy="41" rx="60" ry="40" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(16.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="89" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 90px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Call printValue()</div></div></foreignObject><text x="45" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Call printValue()</text></switch></g><rect x="341" y="16" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(353.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="114" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 115px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">lower case decorator</div></div></foreignObject><text x="57" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">lower case decorator</text></switch></g><rect x="161" y="16" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(181.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="98" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 99px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">validate decorator</div></div></foreignObject><text x="49" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">validate decorator</text></switch></g><rect x="501" y="101" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(516.5,119.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="108" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 109px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">original printValue()</div></div></foreignObject><text x="54" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">original printValue()</text></switch></g><rect x="341" y="181" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(353.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="114" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 115px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">lower case decorator</div></div></foreignObject><text x="57" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">lower case decorator</text></switch></g><rect x="161" y="181" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(181.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="98" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 99px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">validate decorator</div></div></foreignObject><text x="49" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">validate decorator</text></switch></g><ellipse cx="61" cy="206" rx="60" ry="40" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(49.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="22" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 23px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">End</div></div></foreignObject><text x="11" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">End</text></switch></g><path d="M 121 41 L 154.63 41" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 159.88 41 L 152.88 44.5 L 154.63 41 L 152.88 37.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 301 41 L 334.63 41" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 339.88 41 L 332.88 44.5 L 334.63 41 L 332.88 37.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 411 66 L 495.38 111" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 500.01 113.47 L 492.19 113.27 L 495.38 111 L 495.48 107.09 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 500 146 L 416.93 178.67" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 412.04 180.59 L 417.27 174.77 L 416.93 178.67 L 419.84 181.29 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 341 206 L 307.37 206" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 302.12 206 L 309.12 202.5 L 307.37 206 L 309.12 209.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 161 206 L 127.37 206" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 122.12 206 L 129.12 202.5 L 127.37 206 L 129.12 209.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/></g></svg>

## First example: decorating using a simple wrapper (wherein we investigate closures)

What is the most naïve implementation of a decorator I can think of to kick us off? Just wrap an object and return a new object that can do some new behaviour then call the wrapped object.

![Simples!](/media/simples.jpg "Simples!")

> "Simples"

### Show me the code!

First I'll show the code as a whole then I'll take you through it step by step:

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => suffix = suf,
            printValue: value => console.log(`value is ${value + suffix}`)
        }
    }

    function toLowerDecorator(inner) {
        return {
            setSuffix: inner.setSuffix,
            printValue: value => inner.printValue(value.toLowerCase())
        }
    }

    function validatorDecorator(inner) {
        return {
            setSuffix: inner.setSuffix,
            printValue: value => {
                const isValid = ~value.indexOf('My')

                setTimeout(() => {
                    if (isValid) inner.printValue(value)
                    else console.log('not valid man...')
                }, 500)
            }
        }
    }

    const component = validatorDecorator(toLowerDecorator(myComponentFactory()))
    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

So what does that do?

The component factory is still the same as shown above. However, we decorate it by wrapping the object creation in decorator factory methods:

    const component = validatorDecorator(toLowerDecorator(myComponentFactory()))

The decorator factory method takes the original object and returns a wrapper object that just passes the calls through to the original object *except* for the 1 method we want to decorate:

    function toLowerDecorator(inner) {
        return {
            setSuffix: inner.setSuffix,
            printValue: value => inner.printValue(value.toLowerCase())
        }
    }

The decorated function does it's stuff (lower casing the value) and passes the value to the inner function "wrapping" it, or "decorating" it.

We can then keep wrapping our object creation in decorator factory methods on and on waiting for the invocation which will call them like opening a Matryoshka doll.

![Wrapping stuff inside other stuff](/media/matryoshka.jpg "Wrapping stuff inside other stuff")

So after the object creation and decoration is complete we run our test code:

    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

and this outputs:

    "value is my value!
    not valid man..."

The first function executed will be from the outer most decorator. In this instance, that's the validator. The first call is valid, so execution is passed into the second decorator "toLowerCase" which lower cases the value. It in turn calls the original function which logs the lower cased value with the suffix added on.

The second attempt fails validation which shows halting the chain of decorators so the value is never logged.

### What's with that second validator decorator and setTimeout anyway?!

![The waiter analogy](/media/waiters.jpg "The waiter analogy")

[(The waiter analogy is a great way of explaining asynchronous code)](http://www.roidna.com/blog/what-is-node-js-benefits-overview/)

Why am I complicating things with an asynchronous call using setTimeout?

I included a wrapper function here that has some asynchronous code because we are in the land of JavaScript, a single threaded non-blocking language where asynchronous code is king. If your code can't handle "asynchronicity" then it's missing most of the point of the language design of JavaScript.

This "validator" function is emulating going off to a database to check the validity of the value by using setTimeout and calling the inner function in the callback. This way we can test if our implementations are still going to work when dealing with asynchronous code.

### How do we make use of closures?

We could have just stored the inner object on the new object in order to call it later. Why didn't we do that? Because it would have made it public and that would have been weird. I would have confused the user of my object. Do I call `instance.setSuffix()` or `instance._original.setSuffix()`. Far better to make the object a private member.

>"But JavaScript doesn't have private members, oh noes!"

We can use closures to give us this private member access though.

### The official bit: What are closures?

>"A closure allows a function to access captured variables through the closure's reference to them, even when the function is invoked outside the scope of those variables." (me, slightly rewording a definition from [wikipedia](https://en.wikipedia.org/wiki/Closure_(computer_programming)))

A simple example:

    function wow() {
        const val = 5
        return () => console.log(val)
    }

    wow()()

(Just in case it wasn't obvious what the double brackets in `wow()()` do: first it executes the "wow" function, which returns an anonymous function which is immediately executed because of the second brackets.)

This is the simplest example I can imagine in JavaScript. The "wow" function returns a function that logs "val". However once "wow" has returned, "val" is no longer in scope.

This works fine though, because when the function that is capturing a locally scoped variable (in this instance "val") is returned, a closure is created which allows access to this variable even though it has gone out of scope.

### Back to our wrapper:

Look again at the decorator:

    function toLowerDecorator(inner) {
        return {
            setSuffix: inner.setSuffix,
            printValue: value => inner.printValue(value.tolowercase())
        }
    }

It returns an object with the function:

    value => inner.printvalue(value.tolowercase())

but this function is referencing the "inner" object which will go out of scope as soon as the decorator method is returned. However, because there is an inner function referencing this variable, the inner function "captures" this variable and once the function is returned a [closure](https://en.wikipedia.org/wiki/Closure_(computer_programming)) is created.

This means the lifetime of the variable referencing our inner function is extended in order for the nested function to be able to be call it a later time.

Our own function can use the "inner" object because of the closure but it isn't exposed publicly. BOOM. Our private variable.

Closures are one of the most important and useful feature of JavaScript so it's worth making sure you grok them now if you don't already.

![Don't expose your privates](/media/exposes_privates.jpg "Don't expose your privates")

### Pros and cons of this method

We introduce closures here but that has little to do with this wrapper method, in fact every technique we show here uses closures to hide variables that would be better off private.

Other than that this is a very simple implementation which has an obvious downside: we have to wrap every single method on the inner object, not just the one we are decorating. Like this:

    return {
        setSuffix: inner.setSuffix,
        ...
    
That is ugly and a pain. Wouldn't it be great if our decorators could just define the wrapper behaviour they desire and not have to worry about the rest? Happily there are a few techniques that will do just that.

Let's look at monkey patching.

## Second example: decorating using monkey patching

### What is monkey patching?

>"The dynamic modification of a class or module."
 - [(wikipedia)](https://en.wikipedia.org/wiki/Monkey_patch)

Simply put in this context:

>"I'm gonna take advantage of the dynamic nature of JavaScript combined with the mutability of objects and just replace your function with mine!"
 - (me, just then)

So what is decorating using monkey patching?

>"I'm gonna replace your function with mine, then I'll call you from within me - wrapping you in me."
 - (me again)

### Show me HOW!

How do you do that you ask? Well, I'll show you. First I'll show the code as a whole then I'll take you through it step by step:

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => suffix = suf,
            printValue: value => console.log(`value is ${value + suffix}`)
        }
    }

    function decorateWithToLower(inner) {
        const originalPrintValue = inner.printValue
        inner.printValue = value => originalPrintValue(value.toLowerCase())
    }

    function decorateWithValidator(inner) {
        const originalPrintValue = inner.printValue

        inner.printValue = value => {
            const isValid = ~value.indexOf('My')

            setTimeout(() => {
                if (isValid) originalPrintValue(value)
                else console.log('not valid man...')
            }, 500)
        }
    }

    const component = myComponentFactory()
    decorateWithToLower(component)
    decorateWithValidator(component)

    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

So what does that do?

The component is still the same; the decorators are different, and the way they are called is different. Instead of passing an object into a factory method that returns a new object, our decorator method is just operating on the existing object:

    decorateWithToLower(component)

This decorator method does it's monkey patching by storing the original "printValue" method in a local variable:

    const originalPrintValue = inner.printValue
    
and then overwrites the original function with it's own copy, which lower-cases the value, then passes that value onto the stored "inner" function:

    inner.printValue = value => originalPrintValue(value.toLowerCase())

We set up our decorators the same way as before. We wrap the printValue() function in a lower-caser decorator. Then we wrap that in a validator decorator:

    const component = myComponentFactory()
    decorateWithToLower(component)
    decorateWithValidator(component)

Note the use of closure's here to do the storing of the chain of inner functions still. The real difference is that we are just swapping out 1 function in the existing object instead of returning a brand new object wrapper.

### What are the pros and cons of the monkey patching method?

People HATE monkey patching. Often with good reason.

![Monkeys. Cute.](/media/monkeys.jpg "Monkeys. Cute.")

Awwwwww...

Why all the hate? Because when I call a library function I don't expect that functionality to change just because I included some other chumps completely irrelevant library.

Unfortunately if that chump decided to monkey patch some native function or some shared dependencies I'm in for a nasty surprise.

Now it *may* not be so bad if I'm just monkey patching my *own* code, but it's still a little funky and some people "JUST SAY NO" to the technique. Fair enough.

It does have a pro over the previous method though. Our decorator functions only had to operate on the method they wanted to wrap. The rest of the component was left untouched. This means our decorator functions only had to deal with 1 responsibility: wrapping a function with new behaviour.

So if you don't care about monkey patching, your base object has other public methods that need to be maintained and you want to keep the code simple then this technique maybe for you.

Okay what about prototypal inheritance then?

## Third example: Prototypal inheritance

### What is prototype inheritance?

Most developers are used to classical inheritance from Java or C# where a class is based on another class. Quite simply prototype inheritance is all about objects instead of classes: "Objects inherit properties from other objects". Simple as that.

It's mechanism is quite simple in JavaScript too. All objects have a prototype. In fact you can have a chain of prototypes leading all the way back to "Object" itself, which is always at the base of all prototype chains.

### Delegation

![Delegation](/media/delegation.jpg "Delegation")

You can tell 1 object to "base" itself off another object by setting it's prototype. What this really means is: if someone asks to access one of my members, first we will look on my object, but if I don't have it I will delegate the access to my prototype. On this will go, all the way up the prototype chain.

I won't delve into why here, but I don't like the use of the "new" keyword in JavaScript. Go to the end of the article if you care. Therefore, in my opinion the best way to make use of [prototype inheritance is using `Object.create(protoype)`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/create):

    const myBaseObject = { myProperty: 'oh hai' }

    const myNewObject = Object.create(myBaseObject)
    myNewObject.newMethod = () => { console.log(myBaseObject.myProperty) }

Here, not only have we set up myNewObject to inherit from myBaseObject as it's prototype, but we show an example of reaching back to access a member on the base object as well. There is no protected or private scope to concern ourselves with. No virtual members either. If you want to hide the base object and only have the new object exposed, just wrap all that in a function and return what you want. Functions always seem to be the answer to any question you have in JavaScript.

### Show me the code:

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => suffix = suf,
            printValue: value => console.log(`value is ${value + suffix}`)
        }
    }

    function toLowerDecorator(inner) {
        const instance = Object.create(inner)
        instance.printValue = value => inner.printValue(value.toLowerCase())
        return instance
    }

    function validatorDecorator(inner) {
        const instance = Object.create(inner)
        instance.printValue = value => {
            const isValid = ~value.indexOf('My')

            setTimeout(() => {
                if (isValid) inner.printValue(value)
                else console.log('not valid man...')
            }, 500)
        }
        return instance
    }

    const component = validatorDecorator(toLowerDecorator(myComponentFactory()))
    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

This example is extremely similar to the first example where we construct a new object that reaches into the inner object. The first example had the down side of having to copy every member to the new wrapper object in order to pass it through though. Here we take advantage of something called delegation in prototypal inheritance. Our new object is created using the inner object as it's prototype. This way I don't have to define "setSuffix" on my new object as when it is asked for my prototype will be checked for the existence of this member as well.

This is an obvious and efficient way of creating a decorator in JavaScript using inheritance. It is interesting because one of the original design goals of the Decorator pattern was to get around some of the limitations of classical inheritance. Namely the problem of using classical inheritance to build up extra behaviour leads to an inflexible hierarchy. I cannot chain together behaviours in different ways. I have to predefine which class will inherit from which other class. Happily, prototype inheritance doesn't have this limitation. As you can see from above, I can always choose any Object to use as my prototype.

This makes it a great choice for use as a decorator implementation. Onto more learnin' though...

## Fourth example: Proxies

Proxies are the new kid on the block, specified in ES6, and look like a promising way to do some interesting AOP style programming techniques. So let's see if they can help create a decorator.

### What are proxies?

>"The Proxy object is used to define custom behavior for fundamental operations (e.g. property lookup, assignment, enumeration, function invocation, etc)." (from [MDN](https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Proxy))

Oooo neat, we can inject custom behaviour into property lookup and function invocation? Sounds powerful? Well it is.

### Show me the code!

Once again I will show the whole code and then dissect and explain the important bits:

    require('harmony-reflect')

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suff => suffix = suff,
            printValue: value => console.log(`value is ${value + suffix}`)
        }
    }

    function toLowerDecorator(inner) {
        return new Proxy(inner, {
            get: (target, name) => {
                return (name === 'printValue')
                    ? value => target.printValue(value.toLowerCase())
                    : target[name]
            }
        })
    }

    function validatorDecorator(inner) {
        return new Proxy(inner, {
            get: (target, name) => {
                return (name === 'printValue')
                    ? value => {
                        const isValid = ~value.indexOf('my')

                        setTimeout(() => {
                            if (isValid) target.printValue(value)
                            else console.log('not valid man...')
                        }, 500)
                    }
                    : target[name]
            }
        })
    }

    const component = toLowerDecorator(validatorDecorator(myComponentFactory()))
    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

Firstly, what's this?!

    require('harmony-reflect')

Well, I have been testing this code using node.js. And node's support for proxies isn't great yet.

Firstly if you want to make use of proxies you have to run node with a switch:

    node.exe --harmony-proxies

Even then, the Proxy object in node ain't ES6 compliant at the time of writing this blog. However if you:

    npm install harmony-reflect

and require it as above, then you get a lovely up to date ES6 compliant Proxy object to use. You still have to use the switch above though. (I guess the npm module still uses the non-compliant proxy object underneath it all.)

Next you will notice the component factory is identical but the decorator methods look very different:

    function toLowerDecorator(inner) {
        return new Proxy(inner, {
            get: (target, name) => {
                return (name === 'printValue')
                    ? value => target.printValue(value.toLowerCase())
                    : target[name]
            }
        })
    }

Proxies give an amazing amount of power and they are worth reading up about in [the Proxy section of mdn](https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Proxy).

![It's really hard finding a good image about proxies](/media/spiderman_proxies.jpg "It's really hard finding a good image about proxies")

Here, the decorator object takes the inner object and returns a Proxy of this object. We only handle 1 thing in our Proxy object: the property accessors. We do this by specifying custom behaviour for the "get" handler.

Here we test to see if the property is the function we want to decorate. If it is then we return our new decorator function (which in this case lower-cases the value and then passes that into the inner printValue function.)

If it doesn't match the name we just return the inner member. The observant among you will notice that once again we are making use of closures here.

### Pros and cons of the Proxy technique

They key thing here is that although we have had to do some extra work to setup our proxy object, no matter how many members our wrapped object has, our decorators won't get any more complex.

So this technique has the 2 great attributes:

 1. it isn't monkey patching and
 2. we don't have to manually redefine every one of the inner objects members

It is probably overkill however, I cannot see what this approach gives us over prototype inheritance. Proxies were created for AOP style stuff rather than decorators.

Also, as we said earlier, support isn't great yet. If you are in node, then as I showed you earlier you *can* polyfill it.

However if you are in the browser, there is still no support in any version of Internet Explorer. Chrome only got support in version 49. Unfortunately it is my understanding that it is very difficult, if not impossible to polyfill this feature well in the browser.

Possibly this polyfill works: [but it warns of serious performance issues](https://www.npmjs.com/package/babel-plugin-proxy).

## The Fifth technique: "Middleware"?

An excellent characteristic of the previous examples was that **the original object didn't even need to know it was being decorated.** By wrapping it, monkey patching it, inheriting from it or proxying it we were able to extend it's behaviour WITHOUT modifying it - Wow that's the [OCP](http://c2.com/cgi/wiki?OpenClosedPrinciple) of [SOLID](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design)), yay :)

What if our base object knows from the outset of it's own creation that one particular method will be decorated, and what if we want to enable more interaction between the base functionality and the decorators. What if we want to make our decorator code even more simple by pushing some of the decorator logic down to the base object?

### Show me the code!

By now you know the drill: I'll show you the whole code then break it down for you afterwards:

    function myComponentFactory() {
        let suffix = ''
        const instance = {
            setSuffix: suff => suffix = suff,
            printValue: value => console.log(`value is ${value + suffix}`),
            addDecorators: decorators => {
                let printValue = instance.printValue
                decorators.slice().reverse().forEach(decorator => printValue = decorator(printValue))
                instance.printValue = printValue
            }
        }
        return instance
    }

    function toLowerDecorator(inner) {
        return value => inner(value.toLowerCase())
    }

    function validatorDecorator(inner) {
        return value => {
            const isValid = ~value.indexOf('My')

            setTimeout(() => {
                if (isValid) inner(value)
                else console.log('not valid man...')
            }, 500)
        }
    }

    const component = myComponentFactory()
    component.addDecorators([toLowerDecorator, validatorDecorator])
    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

Notice the main difference? Our original object **knows** it will be decorated and provides a specific method for you to add your decorators. Here our component sets up the decorator chain itself, you just have to supply it the list of decorators with the method:

    component.addDecorators([toLowerDecorator, validatorDecorator])
    
The "addDecorators" method then loops through each decorator passing in the inner function into it. It then take the last decorator method and assigns it to the public member. This way it has set up the decorator chain itself. Notice it is choosing to reverse the order of decoration to make the order in the array passed in more readable:

    addDecorators: decorators => {
        let printValue = instance.printValue
        decorators.slice().reverse().forEach(decorator => printValue = decorator(printValue))
        instance.printValue = printValue
    }

The decorator method itself can then be extremely simple. All it has to do is return the wrapper function that wraps the function passed in as a parameter:

    function toLowerDecorator(inner) {
        return value => inner(value.toLowerCase())
    }

### Pros and cons of the "middleware" approach

We get more control over our decorators by setting up the chaining ourselves. We are making use of this here by changing the order in which we wrap the function using `reverse()` on the decorators array.

Taking more control of setting up the decorator chain also leads to vastly simpler decorator methods.

So by setting up our object to allow decoration and do the grunt work of setting up the function chain we accomplish a few goals:

 1. Our decorator functions are simple
 2. It gives us more control over the decorator chaining
 3. It's arguably simpler to setup our decorator list, we just pass an ordered array of decorators into a method instead of worrying about the construction mechanics of our particular decorator pattern implementation
 4. It's *still* OCP - our base implementation allows us to decorate without further touching the original object
 5. and it ain't monkey patching or relying on Proxies

This is the most complex of implementations. If you are setting up some heavy weight decoration that needs more management than simple wrapping then this is probably the implementation for you.

I have called this the "middleware" implementation because:

 a) I can't think of anything better to call it and
 b) It is the method Dan Abramov uses in his [redux middleware](http://redux.js.org/docs/advanced/Middleware.html) implementation

## Conclusion

We looked at five different techniques for implementing the decorator pattern. We learned something along the way.

 1. We did a naive approach which led to us having to manually copy every member from our inner object to our wrapper object. But we learnt about using closures to hide variables as if they were private members.
 2. We solved the "copy every member" problem by monkey patching, but we introduced monkey patching which many "consider harmful".
 3. We solved the "copy every member" problem using prototype inheritance. Everything seemed warmer in the world.
 4. We solved the problem *again* using the ES6 Proxy object. However, we learnt the Proxy object doesn't have great support yet and although is immensely powerful, it's power is probably out of place here. It doesn't really give us anything that prototype inheritance gave us in this instance.
 5. We looked at a "middleware" type implementation where the base object sets up the decorator chain itself. This led to a powerful and flexible implementation with simple decorator methods.

### What can we conclude from all this?

**Every single implementation** made use of closures. That should give you some idea of just how important they are in JavaScript. So if you still don't get them and you skipped that bit, go back and read it. Or go read someone else's better description of what they are.

Which was the clear winner? When I set out to write this I expected each technique to have it's upsides and downsides, its own justification for when it should be used. Actually in the end I think there are only 2 implementations worth using:

 1. The prototype inheritance one and
 2. The "middleware" one

I guess use the middleware one when you need more control of the decorator chain itself. Otherwise the prototype inheritance one seems to me the clear winner. It has all the advantages:

 1. No modification of the base object required
 2. No "copy every member to the new object"
 3. No monkey patching
 4. No poorly supported code
 5. Relatively simple decorator methods

## Postscript: What is middleware

Well it turns out after a little bit of research that defining what middleware is would be a whole thesis in itself. So I'm not going to do this section the justice I wanted to, but just give a "layman's overview".


### Dan Abramov and Redux are AWESOME

[connect middleware for node.js](https://github.com/senchalabs/connect)

[some university course's definition of middleware](http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld002.htm)
http://www.eecg.toronto.edu/~jacobsen/courses/imw/notes/imw1/sld005.htm
examples of middleware: ODBC, CORBA, DCOM...

not knocking redux or Dan, he is clearly awesome and I am in love with redux.

this middleware page confused me for a while though and it wasn't until I started working on a decorator pattern that I realised why.

Middleware comes from Ruby Rack and then used by Node connect - both are the pipeline pattern NOT middleware. However Redux middleware is clearly a decorator and here's why:

IT WRAPS the inner function - so you can do stuff before calling AND afterwards.

The pipeline pattern is subtly different - but think of it more as a 1 way pipe. (TODO: really? can't i do stuff after calling next in connect? - needs more research?)

middleware - define it as a decorator - connect middleware has filters and providers - 
This shows the 2 types of middleware: filters and providers. A classic example of filters is logging, whereas the providers are meant to do a specific action, a classic example is the static middleware which will serve a static file on a route that matches a filename. This will, like the validator, stop subsequent functions executing.

from http://redux.js.org/docs/advanced/Middleware.html:

>"...middleware is some code you can put between the framework receiving a request, and the framework generating a response. For example, Express or Koa middleware may add CORS headers, logging, compression, and more. The best feature of middleware is that it’s composable in a chain. You can use multiple independent third-party middleware in a single project."

from wikipedia entry on decorator pattern:

>"...allows behavior to be added to an individual object, either statically or dynamically, without affecting the behavior of other objects from the same class.[1] The decorator pattern is often useful for adhering to the Single Responsibility Principle, as it allows functionality to be divided between classes with unique areas of concern."

and 

>"...This pattern is designed so that multiple decorators can be stacked on top of each other, each time adding a new functionality to the overridden method(s)."

That sounds an awful lot like "decorator" to me - so we have a new cool word for decorator, that's okay with me I guess...

## Addendum

### ES6

I am using ES6 syntax in this article for a number of reasons:

 1. I have fallen in love with many things about ES6 but especially arrow functions and the const keyword
 2. It (mostly) works natively now in recent versions of node.js and is a simple babel transpilation step away from working in browsers.
 3. It makes it much more easy to write examples in articles like this with less noise getting in the way. (I realise the down side of this is that this is *not* true for readers that haven't learnt the new ES6 syntax yet.)
 4. I have spent a while reading and writing React code recently and it is **all** in ES6 nowadays, so it's time for all of us to jump on board!

The [babel site](https://babeljs.io/docs/learn-es2015/) itself is where I started learning some ES6 if you are looking for a starting place to get your head around it.

### Classes

I am using ES6 syntax but *not* using the class keyword? There are 2 excellent reasons for that:

 1. Classes in ES6 have a lot of problems with them. It's a whole other post, so read up about it here: [https://medium.com/javascript-scene/how-to-fix-the-es6-class-keyword-2d42bb3f4caf#.osnwj4xq5](https://medium.com/javascript-scene/how-to-fix-the-es6-class-keyword-2d42bb3f4caf#.osnwj4xq5) (For me, one of the real annoyances is the lack of private members and one of the conceptual annoyances is just "what's the point" in the keyword that gives you less than a simple factory function does)

 2. Perhaps more importantly for this post: We are talking about decorators, and decorators are *hard* to do well over the top of the ES6 class syntax. So much so there is a proposal for "decorators" in [ES7](https://medium.com/google-developers/exploring-es7-decorators-76ecb65fb841#.3y5ruqbcs) which should solve it. However, as I'm hoping you will see in this post - if you keep to functional syntax, powerful decorators are possible with very simple code in JavaScript.

 3. Both the "new" keyword and the new "class" keyword in ES6 just confuse things. It seems they have been put in JavaScript to make it feel more comfortable for people coming from classical languages like Java, but the result is clunky and just serves to hide the true power *and* simplicity of prototypes. Here is another excellent [article talking about all that](http://aaditmshah.github.io/why-prototypal-inheritance-matters/).

### The scripts

 1. [The "wrapper" method](/scripts/decorator-wrapper.js)
 2. [The "monkey patching" method](/scripts/decorator-monkeypatching.js)
 3. [The "prototype inheritance" method](/scripts/decorator-inheritance.js)
 4. [The "proxy" method](/scripts/decorator-proxy.js)
 5. [The "middleware" method](/scripts/decorator-middleware.js)

