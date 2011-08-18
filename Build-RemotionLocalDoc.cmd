@echo off
set msbuild="C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
if not exist remotion.snk goto nosnk

echo Building re-motion without docs using %msbuild%...
echo.

%msbuild% build\Remotion.build /t:DocumentationTestBuild /maxcpucount /verbosity:minimal
    
if not %ERRORLEVEL%==0 goto build_failed

if not %ERRORLEVEL%==0 goto zip_failed

goto build_succeeded

:build_failed
echo.
echo Building re-motion has failed.
pause

exit /b 1

:build_succeeded
echo.
pause
exit /b 0

:nosnk
echo remotion.snk does not exists. Please run Generate-Snk.cmd from a Visual Studio Command Prompt.
pause

exit /b 2