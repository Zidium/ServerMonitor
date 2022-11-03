@echo off

rmdir /s /q Release

dotnet publish "ServerMonitor\ServerMonitor.csproj" -v:minimal -c:Release -o:.\Release
copy zidium-server-monitor.service Release\

pause