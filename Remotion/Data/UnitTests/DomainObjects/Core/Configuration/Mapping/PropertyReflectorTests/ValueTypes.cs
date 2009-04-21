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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class ValueTypes: BaseTest
  {
    [Test]
    public void GetMetadata_WithBasicType()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (false, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableBasicType()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("NaBooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool?), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.IsNull (actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithEnumProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("EnumProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty", actual.PropertyName);
      Assert.AreSame (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithOptionalRelationProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithGuidKey> ("ClassWithValidRelationsOptional");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional", actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The property type System.Object is not supported.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyReflectorTests.ValueTypes, "
        + "property: ObjectProperty")]
    public void GetMetadata_WithInvalidPropertyType()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ValueTypes> ("ObjectProperty");

      propertyReflector.GetMetadata();
    }

    public object ObjectProperty
    {
      get { return null; }
      set { }
    }
  }
}
