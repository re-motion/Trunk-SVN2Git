USE RemotionSecurityManager
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessControlEntryView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessControlEntryView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessControlListView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessControlListView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PermissionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PermissionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateCombinationView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StateCombinationView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateUsageView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StateUsageView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MetadataObjectView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[MetadataObjectView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EnumValueDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EnumValueDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AbstractRoleDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AbstractRoleDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessTypeDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessTypeDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessTypeReferenceView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessTypeReferenceView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CultureView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CultureView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'LocalizedNameView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[LocalizedNameView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecurableClassDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecurableClassDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StateDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatePropertyDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StatePropertyDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatePropertyReferenceView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StatePropertyReferenceView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[GroupView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupTypeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[GroupTypeView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupTypePositionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[GroupTypePositionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PositionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PositionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'RoleView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[RoleView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TenantView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TenantView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'UserView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[UserView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('AccessControlEntry', 'AccessControlList', 'Permission', 'StateCombination', 'StateUsage', 'EnumValueDefinition', 'AccessTypeReference', 'Culture', 'LocalizedName', 'SecurableClassDefinition', 'StatePropertyDefinition', 'StatePropertyReference', 'Group', 'GroupType', 'GroupTypePosition', 'Position', 'Role', 'Tenant', 'User')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlEntry' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AccessControlEntry]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlList' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AccessControlList]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Permission' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Permission]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateCombination' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StateCombination]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateUsage' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StateUsage]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[EnumValueDefinition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessTypeReference' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AccessTypeReference]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Culture' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Culture]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'LocalizedName' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[LocalizedName]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SecurableClassDefinition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SecurableClassDefinition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyDefinition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StatePropertyDefinition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyReference' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StatePropertyReference]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Group' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Group]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupType' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[GroupType]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupTypePosition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[GroupTypePosition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Position' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Position]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Role' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Role]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Tenant' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Tenant]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'User' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[User]
GO

-- Create all tables
CREATE TABLE [dbo].[AccessControlEntry]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessControlEntry columns
  [Index] int NOT NULL,
  [TenantSelection] int NOT NULL,
  [GroupSelection] int NOT NULL,
  [UserSelection] int NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [GroupID] uniqueidentifier NULL,
  [GroupTypeID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,
  [UserID] uniqueidentifier NULL,
  [AbstractRoleID] uniqueidentifier NULL,
  [AbstractRoleIDClassID] varchar (100) NULL,
  [AccessControlListID] uniqueidentifier NULL,

  CONSTRAINT [PK_AccessControlEntry] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[AccessControlList]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessControlList columns
  [Index] int NOT NULL,
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_AccessControlList] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Permission]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Permission columns
  [Index] int NOT NULL,
  [Allowed] bit NULL,
  [AccessTypeDefinitionID] uniqueidentifier NULL,
  [AccessTypeDefinitionIDClassID] varchar (100) NULL,
  [AccessControlEntryID] uniqueidentifier NULL,

  CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StateCombination]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateCombination columns
  [Index] int NOT NULL,
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,
  [AccessControlListID] uniqueidentifier NULL,

  CONSTRAINT [PK_StateCombination] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StateUsage]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateUsage columns
  [StateDefinitionID] uniqueidentifier NULL,
  [StateDefinitionIDClassID] varchar (100) NULL,
  [StateCombinationID] uniqueidentifier NULL,

  CONSTRAINT [PK_StateUsage] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[EnumValueDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- MetadataObject columns
  [Index] int NOT NULL,
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (200) NOT NULL,

  -- EnumValueDefinition columns
  [Value] int NOT NULL,

  -- AbstractRoleDefinition columns

  -- AccessTypeDefinition columns

  -- StateDefinition columns
  [StatePropertyID] uniqueidentifier NULL,
  [StatePropertyIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_EnumValueDefinition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[AccessTypeReference]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessTypeReference columns
  [Index] int NOT NULL,
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,
  [AccessTypeID] uniqueidentifier NULL,
  [AccessTypeIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_AccessTypeReference] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Culture]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Culture columns
  [CultureName] nvarchar (10) NOT NULL,

  CONSTRAINT [PK_Culture] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[LocalizedName]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- LocalizedName columns
  [Text] nvarchar (max) NOT NULL,
  [CultureID] uniqueidentifier NULL,
  [MetadataObjectID] uniqueidentifier NULL,
  [MetadataObjectIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_LocalizedName] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SecurableClassDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- MetadataObject columns
  [Index] int NOT NULL,
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (200) NOT NULL,

  -- SecurableClassDefinition columns
  [BaseSecurableClassID] uniqueidentifier NULL,
  [BaseSecurableClassIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_SecurableClassDefinition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StatePropertyDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- MetadataObject columns
  [Index] int NOT NULL,
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (200) NOT NULL,

  -- StatePropertyDefinition columns

  CONSTRAINT [PK_StatePropertyDefinition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StatePropertyReference]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StatePropertyReference columns
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,
  [StatePropertyID] uniqueidentifier NULL,
  [StatePropertyIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_StatePropertyReference] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Group]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Group columns
  [Name] nvarchar (100) NOT NULL,
  [ShortName] nvarchar (10) NULL,
  [UniqueIdentifier] nvarchar (100) NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [ParentID] uniqueidentifier NULL,
  [GroupTypeID] uniqueidentifier NULL,

  CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[GroupType]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- GroupType columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_GroupType] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[GroupTypePosition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- GroupTypePosition columns
  [GroupTypeID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,

  CONSTRAINT [PK_GroupTypePosition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Position]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Position columns
  [Name] nvarchar (100) NOT NULL,
  [Delegation] int NOT NULL,

  CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Role]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Role columns
  [GroupID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,
  [UserID] uniqueidentifier NULL,

  CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Tenant]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Tenant columns
  [Name] nvarchar (100) NOT NULL,
  [UniqueIdentifier] nvarchar (100) NOT NULL,
  [IsAbstract] bit NOT NULL,
  [ParentID] uniqueidentifier NULL,

  CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[User]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- User columns
  [Title] nvarchar (100) NULL,
  [FirstName] nvarchar (100) NULL,
  [LastName] nvarchar (100) NOT NULL,
  [UserName] nvarchar (100) NOT NULL,
  [TenantID] uniqueidentifier NULL,
  [OwningGroupID] uniqueidentifier NULL,

  CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[AccessControlEntry] ADD
  CONSTRAINT [FK_AccessControlEntry_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID]),
  CONSTRAINT [FK_AccessControlEntry_GroupID] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Group] ([ID]),
  CONSTRAINT [FK_AccessControlEntry_GroupTypeID] FOREIGN KEY ([GroupTypeID]) REFERENCES [dbo].[GroupType] ([ID]),
  CONSTRAINT [FK_AccessControlEntry_PositionID] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Position] ([ID]),
  CONSTRAINT [FK_AccessControlEntry_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([ID]),
  CONSTRAINT [FK_AccessControlEntry_AbstractRoleID] FOREIGN KEY ([AbstractRoleID]) REFERENCES [dbo].[EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlEntry_AccessControlListID] FOREIGN KEY ([AccessControlListID]) REFERENCES [dbo].[AccessControlList] ([ID])

ALTER TABLE [dbo].[AccessControlList] ADD
  CONSTRAINT [FK_AccessControlList_SecurableClassID] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID])

ALTER TABLE [dbo].[Permission] ADD
  CONSTRAINT [FK_Permission_AccessTypeDefinitionID] FOREIGN KEY ([AccessTypeDefinitionID]) REFERENCES [dbo].[EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_Permission_AccessControlEntryID] FOREIGN KEY ([AccessControlEntryID]) REFERENCES [dbo].[AccessControlEntry] ([ID])

ALTER TABLE [dbo].[StateCombination] ADD
  CONSTRAINT [FK_StateCombination_SecurableClassID] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_StateCombination_AccessControlListID] FOREIGN KEY ([AccessControlListID]) REFERENCES [dbo].[AccessControlList] ([ID])

ALTER TABLE [dbo].[StateUsage] ADD
  CONSTRAINT [FK_StateUsage_StateDefinitionID] FOREIGN KEY ([StateDefinitionID]) REFERENCES [dbo].[EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_StateUsage_StateCombinationID] FOREIGN KEY ([StateCombinationID]) REFERENCES [dbo].[StateCombination] ([ID])

ALTER TABLE [dbo].[EnumValueDefinition] ADD
  CONSTRAINT [FK_EnumValueDefinition_StatePropertyID] FOREIGN KEY ([StatePropertyID]) REFERENCES [dbo].[StatePropertyDefinition] ([ID])

ALTER TABLE [dbo].[AccessTypeReference] ADD
  CONSTRAINT [FK_AccessTypeReference_SecurableClassID] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_AccessTypeReference_AccessTypeID] FOREIGN KEY ([AccessTypeID]) REFERENCES [dbo].[EnumValueDefinition] ([ID])

ALTER TABLE [dbo].[LocalizedName] ADD
  CONSTRAINT [FK_LocalizedName_CultureID] FOREIGN KEY ([CultureID]) REFERENCES [dbo].[Culture] ([ID])

ALTER TABLE [dbo].[SecurableClassDefinition] ADD
  CONSTRAINT [FK_SecurableClassDefinition_BaseSecurableClassID] FOREIGN KEY ([BaseSecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID])

ALTER TABLE [dbo].[StatePropertyReference] ADD
  CONSTRAINT [FK_StatePropertyReference_SecurableClassID] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_StatePropertyReference_StatePropertyID] FOREIGN KEY ([StatePropertyID]) REFERENCES [dbo].[StatePropertyDefinition] ([ID])

ALTER TABLE [dbo].[Group] ADD
  CONSTRAINT [FK_Group_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID]),
  CONSTRAINT [FK_Group_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Group] ([ID]),
  CONSTRAINT [FK_Group_GroupTypeID] FOREIGN KEY ([GroupTypeID]) REFERENCES [dbo].[GroupType] ([ID])

ALTER TABLE [dbo].[GroupTypePosition] ADD
  CONSTRAINT [FK_GroupTypePosition_GroupTypeID] FOREIGN KEY ([GroupTypeID]) REFERENCES [dbo].[GroupType] ([ID]),
  CONSTRAINT [FK_GroupTypePosition_PositionID] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Position] ([ID])

ALTER TABLE [dbo].[Role] ADD
  CONSTRAINT [FK_Role_GroupID] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Group] ([ID]),
  CONSTRAINT [FK_Role_PositionID] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Position] ([ID]),
  CONSTRAINT [FK_Role_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([ID])

ALTER TABLE [dbo].[Tenant] ADD
  CONSTRAINT [FK_Tenant_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Tenant] ([ID])

ALTER TABLE [dbo].[User] ADD
  CONSTRAINT [FK_User_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Tenant] ([ID]),
  CONSTRAINT [FK_User_OwningGroupID] FOREIGN KEY ([OwningGroupID]) REFERENCES [dbo].[Group] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[AccessControlEntryView] ([ID], [ClassID], [Timestamp], [Index], [TenantSelection], [GroupSelection], [UserSelection], [TenantID], [GroupID], [GroupTypeID], [PositionID], [UserID], [AbstractRoleID], [AbstractRoleIDClassID], [AccessControlListID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [TenantSelection], [GroupSelection], [UserSelection], [TenantID], [GroupID], [GroupTypeID], [PositionID], [UserID], [AbstractRoleID], [AbstractRoleIDClassID], [AccessControlListID]
    FROM [dbo].[AccessControlEntry]
    WHERE [ClassID] IN ('AccessControlEntry')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessControlListView] ([ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID]
    FROM [dbo].[AccessControlList]
    WHERE [ClassID] IN ('AccessControlList')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PermissionView] ([ID], [ClassID], [Timestamp], [Index], [Allowed], [AccessTypeDefinitionID], [AccessTypeDefinitionIDClassID], [AccessControlEntryID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [Allowed], [AccessTypeDefinitionID], [AccessTypeDefinitionIDClassID], [AccessControlEntryID]
    FROM [dbo].[Permission]
    WHERE [ClassID] IN ('Permission')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StateCombinationView] ([ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessControlListID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessControlListID]
    FROM [dbo].[StateCombination]
    WHERE [ClassID] IN ('StateCombination')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StateUsageView] ([ID], [ClassID], [Timestamp], [StateDefinitionID], [StateDefinitionIDClassID], [StateCombinationID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [StateDefinitionID], [StateDefinitionIDClassID], [StateCombinationID]
    FROM [dbo].[StateUsage]
    WHERE [ClassID] IN ('StateUsage')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[MetadataObjectView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID], [BaseSecurableClassID], [BaseSecurableClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID], null, null
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'AbstractRoleDefinition', 'AccessTypeDefinition', 'StateDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], null, null, null, [BaseSecurableClassID], [BaseSecurableClassIDClassID]
    FROM [dbo].[SecurableClassDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'AbstractRoleDefinition', 'AccessTypeDefinition', 'StateDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], null, null, null, null, null
    FROM [dbo].[StatePropertyDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'AbstractRoleDefinition', 'AccessTypeDefinition', 'StateDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
GO

CREATE VIEW [dbo].[EnumValueDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'AbstractRoleDefinition', 'AccessTypeDefinition', 'StateDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AbstractRoleDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('AbstractRoleDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessTypeDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('AccessTypeDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessTypeReferenceView] ([ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessTypeID], [AccessTypeIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessTypeID], [AccessTypeIDClassID]
    FROM [dbo].[AccessTypeReference]
    WHERE [ClassID] IN ('AccessTypeReference')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CultureView] ([ID], [ClassID], [Timestamp], [CultureName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CultureName]
    FROM [dbo].[Culture]
    WHERE [ClassID] IN ('Culture')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[LocalizedNameView] ([ID], [ClassID], [Timestamp], [Text], [CultureID], [MetadataObjectID], [MetadataObjectIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Text], [CultureID], [MetadataObjectID], [MetadataObjectIDClassID]
    FROM [dbo].[LocalizedName]
    WHERE [ClassID] IN ('LocalizedName')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SecurableClassDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [BaseSecurableClassID], [BaseSecurableClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [BaseSecurableClassID], [BaseSecurableClassIDClassID]
    FROM [dbo].[SecurableClassDefinition]
    WHERE [ClassID] IN ('SecurableClassDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StateDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('StateDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StatePropertyDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name]
    FROM [dbo].[StatePropertyDefinition]
    WHERE [ClassID] IN ('StatePropertyDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StatePropertyReferenceView] ([ID], [ClassID], [Timestamp], [SecurableClassID], [SecurableClassIDClassID], [StatePropertyID], [StatePropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [SecurableClassID], [SecurableClassIDClassID], [StatePropertyID], [StatePropertyIDClassID]
    FROM [dbo].[StatePropertyReference]
    WHERE [ClassID] IN ('StatePropertyReference')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[GroupView] ([ID], [ClassID], [Timestamp], [Name], [ShortName], [UniqueIdentifier], [TenantID], [ParentID], [GroupTypeID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ShortName], [UniqueIdentifier], [TenantID], [ParentID], [GroupTypeID]
    FROM [dbo].[Group]
    WHERE [ClassID] IN ('Group')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[GroupTypeView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[GroupType]
    WHERE [ClassID] IN ('GroupType')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[GroupTypePositionView] ([ID], [ClassID], [Timestamp], [GroupTypeID], [PositionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [GroupTypeID], [PositionID]
    FROM [dbo].[GroupTypePosition]
    WHERE [ClassID] IN ('GroupTypePosition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PositionView] ([ID], [ClassID], [Timestamp], [Name], [Delegation])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [Delegation]
    FROM [dbo].[Position]
    WHERE [ClassID] IN ('Position')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[RoleView] ([ID], [ClassID], [Timestamp], [GroupID], [PositionID], [UserID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [GroupID], [PositionID], [UserID]
    FROM [dbo].[Role]
    WHERE [ClassID] IN ('Role')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[TenantView] ([ID], [ClassID], [Timestamp], [Name], [UniqueIdentifier], [IsAbstract], [ParentID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [UniqueIdentifier], [IsAbstract], [ParentID]
    FROM [dbo].[Tenant]
    WHERE [ClassID] IN ('Tenant')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[UserView] ([ID], [ClassID], [Timestamp], [Title], [FirstName], [LastName], [UserName], [TenantID], [OwningGroupID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Title], [FirstName], [LastName], [UserName], [TenantID], [OwningGroupID]
    FROM [dbo].[User]
    WHERE [ClassID] IN ('User')
  WITH CHECK OPTION
GO
