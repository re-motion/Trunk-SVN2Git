@echo off
set nant="prereq\Tools\NAnt\bin.net-2.0\nant.exe"

if not exist remotion.snk goto nosnk

echo Building the following targets without docs using %nant%: %*...
echo.

%nant% "-f:Remotion.build" "-D:build.temp.root=\Temp\RemotionLocal" "-t:net-3.5" "-l:Build.log" "-nologo" ^
    "-D:build.update.assembly-info=false" ^
    set-debug %*
    
if not %ERRORLEVEL%==0 goto build_failed

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