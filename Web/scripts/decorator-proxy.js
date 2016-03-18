'use strict'

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

