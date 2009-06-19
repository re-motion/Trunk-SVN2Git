@echo off

if not exist remotion.snk goto nosnk

Build-RemotionLocalDoc.cmd
goto snk

:nosnk
echo.
echo Did not find remotion.snk keyfile. 
echo You can download the latest stable and officially signed re-motion build from the Downloads section at www.re-motion.org.  
echo If you want to build your own version of re-motion, you need to create your own key to sign it; 
echo to do so, please run Generate-Snk.cmd from a Visual Studio Command Prompt in this directory.
echo.
pause

:snk
