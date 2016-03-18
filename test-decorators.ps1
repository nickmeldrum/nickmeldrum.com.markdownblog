echo "> Testing monkey patching..."
&node .\web\scripts\decorator-monkeypatching.js
echo "> Testing closures..."
&node .\web\scripts\decorator-closure.js
echo "> Testing inheritance..."
&node .\web\scripts\decorator-inheritance.js
echo "> Testing proxies..."
&node --harmony-proxies .\web\scripts\decorator-proxy.js
echo "> Testing middleware..."
&node .\web\scripts\decorator-middleware.js
echo "> Testing procedural..."
&node .\web\scripts\decorator-procedural.js

