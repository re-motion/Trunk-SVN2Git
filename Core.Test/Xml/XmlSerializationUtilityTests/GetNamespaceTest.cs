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
using System.Reflection.Emit;
using System.Xml.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using Remotion.Xml;

namespace Remotion.UnitTests.Xml.XmlSerializationUtilityTests
{
  [TestFixture]
  public class GetNamespaceTest
  {
    [Test]
    public void WithXmlTypeAttribute()
    {
      Type type = CreateType ("SampleType", CreateXmlTypeAttributeBuilder ("http://type-namespace"));
      Assert.AreEqual ("http://type-namespace", XmlSerializationUtility.GetNamespace (type));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot determine the xml namespace of type 'SampleType' because neither an "
        + "XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.\r\nParameter name: type")]
    public void WithXmlTypeAttributeWithoutNamespace()
    {
      Type type = CreateType ("SampleType", CreateXmlTypeAttributeBuilder (null));
      XmlSerializationUtility.GetNamespace (type);
    }

    [Test]
    public void WithXmlRootAttribute()
    {
      Type type = CreateType ("SampleType", CreateXmlRootAttributeBuilder ("http://root-namespace"));
      Assert.AreEqual ("http://root-namespace", XmlSerializationUtility.GetNamespace (type));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot determine the xml namespace of type 'SampleType' because neither an "
        + "XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.\r\nParameter name: type")]
    public void WithXmlRootAttributeWithoutNamespace()
    {
      Type type = CreateType ("SampleType", CreateXmlRootAttributeBuilder (null));
      XmlSerializationUtility.GetNamespace (type);
    }

    [Test]
    public void WithXmlRootAttributeWithTypeAlsoHavingAnXmlTypeAttribute()
    {
      Type type = CreateType (
          "SampleType",
          CreateXmlTypeAttributeBuilder ("http://type-namespace"),
          CreateXmlRootAttributeBuilder ("http://root-namespace"));
      Assert.AreEqual ("http://root-namespace", XmlSerializationUtility.GetNamespace (type));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot determine the xml namespace of type 'SampleType' because no neither an XmlTypeAttribute"
        + " nor an XmlRootAttribute has been provided.\r\nParameter name: type")]
    public void WithoutXmlRootAttributeAndWithoutXmlTypeAttribute()
    {
      Type type = CreateType ("SampleType");
      XmlSerializationUtility.GetNamespace (type);
    }

    private Type CreateType (string typeName, params CustomAttributeBuilder[] attributeBuilders)
    {
      ModuleScope moduleScope = new ModuleScope();
      ClassEmitter classEmitter = new ClassEmitter (moduleScope, typeName, typeof (object), new Type [0]);
      foreach (CustomAttributeBuilder attributeBuilder in attributeBuilders)
        classEmitter.TypeBuilder.SetCustomAttribute (attributeBuilder);

      return classEmitter.BuildType ();
    }

    private CustomAttributeBuilder CreateXmlTypeAttributeBuilder (string @namespace)
    {
      ConstructorInfo constructorInfo = typeof (XmlTypeAttribute).GetConstructor (new Type[0]);
      PropertyInfo namespacePropertyInfo = typeof (XmlTypeAttribute).GetProperty ("Namespace");

      return new CustomAttributeBuilder (constructorInfo, new object[0], new PropertyInfo[] {namespacePropertyInfo}, new object[] {@namespace});
    }

    private CustomAttributeBuilder CreateXmlRootAttributeBuilder (string @namespace)
    {
      ConstructorInfo constructorInfo = typeof (XmlRootAttribute).GetConstructor (new Type[0]);
      PropertyInfo namespacePropertyInfo = typeof (XmlRootAttribute).GetProperty ("Namespace");

      return new CustomAttributeBuilder (constructorInfo, new object[0], new PropertyInfo[] {namespacePropertyInfo}, new object[] {@namespace});
    }
  }
}
