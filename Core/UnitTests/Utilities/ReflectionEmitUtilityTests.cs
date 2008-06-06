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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class ReflectionEmitUtilityTests
  {
    public class AttributeWithPropertyParams : Attribute
    {
      public AttributeWithPropertyParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
      {
      }

      public int INamed { get { return 0; } set { } }
      public string SNamed { get { return null; } set { } }
      public object ONamed { get { return null; } set { } }
      public Type TNamed { get { return null; }  set { } }

      public int[] INamedArray { get { return null; }  set { } }
      public string[] SNamedArray { get { return null; }  set { } }
      public object[] ONamedArray { get { return null; }  set { } }
      public Type[] TNamedArray { get { return null; }  set { } }
    }

    public class AttributeWithFieldParams : Attribute
    {
      public AttributeWithFieldParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
      {
      }

      public int INamedF;
      public string SNamedF;
      public object ONamedF;
      public Type TNamedF;

      public int[] INamedArrayF;
      public string[] SNamedArrayF;
      public object[] ONamedArrayF;
      public Type[] TNamedArrayF;
    }

    public class AttributeWithPropertyAndFieldParams : Attribute
    {
      public AttributeWithPropertyAndFieldParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
      {
      }

      public int INamed { get { return 0; } set { } }
      public string SNamed { get { return null; } set { } }
      public object ONamed { get { return null; } set { } }
      public Type TNamed { get { return null; } set { } }

      public int[] INamedArray { get { return null; } set { } }
      public string[] SNamedArray { get { return null; } set { } }
      public object[] ONamedArray { get { return null; } set { } }
      public Type[] TNamedArray { get { return null; } set { } }

      public int INamedF;
      public string SNamedF;
      public object ONamedF;
      public Type TNamedF;

      public int[] INamedArrayF;
      public string[] SNamedArrayF;
      public object[] ONamedArrayF;
      public Type[] TNamedArrayF;
    }

    [AttributeWithPropertyParams (
        1, 
        "1", 
        null, 
        typeof (object),
      
        new int[]{2, 3}, 
        new string[] {"2", "3"}, 
        new object[] {null, "foo", typeof (object)}, new Type[] {typeof (string), typeof (int), typeof(double)},
      
        INamed = 5, 
        SNamed = "P5", 
        ONamed = "Pbla", 
        TNamed = typeof (float),

        INamedArray = new int[] {1, 2, 3}, 
        SNamedArray = new string[] {"P1", null, "P2"}, 
        ONamedArray = new object[] {1, 2, null}, 
        TNamedArray = new Type[] {typeof (Random), null}
        )]
    public class TestAttributeApplicationWithCtorArgumentsAndNamedProperties
    {
    }

    [AttributeWithFieldParams (
        1,
        "1",
        null,
        typeof (object),

        new int[] { 2, 3 },
        new string[] { "2", "3" },
        new object[] { null, "foo", typeof (object) }, new Type[] { typeof (string), typeof (int), typeof (double) },

        INamedF = 5,
        SNamedF = "5",
        ONamedF = "bla",
        TNamedF = typeof (float),

        INamedArrayF = new int[] { 1, 2, 3 },
        SNamedArrayF = new string[] { "1", null, "2" },
        ONamedArrayF = new object[] { 1, 2, null },
        TNamedArrayF = new Type[] { typeof (Random), null }
        )]
    public class TestAttributeApplicationWithCtorArgumentsAndNamedFields
    {
    }

    [AttributeWithPropertyAndFieldParams (
        1,
        "1",
        null,
        typeof (object),

        new int[] { 2, 3 },
        new string[] { "2", "3" },
        new object[] { null, "foo", typeof (object) }, new Type[] { typeof (string), typeof (int), typeof (double) },

        INamed = 5, 
        SNamed = "P5", 
        ONamed = "Pbla", 
        TNamed = typeof (float),

        INamedArray = new int[] {1, 2, 3}, 
        SNamedArray = new string[] {"P1", null, "P2"}, 
        ONamedArray = new object[] {1, 2, null}, 
        TNamedArray = new Type[] {typeof (Random), null},

        INamedF = 5, 
        SNamedF = "5",
        ONamedF = "bla",
        TNamedF = typeof (float),

        INamedArrayF = new int[] { 1, 2, 3 },
        SNamedArrayF = new string[] { "1", null, "2" },
        ONamedArrayF = new object[] { 1, 2, null },
        TNamedArrayF = new Type[] { typeof (Random), null }
        )]
    public class TestAttributeApplicationWithCtorArgumentsNamedPropertiesAndNamedFields
    {
    }

    private static void CheckCtorArgs (ReflectionEmitUtility.CustomAttributeBuilderData data)
    {
      Assert.AreEqual (8, data.ConstructorArgs.Length);

      Assert.AreEqual (1, data.ConstructorArgs[0]);
      Assert.AreEqual ("1", data.ConstructorArgs[1]);
      Assert.AreEqual (null, data.ConstructorArgs[2]);
      Assert.AreEqual (typeof (object), data.ConstructorArgs[3]);

      Assert.That (data.ConstructorArgs[4], Is.EqualTo (new int[] { 2, 3 }));
      Assert.That (data.ConstructorArgs[5], Is.EqualTo (new string[] { "2", "3" }));
      Assert.That (data.ConstructorArgs[6], Is.EqualTo (new object[] { null, "foo", typeof (object) }));
      Assert.That (data.ConstructorArgs[7], Is.EqualTo (new Type[] { typeof (string), typeof (int), typeof (double) }));
    }

    private static void CheckNamedArguments (ReflectionEmitUtility.CustomAttributeBuilderData data, Type attributeType)
    {
      Assert.AreEqual (8, data.NamedProperties.Length);

      Assert.AreEqual (attributeType.GetProperty ("INamed"), data.NamedProperties[0]);
      Assert.AreEqual (attributeType.GetProperty ("SNamed"), data.NamedProperties[1]);
      Assert.AreEqual (attributeType.GetProperty ("ONamed"), data.NamedProperties[2]);
      Assert.AreEqual (attributeType.GetProperty ("TNamed"), data.NamedProperties[3]);

      Assert.AreEqual (attributeType.GetProperty ("INamedArray"), data.NamedProperties[4]);
      Assert.AreEqual (attributeType.GetProperty ("SNamedArray"), data.NamedProperties[5]);
      Assert.AreEqual (attributeType.GetProperty ("ONamedArray"), data.NamedProperties[6]);
      Assert.AreEqual (attributeType.GetProperty ("TNamedArray"), data.NamedProperties[7]);

      Assert.AreEqual (8, data.PropertyValues.Length);

      Assert.AreEqual (5, data.PropertyValues[0]);
      Assert.AreEqual ("P5", data.PropertyValues[1]);
      Assert.AreEqual ("Pbla", data.PropertyValues[2]);
      Assert.AreEqual (typeof (float), data.PropertyValues[3]);

      Assert.That (data.PropertyValues[4], Is.EquivalentTo (new int[] { 1, 2, 3 }));
      Assert.That (data.PropertyValues[5], Is.EquivalentTo (new string[] { "P1", null, "P2" }));
      Assert.That (data.PropertyValues[6], Is.EquivalentTo (new object[] { 1, 2, null }));
      Assert.That (data.PropertyValues[7], Is.EquivalentTo (new Type[] { typeof (Random), null }));
    }

    private static void CheckNamedFields (ReflectionEmitUtility.CustomAttributeBuilderData data, Type attributeType)
    {
      Assert.AreEqual (8, data.NamedFields.Length);

      Assert.AreEqual (attributeType.GetField ("INamedF"), data.NamedFields[0]);
      Assert.AreEqual (attributeType.GetField ("SNamedF"), data.NamedFields[1]);
      Assert.AreEqual (attributeType.GetField ("ONamedF"), data.NamedFields[2]);
      Assert.AreEqual (attributeType.GetField ("TNamedF"), data.NamedFields[3]);

      Assert.AreEqual (attributeType.GetField ("INamedArrayF"), data.NamedFields[4]);
      Assert.AreEqual (attributeType.GetField ("SNamedArrayF"), data.NamedFields[5]);
      Assert.AreEqual (attributeType.GetField ("ONamedArrayF"), data.NamedFields[6]);
      Assert.AreEqual (attributeType.GetField ("TNamedArrayF"), data.NamedFields[7]);

      Assert.AreEqual (8, data.FieldValues.Length);

      Assert.AreEqual (5, data.FieldValues[0]);
      Assert.AreEqual ("5", data.FieldValues[1]);
      Assert.AreEqual ("bla", data.FieldValues[2]);
      Assert.AreEqual (typeof (float), data.FieldValues[3]);

      Assert.That (data.FieldValues[4], Is.EquivalentTo (new int[] { 1, 2, 3 }));
      Assert.That (data.FieldValues[5], Is.EquivalentTo (new string[] { "1", null, "2" }));
      Assert.That (data.FieldValues[6], Is.EquivalentTo (new object[] { 1, 2, null }));
      Assert.That (data.FieldValues[7], Is.EquivalentTo (new Type[] { typeof (Random), null }));
    }

    [Test]
    public void CreateAttributeBuilderFromData_WithCtorArgumentsAndNamedProperties ()
    {
      CustomAttributeData cad = CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplicationWithCtorArgumentsAndNamedProperties))[0];
      ReflectionEmitUtility.CustomAttributeBuilderData data = ReflectionEmitUtility.GetCustomAttributeBuilderData (cad);

      CheckCtorArgs(data);
      CheckNamedArguments(data, typeof (AttributeWithPropertyParams));
    }

    [Test]
    [Ignore ("Due to a bug in the .NET framework, this seems not to work on all .NET 2.0 installations at the moment. Waiting for a service pack...")]
    public void CreateAttributeBuilderFromData_WithCtorArgumentsNamedPropertiesAndNamedFields ()
    {
      CustomAttributeData cad = CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplicationWithCtorArgumentsNamedPropertiesAndNamedFields))[0];
      ReflectionEmitUtility.CustomAttributeBuilderData data = ReflectionEmitUtility.GetCustomAttributeBuilderData (cad);

      CheckCtorArgs (data);
      CheckNamedArguments (data, typeof (AttributeWithPropertyAndFieldParams));
      CheckNamedFields (data, typeof (AttributeWithPropertyAndFieldParams));
    }

    [Test (Description = "This traps the framework bug with named fields. Can be removed once "
                         + "CreateAttributeBuilderFromData_WithCtorArgumentsAndNamedFields works as intended.")]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Type .*AttributeWithFieldParams declares "
                                                                          + "public fields: .*. Due to a bug in CustomAttributeData.GetCustomAttributes, attributes with public fields are currently not supported.",
        MatchType = MessageMatch.Regex)]
    public void CreateAttributeBuilderFromData_WithCtorArgumentsAndNamedFields ()
    {
      CustomAttributeData cad = CustomAttributeData.GetCustomAttributes (typeof (TestAttributeApplicationWithCtorArgumentsAndNamedFields))[0];
      ReflectionEmitUtility.CustomAttributeBuilderData data = ReflectionEmitUtility.GetCustomAttributeBuilderData (cad);
    }
  }
}
