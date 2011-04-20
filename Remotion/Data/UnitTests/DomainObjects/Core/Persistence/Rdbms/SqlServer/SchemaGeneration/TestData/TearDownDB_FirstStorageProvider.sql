USE SchemaGenerationTestDomain1
GO

-- Drop all synonyms that will be created below
GO

-- Drop all indexes that will be created below
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

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AbstractClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AbstractClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedAbstractClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedAbstractClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedDerivedConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedDerivedConcreteClassView]

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

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FirstClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[FirstClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecondClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecondClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecondDerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SiblingOfClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SiblingOfClassWithRelationsView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ThirdClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ThirdClassView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (max)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + schema_name(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id 
WHERE fk.type = 'F' AND schema_name (t.schema_id) + '.' + t.name IN ('dbo.Address', 'dbo.Ceo', 'dbo.TableWithAllDataTypes', 'dbo.TableWithoutProperties', 'dbo.TableWithRelations', 'dbo.Customer', 'dbo.AbstractClass', 'dbo.ConcreteClass', 'dbo.DevelopmentPartner', 'dbo.Employee', 'dbo.FirstClass', 'dbo.Order', 'dbo.OrderItem', 'dbo.SecondClass', 'dbo.SiblingOfTableWithRelations', 'dbo.ThirdClass') 
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

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AbstractClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AbstractClass]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteClass]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DevelopmentPartner' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[DevelopmentPartner]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Employee]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FirstClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[FirstClass]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Order]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OrderItem]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SecondClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SecondClass]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SiblingOfTableWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SiblingOfTableWithRelations]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ThirdClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ThirdClass]
GO
