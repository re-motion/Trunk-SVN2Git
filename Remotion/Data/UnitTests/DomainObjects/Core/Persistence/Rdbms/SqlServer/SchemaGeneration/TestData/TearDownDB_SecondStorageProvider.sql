USE SchemaGenerationTestDomain2
-- Drop all synonyms
GO
-- Drop all indexes
GO
-- Drop all views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OfficialView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SpecialOfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SpecialOfficialView]
GO
-- Drop foreign keys of all tables
GO
-- Drop all tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Official' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Official]
GO
