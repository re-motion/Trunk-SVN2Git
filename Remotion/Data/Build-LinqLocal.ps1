set-alias nant "C:\Program Files\NAnt\bin.net-2.0\nant.exe";

nant "-f:Linq.build" "-D:solution.global-dir=\Development\global" "-D:build.temp.root=\Temp\RemotionLinqLocal" "-t:net-3.5" "-l:Build.log" "-nologo" `
    "-D:build.update.assembly-info=true" `
    cleantemp `
    resources debug;

if ($LastExitCode -ne 0) 
{ 
  [System.Console]::ReadKey($false);
  throw "Build Linq has failed."; 
}

nant "-f:Linq.build" "-D:solution.global-dir=\Development\global" "-D:build.temp.root=\Temp\RemotionLinqLocal" "-t:net-3.5" "-nologo" `
    cleantemp `
    sourcezip zip;

[System.Console]::ReadKey($false);
