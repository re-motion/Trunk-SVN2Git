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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "OrderTable", TestDomainProviderID, typeof (Order), false);
    }

    [Test]
    public void InitializeWithValueType ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (int), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.AreEqual (0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (int), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithNullableValueType ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (int?), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (int?), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithObjectID ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (ObjectID), true, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsTrue (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithEnum ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (ClassWithAllDataTypes.EnumType), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StoragePropertyDefinition.Name);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithNullableEnum ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (ClassWithAllDataTypes.EnumType?), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType?), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithEnumNotDefiningZero ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_classDefinition, "PropertyName", "ColumnName", typeof (EnumNotDefiningZero),  StorageClass.Persistent);
      Assert.AreEqual (EnumNotDefiningZero.First, actual.DefaultValue);
    }

    [Test]
    public void InitializeWithExtensibleEnum ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_classDefinition, "PropertyName", "ColumnName", typeof (Color), StorageClass.Persistent);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
    }

    [Test]
    public void InitializeWithExtensibleEnum_NotNullable ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_classDefinition, "PropertyName", "ColumnName", typeof (Color), false, StorageClass.Persistent);
      Assert.AreEqual (Color.Values.Blue(), actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
    }

    [Test]
    public void InitializeWithNullableStringAndMaxLength ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (string), true, 100, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithNotNullableStringWithoutMaxLength ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (string), false, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.AreEqual (string.Empty, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithNullableArrayAndMaxLength ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (byte[]), true, 100, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StoragePropertyDefinition.Name);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void InitializeWithNotNullableArrayWithoutMaxLength ()
    {
      var propertyName = _classDefinition.ClassType.FullName + ".OrderNumber";
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "ColumnName", typeof (byte[]), false, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", StorageModelTestHelper.GetColumnName(actual));
      Assert.AreEqual (new byte[0], actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (propertyName, actual.PropertyName);
      Assert.AreEqual (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
      Assert.That (actual.PropertyInfo, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void GetToString ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_classDefinition, "ThePropertyName", "TheColumnName", StorageClass.None);

      Assert.That (actual.ToString (), Is.EqualTo (typeof (ReflectionBasedPropertyDefinition).FullName + ": ThePropertyName"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access property 'storagePropertyDefinition' for non-persistent property definitions.")]
    public void NonPersistentProperty ()
    {
      var fakePropertyInfo = ReflectionBasedPropertyDefinitionFactory.GetFakePropertyInfo();
      var actual = ReflectionBasedPropertyDefinitionFactory.Create (
          _classDefinition, "ThePropertyName", typeof (int), null, null, StorageClass.Transaction, fakePropertyInfo, null);
      Assert.AreEqual (StorageClass.Transaction, actual.StorageClass);
      Dev.Null = actual.StoragePropertyDefinition;
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type 'System.Int32'.\r\n  Property: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber")]
    public void IntPropertyWithMaxLength ()
    {
      ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "test", typeof (int), 10);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "IsNullable parameter can only be supplied for reference types but the property is of type 'System.Int32'.\r\n  Property: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber")]
    public void CheckValueTypeCtors ()
    {
      ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, _classDefinition.ClassType, "OrderNumber", "test", typeof (int), false);
    }
  }
}
