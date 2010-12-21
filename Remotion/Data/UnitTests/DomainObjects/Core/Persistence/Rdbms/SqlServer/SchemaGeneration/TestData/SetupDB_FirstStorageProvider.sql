USE TestDomain
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CompanyView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AddressView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AddressView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CeoView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CeoView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithAllDataTypesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithoutPropertiesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithoutPropertiesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelationsBaseView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelationsBaseView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelationsView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CustomerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedOfDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedOfDerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DevelopmentPartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DevelopmentPartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmployeeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EmployeeView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecondDerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SiblingOfClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SiblingOfClassWithRelationsView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Address', 'Ceo', 'TableWithAllDataTypes', 'TableWithoutProperties', 'TableWithRelations', 'Customer', 'ConcreteClass', 'DevelopmentPartner', 'Employee', 'Order', 'OrderItem', 'SiblingOfTableWithRelations')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Address' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Address]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Ceo]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithAllDataTypes]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutProperties' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithoutProperties]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithRelations]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Customer]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteClass]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DevelopmentPartner' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[DevelopmentPartner]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Employee]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Order]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OrderItem]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SiblingOfTableWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SiblingOfTableWithRelations]
GO

-- Create all tables
CREATE TABLE [dbo].[Address]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Street] nvarchar (100) NOT NULL,
  [Zip] nvarchar (10) NOT NULL,
  [City] nvarchar (100) NOT NULL,
  [Country] nvarchar (100) NOT NULL,
  CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Ceo]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithAllDataTypes]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] datetime NOT NULL,
  [DateTime] datetime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float NOT NULL,
  [Enum] int NOT NULL,
  [ExtensibleEnum] varchar (109) NOT NULL,
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
  [ExtensibleEnumWithNullValue] varchar (109) NULL,
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
  CONSTRAINT [PK_TableWithoutProperties] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[TableWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [DerivedClassID] uniqueidentifier NULL,
  [DerivedClassIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_TableWithRelations] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Customer]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,
  [AddressID] uniqueidentifier NULL,
  [CustomerType] int NOT NULL,
  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,
  [PrimaryOfficialID] varchar (255) NULL,
  [LicenseCode] nvarchar (max) NULL,
  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[ConcreteClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PropertyInConcreteClass] nvarchar (100) NOT NULL,
  [PropertyInDerivedClass] nvarchar (100) NULL,
  [PersistentProperty] nvarchar (max) NULL,
  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,
  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,
  [ClassWithRelationsInDerivedOfDerivedClassIDClassID] varchar (100) NULL,
  [PropertyInSecondDerivedClass] nvarchar (100) NULL,
  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,
  [ClassWithRelationsInSecondDerivedClassIDClassID] varchar (100) NULL,
  CONSTRAINT [PK_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[DevelopmentPartner]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,
  [AddressID] uniqueidentifier NULL,
  [Description] nvarchar (255) NOT NULL,
  [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,
  [Competences] nvarchar (255) NOT NULL,
  [LicenseCode] nvarchar (max) NULL,
  CONSTRAINT [PK_DevelopmentPartner] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Employee]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,
  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Order]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Number] int NOT NULL,
  [Priority] int NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  [OfficialID] varchar (255) NULL,
  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[OrderItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Position] int NOT NULL,
  [Product] nvarchar (100) NOT NULL,
  [OrderID] uniqueidentifier NULL,
  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SiblingOfTableWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [IntProperty] int NOT NULL,
  CONSTRAINT [PK_SiblingOfTableWithRelations] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[TableWithRelations] ADD
  CONSTRAINT [FK_TableWithRelations_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])

ALTER TABLE [dbo].[Customer] ADD
  CONSTRAINT [FK_Customer_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])

ALTER TABLE [dbo].[ConcreteClass] ADD
  CONSTRAINT [FK_ConcreteClass_ClassWithRelationsInDerivedOfDerivedClassID] FOREIGN KEY ([ClassWithRelationsInDerivedOfDerivedClassID]) REFERENCES [dbo].[TableWithRelations] ([ID]),
  CONSTRAINT [FK_ConcreteClass_ClassWithRelationsInSecondDerivedClassID] FOREIGN KEY ([ClassWithRelationsInSecondDerivedClassID]) REFERENCES [dbo].[TableWithRelations] ([ID])

ALTER TABLE [dbo].[DevelopmentPartner] ADD
  CONSTRAINT [FK_DevelopmentPartner_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])

ALTER TABLE [dbo].[Employee] ADD
  CONSTRAINT [FK_Employee_SupervisorID] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[Employee] ([ID])

ALTER TABLE [dbo].[Order] ADD
  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])

ALTER TABLE [dbo].[OrderItem] ADD
  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], NULL, NULL, NULL
    FROM [dbo].[Customer]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], NULL, NULL, NULL, [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]
    FROM [dbo].[DevelopmentPartner]
GO

CREATE VIEW [dbo].[AddressView] ([ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country]
    FROM [dbo].[Address]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CeoView] ([ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID]
    FROM [dbo].[Ceo]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithAllDataTypesView] ([ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [BinaryWithoutMaxLength], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary], [NullableBinaryWithoutMaxLength])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [ExtensibleEnum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [StringWithoutMaxLength], [Binary], [BinaryWithoutMaxLength], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaEnum], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [ExtensibleEnumWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaEnumWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary], [NullableBinaryWithoutMaxLength]
    FROM [dbo].[TableWithAllDataTypes]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithoutPropertiesView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[TableWithoutProperties]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithRelationsBaseView] ([ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID], [IntProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID], NULL
    FROM [dbo].[TableWithRelations]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], NULL, NULL, [IntProperty]
    FROM [dbo].[SiblingOfTableWithRelations]
GO

CREATE VIEW [dbo].[ClassWithRelationsView] ([ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID]
    FROM [dbo].[TableWithRelations]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode]
    FROM [dbo].[Customer]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID]
    FROM [dbo].[ConcreteClass]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('DerivedClass', 'DerivedOfDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DerivedOfDerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('DerivedOfDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode]
    FROM [dbo].[DevelopmentPartner]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DevelopmentPartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode]
    FROM [dbo].[DevelopmentPartner]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[EmployeeView] ([ID], [ClassID], [Timestamp], [Name], [SupervisorID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID]
    FROM [dbo].[Employee]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]
    FROM [dbo].[Order]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID]
    FROM [dbo].[OrderItem]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SecondDerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID], [PersistentProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID], [PersistentProperty]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('SecondDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SiblingOfClassWithRelationsView] ([ID], [ClassID], [Timestamp], [IntProperty])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [IntProperty]
    FROM [dbo].[SiblingOfTableWithRelations]
  WITH CHECK OPTION
GO
