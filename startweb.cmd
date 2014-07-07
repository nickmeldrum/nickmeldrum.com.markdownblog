@echo off
"C:\Program Files (x86)\Microsoft SDKs\Windows Azure\Storage Emulator\WAStorageEmulator.exe" start
start cmd.exe /c "C:\Program Files\IIS Express\iisexpress.exe" /site:MarkdownBlog.Net.Web
