USE TestDomain

EXEC sp_fulltext_database 'enable'
CREATE FULLTEXT CATALOG [TestDomain_FT] IN PATH 'C:\Databases\ftdata'
CREATE FULLTEXT INDEX ON [Ceo]([Name]) KEY INDEX [PK_Ceo] ON [TestDomain_FT]