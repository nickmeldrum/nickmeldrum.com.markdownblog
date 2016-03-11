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

