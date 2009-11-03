// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class ExtensibleEnumProperty : BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithExtensibleEnumProperties> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithExtensibleEnumProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (Color), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableFromAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithExtensibleEnumProperties> ("NullableFromAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithExtensibleEnumProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (Color), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithExtensibleEnumProperties> ("NotNullable");

      PropertyDefinition actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithExtensibleEnumProperties.NotNullable",
          actual.PropertyName);
      Assert.AreSame (typeof (Color), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (Color.Values.Blue(), actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The 'Remotion.Data.DomainObjects.ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'Remotion.ExtensibleEnums.IExtensibleEnum'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyReflectorTests.ExtensibleEnumProperty, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ExtensibleEnumProperty> ("Int32Property");

      propertyReflector.GetMetadata ();
    }

    [ExtensibleEnumProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException (); }
    }
  }
}