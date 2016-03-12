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

