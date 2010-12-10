USE TestDomain
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_OfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_OfficialView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_SpecialOfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_SpecialOfficialView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('SchemaGeneration_Official')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Official' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_Official]
GO

-- Create all tables
CREATE TABLE [dbo].[SchemaGeneration_Official]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Official columns
  [Name] nvarchar (100) NOT NULL,
  [ResponsibleForOrderPriority] int NOT NULL,
  [ResponsibleForCustomerType] int NOT NULL,

  -- SchemaGeneration_SpecialOfficial columns
  [Speciality] nvarchar (255) NULL,

  CONSTRAINT [PK_SchemaGeneration_Official] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
GO

-- Create a view for every class
CREATE VIEW [dbo].[SchemaGeneration_OfficialView] ([ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality]
    FROM [dbo].[SchemaGeneration_Official]
    WHERE [ClassID] IN ('SchemaGeneration_Official', 'SchemaGeneration_SpecialOfficial')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_SpecialOfficialView] ([ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality]
    FROM [dbo].[SchemaGeneration_Official]
    WHERE [ClassID] IN ('SchemaGeneration_SpecialOfficial')
  WITH CHECK OPTION
GO
