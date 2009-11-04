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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  [TestFixture]
  public class ViewBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private ViewBuilder _createViewBuilder;

    // construction and disposing

    public ViewBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();
      _createViewBuilder = new ViewBuilder ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (string.Empty, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddView ()
    {
      _createViewBuilder.AddView (OrderClass);

      string expectedScript = "CREATE VIEW \"OrderView\" (\"ID\", \"ClassID\", \"Timestamp\", \"Number\", \"Priority\", \"CustomerID\", \"CustomerIDClassID\", \"OfficialID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Number\", \"Priority\", \"CustomerID\", \"CustomerIDClassID\", \"OfficialID\"\r\n"
          + "    FROM \"Order\"\r\n"
          + "    WHERE \"ClassID\" IN ('Order')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteDerivedClass ()
    {
      _createViewBuilder.AddView (CustomerClass);

      string expectedScript = "CREATE VIEW \"CustomerView\" (\"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", \"CustomerType\", \"CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"PrimaryOfficialID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", \"CustomerType\", \"CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"PrimaryOfficialID\"\r\n"
          + "    FROM \"Customer\"\r\n"
          + "    WHERE \"ClassID\" IN ('Customer')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteBaseClass ()
    {
      _createViewBuilder.AddView (ConcreteClass);

      string expectedScript = "CREATE VIEW \"ConcreteClassView\" (\"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\", \"PropertyInSecondDerivedClass\", \"ClassWithRelationsInSecondDerivedClassID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\", \"PropertyInSecondDerivedClass\", \"ClassWithRelationsInSecondDerivedClassID\"\r\n"
          + "    FROM \"ConcreteClass\"\r\n"
          + "    WHERE \"ClassID\" IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractClass ()
    {
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "CREATE VIEW \"CompanyView\" (\"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", \"CustomerType\", \"CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"PrimaryOfficialID\", \"Description\", \"PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"Competences\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", \"CustomerType\", \"CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"PrimaryOfficialID\", null, null, null\r\n"
          + "    FROM \"Customer\"\r\n"
          + "    WHERE \"ClassID\" IN ('Customer', 'DevelopmentPartner')\r\n"
          + "  UNION ALL\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", null, null, null, \"Description\", \"PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"Competences\"\r\n"
          + "    FROM \"DevelopmentPartner\"\r\n"
          + "    WHERE \"ClassID\" IN ('Customer', 'DevelopmentPartner');\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractClassWithSingleConcreteConcrete ()
    {
      _createViewBuilder.AddView (PartnerClass);

      string expectedScript = "CREATE VIEW \"PartnerView\" (\"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", \"Description\", \"PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"Competences\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Name\", \"PhoneNumber\", \"AddressID\", \"Description\", \"PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches\", \"Competences\"\r\n"
          + "    FROM \"DevelopmentPartner\"\r\n"
          + "    WHERE \"ClassID\" IN ('DevelopmentPartner')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithDerivedClass ()
    {
      _createViewBuilder.AddView (DerivedClass);

      string expectedScript = "CREATE VIEW \"DerivedClassView\" (\"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\"\r\n"
          + "    FROM \"ConcreteClass\"\r\n"
          + "    WHERE \"ClassID\" IN ('DerivedClass', 'DerivedOfDerivedClass')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractWithoutConcreteTable()
    {
      _createViewBuilder.AddView (AbstractWithoutConcreteClass);

      Assert.IsEmpty (_createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewTwice()
    {
      _createViewBuilder.AddView (OrderClass);
      _createViewBuilder.AddView (ConcreteClass);

      string expectedScript = "CREATE VIEW \"OrderView\" (\"ID\", \"ClassID\", \"Timestamp\", \"Number\", \"Priority\", \"CustomerID\", \"CustomerIDClassID\", \"OfficialID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Number\", \"Priority\", \"CustomerID\", \"CustomerIDClassID\", \"OfficialID\"\r\n"
          + "    FROM \"Order\"\r\n"
          + "    WHERE \"ClassID\" IN ('Order')\r\n"
          + "  WITH CHECK OPTION;\r\n"
          + "\r\n"
          + "CREATE VIEW \"ConcreteClassView\" (\"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\", \"PropertyInSecondDerivedClass\", \"ClassWithRelationsInSecondDerivedClassID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\", \"PropertyInSecondDerivedClass\", \"ClassWithRelationsInSecondDerivedClassID\"\r\n"
          + "    FROM \"ConcreteClass\"\r\n"
          + "    WHERE \"ClassID\" IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViews ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderClass);
      classes.Add (ConcreteClass);

      _createViewBuilder.AddViews (classes);

      string expectedScript = "CREATE VIEW \"OrderView\" (\"ID\", \"ClassID\", \"Timestamp\", \"Number\", \"Priority\", \"CustomerID\", \"CustomerIDClassID\", \"OfficialID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"Number\", \"Priority\", \"CustomerID\", \"CustomerIDClassID\", \"OfficialID\"\r\n"
          + "    FROM \"Order\"\r\n"
          + "    WHERE \"ClassID\" IN ('Order')\r\n"
          + "  WITH CHECK OPTION;\r\n"
          + "\r\n"
          + "CREATE VIEW \"ConcreteClassView\" (\"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\", \"PropertyInSecondDerivedClass\", \"ClassWithRelationsInSecondDerivedClassID\")\r\n"
          + "  AS\r\n"
          + "  SELECT \"ID\", \"ClassID\", \"Timestamp\", \"PropertyInConcreteClass\", \"PropertyInDerivedClass\", \"PropertyInDerivedOfDerivedClass\", \"ClassWithRelationsInDerivedOfDerivedClassID\", \"PropertyInSecondDerivedClass\", \"ClassWithRelationsInSecondDerivedClassID\"\r\n"
          + "    FROM \"ConcreteClass\"\r\n"
          + "    WHERE \"ClassID\" IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION;\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithConcreteClass ()
    {
      _createViewBuilder.AddView (OrderClass);

      string expectedScript = "DROP VIEW \"OrderView\";\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithAbstractClass ()
    {
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "DROP VIEW \"CompanyView\";\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithTwoClasses ()
    {
      _createViewBuilder.AddView (OrderClass);
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "DROP VIEW \"OrderView\";\r\n\r\n"
          + "DROP VIEW \"CompanyView\";\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }
  }
}
