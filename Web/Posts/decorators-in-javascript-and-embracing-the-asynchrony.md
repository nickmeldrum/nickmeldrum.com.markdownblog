## A classic decorator pattern example




## Is Middleware just another name for the decorator pattern?

TL/DR Yup seems to be!

middleware - define it as a decorator - connect middleware has filters and providers - 

from http://redux.js.org/docs/advanced/Middleware.html:

"...middleware is some code you can put between the framework receiving a request, and the framework generating a response. For example, Express or Koa middleware may add CORS headers, logging, compression, and more. The best feature of middleware is that it’s composable in a chain. You can use multiple independent third-party middleware in a single project."

from wikipedia entry on decorator pattern:

"...allows behavior to be added to an individual object, either statically or dynamically, without affecting the behavior of other objects from the same class.[1] The decorator pattern is often useful for adhering to the Single Responsibility Principle, as it allows functionality to be divided between classes with unique areas of concern."

and 

"...This pattern is designed so that multiple decorators can be stacked on top of each other, each time adding a new functionality to the overridden method(s)."

That sounds an awful lot like "decorator" to me - so we have a new cool word for decorator, that's okay with me I guess...


so what is middleware?
middleware is a decorator pattern on an object that knows it is to be decorated - this means it can manage the decorators which gives us some more power:

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


