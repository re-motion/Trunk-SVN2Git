--Extendend file-builder comment at the beginning
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END
GO
USE SchemaGenerationTestDomain3
GO

-- Drop all synonyms that will be created below
IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'Test' AND SCHEMA_NAME(schema_id) = 'AddedViewSynonym')
  DROP SYNONYM [Test].[AddedViewSynonym]
GO

-- Drop all indexes that will be created below
IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_NonClusteredUniqueIndex')
  DROP INDEX [IDX_NonClusteredUniqueIndex] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_NonClusteredNonUniqueIndex')
  DROP INDEX [IDX_NonClusteredNonUniqueIndex] ON [dbo].[IndexTestTable]
GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'IndexTestTable' and schema_name (so.schema_id)='dbo' and si.[name] = 'IDX_IndexWithSeveralOptions')
  DROP INDEX [IDX_IndexWithSeveralOptions] ON [dbo].[IndexTestTable]
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
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'InheritanceRoot' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[InheritanceRoot]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'IndexTestTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[IndexTestTable]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'PKTestTable' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[PKTestTable]
GO

