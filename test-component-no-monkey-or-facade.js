'use strict'

//node --harmony-proxies .\test-component-no-monkey-or-facade.js
const Reflect = require('harmony-reflect')

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

