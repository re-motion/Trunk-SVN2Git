USE SchemaGenerationTestDomain2
GO

-- Drop all synonyms that will be created below
GO

-- Drop all indexes that will be created below
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OfficialView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SpecialOfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SpecialOfficialView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (max)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + schema_name(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id 
WHERE fk.type = 'F' AND schema_name (t.schema_id) + '.' + t.name IN ('dbo.Official') 
ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Official' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Official]
GO
