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
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using TypeNameConverter=Remotion.Utilities.TypeNameConverter;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class TypeElement2Test
  {
    [Test]
    public void Initialize()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection) PrivateInvoke.GetNonPublicProperty (typeElement, "Properties");
      Assert.IsNotNull (properties);
      ConfigurationProperty property = properties["type"];
      Assert.IsNotNull (property);
      Assert.AreEqual (typeof (DerivedSampleType), property.DefaultValue);
      Assert.IsInstanceOfType (typeof (TypeNameConverter), property.Converter);
      Assert.IsInstanceOfType (typeof (SubclassTypeValidator), property.Validator);
      Assert.IsTrue (property.IsRequired);
    }

    [Test]
    public void GetType_WithDefaultValue()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      Assert.AreEqual (typeof (DerivedSampleType), typeElement.Type);
    }

    [Test]
    public void CreateInstance_WithoutType()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      Assert.IsInstanceOfType (typeof (DerivedSampleType), typeElement.CreateInstance());
    }

    [Test]
    public void Deserialize_WithValidType()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      string xmlFragment = @"<theElement type=""Remotion.UnitTests::Configuration.SampleType"" />";
      ConfigurationHelper.DeserializeElement (typeElement, xmlFragment);

      Assert.AreEqual (typeof (SampleType), typeElement.Type);
    }
  }
}
