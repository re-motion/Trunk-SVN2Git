--Extendend file-builder comment at the beginning
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END
GO
USE SchemaGenerationTestDomain3
GO

-- Drop all synonyms that will be created below
GO

-- Drop all indexes that will be created below
IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_NonClusteredUniqueIndex')
  DROP INDEX [IDX_NonClusteredUniqueIndex] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_NonClusteredNonUniqueIndex')
  DROP INDEX [IDX_NonClusteredNonUniqueIndex] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_PrimaryXmlIndex')
  DROP INDEX [IDX_PrimaryXmlIndex] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_SecondaryXmlIndex1')
  DROP INDEX [IDX_SecondaryXmlIndex1] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_SecondaryXmlIndex2')
  DROP INDEX [IDX_SecondaryXmlIndex2] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_SecondaryXmlIndex3')
  DROP INDEX [IDX_SecondaryXmlIndex3] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'PKTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_ClusteredUniqueIndex')
  DROP INDEX [IDX_ClusteredUniqueIndex] ON [dbo].[PKTestTable]
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AboveInheritanceRootView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AboveInheritanceRootView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'BelowInheritanceRootView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[BelowInheritanceRootView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'NewViewName' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[NewViewName]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AddedView' AND TABLE_SCHEMA = 'Test')
  DROP VIEW [Test].[AddedView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'IndexTestView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[IndexTestView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PKTestView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PKTestView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (max)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + schema_name(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id 
WHERE fk.type = 'F' AND schema_name (t.schema_id) + '.' + t.name IN ('dbo.InheritanceRoot', 'dbo.IndexTestTable', 'dbo.PKTestTable') 
ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'InheritanceRoot' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[InheritanceRoot]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'IndexTestTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[IndexTestTable]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PKTestTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[PKTestTable]
GO

-- Create all tables
CREATE TABLE [dbo].[InheritanceRoot]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PropertyAboveInheritanceRoot] nvarchar (max) NULL,
  [PropertyInheritanceRoot] nvarchar (max) NULL,
  [PropertyBelowInheritanceRoot] nvarchar (max) NULL,
  CONSTRAINT [PK_InheritanceRoot] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[IndexTestTable]
(
  [ID] uniqueidentifier NOT NULL,
  [FirstName] varchar(100) NOT NULL,
  [LastName] varchar(100) NOT NULL,
  [XmlColumn1] xml NOT NULL,
  CONSTRAINT [PK_IndexTestTable_ID] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[PKTestTable]
(
  [ID] uniqueidentifier NOT NULL,
  [Name] varchar(100) NOT NULL,
  CONSTRAINT [PK_PKTestTable_ID] PRIMARY KEY NONCLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
GO

-- Create a view for every class
CREATE VIEW [dbo].[AboveInheritanceRootView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[BelowInheritanceRootView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
    WHERE [ClassID] IN ('BelowInheritanceRoot')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[NewViewName] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
  WITH CHECK OPTION
GO

CREATE VIEW [Test].[AddedView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
    WHERE [ClassID] IN ('ClassID')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[IndexTestView] ([ID], [FirstName], [LastName], [XmlColumn1])
  WITH SCHEMABINDING AS
  SELECT [ID], [FirstName], [LastName], [XmlColumn1]
    FROM [dbo].[IndexTestTable]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PKTestView] ([ID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [Name]
    FROM [dbo].[PKTestTable]
  WITH CHECK OPTION
GO

-- Create indexes for tables that were created above
CREATE UNIQUE NONCLUSTERED INDEX [IDX_NonClusteredUniqueIndex]
  ON [dbo].[IndexTestTable] ([ID])
  WITH IGNORE_DUP_KEY
GO

CREATE NONCLUSTERED INDEX [IDX_NonClusteredNonUniqueIndex]
  ON [dbo].[IndexTestTable] ([FirstName], [LastName])
  INCLUDE ([ID])
GO

CREATE PRIMARY XML INDEX [IDX_PrimaryXmlIndex]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
GO

CREATE XML INDEX [IDX_SecondaryXmlIndex1]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Path
GO

CREATE XML INDEX [IDX_SecondaryXmlIndex2]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Value
GO

CREATE XML INDEX [IDX_SecondaryXmlIndex3]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Property
GO

CREATE UNIQUE CLUSTERED INDEX [IDX_ClusteredUniqueIndex]
  ON [dbo].[PKTestTable] ([Name])
  WITH IGNORE_DUP_KEY
GO

-- Create synonyms for tables that were created above
GO
--Extendend file-builder comment at the end
