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
  public class ExtensibleEnumProperty : BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithExtensibleEnumProperties> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata ();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithExtensibleEnumProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (TestExtensibleEnum), actual.PropertyType);
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithExtensibleEnumProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (TestExtensibleEnum), actual.PropertyType);
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
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithExtensibleEnumProperties.NotNullable",
          actual.PropertyName);
      Assert.AreSame (typeof (TestExtensibleEnum), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (TestExtensibleEnum.Values.Value1(), actual.DefaultValue);
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

    // ReSharper disable UnusedMember.Local
    [ExtensibleEnumProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException (); }
    }
    // ReSharper restore UnusedMember.Local
  }
}
