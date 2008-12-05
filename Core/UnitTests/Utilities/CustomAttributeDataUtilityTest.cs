// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CustomAttributeDataUtilityTest
  {
    [Test]
    public void ParseCustomAttributeArguments_WithCtorArgumentsAndNamedProperties ()
    {
      CustomAttributeData cad = CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplicationWithCtorArgumentsAndNamedProperties))[0];
      CustomAttributeArguments arguments = CustomAttributeDataUtility.ParseCustomAttributeArguments (cad);

      CheckCtorArgs (arguments);
      CheckNamedProperties (arguments, typeof (AttributeWithPropertyParams));
    }

    [Test]
    public void ParseCustomAttributeArguments_WithCtorArgumentsNamedPropertiesAndNamedFields ()
    {
      CustomAttributeData cad =
          CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplicationWithCtorArgumentsNamedPropertiesAndNamedFields))[0];
      CustomAttributeArguments arguments = CustomAttributeDataUtility.ParseCustomAttributeArguments (cad);

      CheckCtorArgs (arguments);
      CheckNamedProperties (arguments, typeof (AttributeWithPropertyAndFieldParams));
      CheckNamedFields (arguments, typeof (AttributeWithPropertyAndFieldParams));
    }

    [Test] // Fails prior to .NET 2.0 SP1
    public void ParseCustomAttributeArguments_WithCtorArgumentsAndNamedFields ()
    {
      CustomAttributeData cad = CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplicationWithCtorArgumentsAndNamedFields))[0];
      CustomAttributeArguments arguments = CustomAttributeDataUtility.ParseCustomAttributeArguments (cad);

      CheckCtorArgs (arguments);
      CheckNamedFields (arguments, typeof (AttributeWithFieldParams));
    }

    [Test]
    public void DefaultCtor ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("DefaultCtor");
      Assert.IsNull (attribute.T);
      Assert.IsNull (attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void DefaultCtorWithProperty ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("DefaultCtorWithProperty");
      Assert.IsNull (attribute.T);
      Assert.AreEqual ("foo", attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void DefaultCtorWithField ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("DefaultCtorWithField");
      Assert.AreEqual (typeof (object), attribute.T);
      Assert.IsNull (attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void CtorWithTypeAndProperty ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithTypeAndProperty");
      Assert.AreEqual (typeof (void), attribute.T);
      Assert.AreEqual ("string", attribute.S);
      Assert.IsNull (attribute.Os);
    }

    [Test]
    public void CtorWithStringAndParamsArray ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithStringAndParamsArray");
      Assert.IsNull (attribute.T);
      Assert.AreEqual ("s", attribute.S);
      Assert.That (attribute.Os, Is.EqualTo (new object[] { 1, 2, 3, "4" }));
    }

    [Test]
    public void CtorWithStringAndTypeParamsArray ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithStringAndTypeParamsArray");
      Assert.AreEqual (typeof (double), attribute.T);
      Assert.IsNull (attribute.S);
      Assert.That (attribute.Ts, Is.EqualTo (new Type[] { typeof (int), typeof (string) }));
    }

    [Test]
    public void CtorWithIntArray ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithIntArray");
      Assert.That (attribute.Is, Is.EqualTo (new int[] { 1, 2, 3 }));
    }

    [Test]
    public void CtorWithEnumArray ()
    {
      ComplexAttribute attribute = GetInstantiatedAttribute ("CtorWithEnumArray");
      Assert.That (attribute.Es, Is.EqualTo (new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday }));
    }

    private static ComplexAttribute GetInstantiatedAttribute (string methodName)
    {
      CustomAttributeData data = CustomAttributeData.GetCustomAttributes (typeof (ComplexAttributeTarget).GetMethod (methodName))[0];
      return (ComplexAttribute) CustomAttributeDataUtility.InstantiateCustomAttributeData (data);
    }

    private static void CheckCtorArgs (CustomAttributeArguments arguments)
    {
      Assert.AreEqual (8, arguments.ConstructorArgs.Length);

      Assert.AreEqual (1, arguments.ConstructorArgs[0]);
      Assert.AreEqual ("1", arguments.ConstructorArgs[1]);
      Assert.AreEqual (null, arguments.ConstructorArgs[2]);
      Assert.AreEqual (typeof (object), arguments.ConstructorArgs[3]);

      Assert.That (arguments.ConstructorArgs[4], Is.EqualTo (new[] { 2, 3 }));
      Assert.That (arguments.ConstructorArgs[5], Is.EqualTo (new[] { "2", "3" }));
      Assert.That (arguments.ConstructorArgs[6], Is.EqualTo (new object[] { null, "foo", typeof (object) }));
      Assert.That (arguments.ConstructorArgs[7], Is.EqualTo (new[] { typeof (string), typeof (int), typeof (double) }));
    }

    private static void CheckNamedProperties (CustomAttributeArguments arguments, Type attributeType)
    {
      Assert.AreEqual (8, arguments.NamedProperties.Length);

      Assert.AreEqual (attributeType.GetProperty ("INamed"), arguments.NamedProperties[0]);
      Assert.AreEqual (attributeType.GetProperty ("SNamed"), arguments.NamedProperties[1]);
      Assert.AreEqual (attributeType.GetProperty ("ONamed"), arguments.NamedProperties[2]);
      Assert.AreEqual (attributeType.GetProperty ("TNamed"), arguments.NamedProperties[3]);

      Assert.AreEqual (attributeType.GetProperty ("INamedArray"), arguments.NamedProperties[4]);
      Assert.AreEqual (attributeType.GetProperty ("SNamedArray"), arguments.NamedProperties[5]);
      Assert.AreEqual (attributeType.GetProperty ("ONamedArray"), arguments.NamedProperties[6]);
      Assert.AreEqual (attributeType.GetProperty ("TNamedArray"), arguments.NamedProperties[7]);

      Assert.AreEqual (8, arguments.PropertyValues.Length);

      Assert.AreEqual (5, arguments.PropertyValues[0]);
      Assert.AreEqual ("P5", arguments.PropertyValues[1]);
      Assert.AreEqual ("Pbla", arguments.PropertyValues[2]);
      Assert.AreEqual (typeof (float), arguments.PropertyValues[3]);

      Assert.That (arguments.PropertyValues[4], Is.EquivalentTo (new[] { 1, 2, 3 }));
      Assert.That (arguments.PropertyValues[5], Is.EquivalentTo (new[] { "P1", null, "P2" }));
      Assert.That (arguments.PropertyValues[6], Is.EquivalentTo (new object[] { 1, 2, null }));
      Assert.That (arguments.PropertyValues[7], Is.EquivalentTo (new[] { typeof (Random), null }));
    }

    private static void CheckNamedFields (CustomAttributeArguments arguments, Type attributeType)
    {
      Assert.AreEqual (8, arguments.NamedFields.Length);

      Assert.AreEqual (attributeType.GetField ("INamedF"), arguments.NamedFields[0]);
      Assert.AreEqual (attributeType.GetField ("SNamedF"), arguments.NamedFields[1]);
      Assert.AreEqual (attributeType.GetField ("ONamedF"), arguments.NamedFields[2]);
      Assert.AreEqual (attributeType.GetField ("TNamedF"), arguments.NamedFields[3]);

      Assert.AreEqual (attributeType.GetField ("INamedArrayF"), arguments.NamedFields[4]);
      Assert.AreEqual (attributeType.GetField ("SNamedArrayF"), arguments.NamedFields[5]);
      Assert.AreEqual (attributeType.GetField ("ONamedArrayF"), arguments.NamedFields[6]);
      Assert.AreEqual (attributeType.GetField ("TNamedArrayF"), arguments.NamedFields[7]);

      Assert.AreEqual (8, arguments.FieldValues.Length);

      Assert.AreEqual (5, arguments.FieldValues[0]);
      Assert.AreEqual ("5", arguments.FieldValues[1]);
      Assert.AreEqual ("bla", arguments.FieldValues[2]);
      Assert.AreEqual (typeof (float), arguments.FieldValues[3]);

      Assert.That (arguments.FieldValues[4], Is.EquivalentTo (new[] { 1, 2, 3 }));
      Assert.That (arguments.FieldValues[5], Is.EquivalentTo (new[] { "1", null, "2" }));
      Assert.That (arguments.FieldValues[6], Is.EquivalentTo (new object[] { 1, 2, null }));
      Assert.That (arguments.FieldValues[7], Is.EquivalentTo (new[] { typeof (Random), null }));
    }
  }
}
