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

