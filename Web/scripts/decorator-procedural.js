'use strict'

function myComponentFactory() {
    let suffix = ''
    let decorators = []
    return {
        setSuffix: suff => suffix = suff,
        printValue: value => {
            decorators.forEach(item => value = item(value))
            console.log(`value is ${value + suffix}`)
        },
        addDecorators: decs => decorators = decs
    }
}

function toLower(value) {
    return value.toLowerCase()
}

function validator(value) {
    const isValid = ~value.indexOf('My')

    setTimeout(() => {
        if (isValid) return value
        else console.log('not valid man...')
    }, 500)

    return value
}

const component = myComponentFactory()
component.addDecorators([validator, toLower])
component.setSuffix('!')
component.printValue('My Value')
component.printValue('Invalid Value')

