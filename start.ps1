&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" start
&start chrome http://localhost:1400
&"C:\Program Files\IIS Express\iisexpress.exe" /path:"$psscriptroot\Web\" /port:1400

