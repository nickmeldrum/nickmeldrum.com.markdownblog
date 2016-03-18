## Huh? What's this all about then?

Recently I had the opportunity to study various different ways of implementing the ["wrapper" pattern also known as the "decorator" pattern](https://en.wikipedia.org/wiki/Decorator_pattern) in JavaScript.

I thought it was worth sharing what I learnt along the way about some valuable JavaScript techniques as well as the pros and cons of using these techniques to implement a decorator.

### "Okay okay, not *that* type of decorator..."

![Okay, not that type of decorator](/media/decorator.jpg)

The 4 different techniques are:

 1. Closures
 2. Monkey patching
 3. Proxy object (ES6)
 4. Closures again but in a sort of IOC way

At the end I will take a brief diversion into the current hot topic "middleware" and whether any of these could be called a "middleware" implementation or not. Fun times.

I have put some notes about this post in an appendix so as not to disrupt the flow, so if interested in a) why I am using ES6 syntax or b) why I am NOT using classes or c) a list of the source files for you to play with, check towards the end of this post.

## First the simple component I want to decorate:

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

It's a simple component with a `printValue(val)` method which will console log the value with a suffix at the end that can be defined with the `setSuffix(val)` method.

We want to decorate the `printValue(val)` method with 1 decorator to validate our input and another decorator to lower case the value (in order to show chaining of decorators.) We have the `setSuffix(val)` method in order to show the complications when trying to decorate just 1 method in a component that has other members.

It's worth noting that all my examples here except the last one will work for decorating a pure function instead of a member function and is a much simpler case.

## First example: decorating using closures

(Seasoned developers that have read 20 articles describing closures already, feel free to scroll down to the heading "Show me the code" to see the decorator implementation, then scroll to the pretty picture of the dolls.)

### What are closures?

>"A closure allows a function to access captured variables through the closure's reference to them, even when the function is invoked outside the scope of those variables." (me slightly rewording a definition from [wikipedia](https://en.wikipedia.org/wiki/Closure_(computer_programming)))

A simple example:

    function wow() {
        const val = 5
        return () => console.log(val)
    }

    wow()()

(Just in case it wasn't obvious, the double brackets in `wow()()` show that first we execute "wow" then we execute the anonymous function that "wow" returns.)

This is the simplest example I can imagine in JavaScript. The "wow" function returns a function that logs "val". However once "wow" has returned, "val" is no longer in scope.

But this works fine because when the function that is capturing a locally scoped variable (in this instance "val") is returned a closure is created which allows access to this variable even though it has gone out of scope.

### Show me the code!

How do you make use of closures to make a decorator? Well, I'll show you. First I'll show the code as a whole then I'll take you through it step by step:

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

The original component is the same as above, and we are instantiating it the same way as well:

    const component = myComponentFactory()

This returns a new object with 2 methods: `printValue(value)` and `setSuffix(suf)`. Then we decorate it by wrapping the object creation in 2 decorator factory methods:

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

![Wrapping stuff inside other stuff](/media/matryoshka.jpg)

So after the object creation and decoration is complete we run our test code:

    component.setSuffix('!')
    component.printValue('My Value')
    component.printValue('Invalid Value')

and this outputs:

    "value is my value!
    not valid man..."

The first function executed will be from the outer most decorator. In this instance, that's the validator. The first call is valid, so execution is passed into the second decorator "toLowerCase" which lower cases the value. It in turn calls the original function which logs the lower cased value with the suffix added on.

The second attempt fails validation which shows halting the chain of decorators so the value is never logged.

This is a very simple implementation to describe except possibly for one subtle component: the closure.

In this example we aren't storing a copy of the inner object in our decorated object. So how can we call it's methods when it is no longer in scope? It was just a parameter to the factory method that is long gone!

### Enter closures

Look again at the decorator:

    function toLowerDecorator(inner) {
        return {
            setSuffix: inner.setSuffix,
            printValue: value => inner.printvalue(value.tolowercase())
        }
    }

It returns an object with the function:

    value => inner.printvalue(value.tolowercase())

but this function is referencing the "inner" object which will go out of scope as soon as the decorator method is returned. However, because there is an inner function referencing this variable, the inner function "captures" this variable and once the function is returned it becomes a [closure](https://en.wikipedia.org/wiki/Closure_(computer_programming)).

This means the lifetime of the variable referencing our inner function is extended in order for the nested function to be able to be call it a later time.

This means we can take advantage of this "remembering" of the local arguments to the decorator factory methods to store the chain of wrapped methods that need to be called.

Closures are one of the most important and useful feature of JavaScript so it's worth making sure you grok them now if you don't already.

### Pros and cons of this method

In reality, closure's didn't actually have to be used in this example as we were returning a new object. We could have just stored a reference to the inner object in the new wrapper object. However, the reason we didn't was that would have made the inner object publicly available from the wrapper object. We actually utilised closure's to create a private member. We have no interest in confusing our public interface with some "inner" object.

The obvious downside to this method however is that we have to wrap every single method on the inner object, not just the one we are decorating. Like this:

    return {
        setSuffix: inner.setSuffix,
        ...
    
That is ugly and a pain. Wouldn't it be great if our decorators could just define the wrapper behaviour they desire and not have to worry about the rest?

Enter monkey patching.

(Note, if you want to use a simple technique and you are just monkey patching an isolated function, or a component with no other members, then this still may be the technique for you.)

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

The component factory is still the same, however the decorators are different and the way they are called is different. Instead of passing an object into a factory method that returns a new object, our decorator method is just operating on the existing object:

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

### What's with that second validator decorator and setTimeout anyway?!

![The waiter analogy](/media/waiters.jpg)

[(Waiters? Yup, the waiter analogy is a great way of explaining asynchronous code)](http://www.roidna.com/blog/what-is-node-js-benefits-overview/)

Why am I complicating things with an asynchronous call using setTimeout?

I included a wrapper function here that has some asynchronous code because we are in the land of JavaScript, a single threaded non-blocking language where asynchronous code is king. If your code can't handle "asynchronicity" then it's missing most of the point of the language design of JavaScript.

This "validator" function is emulating going off to a database to check the validity of the value by using setTimeout and calling the inner function in the callback. This way we can test if our implementations are still going to work when dealing with asynchronous code.

### What are the pros and cons of the monkey patching method?

People HATE monkey patching. Often with good reason.

![monkeys](/media/monkeys.jpg)

Awwwwww...

Why all the hate? Because when I call a library function I expect it work the same way if I call it the same way. I don't expect that functionality to change just because I included some other chumps completely irrelevant library.

Unfortunately if that chump decided to monkey patch some native function or some shared dependencies I'm in for a nasty surprise.

Now it *may* not be so bad if I'm just monkey patching my *own* code, but it's still a little funky and some people "JUST SAY NO" to the technique. Fair enough.

It does have a pro over the previous method though. Our decorator functions only had to operate on the method they wanted to wrap. The rest of the component was left untouched. This means our decorator functions only had to deal with 1 responsibility: wrapping a function with new behaviour.

So if you don't care about monkey patching, your base object has other public methods that need to be maintained and you want to keep the code simple then this technique maybe for you.

Otherwise, is there another method that doesn't employ monkey patching, but also only requires our decorator methods to define the new behaviour instead of having to pass-through every public member?

Well the answer is... possibly...

## Third example: Proxies

Why possibly? Because Proxies are the new kid on the block, so the support isn't as ubiquitous as we might like yet.

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

Look at that, beautiful. Our decorators now return a proxy object, proxying the wrapped object decorating just 1 function allowing any other call to simply be passed through. 

TODO Better detail on setting up this proxy object

The decorator object takes the inner object and returns a Proxy of this object. We only handle 1 thing in our Proxy object: the property accessors. We do this by specifying custom behaviour for the "get" handler.

Here we test to see if the property is the function we want to decorate. If it is then we return our new decorator function (which in this case lower-cases the value and then passes that into the inner printValue function.)

If it doesn't match the name we just return the inner member.

### Pros and cons of the Proxy technique

They key thing here is that although we have had to do some extra work to setup our proxy object, no matter how many members our wrapped object has, our decorators won't get any more complex.

So this technique has the 2 great attributes:

 1. it isn't monkey patching and
 2. we don't have to manually redefine every one of the inner objects members

However, as we said earlier, support isn't great yet. If you are in node, then as I showed you earlier you can polyfill it.

However if you are in the browser, there is still no support in any version of Internet Explorer. Chrome only got support in version 49. Unfortunately it is my understanding that it is very difficult, if not impossible to polyfill this feature well in the browser.

Possibly this one works: [but it warns of serious performance issues](https://www.npmjs.com/package/babel-plugin-proxy).

So unless you are in a recent version of node or you can guarantee your customer's are using latest Chrome, Edge or Firefox only then this technique is probably not for you.

Still if you are fully embracing ES6 (and why not?), are running on the latest browsers, want to decorate just 1 function in an object with a large public surface then this may well be the implementation for you.

TODO Note weirdness in calling this one wrapping the function calls the other way around

## 4. The final unnamed one : TODO get a name!

So the last 3 examples of implementing the decorator pattern for a function that belongs to an object all had their downsides:

 1. requires facading everything you aren't decorating
 2. requires monkey patching
 3. requires latest browser support of Proxies or some possibly dubious polyfilling

However 1 excellent characteristic of all of them was this: *The original object didn't even need to know it was being decorated* By wrapping it or monkey patching it we were able to extend it's behaviour WITHOUT modifying it - Wow that's the [OCP](http://c2.com/cgi/wiki?OpenClosedPrinciple) of [SOLID](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design)) yay :)

Is there a perfect implementation however? 1 that doesn't require monkey patching, doesn't require facading everything we aren't decorating, doesn't require unsupported ES6 features AND is still SOLID?

### Show me the code!

By now you know the drill: I'll show you the whole code then break it down for you afterwards:

    function myComponentFactory() {
        let suffix = ''
        const instance = {
            setSuffix: suff => suffix = suff,
            printValue: value => console.log(`value is ${value + suffix}`),
            addDecorators: decorators => {
                let printValue = instance.printValue
                decorators.forEach(decorator => printValue = decorator(printValue))
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

Notice the main difference? Our original object KNOWS that is to be decorated and provides a specific method for you to add your decorators. A major benefit of this method is just how simple the decorator functions are - they literally return the wrapper function in order to store a closure of the next function to be called. The `addDecorators()` function then loops through the decorators array assigning each next function to the decorator.

So by setting up our object to allow decoration and do the grunt work of setting up the function chain we accomplish many goals:

 1. Our decorator functions are beautifully simple
 2. It's arguably simpler to setup our decorator list, we just pass an ordered array of decorators into a method instead of worrying about the construction mechanics of our particular decorator pattern implementation
 3. It's STILL OCP - our base implementation allows us to decorate without further touching the original object
 4. and it ain't monkey patching or relying on Proxies

This is actually probably the most complex of implementations in the end, but it satisfies a lot of criteria. If you are setting up some heavy weight decoration then this is probably the implementation for you.

This now looks a lot like [connect middleware for node.js](https://github.com/senchalabs/connect). Change the `addDecorators()` function to a `use()` function which allows you to pass in a decorator 1 at a time and then run the setup after they have all been added and it looks EXACTLY like the connect API.

TODO: learn more connect to expand on this comparison

<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="643px" height="248px" version="1.1" content="%3CmxGraphModel%20dx%3D%22880%22%20dy%3D%22468%22%20grid%3D%221%22%20gridSize%3D%2210%22%20guides%3D%221%22%20tooltips%3D%221%22%20connect%3D%221%22%20arrows%3D%221%22%20fold%3D%221%22%20page%3D%221%22%20pageScale%3D%221%22%20pageWidth%3D%22826%22%20pageHeight%3D%221169%22%20background%3D%22%23ffffff%22%20math%3D%220%22%3E%3Croot%3E%3CmxCell%20id%3D%220%22%2F%3E%3CmxCell%20id%3D%221%22%20parent%3D%220%22%2F%3E%3CmxCell%20id%3D%22114a013724b344ee-1%22%20value%3D%22Call%20doSomething()%22%20style%3D%22ellipse%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22120%22%20y%3D%2260%22%20width%3D%22120%22%20height%3D%2280%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-2%22%20value%3D%22Log%20the%20before%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22460%22%20y%3D%2275%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-3%22%20value%3D%22Upper%20case%20the%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22280%22%20y%3D%2275%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-4%22%20value%3D%22Do%20the%20original%20function%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22620%22%20y%3D%22160%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-5%22%20value%3D%22Log%20the%20after%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22460%22%20y%3D%22240%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-6%22%20value%3D%22Upper%20case%20the%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22280%22%20y%3D%22240%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-7%22%20value%3D%22End%22%20style%3D%22ellipse%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22120%22%20y%3D%22225%22%20width%3D%22120%22%20height%3D%2280%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-8%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0%3BentryY%3D0.5%3BexitX%3D1%3BexitY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-1%22%20target%3D%22114a013724b344ee-3%22%3E%3CmxGeometry%20width%3D%2250%22%20height%3D%2250%22%20relative%3D%221%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%2210%22%20y%3D%2260%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%2260%22%20y%3D%2210%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-9%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0%3BentryY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20target%3D%22114a013724b344ee-2%22%3E%3CmxGeometry%20x%3D%22250%22%20y%3D%22110%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22420%22%20y%3D%22100%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22290%22%20y%3D%22110%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-10%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0%3BentryY%3D0.25%3BexitX%3D0.5%3BexitY%3D1%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-2%22%20target%3D%22114a013724b344ee-4%22%3E%3CmxGeometry%20x%3D%22260%22%20y%3D%22120%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22430%22%20y%3D%22110%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22470%22%20y%3D%22110%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-11%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0.5%3BentryY%3D0%3BexitX%3D-0.007%3BexitY%3D0.9%3BexitPerimeter%3D0%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-4%22%20target%3D%22114a013724b344ee-5%22%3E%3CmxGeometry%20x%3D%22270%22%20y%3D%22130%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22540%22%20y%3D%22135%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22605%22%20y%3D%22170%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-12%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D1%3BentryY%3D0.5%3BexitX%3D0%3BexitY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-5%22%20target%3D%22114a013724b344ee-6%22%3E%3CmxGeometry%20x%3D%22280%22%20y%3D%22140%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22550%22%20y%3D%22145%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22615%22%20y%3D%22180%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-13%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D1%3BentryY%3D0.5%3BexitX%3D0%3BexitY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-6%22%20target%3D%22114a013724b344ee-7%22%3E%3CmxGeometry%20x%3D%22290%22%20y%3D%22150%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22560%22%20y%3D%22155%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22625%22%20y%3D%22190%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3C%2Froot%3E%3C%2FmxGraphModel%3E" style="background-color: rgb(255, 255, 255);"><defs/><g transform="translate(0.5,0.5)"><ellipse cx="61" cy="41" rx="60" ry="40" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(7.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="106" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 107px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Call doSomething()</div></div></foreignObject><text x="53" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Call doSomething()</text></switch></g><rect x="341" y="16" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(354.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="112" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 113px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Log the before value</div></div></foreignObject><text x="56" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Log the before value</text></switch></g><rect x="161" y="16" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(172.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="116" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 117px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Upper case the value</div></div></foreignObject><text x="58" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Upper case the value</text></switch></g><rect x="501" y="101" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(507.5,119.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="126" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 127px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Do the original function</div></div></foreignObject><text x="63" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Do the original function</text></switch></g><rect x="341" y="181" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(360.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="101" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 102px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Log the after value</div></div></foreignObject><text x="51" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Log the after value</text></switch></g><rect x="161" y="181" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(172.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="116" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 117px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Upper case the value</div></div></foreignObject><text x="58" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Upper case the value</text></switch></g><ellipse cx="61" cy="206" rx="60" ry="40" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(49.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="22" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 23px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">End</div></div></foreignObject><text x="11" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">End</text></switch></g><path d="M 121 41 L 154.63 41" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 159.88 41 L 152.88 44.5 L 154.63 41 L 152.88 37.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 301 41 L 334.63 41" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 339.88 41 L 332.88 44.5 L 334.63 41 L 332.88 37.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 411 66 L 495.38 111" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 500.01 113.47 L 492.19 113.27 L 495.38 111 L 495.48 107.09 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 500 146 L 416.93 178.67" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 412.04 180.59 L 417.27 174.77 L 416.93 178.67 L 419.84 181.29 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 341 206 L 307.37 206" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 302.12 206 L 309.12 202.5 L 307.37 206 L 309.12 209.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 161 206 L 127.37 206" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 122.12 206 L 129.12 202.5 L 127.37 206 L 129.12 209.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/></g></svg>

### pros and cons

## 5. BONUS: Why closures and functional composition? Wouldn't a procedural approach be simpler?

### The procedural approach

### Why doesn't this work?

If you only want synchronous code in your decorators then sure by all means do this approach and you can get your for-each loops back. This is a "Bad Idea"(tm) though as it breaks as soon as you do anything asynchronous, which let's face it is idiomatic in JavaScript.

## conclusion

imo No. 4. is best, can't see a con but in the end the differences are mostly stylistic

## Redux middleware

### Dan Abramov and Redux are AWESOME

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


## addendum:

TODO:

 * talk about using es6
 * talk about not using classes
 * link to the scripts source

### ES6 and classes

You will notice I am using ES6 syntax. Yup, works in node.js and easy to transpile using babel for browser code and is a much nicer version of JavaScript to code in. The [babel site](https://babeljs.io/docs/learn-es2015/) itself is where I started learning some ES6.

Except I'm not using Classes from ES6? Well there are 2 excellent reasons for that:

 1. Classes in ES6 have alot of problems with them It's a whole other post, so read up about it here: https://medium.com/javascript-scene/how-to-fix-the-es6-class-keyword-2d42bb3f4caf#.osnwj4xq5 (for me one of the real annoyances is the lack of private members and one of the conceptual annoyances is just "what's the point" in this keyword that gives you less than a simple factory function)

 2. Perhaps more importantly for this post: We are talking about decorators, and decorators are *very hard* to do well over the top of the ES6 class syntax. So much so there is a proposal for "decorators" in [ES7](https://medium.com/google-developers/exploring-es7-decorators-76ecb65fb841#.3y5ruqbcs) which should solve it. However, as I'm hoping you will see in this post - if you keep to functional syntax, powerful decorators are possible with very simple code in JavaScript.

#### Inheritance

 A side note on no. 2. We are talking about decorators here - NOT subclassing. I lost count of the number of articles I read introducing the decorator pattern by using inheritance. Um wat? 2 of the main points of the decorator pattern are to solve specific issues with inheritance:

 1. You stop type explosion - functionality can be composed from simple pieces instead of having to build up complex inheritance heirachies to support all combinations and
 2. you can choose what behaviours to add at runtime instead of at design time

 So guys, please please please stop trying to say you are implementing the decorator pattern when you are just subclassing.


