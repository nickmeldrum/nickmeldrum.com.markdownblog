'use strict'

function myComponentFactory() {
    let suffix = ''
    let decorators = []
    return {
        setSuffix: suf => {
            suffix = suf
        },
        printValue: value => {
            decorators.re
            console.log(`value is ${value + suffix}`)
        },
        addDecorators: decs => decorators = decs
    }
}

function spacer() {
    value.split('').join(' ')
}

function upperCaser() {
    value.toUpperCase()
}

function validator() {
    const isValid = ~value.indexOf('my')

    setTimeout(() => {
        if (isValid)
            next(value)
        else
            console.log('not valid man...')
    }, 1000)
}

const component = myComponentFactory()
component.addDecorators([validatorPrintValueDecorator, spacerPrintValueDecorator, upperCaserPrintValueDecorator])
component.setSuffix('END')
component.printValue('my value')
component.printValue('invalid value')

