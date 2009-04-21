// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.CodeGenerator.Sql.SqlServer;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.NullableValueTypes;
using System.Collections;
using log4net.Appender;
using log4net.Config;
using log4net.Core;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  [TestFixture]
  public class ConstraintBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private ConstraintBuilder _constraintBuilder;

    // construction and disposing

    public ConstraintBuilderTest ()
    {
    }

    // methods and properties


    public override void SetUp ()
    {
      base.SetUp ();

      _constraintBuilder = new ConstraintBuilder ();
    }

    [Test]
    public void AddConstraintWithRelationToSameStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);

      string expectedScript = "ALTER TABLE \"OrderItem\" ADD CONSTRAINT \"FK_OrderToOrderItem\" FOREIGN KEY (\"OrderID\") REFERENCES \"Order\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationToOtherStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript = "ALTER TABLE \"Order\" ADD CONSTRAINT \"FK_CustomerToOrder\" FOREIGN KEY (\"CustomerID\") REFERENCES \"Customer\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintMultipleTimes ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE \"OrderItem\" ADD CONSTRAINT \"FK_OrderToOrderItem\" FOREIGN KEY (\"OrderID\") REFERENCES \"Order\" (\"ID\");\r\n\r\n"
        + "ALTER TABLE \"Order\" ADD CONSTRAINT \"FK_CustomerToOrder\" FOREIGN KEY (\"CustomerID\") REFERENCES \"Customer\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());

    }

    [Test]
    public void AddConstraintWithTwoConstraints ()
    {
      ClassDefinition firstClass = new ClassDefinition (
          "FirstClass", "FirstEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      firstClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("SecondClass", "SecondClassID", TypeInfo.ObjectIDMappingTypeName, false, true, NaInt32.Null));

      firstClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("ThirdClass", "ThirdClassID", TypeInfo.ObjectIDMappingTypeName, false, true, NaInt32.Null));

      ClassDefinition secondClass = new ClassDefinition (
          "SecondClass", "SecondEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      ClassDefinition thirdClass = new ClassDefinition (
          "ThirdClass", "ThirdEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      RelationDefinition relationDefinition1 = new RelationDefinition (
          "FirstClassToSecondClass",
          new RelationEndPointDefinition (firstClass, "SecondClass", false),
          new VirtualRelationEndPointDefinition (secondClass, "FirstClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));
      firstClass.MyRelationDefinitions.Add (relationDefinition1);
      secondClass.MyRelationDefinitions.Add (relationDefinition1);

      RelationDefinition relationDefinition2 = new RelationDefinition (
          "FirstClassToThirdClass",
          new RelationEndPointDefinition (firstClass, "ThirdClass", false),
          new VirtualRelationEndPointDefinition (thirdClass, "FirstClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));
      firstClass.MyRelationDefinitions.Add (relationDefinition2);
      thirdClass.MyRelationDefinitions.Add (relationDefinition2);

      _constraintBuilder.AddConstraint (firstClass);

      string expectedScript =
          "ALTER TABLE \"FirstEntity\" ADD CONSTRAINT \"FK_FirstClassToSecondClass\" FOREIGN KEY (\"SecondClassID\") REFERENCES \"SecondEntity\" (\"ID\");\r\n"
        + "ALTER TABLE \"FirstEntity\" ADD CONSTRAINT \"FK_FirstClassToThirdClass\" FOREIGN KEY (\"ThirdClassID\") REFERENCES \"ThirdEntity\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithNoConstraintNecessary ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions.GetMandatory ("Official"));
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationInDerivedClass ()
    {
      ClassDefinition baseClass = new ClassDefinition (
          "BaseClass", "BaseClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      ClassDefinition derivedClass = new ClassDefinition (
          "DerivedClass", "BaseClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, baseClass);

      derivedClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("OtherClass", "OtherClassID", TypeInfo.ObjectIDMappingTypeName, false, true, NaInt32.Null));

      ClassDefinition otherClass = new ClassDefinition (
          "OtherClass", "OtherClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      RelationDefinition relationDefinition1 = new RelationDefinition (
          "OtherClassToDerivedClass",
          new RelationEndPointDefinition (derivedClass, "OtherClass", false),
          new VirtualRelationEndPointDefinition (otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));

      derivedClass.MyRelationDefinitions.Add (relationDefinition1);
      otherClass.MyRelationDefinitions.Add (relationDefinition1);

      _constraintBuilder.AddConstraint (baseClass);

      string expectedScript = 
          "ALTER TABLE \"BaseClassEntity\" ADD CONSTRAINT \"FK_OtherClassToDerivedClass\" FOREIGN KEY (\"OtherClassID\") REFERENCES \"OtherClassEntity\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationToDerivedOfConcreteClass ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript =
          "ALTER TABLE \"ClassWithRelations\" ADD CONSTRAINT \"FK_DerivedClassToClassWithRelations\" FOREIGN KEY (\"DerivedClassID\") REFERENCES \"ConcreteClass\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationNameExceedsMaximumLength ()
    {
      MemoryAppender memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (memoryAppender);

      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript =
          "ALTER TABLE \"ClassWithRelations\" ADD CONSTRAINT \"FK_DerivedClassToClassWithRelations\" FOREIGN KEY (\"DerivedClassID\") REFERENCES \"ConcreteClass\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
      
      LoggingEvent[] events = memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Warn, events[0].Level);
      Assert.AreEqual ("The relation name 'DerivedClassToClassWithRelations' in class 'ClassWithRelations' is too long (32 characters). Maximum length: 25", events[0].RenderedMessage);
    }

    [Test]
    public void AddConstraintWithRelationToAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CeoClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CompanyClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void AddConstraintWithDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (SecondDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void AddConstraintWithDerivedOfDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (DerivedOfDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void AddConstraints ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderItemClass);
      classes.Add (OrderClass);

      _constraintBuilder.AddConstraints (classes);

      string expectedScript =
          "ALTER TABLE \"OrderItem\" ADD CONSTRAINT \"FK_OrderToOrderItem\" FOREIGN KEY (\"OrderID\") REFERENCES \"Order\" (\"ID\");\r\n\r\n"
          + "ALTER TABLE \"Order\" ADD CONSTRAINT \"FK_CustomerToOrder\" FOREIGN KEY (\"CustomerID\") REFERENCES \"Customer\" (\"ID\");\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void GetDropConstraintsScript ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript = "DECLARE\r\n"
          + "  tableName varchar2 (200);\r\n"
          + "  constraintName varchar2 (200);\r\n"
          + "  CURSOR dropConstraintsCursor IS SELECT TABLE_NAME, CONSTRAINT_NAME\r\n"
          + "    FROM USER_CONSTRAINTS\r\n"
          + "    WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME IN ('ClassWithRelations')\r\n"
          + "    ORDER BY TABLE_NAME, CONSTRAINT_NAME;\r\n"
          + "BEGIN\r\n"
          + "  OPEN dropConstraintsCursor;\r\n"
          + "  LOOP\r\n"
          + "    FETCH dropConstraintsCursor INTO tableName, constraintName;\r\n"
          + "    EXIT WHEN dropConstraintsCursor%NOTFOUND;\r\n"
          + "    EXECUTE IMMEDIATE 'ALTER TABLE \"' || tableName || '\" DROP CONSTRAINT \"' || constraintName || '\"';\r\n"
          + "  END LOOP;\r\n"
          + "  CLOSE dropConstraintsCursor;\r\n"
          + "END;\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript ());
    }



    [Test]
    public void GetDropConstraintsScriptWithMultipleEntities ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);
      _constraintBuilder.AddConstraint (ConcreteClass);

      string expectedScript = "DECLARE\r\n"
          + "  tableName varchar2 (200);\r\n"
          + "  constraintName varchar2 (200);\r\n"
          + "  CURSOR dropConstraintsCursor IS SELECT TABLE_NAME, CONSTRAINT_NAME\r\n"
          + "    FROM USER_CONSTRAINTS\r\n"
          + "    WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME IN ('ClassWithRelations', 'ConcreteClass')\r\n"
          + "    ORDER BY TABLE_NAME, CONSTRAINT_NAME;\r\n"
          + "BEGIN\r\n"
          + "  OPEN dropConstraintsCursor;\r\n"
          + "  LOOP\r\n"
          + "    FETCH dropConstraintsCursor INTO tableName, constraintName;\r\n"
          + "    EXIT WHEN dropConstraintsCursor%NOTFOUND;\r\n"
          + "    EXECUTE IMMEDIATE 'ALTER TABLE \"' || tableName || '\" DROP CONSTRAINT \"' || constraintName || '\"';\r\n"
          + "  END LOOP;\r\n"
          + "  CLOSE dropConstraintsCursor;\r\n"
          + "END;\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript ());
    }
  }
}
