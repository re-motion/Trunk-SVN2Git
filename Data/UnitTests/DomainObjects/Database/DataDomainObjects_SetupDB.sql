USE TestDomain
GO

-- Drop all views that will be created below

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_AddressView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_AddressView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_ClientView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_ClientView]
  
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_DomainBaseView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_DomainBaseView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_PersonView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_PersonView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_CustomerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_FileSystemItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_FileSystemItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_FileView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_FileView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_HistoryEntryView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_HistoryEntryView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_OrderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_OrganizationalUnitView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_OrganizationalUnitView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_RegionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_RegionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_FolderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_FolderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TI_SpecificFolderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TI_SpecificFolderView]
  
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CeoView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CeoView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClientView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClientView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ComputerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ComputerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CompanyView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CustomerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DistributorView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DistributorView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmployeeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EmployeeView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileSystemItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FileSystemItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FileView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FolderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FolderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'IndustrialSectorView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[IndustrialSectorView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'LocationView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[LocationView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemWithNewPropertyAccessView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemWithNewPropertyAccessView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderTicketView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderTicketView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PersonView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PersonView]
  
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SupplierView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SupplierView]
GO


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_Target')
BEGIN
  ALTER TABLE [MixedDomains_Target] DROP CONSTRAINT [FK_MixedDomains_Target_RelationProperty]
  ALTER TABLE [MixedDomains_Target] DROP CONSTRAINT [FK_MixedDomains_Target_CollectionPropertyNSide]
  ALTER TABLE [MixedDomains_Target] DROP CONSTRAINT [FK_MixedDomains_Target_UnidirectionalRelationProperty]
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_RelationTarget')
BEGIN
  ALTER TABLE [MixedDomains_RelationTarget] DROP CONSTRAINT [FK_MixedDomains_RelationTarget_RelationProperty2]
  ALTER TABLE [MixedDomains_RelationTarget] DROP CONSTRAINT [FK_MixedDomains_RelationTarget_RelationProperty3ID]
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_Target') 
  DROP TABLE [MixedDomains_Target]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_RelationTarget') 
  DROP TABLE [MixedDomains_RelationTarget]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetWithTwoUnidirectionalMixins') 
  DROP TABLE [MixedDomains_TargetWithTwoUnidirectionalMixins]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetWithUnidirectionalMixin1') 
  DROP TABLE [MixedDomains_TargetWithUnidirectionalMixin1]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'MixedDomains_TargetWithUnidirectionalMixin2') 
  DROP TABLE [MixedDomains_TargetWithUnidirectionalMixin2]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Location') 
  DROP TABLE [Location]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client') 
  DROP TABLE [Client]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Computer') 
  DROP TABLE [Computer]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee') 
  DROP TABLE [Employee]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes') 
  DROP TABLE [TableWithAllDataTypes]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutRelatedClassIDColumnAndDerivation') 
  DROP TABLE [TableWithoutRelatedClassIDColumnAndDerivation]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithOptionalOneToOneRelationAndOppositeDerivedClass') 
  DROP TABLE [TableWithOptionalOneToOneRelationAndOppositeDerivedClass]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutRelatedClassIDColumn') 
DROP TABLE [TableWithoutRelatedClassIDColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo') 
  DROP TABLE [Ceo]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderTicket') 
  DROP TABLE [OrderTicket]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem') 
  DROP TABLE [OrderItem]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order') 
  DROP TABLE [Order]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItemWithNewPropertyAccess') 
  DROP TABLE [OrderItemWithNewPropertyAccess]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderWithNewPropertyAccess') 
  DROP TABLE [OrderWithNewPropertyAccess]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Company') 
  DROP TABLE [Company]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'IndustrialSector') 
  DROP TABLE [IndustrialSector]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Person') 
  DROP TABLE [Person]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FileSystemItem')
  DROP TABLE [FileSystemItem]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithValidRelations') 
  DROP TABLE [TableWithValidRelations]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithInvalidRelation') 
  DROP TABLE [TableWithInvalidRelation]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithRelatedClassIDColumnAndNoInheritance') 
  DROP TABLE [TableWithRelatedClassIDColumnAndNoInheritance]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithGuidKey') 
  DROP TABLE [TableWithGuidKey]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithKeyOfInvalidType') 
  DROP TABLE [TableWithKeyOfInvalidType]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutIDColumn') 
  DROP TABLE [TableWithoutIDColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutClassIDColumn') 
  DROP TABLE [TableWithoutClassIDColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutTimestampColumn') 
  DROP TABLE [TableWithoutTimestampColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Order') 
  DROP TABLE [TableInheritance_Order]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Address') 
  DROP TABLE [TableInheritance_Address]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_HistoryEntry') 
  DROP TABLE [TableInheritance_HistoryEntry]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Person') 
  DROP TABLE [TableInheritance_Person]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Region') 
  DROP TABLE [TableInheritance_Region]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_OrganizationalUnit') 
  DROP TABLE [TableInheritance_OrganizationalUnit]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_DerivedClassWithEntityWithHierarchy') 
  DROP TABLE [TableInheritance_DerivedClassWithEntityWithHierarchy]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Client') 
  DROP TABLE [TableInheritance_Client]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_TableWithUnidirectionalRelation') 
  DROP TABLE [TableInheritance_TableWithUnidirectionalRelation]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_BaseClassWithInvalidRelationClassIDColumns') 
  DROP TABLE [TableInheritance_BaseClassWithInvalidRelationClassIDColumns]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_File') 
  DROP TABLE [TableInheritance_File]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Folder') 
  DROP TABLE [TableInheritance_Folder]
GO

IF OBJECT_ID ('rpf_testSPQuery', 'P') IS NOT NULL 
  DROP PROCEDURE rpf_testSPQuery;
GO

IF OBJECT_ID ('rpf_testSPQueryWithParameter', 'P') IS NOT NULL 
  DROP PROCEDURE rpf_testSPQueryWithParameter;
GO

CREATE TABLE [Employee] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Supervisor_Employee] FOREIGN KEY ([SupervisorID]) REFERENCES [Employee] ([ID]),
) 
GO

CREATE TABLE [Computer] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [SerialNumber] varchar (20) NOT NULL,
  [EmployeeID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Computer] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Computer_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [Employee] ([ID]),
) 
GO

CREATE TABLE [Person] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [IndustrialSector] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_IndustrialSector] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [Company] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- Company columns
  [Name] varchar (100) NULL,
  [IndustrialSectorID] uniqueidentifier NULL,
  
  -- Customer columns
  CustomerSince datetime NULL,
  CustomerType int NULL,
  
  -- Partner columns
  ContactPersonID uniqueidentifier NULL,
  
  -- Supplier columns
  SupplierQuality int NULL, 
  
  -- Distributor columns
  NumberOfShops int NULL
  
  CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Partner_Person] FOREIGN KEY ([ContactPersonID]) REFERENCES [Person] ([ID]),
  CONSTRAINT [FK_Company_IndustrialSector] FOREIGN KEY ([IndustrialSectorID]) REFERENCES [IndustrialSector] ([ID])
) 
GO

CREATE TABLE [Order] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderNo] int NOT NULL,
  [DeliveryDate] datetime NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  [OfficialID] varchar (255) NULL,
  
  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Order_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [OrderItem] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderID] uniqueidentifier NULL,
  [Position] int NOT NULL,
  [Product] varchar (100) NOT NULL DEFAULT (''),
  
  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID]),
  
  -- A foreign key cannot be part of a unique constraint:
  -- CONSTRAINT [UN_OrderItem_Position] UNIQUE ([OrderID], [Position]),
  
  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])
) 
GO

CREATE TABLE [OrderWithNewPropertyAccess] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderNo] int NOT NULL,
  [DeliveryDate] datetime NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_OrderWithNewPropertyAccess] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_OrderWithNewPropertyAccess_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [OrderItemWithNewPropertyAccess] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderID] uniqueidentifier NULL,
  [Position] int NOT NULL,
  [Product] varchar (100) NOT NULL DEFAULT (''),
  
  CONSTRAINT [PK_OrderItemWithNewPropertyAccess] PRIMARY KEY CLUSTERED ([ID]),
  
  -- A foreign key cannot be part of a unique constraint:
  -- CONSTRAINT [UN_OrderItem_Position] UNIQUE ([OrderID], [Position]),
  
  CONSTRAINT [FK_OrderItemWithNewPropertyAccess_OrderWithNewPropertyAccess] FOREIGN KEY ([OrderID]) REFERENCES [OrderWithNewPropertyAccess] ([ID])
) 
GO

CREATE TABLE [OrderTicket] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [FileName] nvarchar (255) NOT NULL,
  [OrderID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_OrderTicket] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_OrderTicket_Order] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])
) 
GO

CREATE TABLE [Ceo] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] nvarchar (100) NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Ceo_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [Client] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ParentClientID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_ParentClient_ChildClient] FOREIGN KEY ([ParentClientID]) REFERENCES [Client] ([ID])
) 
GO

CREATE TABLE [Location] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ClientID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Client_Location] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
) 
GO

CREATE TABLE [TableWithoutRelatedClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [DistributorID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumn] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumn_Distributor] FOREIGN KEY ([DistributorID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [FileSystemItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- FileSystemItem columns
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,

  -- Folder columns

  -- File columns

  CONSTRAINT [PK_FileSystemItem] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_FolderToFileSystemItem] FOREIGN KEY ([ParentFolderID]) REFERENCES [FileSystemItem] ([ID])
)
GO

CREATE TABLE [TableWithoutRelatedClassIDColumnAndDerivation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [CompanyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumnAndDerivation] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumnAndDerivation_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [TableWithOptionalOneToOneRelationAndOppositeDerivedClass] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
 
  CONSTRAINT [PK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
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
  [StringWithoutMaxLength] nvarchar (max) NOT NULL,
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
      
  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithGuidKey] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithGuidKey] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithKeyOfInvalidType] (
  [ID] datetime NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithKeyOfInvalidType] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithoutIDColumn] (
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL
) 
GO

CREATE TABLE [TableWithoutClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithoutClassIDColumn] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithoutTimestampColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableWithoutTimestampColumn] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithValidRelations] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyOptionalID] uniqueidentifier NULL,
  [TableWithGuidKeyNonOptionalID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithValidRelations] PRIMARY KEY CLUSTERED ([ID]),
  
  CONSTRAINT [FK_TableWithGuidKey_TableWithValidRelations_Optional] 
      FOREIGN KEY ([TableWithGuidKeyOptionalID]) REFERENCES [TableWithGuidKey] ([ID]),
      
  CONSTRAINT [FK_TableWithGuidKey_TableWithValidRelations_NonOptional] 
      FOREIGN KEY ([TableWithGuidKeyNonOptionalID]) REFERENCES [TableWithGuidKey] ([ID])      
) 
GO

CREATE TABLE [TableWithInvalidRelation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithInvalidRelation] PRIMARY KEY CLUSTERED ([ID])
  -- Note the lack of a foreign key referring to TableWithGuidKey
) 
GO

CREATE TABLE [TableWithRelatedClassIDColumnAndNoInheritance] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyID] uniqueidentifier NULL,
  [TableWithGuidKeyIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableWithRelatedClassIDColumnAndNoInheritance] PRIMARY KEY CLUSTERED ([ID]),
  
  CONSTRAINT [FK_TableWithGuidKey_TableWithRelatedClassIDColumnAndNoInheritance] 
      FOREIGN KEY ([TableWithGuidKeyID]) REFERENCES [TableWithGuidKey] ([ID])
) 
GO

CREATE TABLE [TableInheritance_Client] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableInheritance_Client] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_TableWithUnidirectionalRelation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [DomainBaseID] uniqueidentifier NULL,
  [DomainBaseIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_TableInheritance_TableWithUnidirectionalRelation] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_OrganizationalUnit] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- DomainBase columns
  [CreatedBy] varchar (100) NOT NULL,
  [CreatedAt] datetime NOT NULL,
  [ClientID] uniqueidentifier NULL,
  
  -- OrganizationalUnit columns
  [Name] varchar (100) NOT NULL,
    
  CONSTRAINT [PK_TableInheritance_OrganizationalUnit] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_OrganizationalUnit] FOREIGN KEY ([ClientID]) REFERENCES [TableInheritance_Client] ([ID])
) 
GO

CREATE TABLE [TableInheritance_Region] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableInheritance_Region] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_Person] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- DomainBase columns
  [CreatedBy] varchar (100) NOT NULL,
  [CreatedAt] datetime NOT NULL,
  [ClientID] uniqueidentifier NULL,
  
  -- Person columns
  [FirstName] varchar (100) NOT NULL,
  [LastName] varchar (100) NOT NULL,
  [DateOfBirth] datetime NOT NULL,
  [Photo] image NULL,   

  -- Customer columns
  [CustomerType] int NULL,
  [CustomerSince] datetime NULL,
  [RegionID] uniqueidentifier NULL,
    
  CONSTRAINT [PK_TableInheritance_Person] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_Person] FOREIGN KEY ([ClientID]) REFERENCES [TableInheritance_Client] ([ID]),
  CONSTRAINT [FK_TableInheritance_Region_TableInheritance_Customer] FOREIGN KEY ([RegionID]) REFERENCES [TableInheritance_Region] ([ID])
) 
GO

CREATE TABLE [TableInheritance_HistoryEntry] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- Person columns
  [HistoryDate] datetime NOT NULL,
  [Text] varchar (250) NOT NULL,
  [OwnerID] uniqueidentifier NULL, -- Note: OwnerID has no FK, because it refers to multiple tables (concrete table inheritance).
  [OwnerIDClassID] varchar (100) NULL,
   
  CONSTRAINT [PK_TableInheritance_HistoryEntry] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_Address] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Street] nvarchar (100) NOT NULL,
  [Zip] nvarchar (10) NOT NULL,
  [City] nvarchar (100) NOT NULL,
  [Country] nvarchar (100) NOT NULL,
  [PersonID] uniqueidentifier NULL,
  [PersonIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_Address] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Person_TableInheritance_Address] FOREIGN KEY ([PersonID]) REFERENCES [TableInheritance_Person] ([ID])  
) 
GO

CREATE TABLE [TableInheritance_Order] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Number] int NOT NULL,
  [OrderDate] datetime NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_Order] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Customer_TableInheritance_Order] FOREIGN KEY ([CustomerID]) REFERENCES [TableInheritance_Person] ([ID])  
) 
GO

CREATE TABLE [TableInheritance_Folder] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  [FolderCreatedAt] datetime NOT NULL,
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_Folder] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Folder_TableInheritance_FileSystemItem] FOREIGN KEY ([ParentFolderID]) REFERENCES [TableInheritance_Folder] ([ID])  
) 
GO

CREATE TABLE [TableInheritance_File] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  [Size] int NOT NULL,
  [FileCreatedAt] datetime NOT NULL,
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_File] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Folder_TableInheritance_File] FOREIGN KEY ([ParentFolderID]) REFERENCES [TableInheritance_Folder] ([ID])  
) 
GO

CREATE TABLE [TableInheritance_DerivedClassWithEntityWithHierarchy] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  [ParentAbstractBaseClassWithHierarchyID] uniqueidentifier NULL,
  [ParentAbstractBaseClassWithHierarchyIDClassID] varchar (100) NULL,
  [ParentDerivedClassWithEntityWithHierarchyID] uniqueidentifier NULL,
  [ParentDerivedClassWithEntityWithHierarchyIDClassID] varchar (100) NULL,
  [ParentDerivedClassWithEntityFromBaseClassWithHierarchyID] uniqueidentifier NULL,
  [ParentDerivedClassWithEntityFromBaseClassWithHierarchyIDClassID] varchar (100) NULL,
  
  [ClientFromAbstractBaseClassID] uniqueidentifier NULL,
  [ClientFromDerivedClassWithEntityID] uniqueidentifier NULL,
  [ClientFromDerivedClassWithEntityFromBaseClassID] uniqueidentifier NULL,

  [FileSystemItemFromAbstractBaseClassID] uniqueidentifier NULL,
  [FileSystemItemFromAbstractBaseClassIDClassID] varchar (100) NULL,
  [FileSystemItemFromDerivedClassWithEntityID] uniqueidentifier NULL,
  [FileSystemItemFromDerivedClassWithEntityIDClassID] varchar (100) NULL,
  [FileSystemItemFromDerivedClassWithEntityFromBaseClassID] uniqueidentifier NULL,
  [FileSystemItemFromDerivedClassWithEntityFromBaseClassIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_DerivedClassWithEntityWithHierarchy] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_Hierarchy1] FOREIGN KEY ([ParentAbstractBaseClassWithHierarchyID]) 
      REFERENCES [TableInheritance_DerivedClassWithEntityWithHierarchy] ([ID]),
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_Hierarchy2] FOREIGN KEY ([ParentDerivedClassWithEntityWithHierarchyID]) 
      REFERENCES [TableInheritance_DerivedClassWithEntityWithHierarchy] ([ID]),
  CONSTRAINT [FK_TableInheritance_DerivedClassWithEntityWithHierarchy_Hierarchy3] FOREIGN KEY ([ParentDerivedClassWithEntityFromBaseClassWithHierarchyID]) 
      REFERENCES [TableInheritance_DerivedClassWithEntityWithHierarchy] ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_DerivedClassWithEntityWithHierarchy_1] FOREIGN KEY ([ClientFromAbstractBaseClassID]) 
      REFERENCES [TableInheritance_Client] ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_DerivedClassWithEntityWithHierarchy_2] FOREIGN KEY ([ClientFromDerivedClassWithEntityID]) 
      REFERENCES [TableInheritance_Client] ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_DerivedClassWithEntityWithHierarchy_3] FOREIGN KEY ([ClientFromDerivedClassWithEntityFromBaseClassID]) 
      REFERENCES [TableInheritance_Client] ([ID])
) 
GO

CREATE TABLE [MixedDomains_Target] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [PersistentProperty] int NOT NULL,
  [ExtraPersistentProperty] int NOT NULL,
  [RelationPropertyID] uniqueidentifier NULL,
  [CollectionPropertyNSideID] uniqueidentifier NULL,
  [UnidirectionalRelationPropertyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_MixedDomains_Target] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [MixedDomains_RelationTarget] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [RelationProperty2ID] uniqueidentifier NULL,
  [RelationProperty3ID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_MixedDomains_RelationTarget] PRIMARY KEY CLUSTERED ([ID])
) 
GO

ALTER TABLE [MixedDomains_Target] ADD CONSTRAINT [FK_MixedDomains_Target_RelationProperty] FOREIGN KEY ([RelationPropertyID]) REFERENCES [MixedDomains_RelationTarget] ([ID])
ALTER TABLE [MixedDomains_Target] ADD CONSTRAINT [FK_MixedDomains_Target_CollectionPropertyNSide] FOREIGN KEY ([CollectionPropertyNSideID]) REFERENCES [MixedDomains_RelationTarget] ([ID])
ALTER TABLE [MixedDomains_Target] ADD CONSTRAINT [FK_MixedDomains_Target_UnidirectionalRelationProperty] FOREIGN KEY ([UnidirectionalRelationPropertyID]) REFERENCES [MixedDomains_RelationTarget] ([ID])
GO

ALTER TABLE [MixedDomains_RelationTarget] ADD CONSTRAINT [FK_MixedDomains_RelationTarget_RelationProperty2] FOREIGN KEY ([RelationProperty2ID]) REFERENCES [MixedDomains_Target] ([ID])
ALTER TABLE [MixedDomains_RelationTarget] ADD CONSTRAINT [FK_MixedDomains_RelationTarget_RelationProperty3ID] FOREIGN KEY ([RelationProperty3ID]) REFERENCES [MixedDomains_Target] ([ID])
GO

CREATE TABLE [MixedDomains_TargetWithTwoUnidirectionalMixins] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ComputerID] uniqueidentifier NULL,
  [Computer2ID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_MixedDomains_TargetWithTwoUnidirectionalMixins] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_MixedDomains_TargetWithTwoUnidirectionalMixins_Computer] FOREIGN KEY ([ComputerID]) REFERENCES [Computer] ([ID]),
  CONSTRAINT [FK_MixedDomains_TargetWithTwoUnidirectionalMixins_Computer2] FOREIGN KEY ([Computer2ID]) REFERENCES [Computer] ([ID])
) 
GO

CREATE TABLE [MixedDomains_TargetWithUnidirectionalMixin1] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ComputerID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_MixedDomains_TargetWithUnidirectionalMixin1] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_MixedDomains_TargetWithUnidirectionalMixin1_Computer] FOREIGN KEY ([ComputerID]) REFERENCES [Computer] ([ID])
) 
GO

CREATE TABLE [MixedDomains_TargetWithUnidirectionalMixin2] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ComputerID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_MixedDomains_TargetWithUnidirectionalMixin2] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_MixedDomains_TargetWithUnidirectionalMixin2_Computer] FOREIGN KEY ([ComputerID]) REFERENCES [Computer] ([ID])
) 
GO

CREATE PROCEDURE rpf_testSPQuery
AS
  SELECT * FROM [Order] WHERE [OrderNo] = 1 OR [OrderNo] = 3 ORDER BY [OrderNo] ASC
GO

CREATE PROCEDURE rpf_testSPQueryWithParameter
  @customerID uniqueidentifier
AS
  SELECT * FROM [Order] WHERE [CustomerID] = @customerID ORDER BY [OrderNo] ASC
GO



-- Tables with invalid database structure for exception testing only

CREATE TABLE [TableInheritance_BaseClassWithInvalidRelationClassIDColumns] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ClientID] uniqueidentifier NULL, -- Note: To conform to mapping column ClientIDClassID must not be defined.
  [ClientIDClassID] varchar (100) NULL, 
  [DomainBaseID] uniqueidentifier NULL, -- Note: To conform to mapping column DomainBaseIDClassID must be defined.
  
  [DomainBaseWithInvalidClassIDValueID] uniqueidentifier NULL, 
  [DomainBaseWithInvalidClassIDValueIDClassID] varchar (100) NULL, 
  [DomainBaseWithInvalidClassIDNullValueID] uniqueidentifier NULL, 
  [DomainBaseWithInvalidClassIDNullValueIDClassID] varchar (100) NULL, 
  
  -- Note: This table does not need to have foreign keys, because rows cannot be read because of invalid ClassID column structure
  CONSTRAINT [PK_TableInheritance_BaseClassWithInvalidRelationClassIDColumns] PRIMARY KEY CLUSTERED ([ID])
) 
GO

-- Create a view for every class

CREATE VIEW [dbo].[TI_AddressView] ([ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country], [PersonID], [PersonIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Street], [Zip], [City], [Country], [PersonID], [PersonIDClassID]
    FROM [dbo].[TableInheritance_Address]
    WHERE [ClassID] IN ('TI_Address')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_ClientView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[TableInheritance_Client]
    WHERE [ClassID] IN ('TI_Client')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_DomainBaseView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID], null
    FROM [dbo].[TableInheritance_Person]
    WHERE [ClassID] IN ('TI_Person', 'TI_Customer', 'TI_OrganizationalUnit')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], null, null, null, null, null, null, null, [Name]
    FROM [dbo].[TableInheritance_OrganizationalUnit]
    WHERE [ClassID] IN ('TI_Person', 'TI_Customer', 'TI_OrganizationalUnit')
GO

CREATE VIEW [dbo].[TI_PersonView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID]
    FROM [dbo].[TableInheritance_Person]
    WHERE [ClassID] IN ('TI_Person', 'TI_Customer')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_CustomerView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], [CustomerType], [CustomerSince], [RegionID]
    FROM [dbo].[TableInheritance_Person]
    WHERE [ClassID] IN ('TI_Customer')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_FileSystemItemView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt], [FolderCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt], null
    FROM [dbo].[TableInheritance_File]
    WHERE [ClassID] IN ('TI_File', 'TI_Folder', 'TI_SpecificFolder')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], null, null, [FolderCreatedAt]
    FROM [dbo].[TableInheritance_Folder]
    WHERE [ClassID] IN ('TI_File', 'TI_Folder', 'TI_SpecificFolder')
GO

CREATE VIEW [dbo].[TI_FileView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt]
    FROM [dbo].[TableInheritance_File]
    WHERE [ClassID] IN ('TI_File')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_HistoryEntryView] ([ID], [ClassID], [Timestamp], [HistoryDate], [Text], [OwnerID], [OwnerIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [HistoryDate], [Text], [OwnerID], [OwnerIDClassID]
    FROM [dbo].[TableInheritance_HistoryEntry]
    WHERE [ClassID] IN ('TI_HistoryEntry')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_OrderView] ([ID], [ClassID], [Timestamp], [Number], [OrderDate], [CustomerID], [CustomerIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Number], [OrderDate], [CustomerID], [CustomerIDClassID]
    FROM [dbo].[TableInheritance_Order]
    WHERE [ClassID] IN ('TI_Order')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_OrganizationalUnitView] ([ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [Name]
    FROM [dbo].[TableInheritance_OrganizationalUnit]
    WHERE [ClassID] IN ('TI_OrganizationalUnit')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_RegionView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[TableInheritance_Region]
    WHERE [ClassID] IN ('TI_Region')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_FolderView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt]
    FROM [dbo].[TableInheritance_Folder]
    WHERE [ClassID] IN ('TI_Folder', 'TI_SpecificFolder')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TI_SpecificFolderView] ([ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ParentFolderID], [ParentFolderIDClassID], [FolderCreatedAt]
    FROM [dbo].[TableInheritance_Folder]
    WHERE [ClassID] IN ('TI_SpecificFolder')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CeoView] ([ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID]
    FROM [dbo].[Ceo]
    WHERE [ClassID] IN ('Ceo')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClientView] ([ID], [ClassID], [Timestamp], [ParentClientID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentClientID]
    FROM [dbo].[Client]
    WHERE [ClassID] IN ('Client')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ComputerView] ([ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID]
    FROM [dbo].[Computer]
    WHERE [ClassID] IN ('Computer')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType], [ContactPersonID], [NumberOfShops], [SupplierQuality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType], [ContactPersonID], [NumberOfShops], [SupplierQuality]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Company', 'Customer', 'Partner', 'Distributor', 'Supplier')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Customer')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops], [SupplierQuality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops], [SupplierQuality]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Partner', 'Distributor', 'Supplier')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DistributorView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Distributor')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[EmployeeView] ([ID], [ClassID], [Timestamp], [Name], [SupervisorID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID]
    FROM [dbo].[Employee]
    WHERE [ClassID] IN ('Employee')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[FileSystemItemView] ([ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID]
    FROM [dbo].[FileSystemItem]
    WHERE [ClassID] IN ('FileSystemItem', 'File', 'Folder')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[FileView] ([ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID]
    FROM [dbo].[FileSystemItem]
    WHERE [ClassID] IN ('File')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[FolderView] ([ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ParentFolderID], [ParentFolderIDClassID]
    FROM [dbo].[FileSystemItem]
    WHERE [ClassID] IN ('Folder')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[IndustrialSectorView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[IndustrialSector]
    WHERE [ClassID] IN ('IndustrialSector')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[LocationView] ([ID], [ClassID], [Timestamp], [ClientID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ClientID]
    FROM [dbo].[Location]
    WHERE [ClassID] IN ('Location')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [OrderNo], [DeliveryDate], [OfficialID], [CustomerID], [CustomerIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [OrderNo], [DeliveryDate], [OfficialID], [CustomerID], [CustomerIDClassID]
    FROM [dbo].[Order]
    WHERE [ClassID] IN ('Order')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID]
    FROM [dbo].[OrderItem]
    WHERE [ClassID] IN ('OrderItem')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderTicketView] ([ID], [ClassID], [Timestamp], [FileName], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [FileName], [OrderID]
    FROM [dbo].[OrderTicket]
    WHERE [ClassID] IN ('OrderTicket')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PersonView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[Person]
    WHERE [ClassID] IN ('Person')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SupplierView] ([ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [SupplierQuality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [SupplierQuality]
    FROM [dbo].[Company]
    WHERE [ClassID] IN ('Supplier')
  WITH CHECK OPTION
GO
