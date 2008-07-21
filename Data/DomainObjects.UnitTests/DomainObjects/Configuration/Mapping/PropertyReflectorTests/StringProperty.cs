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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StringProperty: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableFromAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("NullableFromAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("NotNullable");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullable",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithMaximumLength()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("MaximumLength");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.MaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableAndMaximumLength()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties> ("NotNullableAndMaximumLength");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullableAndMaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Remotion.Data.DomainObjects.StringPropertyAttribute' may be only applied to properties of type 'System.String'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping.PropertyReflectorTests.StringProperty, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<StringProperty> ("Int32Property");

      propertyReflector.GetMetadata();
    }

    [StringProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}
