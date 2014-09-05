@echo Off
set target=%1
if "%target%" == "" (
   set target=Build
)
set config=%2
if "%config%" == "" (
   set config=Release
)

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Hangfire.Dashboard.Authorization.sln /t:"%target%" /p:Configuration="%config%" /m /fl /flp:LogFile=msbuild.log;Verbosity=Normal;Encoding=UTF-8 /nr:false
.nuget\nuget.exe pack Hangfire.Dashboard.Authorization.nuspec -OutputDirectory build