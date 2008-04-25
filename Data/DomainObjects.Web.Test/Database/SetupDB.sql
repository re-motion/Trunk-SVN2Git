USE RpaTest
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes') 
DROP TABLE [TableWithAllDataTypes]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableForRelationTest') 
DROP TABLE [TableForRelationTest]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutColumns') 
DROP TABLE [TableWithoutColumns]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithUndefinedEnum') 
DROP TABLE [TableWithUndefinedEnum]
GO

CREATE TABLE [TableWithAllDataTypes] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] dateTime NOT NULL,
  [DateTime] dateTime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float (53) NOT NULL,
  [Enum] int NOT NULL,
  [Guid] uniqueidentifier NOT NULL,
  [Int16] smallint NOT NULL,
  [Int32] int NOT NULL,
  [Int64] bigint NOT NULL,
  [Single] real NOT NULL,
  [String] nvarchar (100) NOT NULL,
  [StringWithoutMaxLength] text NOT NULL,
  [DelimitedStringArray] nvarchar (1000) NOT NULL,
  [Binary] image NOT NULL,

  [NaBoolean] bit NULL,
  [NaByte] tinyint NULL,
  [NaDate] dateTime NULL,
  [NaDateTime] dateTime NULL,
  [NaDecimal] decimal (38, 3) NULL,
  [NaDouble] float NULL,
  [NaEnum] int NULL,
  [NaGuid] uniqueidentifier NULL,
  [NaInt16] smallint NULL,
  [NaInt32] int NULL,
  [NaInt64] bigint NULL,
  [NaSingle] real NULL,
  
  [StringWithNullValue] nvarchar (100) NULL,
  [DelimitedNullStringArray] nvarchar (1000) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaByteWithNullValue] tinyint NULL,
  [NaDateWithNullValue] dateTime NULL,
  [NaDateTimeWithNullValue] dateTime NULL,
  [NaDecimalWithNullValue] decimal (38, 3) NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaEnumWithNullValue] int NULL,
  [NaGuidWithNullValue] uniqueidentifier NULL,
  [NaInt16WithNullValue] smallint NULL,
  [NaInt32WithNullValue] int NULL,
  [NaInt64WithNullValue] bigint NULL,
  [NaSingleWithNullValue] real NULL,
  [NullableBinary] image NULL,
  
  [TableForRelationTestMandatory] uniqueidentifier NULL,
  [TableForRelationTestOptional] uniqueidentifier NULL,
      
  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableForRelationTest] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] nvarchar (100) NOT NULL,
  
  [TableWithAllDataTypesMandatory] uniqueidentifier NULL,
  [TableWithAllDataTypesOptional] uniqueidentifier NULL,
      
  CONSTRAINT [PK_TableForRelationTest] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithoutColumns] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  CONSTRAINT [PK_TableWithoutColumns] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithUndefinedEnum] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [UndefinedEnum] int NOT NULL,

  CONSTRAINT [PK_TableWithUndefinedEnum] PRIMARY KEY CLUSTERED ([ID])
) 
GO
