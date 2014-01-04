USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'RemotionDataDomainObjectsObjectBindingTestDomain')
BEGIN
  ALTER DATABASE RemotionDataDomainObjectsObjectBindingTestDomain SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE RemotionDataDomainObjectsObjectBindingTestDomain
END
GO

CREATE DATABASE RemotionDataDomainObjectsObjectBindingTestDomain
ON PRIMARY (
	Name = 'RemotionDataDomainObjectsObjectBindingTestDomain_Data',
	Filename = 'C:\Databases\RemotionDataDomainObjectsObjectBindingTestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RemotionDataDomainObjectsObjectBindingTestDomain_Log',
	Filename = 'C:\Databases\RemotionDataDomainObjectsObjectBindingTestDomain.ldf',
	Size = 10MB
)
GO

ALTER DATABASE RemotionDataDomainObjectsObjectBindingTestDomain SET RECOVERY SIMPLE
GO
