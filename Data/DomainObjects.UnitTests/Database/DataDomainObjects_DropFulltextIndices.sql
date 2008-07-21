USE TestDomain

IF EXISTS (SELECT * FROM sys.fulltext_indexes INNER JOIN sys.fulltext_catalogs ON sys.fulltext_indexes.fulltext_catalog_id = sys.fulltext_catalogs.fulltext_catalog_id WHERE [Name] = 'TestDomain_FT')
BEGIN
  DROP FULLTEXT INDEX ON [Ceo]
END 
GO

IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE [Name] = 'TestDomain_FT')
BEGIN
  DROP FULLTEXT CATALOG [TestDomain_FT]
END
GO
