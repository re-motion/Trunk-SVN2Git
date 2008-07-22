.\..\..\Data\DomainObjects.RdbmsTools.Console\bin\Debug\dbschema.exe `
    "/baseDirectory:bin\debug" `
    "/config:..\Clients.Web\web.config" `
    "/schema" `
    "/schemaDirectory:Database"


remove-item Database\SecurityManagerSetupDB.sql

rename-item Database\SetupDB.sql SecurityManagerSetupDB.sql
