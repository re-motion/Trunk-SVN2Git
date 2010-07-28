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
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class BinaryProperty: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableFromAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("NullableFromAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("NotNullable");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.NotNullable",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithMaximumLength()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("MaximumLength");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.MaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableAndMaximumLength()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithBinaryProperties> ("NotNullableAndMaximumLength");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBinaryProperties.NotNullableAndMaximumLength",
          actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Remotion.Data.DomainObjects.BinaryPropertyAttribute' may be only applied to properties of type 'System.Byte[]'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyReflectorTests.BinaryProperty, property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<BinaryProperty> ("Int32Property");

      propertyReflector.GetMetadata();
    }

    [BinaryProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}
