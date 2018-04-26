@echo off
set msbuild.exe=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe

rmdir /s /q Release

"%msbuild.exe%" WindowsService\WindowsService.csproj /t:Build /p:Configuration=Release /v:minimal

xcopy /E /Y /S WindowsService\bin\Release\*.pdb Release\WindowsService\
xcopy /E /Y /S WindowsService\bin\Release\Zidium.config Release\WindowsService\
xcopy /E /Y /S WindowsService\bin\Release\ZidiumServerMonitor.exe.config Release\WindowsService\
xcopy /E /Y /S WindowsService\bin\Release\*.exe Release\WindowsService\
xcopy /E /Y /S WindowsService\bin\Release\*.dll Release\WindowsService\
xcopy /E /Y /S WindowsService\bin\Release\settings.json Release\WindowsService\

"%msbuild.exe%" NetCoreConsoleApplication\NetCoreConsoleApplication.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml

xcopy /E /Y /S NetCoreConsoleApplication\bin\Release\PublishOutput\*.* Release\NetCoreConsoleApplication\

pause