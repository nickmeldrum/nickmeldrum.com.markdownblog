# Decorators in JavaScript (and why redux middleware is not middleware!)

## introduction
    Going to show 4 ways of creating a decorator:
        monkey patching
        closures
        Proxy object
        closures ioc style (TODO: need better name)
    Talk about the pros and cons of each method
    Then talk about why this is NOT middleware, and the difference between the decorator pattern and the pipeline pattern
    Notes: have put at the end my arguments for a) using ES6 syntax and b) NOT using classes. At end as I don't really think it's that important

## 1. Monkeypatching

### what is monkeypatching?

### show monkeypatch version step by step to introduce the component

### pros and cons of monkeypatch version

## 2. Closures

### what are closures?

### the closures version

### pros and cons

## 3. Proxies

### what are closures?

### the closures version

### pros and cons

Note weirdness in calling this one wrapping the function calls the other way around


## 4. The final unnamed one : TODO get a name!

### The unnamed version

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

## addendum:

 * talk about using es6
 * talk about not using classes

