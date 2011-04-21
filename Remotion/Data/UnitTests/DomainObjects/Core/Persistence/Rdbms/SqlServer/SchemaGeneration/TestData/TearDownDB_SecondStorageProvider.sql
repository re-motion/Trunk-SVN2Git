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
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Official' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Official]
GO
