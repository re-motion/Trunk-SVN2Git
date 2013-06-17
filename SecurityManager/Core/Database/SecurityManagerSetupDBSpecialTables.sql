USE RemotionSecurityManager
GO

-- Create all tables
CREATE TABLE [dbo].[Revision]
(
  [GlobalKey] UNIQUEIDENTIFIER NOT NULL,
  [LocalKey] NVARCHAR (100) NULL,
  [Value] INT NOT NULL
)

CREATE UNIQUE NONCLUSTERED INDEX [IX_Revision_GlobalKey_LocalKey] ON [dbo].[Revision] ([GlobalKey], [LocalKey]) INCLUDE ([Value])
CREATE NONCLUSTERED INDEX [IX_Revision_LocalKey_GlobalKey] ON [dbo].[Revision] ([LocalKey], [GlobalKey]) INCLUDE ([Value])

GO
