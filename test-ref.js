    function aModule() {
        return {
            doSomething: (obj) => {
                console.log(`um wow, value is ${obj.value}`)
            }
        }
    }

    function decorateWithSpacer(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (obj) => {
            obj.value = obj.value.split('').join(' ')
            console.log(`in spacer: ${obj.value}`)
            originalFunc(obj)
            console.log(`in spacer: ${obj.value}`)
        }
    }

    function decorateWithUpperCaser(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (obj) => {
            obj.value = obj.value.toUpperCase()
            console.log(`in caser: ${obj.value}`)
            originalFunc(obj)
            console.log(`in caser: ${obj.value}`)
        }
    }

    function decorateWithCheckValueIsValid(aModuleInstance) {
        const originalFunc = aModuleInstance.doSomething

        aModuleInstance.doSomething = (obj) => {
            const isValid = ~obj.value.indexOf('my')

            setTimeout(() => {
                if (isValid)
                    originalFunc(obj)
                else
                    console.log('not valid man...')
            }, 1000)
        }
    }

    const instance3 = aModule()
    decorateWithSpacer(instance3)
    decorateWithUpperCaser(instance3)
    decorateWithCheckValueIsValid(instance3)
    instance3.doSomething({value: 'my value'})

