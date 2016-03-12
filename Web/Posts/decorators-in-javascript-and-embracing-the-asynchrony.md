## A classic decorator pattern example

### Introduction

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

### Let's get started

#### What is a decorator

#### What is middleware

Well that would be spoiling the suprise a little, but what does ruby rack/ connect/ redux say it is?

todo: research middleware in ruby racks (see if they relate it to decorator pattern - find origin of term)

## Is Middleware just another name for the decorator pattern?

TL/DR Yup seems to be!

middleware - define it as a decorator - connect middleware has filters and providers - 

from http://redux.js.org/docs/advanced/Middleware.html:

>"...middleware is some code you can put between the framework receiving a request, and the framework generating a response. For example, Express or Koa middleware may add CORS headers, logging, compression, and more. The best feature of middleware is that it’s composable in a chain. You can use multiple independent third-party middleware in a single project."

from wikipedia entry on decorator pattern:

>"...allows behavior to be added to an individual object, either statically or dynamically, without affecting the behavior of other objects from the same class.[1] The decorator pattern is often useful for adhering to the Single Responsibility Principle, as it allows functionality to be divided between classes with unique areas of concern."

and 

>"...This pattern is designed so that multiple decorators can be stacked on top of each other, each time adding a new functionality to the overridden method(s)."

That sounds an awful lot like "decorator" to me - so we have a new cool word for decorator, that's okay with me I guess...


so what is middleware?
middleware is a decorator pattern on an object that knows it is to be decorated - this means it can manage the decorators which gives us some more power:

## Onto the code!

Let's look at varioius ways of implementing the decorator pattern in JavaScript:

### First attempt: Decorating a function

This shows the basic method I would recommend for setting up a decorator in JavaScript. Here we are decorating a simple function rather than a function of an Object. We do it by passing in the next function into the decorator function. The decorator function then returns the wrapper function with the new behaviour. So when the wrapper function is called it can call the next function in the chain that was created as a closure by the decorator function.

    'use strict'

    function printValue(value) {
        console.log(`value is ${value}`)
    }

    function decorateWithSpacer(func) {
        return value => {
            value = value.split('').join(' ')
            func(value)
        }
    }

    function decorateWithUpperCaser(func) {
        return value => {
            value = value.toUpperCase()
            func(value)
        }
    }

    function decorateWithCheckValueIsValid(func) {
        return value => {
            const isValid = ~value.indexOf('my')

            setTimeout(() => {
                if (isValid)
                    func(value)
                else
                    console.log('not valid man...')
            }, 1000)
        }
    }

    let func = printValue
    func = decorateWithSpacer(func)
    func = decorateWithUpperCaser(func)
    func = decorateWithCheckValueIsValid(func)

    func('my value')
    func('invalid value')

Here we show that we can even have an asynchronous method in the decorator chain (the validator function here emulating a database call via `setTimeout()`) and it all still works.

This will output the following text to the console because the argument "my value" validates fine, is upper cased, then spaced out, then eventually console.logged by the original function. The second valid immediately fails the validate function and we can see the chain is broken by the fact this branch does not make a call to the next function in the decorator chain.

    "value is M Y   V A L U E
    not valid man..."

This shows the 2 types of middleware: filters and providers. A classic example of filters is logging, whereas the providers are meant to do a specific action, a classic example is the static middleware which will serve a static file on a route that matches a filename. This will, like the validator, stop subsequent functions executing.

### Next example: Decorating a function that is part of an Object by Monkey Patching

What is monkey patching? todo: explain here

    'use strict'

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => {
                suffix = suf
            },
            printValue: value => {
                console.log(`value is ${value + suffix}`)
            }
        }
    }

    function decorateWithSpacer(component) {
        const originalPrintValue = component.printValue

        component.printValue = value => {
            originalPrintValue(value.split('').join(' '))
        }
    }

    function decorateWithUpperCaser(component) {
        const originalPrintValue = component.printValue

        component.printValue = value => {
            originalPrintValue(value.toUpperCase())
        }
    }

    function decorateWithCheckValueIsValid(component) {
        const originalPrintValue = component.printValue

        component.printValue = value => {
            const isValid = ~value.indexOf('my')

            setTimeout(() => {
                if (isValid)
                    originalPrintValue(value)
                else
                    console.log('not valid man...')
            }, 1000)
        }
    }

    const component = myComponentFactory()
    decorateWithSpacer(component)
    decorateWithUpperCaser(component)
    decorateWithCheckValueIsValid(component)

    component.setSuffix('END')
    component.printValue('my value')
    component.printValue('invalid value')

The output of this will be:

    "value is M Y   V A L U EEND
    not valid man..."

This method has the advantage that even though the decorator functions only wrap 1 of the objects functions, the component still has all of it's base behaviour available. However some people don't like monkey patching, indeed sometimes monkey patching is evil. if you are in control of the base function you are monkey patching it's probably not so evil, but anyway we can look at another way of wrapping a component that doesn't use monkey patching next. If your wrapped object has alot of properties and methods and you only want to decorate one of them without touching the rest though this may well be the implementation for you.

    'use strict'

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => {
                suffix = suf
            },
            printValue: value => {
                console.log(`value is ${value + suffix}`)
            }
        }
    }

    function spacerDecorator(component) {
        return {
            setSuffix: component.setSuffix,
            printValue: value => {
                component.printValue(value.split('').join(' '))
            }
        }
    }

    function upperCaserDecorator(component) {
        return {
            setSuffix: component.setSuffix,
            printValue: value => {
                component.printValue(value.toUpperCase())
            }
        }
    }

    const component = upperCaserDecorator(spacerDecorator(myComponentFactory()))
    component.setSuffix('END')
    component.printValue('my value')
    component.printValue('invalid value')

How is this one different? How do we get away with not monkey patching? Well instead we are passing back a whole new object that is working as a facade against the wrapped object. This has the advantage that we could decorate a number of different functions if we liked very easily. However it also has the downside that we still have to manually expose every inner objects functions and properties in the new wrapped object which could be a real pain. However if you are decorating more functions than you are leaving this still may be the implementation for you.

Can we get the best of both worlds? Can we only require our decorator functions to define the new behaviour without any cruft AND without monkey patching? Well this might be tough in ES5, but as we are in the new world of ES6, sure we can! We can make use of the new Proxy API:

    'use strict'

    function myComponentFactory() {
        let suffix = ''

        return {
            setSuffix: suf => {
                suffix = suf
            },
            printValue: value => {
                console.log(`value is ${value + suffix}`)
            }
        }
    }

    function spacerDecorator(component) {
        return new Proxy(component, {
            get: function(target, name) {
                return (name === 'printValue')
                    ? function(value) { target.printValue(value.split('').join(' ')) }
                    : target[name]
            }
        })
    }

    function upperCaserDecorator(component) {
        return new Proxy(component, {
            get: function(target, name) {
                return (name === 'printValue')
                    ? function(value) { target.printValue(value.toUpperCase()) }
                    : target[name]
            }
        })
    }

    function validatorDecorator(component) {
        return new Proxy(component, {
            get: function(target, name) {
                return (name === 'printValue')
                    ? function(value) {
                        const isValid = ~value.indexOf('my')

                        setTimeout(() => {
                            if (isValid)
                                target.printValue(value)
                            else
                                console.log('not valid man...')
                        }, 1000)
                    }
                        : target[name]
            }
        })
    }

    const component = validatorDecorator(upperCaserDecorator(spacerDecorator(myComponentFactory())))
    component.setSuffix('END')
    component.printValue('my value')
    component.printValue('invalid value')

Look at that, beautiful. Our decorators now return a proxy object, proxying the wrapped object decorating just 1 function allowing any other call to simply be passed through. 

If you want to run this in node you will need to do 2 things:

 1. Firstly, `npm install harmony-reflect` then in your node code: `require('harmony-reflect')`
 2. Secondly make sure you run node with the following setting: `node --harmony-proxies .\index.js`

If you want to run this in a browser, then be careful as [browser support](http://kangax.github.io/compat-table/es6/#Proxy) is very recent so older browsers won't support it. It obviously can't be transpiled and the polyfills are rare or non-existent, possibly this one works: [but it warns of serious performance issues](https://www.npmjs.com/package/babel-plugin-proxy)

Still if you are fully embracing ES6 (and why not?), are running on the latest browsers, want to decorate just 1 function in an object with a large public surface then this may well be the implementation for you.

### The last implementation

So the last 3 examples of implementing the decorator pattern for a function that belongs to an object all had their downsides:

 1. requires monkey patching
 2. requires facading everything you aren't decorating
 3. requires latest browser support of Proxies or some possibly dubious polyfilling

However 1 excellent characteristic of all of them was this: *The original object didn't even need to know it was being decorated* By wrapping it or monkey patching it we were able to extend it's behaviour WITHOUT modifying it - Wow that's the [OCP](http://c2.com/cgi/wiki?OpenClosedPrinciple) of [SOLID](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design)) yay :)

Is there a perfect implementation however? 1 that doesn't require monkey patching, doesn't require facading everything we aren't decorating, doesn't require unsupported ES6 features AND is still SOLID?

    'use strict'

    function myComponentFactory() {
        let suffix = ''
        const instance = {
            setSuffix: suf => {
                suffix = suf
            },
            printValue: value => {
                console.log(`value is ${value + suffix}`)
            },
            addDecorators: decorators => {
                let printValue = instance.printValue
                decorators.reverse().forEach(decorator => {
                    printValue = decorator(printValue)
                })
                instance.printValue = printValue
            }
        }
        return instance
    }

    function spacerPrintValueDecorator(next) {
        return value => {
            next(value.split('').join(' '))
        }
    }

    function upperCaserPrintValueDecorator(next) {
        return value => {
            next(value.toUpperCase())
        }
    }

    function validatorPrintValueDecorator(next) {
        return value => {
            const isValid = ~value.indexOf('my')

            setTimeout(() => {
                if (isValid)
                    next(value)
                else
                    console.log('not valid man...')
            }, 1000)
        }
    }

    const component = myComponentFactory()
    component.addDecorators([validatorPrintValueDecorator, spacerPrintValueDecorator, upperCaserPrintValueDecorator])
    component.setSuffix('END')
    component.printValue('my value')
    component.printValue('invalid value')

Notice the main difference? Our original object KNOWS that is to be decorated and provides a specific method for you to add your decorators. A major benefit of this method is just how simple the decorator functions are - they literally return the wrapper function in order to store a closure of the next function to be called. The `addDecorators()` function then loops through the decorators array assigning each next function to the decorator.

So by setting up our object to allow decoration and do the grunt work of setting up the function chain we accomplish many goals:

 1. Our decorator functions are beautifully simple
 2. It's arguably simpler to setup our decorator list, we just pass an ordered array of decorators into a method instead of worrying about the construction mechanics of our particular decorator pattern implementation
 3. It's STILL OCP - our base implementation allows us to decorate without further touching the original object
 4. and it ain't monkey patching or relying on Proxies

This is actually probably the most complex of implementations in the end, but it satisfies a lot of criteria. If you are setting up some heavy weight decoration then this is probably the implementation for you.

This now looks a lot like [connect middleware for node.js](https://github.com/senchalabs/connect). Change the `addDecorators()` function to a `use()` function which allows you to pass in a decorator 1 at a time and then run the setup after they have all been added and it looks EXACTLY like the connect API.

Bang - middleware is a decorator pattern ;)

todo: learn more connect to expand on this comparison


First the original module:

    function aModule() {
        return {
            doSomething: (value) => {
                console.log(`um wow, value is ${value.val}`)
            }
        }
    }

    const instance = aModule()
    instance.doSomething({val:'my value'})

But we want to add multiple behaviours to the `doSomething()` method so we decorate it. easy in JavaScript with "monkey patching":

    function decorateWithLogger(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (value) => {
            console.log(`logging before func, argument is: ${value}`)
            originalFunc(value)
            console.log(`logging after func, argument is: ${value}`)
        }
    }

    decorateWithLogger(instance)
    instance.doSomething('my value')

We can then add another behaviour such as uppercasing my value:

    function decorateWithUpperCaser(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (value) => {
            originalFunc(value.val.toUpperCase())
        }
    }

    decorateWithUpperCaser(instance)
    instance.doSomething('my value')

Note, now the order in which we decorate becomes important - if you played along and decorated first with logger, then wrapped that decoration with the upper caser decoration then the upper caser happens first and we log the upper case value instead of the original value and the output is:

    "logging before func, argument is: MY VALUE
    um wow, value is MY VALUE
    logging after func, argument is: MY VALUE"

However switch the order we decorate:

    const instance2 = aModule()
    decorateWithUpperCaser(instance2)
    decorateWithLogger(instance2)
    instance2.doSomething('my value')

And the output is now:

    "logging before func, argument is: my value
    um wow, value is MY VALUE
    logging after func, argument is: my value"

Yes even the AFTER logging call has the original value. We are passing a string, so it's called by value - the value isn't actually being passed around to be mutated - however if it was an `Object` it would be.

I won't go on, but you can see we could even have a decorator here do a test and decide to stop processing of the decorator chain (and original function) because of not meeting some criteria - by just not calling `originalFunc()` if you so desire.

There are (like with everything in JavaScript) a million different ways of writing this kind of decorator pattern but this seems the most succinct and simple way to me so I won't bother labouring on the other ways.

### HOWEVER:

We ain't embracing the asynchrony yet! JavaScript is a single threaded language using non blocking I/O and an event loop. This brings enormous advantages (in not having to manage threads) but you GOTTA embrace the asynchrony man.

What if we need to make an asynchronous call in one of our decorator functions? No problem, you say you can just make a call to the `originalFunc()` in the callback/thenable. Let's try it then. Imagine we have a call that wants to check if our value is valid. It needs to ask another server about this. We will emulate this asynchronous call with a setTimeout. Here we assume all values are valid if they include the text `'my'`:

    function decorateWithCheckValueIsValid(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (value) => {
            const isValid = ~value.indexOf('my')

            setTimeout(() => {
                if (isValid)
                    originalFunc(value)
                else
                    console.log('not valid man...')
            }, 1000)
        }
    }

    const instance3 = aModule()
    decorateWithCheckValueIsValid(instance3)
    decorateWithLogger(instance3)
    instance3.doSomething('my value')

The output is:

    "logging before func, argument is: my value
    logging after func, argument is: my value
    um wow, value is my value"

Hmm something wrong here? My logging is "out of time"...

<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="643px" height="248px" version="1.1" content="%3CmxGraphModel%20dx%3D%22880%22%20dy%3D%22468%22%20grid%3D%221%22%20gridSize%3D%2210%22%20guides%3D%221%22%20tooltips%3D%221%22%20connect%3D%221%22%20arrows%3D%221%22%20fold%3D%221%22%20page%3D%221%22%20pageScale%3D%221%22%20pageWidth%3D%22826%22%20pageHeight%3D%221169%22%20background%3D%22%23ffffff%22%20math%3D%220%22%3E%3Croot%3E%3CmxCell%20id%3D%220%22%2F%3E%3CmxCell%20id%3D%221%22%20parent%3D%220%22%2F%3E%3CmxCell%20id%3D%22114a013724b344ee-1%22%20value%3D%22Call%20doSomething()%22%20style%3D%22ellipse%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22120%22%20y%3D%2260%22%20width%3D%22120%22%20height%3D%2280%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-2%22%20value%3D%22Log%20the%20before%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22460%22%20y%3D%2275%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-3%22%20value%3D%22Upper%20case%20the%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22280%22%20y%3D%2275%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-4%22%20value%3D%22Do%20the%20original%20function%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22620%22%20y%3D%22160%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-5%22%20value%3D%22Log%20the%20after%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22460%22%20y%3D%22240%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-6%22%20value%3D%22Upper%20case%20the%20value%22%20style%3D%22rounded%3D1%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22280%22%20y%3D%22240%22%20width%3D%22140%22%20height%3D%2250%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-7%22%20value%3D%22End%22%20style%3D%22ellipse%3BwhiteSpace%3Dwrap%3Bhtml%3D1%3B%22%20vertex%3D%221%22%20parent%3D%221%22%3E%3CmxGeometry%20x%3D%22120%22%20y%3D%22225%22%20width%3D%22120%22%20height%3D%2280%22%20as%3D%22geometry%22%2F%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-8%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0%3BentryY%3D0.5%3BexitX%3D1%3BexitY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-1%22%20target%3D%22114a013724b344ee-3%22%3E%3CmxGeometry%20width%3D%2250%22%20height%3D%2250%22%20relative%3D%221%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%2210%22%20y%3D%2260%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%2260%22%20y%3D%2210%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-9%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0%3BentryY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20target%3D%22114a013724b344ee-2%22%3E%3CmxGeometry%20x%3D%22250%22%20y%3D%22110%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22420%22%20y%3D%22100%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22290%22%20y%3D%22110%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-10%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0%3BentryY%3D0.25%3BexitX%3D0.5%3BexitY%3D1%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-2%22%20target%3D%22114a013724b344ee-4%22%3E%3CmxGeometry%20x%3D%22260%22%20y%3D%22120%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22430%22%20y%3D%22110%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22470%22%20y%3D%22110%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-11%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D0.5%3BentryY%3D0%3BexitX%3D-0.007%3BexitY%3D0.9%3BexitPerimeter%3D0%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-4%22%20target%3D%22114a013724b344ee-5%22%3E%3CmxGeometry%20x%3D%22270%22%20y%3D%22130%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22540%22%20y%3D%22135%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22605%22%20y%3D%22170%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-12%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D1%3BentryY%3D0.5%3BexitX%3D0%3BexitY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-5%22%20target%3D%22114a013724b344ee-6%22%3E%3CmxGeometry%20x%3D%22280%22%20y%3D%22140%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22550%22%20y%3D%22145%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22615%22%20y%3D%22180%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3CmxCell%20id%3D%22114a013724b344ee-13%22%20value%3D%22%22%20style%3D%22endArrow%3Dclassic%3Bhtml%3D1%3BentryX%3D1%3BentryY%3D0.5%3BexitX%3D0%3BexitY%3D0.5%3B%22%20edge%3D%221%22%20parent%3D%221%22%20source%3D%22114a013724b344ee-6%22%20target%3D%22114a013724b344ee-7%22%3E%3CmxGeometry%20x%3D%22290%22%20y%3D%22150%22%20width%3D%2250%22%20height%3D%2250%22%20as%3D%22geometry%22%3E%3CmxPoint%20x%3D%22560%22%20y%3D%22155%22%20as%3D%22sourcePoint%22%2F%3E%3CmxPoint%20x%3D%22625%22%20y%3D%22190%22%20as%3D%22targetPoint%22%2F%3E%3C%2FmxGeometry%3E%3C%2FmxCell%3E%3C%2Froot%3E%3C%2FmxGraphModel%3E" style="background-color: rgb(255, 255, 255);"><defs/><g transform="translate(0.5,0.5)"><ellipse cx="61" cy="41" rx="60" ry="40" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(7.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="106" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 107px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Call doSomething()</div></div></foreignObject><text x="53" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Call doSomething()</text></switch></g><rect x="341" y="16" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(354.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="112" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 113px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Log the before value</div></div></foreignObject><text x="56" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Log the before value</text></switch></g><rect x="161" y="16" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(172.5,34.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="116" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 117px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Upper case the value</div></div></foreignObject><text x="58" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Upper case the value</text></switch></g><rect x="501" y="101" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(507.5,119.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="126" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 127px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Do the original function</div></div></foreignObject><text x="63" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Do the original function</text></switch></g><rect x="341" y="181" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(360.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="101" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 102px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Log the after value</div></div></foreignObject><text x="51" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Log the after value</text></switch></g><rect x="161" y="181" width="140" height="50" rx="7.5" ry="7.5" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(172.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="116" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 117px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">Upper case the value</div></div></foreignObject><text x="58" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">Upper case the value</text></switch></g><ellipse cx="61" cy="206" rx="60" ry="40" fill="#ffffff" stroke="#000000" pointer-events="none"/><g transform="translate(49.5,199.5)"><switch><foreignObject style="overflow:visible;" pointer-events="all" width="22" height="12" requiredFeatures="http://www.w3.org/TR/SVG11/feature#Extensibility"><div xmlns="http://www.w3.org/1999/xhtml" style="display: inline-block; font-size: 12px; font-family: Helvetica; color: rgb(0, 0, 0); line-height: 1.2; vertical-align: top; width: 23px; white-space: nowrap; text-align: center;"><div xmlns="http://www.w3.org/1999/xhtml" style="display:inline-block;text-align:inherit;text-decoration:inherit;">End</div></div></foreignObject><text x="11" y="12" fill="#000000" text-anchor="middle" font-size="12px" font-family="Helvetica">End</text></switch></g><path d="M 121 41 L 154.63 41" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 159.88 41 L 152.88 44.5 L 154.63 41 L 152.88 37.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 301 41 L 334.63 41" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 339.88 41 L 332.88 44.5 L 334.63 41 L 332.88 37.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 411 66 L 495.38 111" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 500.01 113.47 L 492.19 113.27 L 495.38 111 L 495.48 107.09 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 500 146 L 416.93 178.67" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 412.04 180.59 L 417.27 174.77 L 416.93 178.67 L 419.84 181.29 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 341 206 L 307.37 206" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 302.12 206 L 309.12 202.5 L 307.37 206 L 309.12 209.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 161 206 L 127.37 206" fill="none" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/><path d="M 122.12 206 L 129.12 202.5 L 127.37 206 L 129.12 209.5 Z" fill="#000000" stroke="#000000" stroke-miterlimit="10" pointer-events="none"/></g></svg>

#### Embracing the asynchrony

Why all this use of closures and functions returning functions?

>"Are you one of those wonks who loves to make code look more difficult to make yourself look smart?"

Quite the opposite, in fact I have gone out of my way to make these samples as simple as possible in as readable way as possible. We are using closures in order to have 1 decorator "remember" the next function it needs to call in the chain because then it can carry on the chain in a callback. We are using JavaScript here, and therefore we make use of callbacks *all the time*. JavaScript patterns have to allow for it.

What's wrong with just calling each decorator in a for loop?

example of for-each here...

why don't we do this proceduraly (just loop through the array of decorator functions calling one after the other instead of setting up a chain of next functions stored as closures in order to be called by each consecutive decorator function? Easy - asynchronous code.

example of the validator here...

If we just called one after the other - what about the validator function that has a callback to state whether it's valid or not? We can't control the calling of the next decorator function from a callback, only from a return value from the initial function.


