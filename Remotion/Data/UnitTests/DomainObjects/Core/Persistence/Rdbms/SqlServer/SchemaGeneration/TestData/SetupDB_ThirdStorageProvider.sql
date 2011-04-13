--Extendend file-builder comment at the beginning
CREATE SCHEMA Test
GO
USE SchemaGenerationTestDomain3
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AboveInheritanceRootView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AboveInheritanceRootView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'BelowInheritanceRootView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[BelowInheritanceRootView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'InheritanceRootView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[InheritanceRootView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AddedView' AND TABLE_SCHEMA = 'Test')
  DROP VIEW [Test].[AddedView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (max)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + schema_name(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id 
WHERE fk.type = 'F' AND schema_name (t.schema_id) + '.' + t.name IN ('Test.NewTableName') 
ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'NewTableName' AND TABLE_SCHEMA = 'Test')
  DROP TABLE [Test].[NewTableName]
GO

-- Create all tables
CREATE TABLE [Test].[NewTableName]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PropertyAboveInheritanceRoot] nvarchar (max) NULL,
  [PropertyInheritanceRoot] nvarchar (max) NULL,
  [PropertyBelowInheritanceRoot] nvarchar (max) NULL,
  CONSTRAINT [PK_InheritanceRoot] PRIMARY KEY CLUSTERED ([ID])
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

CREATE VIEW [dbo].[InheritanceRootView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[NewTableName]
  WITH CHECK OPTION
GO

CREATE VIEW [Test].[AddedView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [Test].[InheritanceRoot]
    WHERE [ClassID] IN ('ClassID')
  WITH CHECK OPTION
GO
DROP SCHEMA Test
--Extendend file-builder comment at the end
