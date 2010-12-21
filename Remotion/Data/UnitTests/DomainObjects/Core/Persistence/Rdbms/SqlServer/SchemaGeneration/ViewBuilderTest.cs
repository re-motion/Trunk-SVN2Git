// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class ViewBuilderTest : SchemaGenerationTestBase
  {
    private ViewBuilder _viewBuilder;

    public override void SetUp ()
    {
      base.SetUp();
      _viewBuilder = new ViewBuilder();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (string.Empty, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddView ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddView_NonEntityDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Order), SchemaGenerationFirstStorageProviderDefinition);
      classDefinition.SetStorageEntity (MockRepository.GenerateStub<IStorageEntityDefinition> ());
      
      _viewBuilder.AddView (classDefinition);
      Assert.AreEqual ("", _viewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteDerivedClass ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Customer)]);

      string expectedScript =
          "CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode]\r\n"
          + "    FROM [dbo].[Customer]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithConcreteBaseClass ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (ConcreteClass)]);

      string expectedScript =
          "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithAbstractClass ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Company)]);

      string expectedScript =
          "CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [LicenseCode], NULL, NULL, NULL\r\n"
          + "    FROM [dbo].[Customer]\r\n"
          + "  UNION ALL\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], NULL, NULL, NULL, [LicenseCode], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\r\n"
          + "    FROM [dbo].[DevelopmentPartner]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithAbstractClassWithSingleConcreteConcrete ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Partner)]);

      string expectedScript =
          "CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences], [LicenseCode]\r\n"
          + "    FROM [dbo].[DevelopmentPartner]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithDerivedClass ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (DerivedClass)]);

      string expectedScript =
          "CREATE VIEW [dbo].[DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('DerivedClass', 'DerivedOfDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithClassWithoutProperties ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (ClassWithoutProperties)]);

      string expectedScript =
          "CREATE VIEW [dbo].[ClassWithoutPropertiesView] ([ID], [ClassID], [Timestamp])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp]\r\n"
          + "    FROM [dbo].[TableWithoutProperties]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractWithoutConcreteTable ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (AbstractWithoutConcreteClass)]);

      Assert.IsEmpty (_viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewTwice ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Order)]);
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (ConcreteClass)]);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "  WITH CHECK OPTION\r\n"
          + "GO\r\n\r\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViews ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (Order)]);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (ConcreteClass)]);

      _viewBuilder.AddViews (classes);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "  WITH CHECK OPTION\r\n"
          + "GO\r\n\r\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PersistentProperty], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [ClassWithRelationsInDerivedOfDerivedClassIDClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [ClassWithRelationsInSecondDerivedClassIDClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void GetDropViewScriptWithConcreteClass ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      string expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript());
    }

    [Test]
    public void GetDropViewScriptWithAbstractClass ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Company)]);

      string expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CompanyView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript());
    }

    [Test]
    public void GetDropViewScriptWithTwoClasses ()
    {
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Order)]);
      _viewBuilder.AddView (MappingConfiguration.ClassDefinitions[typeof (Company)]);

      string expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CompanyView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript());
    }
  }
}
