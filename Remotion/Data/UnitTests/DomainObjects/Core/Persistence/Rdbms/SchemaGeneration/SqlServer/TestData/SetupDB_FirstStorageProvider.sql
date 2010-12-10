USE TestDomain
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_CompanyView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_AddressView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_AddressView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_CeoView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_CeoView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_ClassWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_ClassWithAllDataTypesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_ClassWithoutPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_ClassWithoutPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_ClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_ClassWithRelationsView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_CustomerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_ConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_ConcreteClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_DerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_DerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_DerivedOfDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_DerivedOfDerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_PartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_PartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_DevelopmentPartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_DevelopmentPartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_EmployeeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_EmployeeView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_OrderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_OrderItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SchemaGeneration_SecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SchemaGeneration_SecondDerivedClassView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('SchemaGeneration_Address', 'SchemaGeneration_Ceo', 'TableWithAllDataTypes', 'TableWithoutProperties', 'TableWithRelations', 'SchemaGeneration_Customer', 'SchemaGeneration_ConcreteClass', 'SchemaGeneration_DevelopmentPartner', 'SchemaGeneration_Employee', 'SchemaGeneration_Order', 'SchemaGeneration_OrderItem')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Address' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_Address]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Ceo' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_Ceo]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithAllDataTypes]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithoutProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithRelations]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Customer' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_Customer]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_ConcreteClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_ConcreteClass]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_DevelopmentPartner' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_DevelopmentPartner]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Employee' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_Employee]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_Order]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_OrderItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SchemaGeneration_OrderItem]
GO

-- Create all tables
CREATE TABLE [dbo].[SchemaGeneration_Address]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Address columns
  [Street] nvarchar (100) NOT NULL,
  [Zip] nvarchar (10) NOT NULL,
  [City] nvarchar (100) NOT NULL,
  [Country] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_SchemaGeneration_Address] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_Ceo]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Ceo columns
  [Name] nvarchar (100) NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_SchemaGeneration_Ceo] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithAllDataTypes]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_ClassWithAllDataTypes columns
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] datetime NOT NULL,
  [DateTime] datetime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float NOT NULL,
  [Enum] int NOT NULL,
  [ExtensibleEnum] varchar (110) NOT NULL,
  [Guid] uniqueidentifier NOT NULL,
  [Int16] smallint NOT NULL,
  [Int32] int NOT NULL,
  [Int64] bigint NOT NULL,
  [Single] real NOT NULL,
  [String] nvarchar (100) NOT NULL,
  [StringWithoutMaxLength] nvarchar (max) NOT NULL,
  [Binary] varbinary (100) NOT NULL,
  [BinaryWithoutMaxLength] varbinary (max) NOT NULL,
  [NaBoolean] bit NULL,
  [NaByte] tinyint NULL,
  [NaDate] datetime NULL,
  [NaDateTime] datetime NULL,
  [NaDecimal] decimal (38, 3) NULL,
  [NaDouble] float NULL,
  [NaEnum] int NULL,
  [NaGuid] uniqueidentifier NULL,
  [NaInt16] smallint NULL,
  [NaInt32] int NULL,
  [NaInt64] bigint NULL,
  [NaSingle] real NULL,
  [StringWithNullValue] nvarchar (100) NULL,
  [ExtensibleEnumWithNullValue] varchar (110) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaByteWithNullValue] tinyint NULL,
  [NaDateWithNullValue] datetime NULL,
  [NaDateTimeWithNullValue] datetime NULL,
  [NaDecimalWithNullValue] decimal (38, 3) NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaEnumWithNullValue] int NULL,
  [NaGuidWithNullValue] uniqueidentifier NULL,
  [NaInt16WithNullValue] smallint NULL,
  [NaInt32WithNullValue] int NULL,
  [NaInt64WithNullValue] bigint NULL,
  [NaSingleWithNullValue] real NULL,
  [NullableBinary] varbinary (100) NULL,
  [NullableBinaryWithoutMaxLength] varbinary (max) NULL,

  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithoutProperties]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_ClassWithoutProperties columns

  CONSTRAINT [PK_TableWithoutProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_ClassWithRelations columns
  [DerivedClassID] uniqueidentifier NULL,
  [DerivedClassIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_TableWithRelations] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_Customer]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Company columns
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,
  [AddressID] uniqueidentifier NULL,

  -- SchemaGeneration_Customer columns
  [CustomerType] int NOT NULL,
  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,
  [PrimaryOfficialID] varchar (255) NULL,
  [LicenseCode] nvarchar (max) NULL,

  CONSTRAINT [PK_SchemaGeneration_Customer] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_ConcreteClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_ConcreteClass columns
  [PropertyInConcreteClass] nvarchar (100) NOT NULL,

  -- SchemaGeneration_DerivedClass columns
  [PropertyInDerivedClass] nvarchar (100) NULL,
  [PersistentProperty] nvarchar (max) NULL,

  -- SchemaGeneration_DerivedOfDerivedClass columns
  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,
  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,

  -- SchemaGeneration_SecondDerivedClass columns
  [PropertyInSecondDerivedClass] nvarchar (100) NULL,
  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,

  CONSTRAINT [PK_SchemaGeneration_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_DevelopmentPartner]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Company columns
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,
  [AddressID] uniqueidentifier NULL,

  -- SchemaGeneration_Partner columns
  [Description] nvarchar (255) NOT NULL,
  [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,

  -- SchemaGeneration_DevelopmentPartner columns
  [Competences] nvarchar (255) NOT NULL,
  [LicenseCode] nvarchar (max) NULL,

  CONSTRAINT [PK_SchemaGeneration_DevelopmentPartner] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_Employee]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Employee columns
  [Name] nvarchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,

  CONSTRAINT [PK_SchemaGeneration_Employee] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_Order]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_Order columns
  [Number] int NOT NULL,
  [Priority] int NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  [OfficialID] varchar (255) NULL,

  CONSTRAINT [PK_SchemaGeneration_Order] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SchemaGeneration_OrderItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SchemaGeneration_OrderItem columns
  [Position] int NOT NULL,
  [Product] nvarchar (100) NOT NULL,
  [OrderID] uniqueidentifier NULL,

  CONSTRAINT [PK_SchemaGeneration_OrderItem] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[TableWithRelations] ADD
  CONSTRAINT [FK_TableWithRelations_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[SchemaGeneration_ConcreteClass] ([ID])

ALTER TABLE [dbo].[SchemaGeneration_Customer] ADD
  CONSTRAINT [FK_SchemaGeneration_Customer_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[SchemaGeneration_Address] ([ID])

ALTER TABLE [dbo].[SchemaGeneration_ConcreteClass] ADD
  CONSTRAINT [FK_SchemaGeneration_ConcreteClass_ClassWithRelationsInDerivedOfDerivedClassID] FOREIGN KEY ([ClassWithRelationsInDerivedOfDerivedClassID]) REFERENCES [dbo].[TableWithRelations] ([ID]),
  CONSTRAINT [FK_SchemaGeneration_ConcreteClass_ClassWithRelationsInSecondDerivedClassID] FOREIGN KEY ([ClassWithRelationsInSecondDerivedClassID]) REFERENCES [dbo].[TableWithRelations] ([ID])

ALTER TABLE [dbo].[SchemaGeneration_DevelopmentPartner] ADD
  CONSTRAINT [FK_SchemaGeneration_DevelopmentPartner_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[SchemaGeneration_Address] ([ID])

ALTER TABLE [dbo].[SchemaGeneration_Employee] ADD
  CONSTRAINT [FK_SchemaGeneration_Employee_SupervisorID] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[SchemaGeneration_Employee] ([ID])

ALTER TABLE [dbo].[SchemaGeneration_Order] ADD
  CONSTRAINT [FK_SchemaGeneration_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[SchemaGeneration_Customer] ([ID])

ALTER TABLE [dbo].[SchemaGeneration_OrderItem] ADD
  CONSTRAINT [FK_SchemaGeneration_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[SchemaGeneration_Order] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[SchemaGeneration_CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], null, null, null
    FROM [dbo].[SchemaGeneration_Customer]
    WHERE [ClassID] IN ('SchemaGeneration_Customer', 'SchemaGeneration_DevelopmentPartner')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], null, null, null, [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]
    FROM [dbo].[SchemaGeneration_DevelopmentPartner]
    WHERE [ClassID] IN ('SchemaGeneration_Customer', 'SchemaGeneration_DevelopmentPartner')
GO

CREATE VIEW [dbo].[SchemaGeneration_AddressView] ([ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country]
    FROM [dbo].[SchemaGeneration_Address]
    WHERE [ClassID] IN ('SchemaGeneration_Address')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_CeoView] ([ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID]
    FROM [dbo].[SchemaGeneration_Ceo]
    WHERE [ClassID] IN ('SchemaGeneration_Ceo')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_ClassWithAllDataTypesView] ([ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [BinaryWithoutMaxLength], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary], [NullableBinaryWithoutMaxLength])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [BinaryWithoutMaxLength], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary], [NullableBinaryWithoutMaxLength]
    FROM [dbo].[TableWithAllDataTypes]
    WHERE [ClassID] IN ('SchemaGeneration_ClassWithAllDataTypes')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_ClassWithoutPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[TableWithoutProperties]
    WHERE [ClassID] IN ('SchemaGeneration_ClassWithoutProperties')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_ClassWithRelationsView] ([ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID]
    FROM [dbo].[TableWithRelations]
    WHERE [ClassID] IN ('SchemaGeneration_ClassWithRelations')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_CustomerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode]
    FROM [dbo].[SchemaGeneration_Customer]
    WHERE [ClassID] IN ('SchemaGeneration_Customer')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]
    FROM [dbo].[SchemaGeneration_ConcreteClass]
    WHERE [ClassID] IN ('SchemaGeneration_ConcreteClass', 'SchemaGeneration_DerivedClass', 'SchemaGeneration_DerivedOfDerivedClass', 'SchemaGeneration_SecondDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID]
    FROM [dbo].[SchemaGeneration_ConcreteClass]
    WHERE [ClassID] IN ('SchemaGeneration_DerivedClass', 'SchemaGeneration_DerivedOfDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_DerivedOfDerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID]
    FROM [dbo].[SchemaGeneration_ConcreteClass]
    WHERE [ClassID] IN ('SchemaGeneration_DerivedOfDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_PartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode]
    FROM [dbo].[SchemaGeneration_DevelopmentPartner]
    WHERE [ClassID] IN ('SchemaGeneration_DevelopmentPartner')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_DevelopmentPartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode]
    FROM [dbo].[SchemaGeneration_DevelopmentPartner]
    WHERE [ClassID] IN ('SchemaGeneration_DevelopmentPartner')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_EmployeeView] ([ID], [ClassID], [Timestamp], [Name], [SupervisorID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID]
    FROM [dbo].[SchemaGeneration_Employee]
    WHERE [ClassID] IN ('SchemaGeneration_Employee')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]
    FROM [dbo].[SchemaGeneration_Order]
    WHERE [ClassID] IN ('SchemaGeneration_Order')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_OrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID]
    FROM [dbo].[SchemaGeneration_OrderItem]
    WHERE [ClassID] IN ('SchemaGeneration_OrderItem')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SchemaGeneration_SecondDerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PersistentProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PersistentProperty]
    FROM [dbo].[SchemaGeneration_ConcreteClass]
    WHERE [ClassID] IN ('SchemaGeneration_SecondDerivedClass')
  WITH CHECK OPTION
GO
