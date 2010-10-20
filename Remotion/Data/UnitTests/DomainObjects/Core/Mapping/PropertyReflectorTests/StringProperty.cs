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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NoAttribute",
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NullableFromAttribute",
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullable",
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.MaximumLength",
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullableAndMaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Remotion.Data.DomainObjects.StringPropertyAttribute' may be only applied to properties of type 'System.String'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests.StringProperty, "
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
