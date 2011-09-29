USE SchemaGenerationTestDomain1
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
CREATE TABLE [dbo].[AbstractClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PropertyInAbstractClass] nvarchar (max) NULL,
  [PropertyInAbstractDerivedClass] nvarchar (max) NULL,
  [PropertyInDerivedConcreteClass] nvarchar (max) NULL,
  CONSTRAINT [PK_AbstractClass] PRIMARY KEY CLUSTERED ([ID])
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
CREATE TABLE [dbo].[FirstClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [SecondClassID] uniqueidentifier NULL,
  [ThirdClassID] uniqueidentifier NULL,
  CONSTRAINT [PK_FirstClass] PRIMARY KEY CLUSTERED ([ID])
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
CREATE TABLE [dbo].[SecondClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_SecondClass] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[SiblingOfTableWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [IntProperty] int NOT NULL,
  CONSTRAINT [PK_SiblingOfTableWithRelations] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[ThirdClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  CONSTRAINT [PK_ThirdClass] PRIMARY KEY CLUSTERED ([ID])
)
-- Create foreign key constraints for tables that were created above
ALTER TABLE [dbo].[TableWithRelations] ADD
  CONSTRAINT [FK_TableWithRelations_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])
ALTER TABLE [dbo].[Customer] ADD
  CONSTRAINT [FK_Customer_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])
ALTER TABLE [dbo].[ConcreteClass] ADD
  CONSTRAINT [FK_ConcreteClass_ClassWithRelationsInDerivedOfDerivedClassID] FOREIGN KEY ([ClassWithRelationsInDerivedOfDerivedClassID]) REFERENCES [dbo].[TableWithRelations] ([ID])
ALTER TABLE [dbo].[ConcreteClass] ADD
  CONSTRAINT [FK_ConcreteClass_ClassWithRelationsInSecondDerivedClassID] FOREIGN KEY ([ClassWithRelationsInSecondDerivedClassID]) REFERENCES [dbo].[TableWithRelations] ([ID])
ALTER TABLE [dbo].[DevelopmentPartner] ADD
  CONSTRAINT [FK_DevelopmentPartner_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])
ALTER TABLE [dbo].[Employee] ADD
  CONSTRAINT [FK_Employee_SupervisorID] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[Employee] ([ID])
ALTER TABLE [dbo].[FirstClass] ADD
  CONSTRAINT [FK_FirstClass_SecondClassID] FOREIGN KEY ([SecondClassID]) REFERENCES [dbo].[SecondClass] ([ID])
ALTER TABLE [dbo].[FirstClass] ADD
  CONSTRAINT [FK_FirstClass_ThirdClassID] FOREIGN KEY ([ThirdClassID]) REFERENCES [dbo].[ThirdClass] ([ID])
ALTER TABLE [dbo].[Order] ADD
  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])
ALTER TABLE [dbo].[OrderItem] ADD
  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])
-- Create a view for every class
GO
CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], NULL, NULL, NULL
    FROM [dbo].[Customer]
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], NULL, NULL, NULL, [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]
    FROM [dbo].[DevelopmentPartner]
GO
CREATE VIEW [dbo].[AbstractWithoutConcreteClassView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID])
  AS
  SELECT CONVERT(uniqueidentifier,NULL) AS [ID], CONVERT(varchar (100),NULL) AS [ClassID], CONVERT(rowversion,NULL) AS [Timestamp], CONVERT(nvarchar (100),NULL) AS [Name], CONVERT(nvarchar (100),NULL) AS [PhoneNumber], CONVERT(uniqueidentifier,NULL) AS [AddressID]
    WHERE 1 = 0
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
CREATE VIEW [dbo].[AbstractClassView] ([ID], [ClassID], [Timestamp], [PropertyInAbstractClass], [PropertyInAbstractDerivedClass], [PropertyInDerivedConcreteClass])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInAbstractClass], [PropertyInAbstractDerivedClass], [PropertyInDerivedConcreteClass]
    FROM [dbo].[AbstractClass]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedAbstractClassView] ([ID], [ClassID], [Timestamp], [PropertyInAbstractClass], [PropertyInAbstractDerivedClass], [PropertyInDerivedConcreteClass])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInAbstractClass], [PropertyInAbstractDerivedClass], [PropertyInDerivedConcreteClass]
    FROM [dbo].[AbstractClass]
    WHERE [ClassID] IN ('DerivedAbstractClass', 'DerivedDerivedConcreteClass')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[DerivedDerivedConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInAbstractClass], [PropertyInAbstractDerivedClass], [PropertyInDerivedConcreteClass])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInAbstractClass], [PropertyInAbstractDerivedClass], [PropertyInDerivedConcreteClass]
    FROM [dbo].[AbstractClass]
    WHERE [ClassID] IN ('DerivedDerivedConcreteClass')
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
CREATE VIEW [dbo].[FirstClassView] ([ID], [ClassID], [Timestamp], [SecondClassID], [ThirdClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [SecondClassID], [ThirdClassID]
    FROM [dbo].[FirstClass]
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
CREATE VIEW [dbo].[SecondClassView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[SecondClass]
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
CREATE VIEW [dbo].[ThirdClassView] ([ID], [ClassID], [Timestamp])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp]
    FROM [dbo].[ThirdClass]
  WITH CHECK OPTION
GO
-- Create indexes for tables that were created above
-- Create synonyms for tables that were created above
