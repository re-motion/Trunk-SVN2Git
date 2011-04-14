--Extendend file-builder comment at the beginning
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END
GO
USE SchemaGenerationTestDomain3
GO

-- Drop all indexes that will be created below
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
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (max)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + schema_name(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id 
WHERE fk.type = 'F' AND schema_name (t.schema_id) + '.' + t.name IN ('dbo.InheritanceRoot') 
ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'InheritanceRoot' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[InheritanceRoot]
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
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
    WHERE [ClassID] IN ('ClassID')
  WITH CHECK OPTION
GO

-- Create indexes for tables that were created above
GO
--Extendend file-builder comment at the end
