USE PhoneBook
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Person', 'Location', 'PhoneNumber')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Person')
DROP TABLE [Person]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Location')
DROP TABLE [Location]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PhoneNumber')
DROP TABLE [PhoneNumber]
GO

CREATE TABLE [Person]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Person columns
  [FirstName] nvarchar (255) NULL,
  [LastName] nvarchar (255) NOT NULL,
  [EMailAddress] nvarchar (255) NULL,
  [Location] uniqueidentifier NULL,

  CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Location]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Location columns
  [Name] nvarchar (255) NOT NULL,
  [ZipCode] nvarchar (12) NULL,
  [City] nvarchar (255) NULL,
  [Street] nvarchar (255) NULL,
  [Country] int NOT NULL,

  CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [PhoneNumber]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- PhoneNumber columns
  [CountryCode] nvarchar (6) NULL,
  [AreaCode] nvarchar (6) NULL,
  [Number] nvarchar (80) NOT NULL,
  [Extension] nvarchar (6) NULL,
  [Person] uniqueidentifier NULL,

  CONSTRAINT [PK_PhoneNumber] PRIMARY KEY CLUSTERED ([ID])
)
GO

ALTER TABLE [Person] ADD
  CONSTRAINT [FK_LocationToPerson] FOREIGN KEY ([Location]) REFERENCES [Location] ([ID])
GO

ALTER TABLE [PhoneNumber] ADD
  CONSTRAINT [FK_PersonToPhoneNumber] FOREIGN KEY ([Person]) REFERENCES [Person] ([ID])
GO

