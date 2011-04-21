USE SchemaGenerationTestDomain3
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
  WITH (IGNORE_DUP_KEY = ON, ONLINE = OFF)
GO

CREATE NONCLUSTERED INDEX [IDX_NonClusteredNonUniqueIndex]
  ON [dbo].[IndexTestTable] ([FirstName], [LastName])
  INCLUDE ([ID])
  WITH (IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IDX_IndexWithSeveralOptions]
  ON [dbo].[IndexTestTable] ([FirstName] DESC)
  WITH (IGNORE_DUP_KEY = ON, ONLINE = OFF, PAD_INDEX = ON, FILLFACTOR = 5, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)
GO

CREATE PRIMARY XML INDEX [IDX_PrimaryXmlIndex]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  WITH (PAD_INDEX = ON, FILLFACTOR = 3, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)
GO

CREATE XML INDEX [IDX_SecondaryXmlIndex1]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Path
  WITH (PAD_INDEX = ON, SORT_IN_TEMPDB = OFF, STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF)
GO

CREATE XML INDEX [IDX_SecondaryXmlIndex2]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Value
  WITH (PAD_INDEX = OFF, FILLFACTOR = 8, SORT_IN_TEMPDB = ON)
GO

CREATE XML INDEX [IDX_SecondaryXmlIndex3]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Property
  WITH (STATISTICS_NORECOMPUTE = ON, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF)
GO

CREATE UNIQUE CLUSTERED INDEX [IDX_ClusteredUniqueIndex]
  ON [dbo].[PKTestTable] ([Name])
  WITH (IGNORE_DUP_KEY = ON, ONLINE = OFF)
GO

-- Create synonyms for tables that were created above
CREATE SYNONYM [Test].[AddedViewSynonym] FOR [Test].[AddedView]
GO

--Extendend file-builder comment at the end
