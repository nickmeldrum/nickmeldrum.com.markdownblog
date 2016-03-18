echo "> Testing monkey patching..."
&node .\web\scripts\decorator-monkeypatching.js
echo "> Testing closures..."
&node .\web\scripts\decorator-closure.js
echo "> Testing proxies..."
&node --harmony-proxies .\web\scripts\decorator-proxy.js
echo "> Testing component setup..."
&node .\web\scripts\decorator-component-setup.js
echo "> Testing procedural..."
&node .\web\scripts\decorator-procedural.js

