/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "OrderTable", c_testDomainProviderID, typeof (Order), false);
    }

    [Test]
    public void InitializeWithValueType ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (int), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (int), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableValueType ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (int?), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (int?), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithObjectID ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (ObjectID), true, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsTrue (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithEnum ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (ClassWithAllDataTypes.EnumType), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableEnum ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (ClassWithAllDataTypes.EnumType?), null, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType?), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableStringAndMaxLength ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (string), true, 100, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNotNullableStringWithoutMaxLength ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (string), false, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableArrayAndMaxLength ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (byte[]), true, 100, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNotNullableArrayWithoutMaxLength ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "PropertyName", "ColumnName", typeof (byte[]), false, null, StorageClass.Persistent);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (StorageClass.Persistent, actual.StorageClass);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void GetToString ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "ThePropertyName", "TheColumnName", typeof (int), null, null, StorageClass.None);

      Assert.That (actual.ToString (), Is.EqualTo (typeof (ReflectionBasedPropertyDefinition).FullName + ": ThePropertyName"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access property 'StorageSpecificName' for non-persistent property definitions.")]
    public void NonPersistentProperty ()
    {
      PropertyDefinition actual = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "ThePropertyName", "TheColumnName", typeof (int), null, null, StorageClass.Transaction);
      Assert.AreEqual (StorageClass.Transaction, actual.StorageClass);
      Dev.Null = actual.StorageSpecificName;
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type 'System.Int32'.\r\n  Property: test")]
    public void IntPropertyWithMaxLength ()
    {
      ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (int), 10);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "IsNullable parameter can only be supplied for reference types but the property is of type 'System.Int32'.\r\n  Property: test")]
    public void CheckValueTypeCtors ()
    {
      ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (int), false);
    }
  }
}
