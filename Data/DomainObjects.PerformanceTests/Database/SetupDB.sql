USE PerformanceTestDomain
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Client', 'File', 'Person', 'Company')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client')
DROP TABLE [Client]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'File')
DROP TABLE [File]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Person')
DROP TABLE [Person]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Company')
DROP TABLE [Company]
GO

CREATE TABLE [Client]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Client columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [File]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- File columns
  [Number] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,

  CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Person]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClientBoundBaseClass columns
  [ClientID] uniqueidentifier NULL,

  -- Person columns
  [FirstName] nvarchar (100) NOT NULL,
  [LastName] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Company]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClientBoundBaseClass columns
  [ClientID] uniqueidentifier NULL,

  -- Company columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([ID])
)
GO

ALTER TABLE [File] ADD
  CONSTRAINT [FK_ClientToFile] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
GO

ALTER TABLE [Person] ADD
  CONSTRAINT [FK_ClientToClientBoundBaseClass1] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
GO

ALTER TABLE [Company] ADD
  CONSTRAINT [FK_ClientToClientBoundBaseClass2] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
GO

