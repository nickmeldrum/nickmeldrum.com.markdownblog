## learning to love the call stack in chrome:

1. stop the anonymous functions madness! Use inline functions (research proper term/ use for inline) so you can see detailed function names in the stack - get your context at a glance
2. "blackbox a script" - you will probably have a stack covered with library functions that you don't wanna debug - blackbox the whole script: the script debugger will not pause execution there and they will nicely fade into the background in the stacktrace.

