USE RemotionSecurityManagerWebClientTest
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileView')
  DROP VIEW [dbo].[FileView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileItemView')
  DROP VIEW [dbo].[FileItemView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('File', 'FileItem')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'File')
  DROP TABLE [dbo].[File]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FileItem')
  DROP TABLE [dbo].[FileItem]
GO

-- Create all tables
CREATE TABLE [dbo].[File]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- File columns
  [Name] nvarchar (100) NOT NULL,
  [Confidentiality] int NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [CreatorUserID] uniqueidentifier NULL,
  [ClerkUserID] uniqueidentifier NULL,

  CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[FileItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- FileItem columns
  [Name] nvarchar (100) NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [FileID] uniqueidentifier NULL,

  CONSTRAINT [PK_FileItem] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[File] ADD
  CONSTRAINT [FK_TenantToFile] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID]),
  CONSTRAINT [FK_OwnerUserToFile] FOREIGN KEY ([CreatorUserID]) REFERENCES [dbo].[User] ([ID]),
  CONSTRAINT [FK_ClerkUserToFile] FOREIGN KEY ([ClerkUserID]) REFERENCES [dbo].[User] ([ID])

ALTER TABLE [dbo].[FileItem] ADD
  CONSTRAINT [FK_FileItemToFile] FOREIGN KEY ([FileID]) REFERENCES [dbo].[File] ([ID]),
  CONSTRAINT [FK_TenantToFileItem] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[FileView] ([ID], [ClassID], [Timestamp], [Name], [Confidentiality], [TenantID], [CreatorUserID], [ClerkUserID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [Confidentiality], [TenantID], [CreatorUserID], [ClerkUserID]
    FROM [dbo].[File]
    WHERE [ClassID] IN ('File')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[FileItemView] ([ID], [ClassID], [Timestamp], [Name], [TenantID], [FileID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [TenantID], [FileID]
    FROM [dbo].[FileItem]
    WHERE [ClassID] IN ('FileItem')
  WITH CHECK OPTION
GO
