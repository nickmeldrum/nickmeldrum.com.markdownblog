    function aModule() {
        return {
            doSomething: (value) => {
                console.log(`um wow, value is ${value}`)
            }
        }
    }

    function decorateWithSpacer(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (value) => {
            value = value.split('').join(' ')
            console.log(`in spacer: ${value}`)
            originalFunc(value)
            console.log(`in spacer: ${value}`)
        }
    }

    function decorateWithUpperCaser(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (value) => {
            value = value.toUpperCase()
            console.log(`in caser: ${value}`)
            originalFunc(value)
            console.log(`in caser: ${value}`)
        }
    }

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
    decorateWithSpacer(instance3)
    decorateWithUpperCaser(instance3)
    decorateWithCheckValueIsValid(instance3)
    instance3.doSomething('my value')

