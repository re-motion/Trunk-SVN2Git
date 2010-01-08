@echo off
set nant="prereq\Tools\NAnt\bin.net-2.0\nant.exe"

if not exist remotion.snk goto nosnk

echo Building re-motion without docs using %nant%...
echo.

%nant% "-f:Remotion.build" "-D:build.temp.root=\Temp\RemotionLocal" "-t:net-3.5" "-l:Build.log" "-nologo" ^
    "-D:build.update.assembly-info=false" ^
    clean cleantemp ^
    resources debug
    
if not %ERRORLEVEL%==0 goto build_failed

echo Zipping up the build results...
echo.

%nant% "-f:Remotion.build" "-D:build.temp.root=\Temp\RemotionLocal" "-D:release-notes.directory=none" "-t:net-3.5" "-nologo" ^
    cleantemp ^
    sourcezip zip ^
    relinq-sourcezip relinq-zip ^
    dms-sourcezip dms-zip ^
    securityManager-sourcezip securityManager-zip

if not %ERRORLEVEL%==0 goto zip_failed

goto build_succeeded

:build_failed
echo.
echo Building re-motion has failed.
pause

exit /b 1

:zip_failed
echo.
echo Zipping re-motion has failed.
pause

exit /b 3

:build_succeeded
echo.
pause
exit /b 0

:nosnk
echo remotion.snk does not exists. Please run Generate-Snk.cmd from a Visual Studio Command Prompt.
pause

exit /b 2