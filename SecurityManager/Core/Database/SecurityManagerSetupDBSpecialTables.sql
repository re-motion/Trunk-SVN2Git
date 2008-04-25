USE RemotionSecurityManager
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Revision')
  DROP TABLE [dbo].[Revision]

-- Create all tables
CREATE TABLE [dbo].[Revision]
(
  [Value] int NOT NULL
)

GO

INSERT INTO [dbo].[Revision] 
([Value]) VALUES (0)

GO
