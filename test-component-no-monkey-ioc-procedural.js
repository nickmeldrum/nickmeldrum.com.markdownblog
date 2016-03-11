'use strict'

function myComponentFactory() {
    let suffix = ''
    let decorators = []
    return {
        setSuffix: suf => {
            suffix = suf
        },
        printValue: value => {
            console.log(`value is ${value + suffix}`)
        },
        addDecorators = decs => decorators = decs
    }
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

