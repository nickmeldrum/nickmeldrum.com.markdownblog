'use strict'

function myComponentFactory() {
    let suffix = ''
    const instance = {
        setSuffix: suff => suffix = suff,
        printValue: value => console.log(`value is ${value + suffix}`),
        addDecorators: decorators => {
            let printValue = instance.printValue
            decorators.slice().reverse().forEach(decorator => printValue = decorator(printValue))
            instance.printValue = printValue
        }
    }
    return instance
}

function toLowerDecorator(inner) {
    return value => inner(value.toLowerCase())
}

function validatorDecorator(inner) {
    return value => {
        const isValid = ~value.indexOf('My')

        setTimeout(() => {
            if (isValid) inner(value)
            else console.log('not valid man...')
        }, 500)
    }
}

const component = myComponentFactory()
component.addDecorators([validatorDecorator, toLowerDecorator])
component.setSuffix('!')
component.printValue('My Value')
component.printValue('Invalid Value')

