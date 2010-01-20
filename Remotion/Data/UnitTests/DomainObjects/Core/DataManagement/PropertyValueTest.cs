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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ExtensibleEnums;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class PropertyValueTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _orderClassDefinition;
    private PropertyDefinition _orderNumberPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", c_testDomainProviderID, typeof (Order), false);
      _orderNumberPropertyDefinition = CreatePropertyDefinition ("OrderNumber", typeof (int), null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The property 'test' (declared on class 'Order') is invalid because its "
                                                                          +
                                                                          "values cannot be copied. Only value types, strings, the Type type, byte arrays, and ObjectIDs are currently supported, but the property's "
                                                                          +
                                                                          "type is 'System.Collections.Generic.List`1[[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]'."
        )]
    public void PropertyValue_WithReferenceType_NotAllowed ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (List<object>), null);
      new PropertyValue (propertyDefinition, null);
    }

    [Test]
    public void PropertyValue_WithValueType_Allowed ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (DateTime), null);
      var propertyValue = new PropertyValue (propertyDefinition, DateTime.Now);
      Assert.That (propertyValue.Definition.PropertyType, Is.EqualTo (typeof (DateTime)));
    }

    [Test]
    public void PropertyValue_WithString_Allowed ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (string), null);
      var propertyValue = new PropertyValue (propertyDefinition, null);
      Assert.That (propertyValue.Definition.PropertyType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void PropertyValue_WithType_Allowed ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (Type), null);
      var propertyValue = new PropertyValue (propertyDefinition, null);
      Assert.That (propertyValue.Definition.PropertyType, Is.EqualTo (typeof (Type)));
    }

    [Test]
    public void PropertyValue_WithExtensibleEnum_Allowed ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (Color), true);
      var propertyValue = new PropertyValue (propertyDefinition, null);
      Assert.That (propertyValue.Definition.PropertyType, Is.EqualTo (typeof (Color)));
    }

    [Test]
    public void TestEquals ()
    {
      PropertyDefinition intDefinition = CreateIntPropertyDefinition ("test");
      var propertyValue1 = new PropertyValue (intDefinition, 5);
      var propertyValue2 = new PropertyValue (intDefinition, 5);
      Assert.IsTrue (propertyValue1.Equals (propertyValue2), "Initial values");

      propertyValue1.Value = 10;
      Assert.IsFalse (propertyValue1.Equals (propertyValue2), "After changing first value.");

      propertyValue1.Value = 5;
      Assert.IsTrue (propertyValue1.Equals (propertyValue2), "After changing first value back to initial value.");

      propertyValue1.Value = 10;
      propertyValue2.Value = 10;
      Assert.IsTrue (propertyValue1.Equals (propertyValue2), "After changing both values.");

      PropertyValue propertyValue3 = CreateIntPropertyValue ("test", 10);
      propertyValue3.Value = 10;
      Assert.IsFalse (propertyValue1.Equals (propertyValue3), "Different original values.");
    }

    [Test]
    public void HashCode ()
    {
      PropertyValue propertyValue1 = CreateIntPropertyValue ("test", 5);
      PropertyValue propertyValue2 = CreateIntPropertyValue ("test", 5);
      Assert.IsTrue (propertyValue1.GetHashCode() == propertyValue2.GetHashCode(), "Initial values");

      propertyValue1.Value = 10;
      Assert.IsFalse (propertyValue1.GetHashCode() == propertyValue2.GetHashCode(), "After changing first value.");

      propertyValue1.Value = 5;
      Assert.IsTrue (propertyValue1.GetHashCode() == propertyValue2.GetHashCode(), "After changing first value back to initial value.");

      propertyValue1.Value = 10;
      propertyValue2.Value = 10;
      Assert.IsTrue (propertyValue1.GetHashCode() == propertyValue2.GetHashCode(), "After changing both values.");

      PropertyValue propertyValue3 = CreateIntPropertyValue ("test", 10);
      Assert.IsFalse (propertyValue1.GetHashCode() == propertyValue3.GetHashCode(), "Different original values.");
    }

    [Test]
    public void IsRelationProperty_False ()
    {
      PropertyDefinition intDefinition = CreateIntPropertyDefinition ("test");
      var propertyValue1 = new PropertyValue (intDefinition, 5);
      Assert.IsFalse (propertyValue1.IsRelationProperty);
    }

    [Test]
    public void IsRelationProperty_True ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (ObjectID), null);
      var propertyValue1 = new PropertyValue (propertyDefinition, null);
      Assert.IsTrue (propertyValue1.IsRelationProperty);
    }

    [Test]
    public void SettingOfValueForValueType ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue ("test", 5);

      Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
      Assert.AreEqual (5, propertyValue.Value, "Value after initialization");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse (propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = 5;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
      Assert.AreEqual (5, propertyValue.Value, "Value after change #1");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = 10;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
      Assert.AreEqual (10, propertyValue.Value, "Value after change #2");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = 20;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
      Assert.AreEqual (20, propertyValue.Value, "Value after change #3");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #3");

      propertyValue.Value = 5;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #4");
      Assert.AreEqual (5, propertyValue.Value, "Value after change #4");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #4");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #4");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #4");
    }

    [Test]
    public void SettingOfNullValueForNullableValueType ()
    {
      PropertyValue propertyValue = CreateNullableIntPropertyValue ("test", null);

      Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
      Assert.IsNull (propertyValue.Value, "Value after initialization");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse (propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
      Assert.IsNull (propertyValue.Value, "Value after change #1");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = 10;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
      Assert.AreEqual (10, propertyValue.Value, "Value after change #2");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
      Assert.IsNull (propertyValue.Value, "Value after change #3");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    public void SettingOfNullValueForString ()
    {
      PropertyValue propertyValue = CreateStringPropertyValue ("test", null);

      Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
      Assert.IsNull (propertyValue.Value, "Value after initialization");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse (propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
      Assert.IsNull (propertyValue.Value, "Value after change #1");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = "Test Value";

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
      Assert.AreEqual ("Test Value", propertyValue.Value, "Value after change #2");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
      Assert.IsNull (propertyValue.Value, "Value after change #3");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException))]
    public void MaxLengthCheck ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (string), 10);
      var propertyValue = new PropertyValue (definition, "12345");
      propertyValue.Value = "12345678901";
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException))]
    public void MaxLengthCheckInConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (string), 10);
      new PropertyValue (definition, "12345678901");
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void TypeCheckInConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (string), 10);
      new PropertyValue (definition, 123);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void TypeCheck ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (string), 10);
      var propertyValue = new PropertyValue (definition, "123");
      propertyValue.Value = 123;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableStringToNull ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (string), false, 10);
      var propertyValue = new PropertyValue (definition, string.Empty);

      propertyValue.Value = null;
    }

    [Test]
    public void SetNullableBinary ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (byte[]), true);
      var propertyValue = new PropertyValue (definition, null);
      Assert.IsNull (propertyValue.Value);
    }

    [Test]
    public void SetNotNullableBinary ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (byte[]), false);

      var propertyValue = new PropertyValue (definition, new byte[0]);
      ResourceManager.IsEmptyImage ((byte[]) propertyValue.Value);

      propertyValue.Value = ResourceManager.GetImage1();
      ResourceManager.IsEqualToImage1 ((byte[]) propertyValue.Value);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void SetBinaryWithInvalidType ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (byte[]), false);
      new PropertyValue (definition, new int[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableBinaryToNullViaConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (byte[]), false);
      new PropertyValue (definition, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableBinaryToNullViaProperty ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (byte[]), false);
      var propertyValue = new PropertyValue (definition, ResourceManager.GetImage1());
      propertyValue.Value = null;
    }

    [Test]
    public void SetNullableExtensibleEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (Color),
          true);

      var propertyValue = new PropertyValue (definition, null);
      Assert.IsNull (propertyValue.Value);
    }

    [Test]
    public void SetNotNullableExtensibleEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (Color),
          false);

      var propertyValue = new PropertyValue (definition, ExtensibleEnum<Color>.Values.Red());
      Assert.AreEqual (ExtensibleEnum<Color>.Values.Red(), propertyValue.Value);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void SetExtensibleEnumWithInvalidType ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (Color),
          false);
      new PropertyValue (definition, 12);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableExtensibleEnumToNullViaConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (Color),
          false);
      new PropertyValue (definition, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableExtensibleEnumToNullViaProperty ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (Color),
          false);
      var propertyValue = new PropertyValue (definition, ExtensibleEnum<Color>.Values.Red());
      propertyValue.Value = null;
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException), ExpectedMessage = "Value for property 'test' is too large. Maximum size: 1000000.")]
    public void SetBinaryLargerThanMaxLength ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (byte[]), true, 1000000);
      var propertyValue = new PropertyValue (definition, new byte[0]);
      propertyValue.Value = ResourceManager.GetImageLarger1MB();
    }

    [Test]
    public void EnumCheck_ValidNonFlagsEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (DayOfWeek));

      var propertyValue = new PropertyValue (definition, DayOfWeek.Monday);
      propertyValue.Value = DayOfWeek.Monday;
      Assert.That (propertyValue.Value, Is.EqualTo (DayOfWeek.Monday));
    }

    [Test]
    [ExpectedException (typeof (InvalidEnumValueException), ExpectedMessage = "Value '17420' for property 'test' is not defined by enum type "
                                                                              + "'System.DayOfWeek'.")]
    public void EnumCheck_InvalidNonFlagsEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (DayOfWeek));

      var propertyValue = new PropertyValue (definition, DayOfWeek.Monday);
      propertyValue.Value = (DayOfWeek) 17420;
    }

    [Test]
    public void EnumCheck_ValidFlagsEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (AttributeTargets));

      var propertyValue = new PropertyValue (definition, AttributeTargets.Method);
      propertyValue.Value = AttributeTargets.Field | AttributeTargets.Method;
      Assert.That (propertyValue.Value, Is.EqualTo (AttributeTargets.Field | AttributeTargets.Method));
    }

    [Test]
    [ExpectedException (typeof (InvalidEnumValueException), ExpectedMessage = "Value '-1' for property 'test' is not defined by enum type "
                                                                              + "'System.AttributeTargets'.")]
    public void EnumCheck_InvalidFlagsEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (AttributeTargets));

      var propertyValue = new PropertyValue (definition, AttributeTargets.Method);
      propertyValue.Value = (AttributeTargets) (-1);
    }

    [Test]
    public void EnumCheck_ValidNullEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (DayOfWeek?));

      var propertyValue = new PropertyValue (definition, DayOfWeek.Monday);
      propertyValue.Value = DayOfWeek.Monday;
      Assert.That (propertyValue.Value, Is.EqualTo (DayOfWeek.Monday));
      propertyValue.Value = null;
      Assert.That (propertyValue.Value, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidEnumValueException), ExpectedMessage = "Value '17420' for property 'test' is not defined by enum type "
                                                                              + "'System.DayOfWeek'.")]
    public void EnumCheck_InvalidNullEnum ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (DayOfWeek?));

      var propertyValue = new PropertyValue (definition, DayOfWeek.Monday);
      propertyValue.Value = (DayOfWeek) 17420;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void EnumCheck_InvalidNonNullEnum_Null ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (DayOfWeek));

      var propertyValue = new PropertyValue (definition, DayOfWeek.Monday);
      propertyValue.Value = null;
    }

    [Test]
    [ExpectedException (typeof (InvalidEnumValueException), ExpectedMessage = "Value '17420' for property 'test' is not defined by enum type "
                                                                              + "'System.DayOfWeek'.")]
    public void EnumCheckInConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition,
          "test",
          "test",
          typeof (DayOfWeek));

      new PropertyValue (definition, (DayOfWeek) 17420);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The relation property 'test' cannot be set directly.")]
    public void SetRelationPropertyDirectly ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, "test", "test", typeof (ObjectID), true);
      var propertyValue = new PropertyValue (definition, null);

      propertyValue.Value = DomainObjectIDs.Customer1;
    }

    [Test]
    public void Commit ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue ("testProperty", 0);
      propertyValue.Value = 5;
      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (5, propertyValue.Value);
      Assert.IsTrue (propertyValue.HasChanged);
      Assert.IsTrue (propertyValue.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "CommitState");

      Assert.AreEqual (5, propertyValue.OriginalValue);
      Assert.AreEqual (5, propertyValue.Value);
      Assert.IsFalse (propertyValue.HasChanged);
      Assert.IsFalse (propertyValue.HasBeenTouched);
    }

    [Test]
    public void Rollback ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue ("testProperty", 0);
      propertyValue.Value = 5;
      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (5, propertyValue.Value);
      Assert.IsTrue (propertyValue.HasChanged);
      Assert.IsTrue (propertyValue.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "RollbackState");

      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (0, propertyValue.Value);
      Assert.IsFalse (propertyValue.HasChanged);
      Assert.IsFalse (propertyValue.HasBeenTouched);
    }

    [Test]
    public void SetValueFrom_SetsValue ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue (_orderNumberPropertyDefinition, 2);

      target.SetValueFrom (source);

      Assert.That (target.Value, Is.EqualTo (1));
    }

    [Test]
    public void SetValueFrom_SetsDiscarded ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      PrivateInvoke.InvokeNonPublicMethod (source, "Discard");

      var target = new PropertyValue (_orderNumberPropertyDefinition, 2);
      Assert.That (target.IsDiscarded, Is.False);

      target.SetValueFrom (source);

      Assert.That (target.IsDiscarded, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfPropertyWasTouched ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue (_orderNumberPropertyDefinition, 1);

      target.Touch();
      Assert.That (source.HasBeenTouched, Is.False);
      Assert.That (target.HasBeenTouched, Is.True);

      target.SetValueFrom (source);

      Assert.That (target.HasChanged, Is.False);
      Assert.That (target.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfSourcePropertyWasTouched ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue (_orderNumberPropertyDefinition, 1);

      source.Touch();
      Assert.That (source.HasBeenTouched, Is.True);
      Assert.That (target.HasBeenTouched, Is.False);

      target.SetValueFrom (source);

      Assert.That (target.HasChanged, Is.False);
      Assert.That (target.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfDataWasChanged ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue (_orderNumberPropertyDefinition, 2);

      Assert.That (source.HasBeenTouched, Is.False);
      Assert.That (target.HasBeenTouched, Is.False);

      target.SetValueFrom (source);

      Assert.That (target.HasChanged, Is.True);
      Assert.That (target.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_FalseIfNothingHappened ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue (_orderNumberPropertyDefinition, 1);

      Assert.That (source.HasBeenTouched, Is.False);
      Assert.That (target.HasBeenTouched, Is.False);

      target.SetValueFrom (source);

      Assert.That (target.HasChanged, Is.False);
      Assert.That (target.HasBeenTouched, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot set this property's value from 'Remotion.Data.DomainObjects.Mapping.ReflectionBasedPropertyDefinition: OrderNumber'; the properties "
        + "do not have the same property definition.\r\nParameter name: source")]
    public void SetValueFrom_InvalidDefinition ()
    {
      var source = new PropertyValue (_orderNumberPropertyDefinition, 1);
      var target = new PropertyValue (CreateIntPropertyDefinition ("test"), 1);

      target.SetValueFrom (source);
    }

    [Test]
    [Ignore ("TODO 954: Fix this bug! https://dev.rubicon-it.com/jira/browse/COMMONS-954")]
    public void BinaryDataBug ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (byte[]), null);
      var propertyValue = new PropertyValue (definition, new byte[] { 1, 2, 3 });

      ((byte[]) propertyValue.Value)[0] = 7;
      Assert.That (propertyValue.HasChanged, Is.True);
      Assert.That (((byte[]) propertyValue.Value)[0], Is.EqualTo (7));
      Assert.That (((byte[]) propertyValue.OriginalValue)[0], Is.EqualTo (1));

      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "Rollback");
      Assert.That (propertyValue.HasChanged, Is.False);
      Assert.That (((byte[]) propertyValue.Value)[0], Is.EqualTo (1));

      ((byte[]) propertyValue.Value)[0] = 7;
      Assert.That (propertyValue.HasChanged, Is.True);
      Assert.That (((byte[]) propertyValue.Value)[0], Is.EqualTo (7));

      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "Commit");
      Assert.That (propertyValue.HasChanged, Is.False);
      Assert.That (((byte[]) propertyValue.Value)[0], Is.EqualTo (7));
    }

    [Test]
    public void DefaultPropertyValueWithEnumNotDefiningZero ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty", typeof (EnumNotDefiningZero), null);
      var propertyValue = new PropertyValue (definition);
      Assert.That (propertyValue.Value, Is.EqualTo (definition.DefaultValue));
      Assert.That (propertyValue.OriginalValue, Is.EqualTo (definition.DefaultValue));
    }

    [Test]
    public void GetValueWithoutEvents_Current ()
    {
      var propertyValue = new PropertyValue (_orderNumberPropertyDefinition, 11);

      propertyValue.Value = 10;
      Assert.That (propertyValue.GetValueWithoutEvents (ValueAccess.Current), Is.EqualTo (10));
    }

    [Test]
    public void GetValueWithoutEvents_Original ()
    {
      var propertyValue = new PropertyValue (_orderNumberPropertyDefinition, 11);

      propertyValue.Value = 10;
      Assert.That (propertyValue.GetValueWithoutEvents (ValueAccess.Original), Is.EqualTo (11));
    }

    [Test]
    public void GetValueWithoutEvents_NoEvents ()
    {
      var clientTransactionMock = new ClientTransactionMock();
      using (clientTransactionMock.EnterDiscardingScope())
      {
        PropertyValue propertyValue = Order.NewObject().InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"];

        ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (clientTransactionMock);

        Dev.Null = propertyValue.GetValueWithoutEvents (ValueAccess.Current);
      }
    }

    private PropertyValue CreateIntPropertyValue (string name, int intValue)
    {
      return CreatePropertyValue (name, typeof (int), null, intValue);
    }

    private PropertyValue CreateNullableIntPropertyValue (string name, int? intValue)
    {
      return CreatePropertyValue (name, typeof (int?), null, intValue);
    }

    private PropertyValue CreateStringPropertyValue (string name, string stringValue)
    {
      bool isNullable = (stringValue == null) ? true : false;
      return CreatePropertyValue (name, typeof (string), isNullable, stringValue);
    }

    private PropertyDefinition CreateIntPropertyDefinition (string name)
    {
      return CreatePropertyDefinition (name, typeof (int), null);
    }

    private PropertyDefinition CreatePropertyDefinition (string name, Type propertyType, bool? isNullable)
    {
      int? maxLength = (propertyType == typeof (string)) ? (int?) 100 : null;

      return ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          _orderClassDefinition, name, name, propertyType, isNullable, maxLength, StorageClass.Persistent);
    }

    private PropertyValue CreatePropertyValue (string name, Type propertyType, bool? isNullable, object value)
    {
      return new PropertyValue (CreatePropertyDefinition (name, propertyType, isNullable), value);
    }
  }
}
