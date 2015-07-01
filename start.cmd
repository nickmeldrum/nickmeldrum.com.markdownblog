@echo off
"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" start
"C:\Program Files\IIS Express\iisexpress.exe" /site:MarkdownBlog.Net.Web
