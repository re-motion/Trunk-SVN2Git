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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private IPropertyInformation _propertyInfo;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (
           "Order", "OrderTable", TestDomainStorageProviderDefinition, typeof (Order), false);
      _propertyInfo = MockRepository.GenerateStub<IPropertyInformation>();
    }

    [Test]
    public void Initialize ()
    {
      var actual = new PropertyDefinition (
          _classDefinition, _propertyInfo, "Test", typeof (string), false, false, null, StorageClass.Persistent);

      Assert.That (actual.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (actual.IsNullable, Is.False);
      Assert.That (actual.MaxLength, Is.Null);
      Assert.That (actual.PropertyName, Is.EqualTo ("Test"));
      Assert.That (actual.PropertyType, Is.EqualTo (typeof (string)));
      Assert.That (actual.StorageClass, Is.EqualTo (StorageClass.Persistent));
      Assert.That (actual.IsObjectID, Is.False);
      Assert.That (actual.PropertyInfo, Is.SameAs (_propertyInfo));
    }

    [Test]
    public void Initialize_WithReferenceType_IsNullableFalse ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (string), false, false, null, StorageClass.Persistent);

      Assert.That (actual.IsNullable, Is.False);
    }

    [Test]
    public void Initialize_WithReferenceType_IsNullableTrue ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (string), false, true, null, StorageClass.Persistent);

      Assert.That (actual.IsNullable, Is.True);
    }

    [Test]
    public void Initialize_WithNonNullableValueType_IsNullableFalse ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int), false, false, null, StorageClass.Persistent);

      Assert.That (actual.IsNullable, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Properties cannot be nullable when they have a non-nullable value type.\r\n  Property: Test")]
    public void Initialize_WithNonNullableValueType_IsNullableTrue ()
    {
      new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int), false, true, null, StorageClass.Persistent);
    }

    [Test]
    public void Initialize_WithNullableValueType_IsNullableFalse ()
    {
      var actual = new PropertyDefinition (
          _classDefinition, _propertyInfo, "Test", typeof (int?), false, false, null, StorageClass.Persistent);

      Assert.That (actual.IsNullable, Is.False);
    }

    [Test]
    public void Initialize_WithNullableValueType_IsNullableTrue ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int?), false, true, null, StorageClass.Persistent);

      Assert.That (actual.IsNullable, Is.True);
    }

    [Test]
    public void Initialize_WithString_MaxLengthSet ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (string), false, true, 50, StorageClass.Persistent);

      Assert.That (actual.MaxLength, Is.EqualTo (50));
    }

    [Test]
    public void Initialize_WithString_MaxLengthNotSet ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (string), false, true, null, StorageClass.Persistent);

      Assert.That (actual.MaxLength, Is.Null);
    }

    [Test]
    public void Initialize_WithByteArray_MaxLengthSet ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (byte[]), false, true, 50, StorageClass.Persistent);

      Assert.That (actual.MaxLength, Is.EqualTo (50));
    }

    [Test]
    public void Initialize_WithByteArrayString_MaxLengthNotSet ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (byte[]), false, true, null, StorageClass.Persistent);

      Assert.That (actual.MaxLength, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type 'System.Int32'.\r\n  Property: Test")]
    public void Initialize_WithOtherType_MaxLengthSet ()
    {
      new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int), false, false, 50, StorageClass.Persistent);
    }

    [Test]
    public void Initialize_WithOtherType_MaxLengthNotSet ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int), false, false, null, StorageClass.Persistent);

      Assert.That (actual.MaxLength, Is.Null);
    }

    [Test]
    public void IsObjectID_True ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (ObjectID), true, false, null, StorageClass.Persistent);
      Assert.IsTrue (actual.IsObjectID);
    }

    [Test]
    public void IsObjectID_False ()
    {
      var actual = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int), false, false, null, StorageClass.Persistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void DefaultValue_Nullable ()
    {
      var nullableValueProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int?), false, true, null, StorageClass.Persistent);
      Assert.That (nullableValueProperty.DefaultValue, Is.Null);

      var nullableReferenceProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (string), false, true, null, StorageClass.Persistent);
      Assert.That (nullableReferenceProperty.DefaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_NotNullable_Array ()
    {
      var nullableValueProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (byte[]), false, false, null, StorageClass.Persistent);
      Assert.That (nullableValueProperty.DefaultValue, Is.EqualTo (new byte[0]));
    }

    [Test]
    public void DefaultValue_NotNullable_String ()
    {
      var nullableValueProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (string), false, false, null, StorageClass.Persistent);
      Assert.That (nullableValueProperty.DefaultValue, Is.EqualTo (""));
    }

    [Test]
    public void DefaultValue_NotNullable_Enum ()
    {
      var nullableValueProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (EnumNotDefiningZero), false, false, null, StorageClass.Persistent);
      Assert.That (nullableValueProperty.DefaultValue, Is.EqualTo (EnumNotDefiningZero.First));
    }

    [Test]
    public void DefaultValue_NotNullable_ExtensibleEnum ()
    {
      var nullableValueProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (Color), false, false, null, StorageClass.Persistent);
      Assert.That (nullableValueProperty.DefaultValue, Is.EqualTo (Color.Values.Blue ()));
    }

    [Test]
    public void DefaultValue_NotNullable_OtherType ()
    {
      var nullableValueProperty = new PropertyDefinition (_classDefinition, _propertyInfo, "Test", typeof (int), false, false, null, StorageClass.Persistent);
      Assert.That (nullableValueProperty.DefaultValue, Is.EqualTo (0));
    }

    [Test]
    public void GetToString ()
    {
      PropertyDefinition propertyDefinition = new TestablePropertyDefinition (_classDefinition, "ThePropertyName", null, StorageClass.None);

      Assert.That (propertyDefinition.ToString (), Is.EqualTo (typeof (TestablePropertyDefinition).FullName + ": ThePropertyName"));
    }

    [Test]
    public void SetStorageProperty ()
    {
      PropertyDefinition propertyDefinition = new TestablePropertyDefinition (_classDefinition, "ThePropertyName", null, StorageClass.Persistent);
      var columnDefinition = new SimpleColumnDefinition ("Test", typeof (string), "varchar", true, false);

      propertyDefinition.SetStorageProperty (columnDefinition);

      Assert.That (propertyDefinition.StoragePropertyDefinition, Is.SameAs (columnDefinition));
    }
  }
}