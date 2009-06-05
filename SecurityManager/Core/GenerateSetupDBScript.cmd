@echo off
echo Running .\..\..\Remotion\Data\DomainObjects.RdbmsTools.Console\bin\Debug\dbschema.exe from %CD%...

.\..\..\Remotion\Data\DomainObjects.RdbmsTools.Console\bin\Debug\dbschema.exe ^
    "/baseDirectory:bin\debug" ^
    "/config:..\Clients.Web\web.config" ^
    "/schema" ^
    "/schemaDirectory:Database"

if not %ERRORLEVEL%==0 goto dbschema_error
echo.

del Database\SecurityManagerSetupDB.sql 2> NUL
ren Database\SetupDB.sql SecurityManagerSetupDB.sql

if not %ERRORLEVEL%==0 goto ren_error

goto end

:dbschema_error
echo.
echo There was an error running dbschema.exe.
exit 1

:ren_error
echo.
echo There was an error renaming SetupDB.sql to SecurityManagerSetupDB.sql.
exit 2

:end