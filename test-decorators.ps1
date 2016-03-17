echo "monkey patching..."
&node .\web\scripts\decorator-monkeypatching.js
echo "closures..."
&node .\web\scripts\decorator-closure.js
echo "proxies..."
&node --harmony-proxies .\web\scripts\decorator-proxy.js
echo "component setup..."
&node .\web\scripts\decorator-component-setup.js
echo "procedural..."
&node .\web\scripts\decorator-procedural.js

