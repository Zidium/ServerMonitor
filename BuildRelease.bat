@echo off
set msbuild.exe=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe

rmdir /s /q Release

"%msbuild.exe%" WindowsService\WindowsService.csproj /t:Build /p:Configuration=Release /v:minimal

xcopy /Y WindowsService\bin\Release\*.pdb Release\WindowsService\
xcopy /Y WindowsService\bin\Release\Zidium.config Release\WindowsService\
xcopy /Y WindowsService\bin\Release\ZidiumServerMonitor.exe.config Release\WindowsService\
xcopy /Y WindowsService\bin\Release\*.exe Release\WindowsService\
xcopy /Y WindowsService\bin\Release\*.dll Release\WindowsService\
xcopy /Y WindowsService\bin\Release\settings.json Release\WindowsService\

dotnet publish NetCoreConsoleApplication\NetCoreConsoleApplication.csproj -c Release -o .\bin\Publish -v normal

xcopy /E /Y /S NetCoreConsoleApplication\bin\Publish\*.* Release\NetCoreConsoleApplication\

pause