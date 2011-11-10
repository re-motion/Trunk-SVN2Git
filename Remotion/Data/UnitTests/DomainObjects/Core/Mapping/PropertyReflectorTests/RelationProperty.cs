// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class RelationProperty: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithRealRelationEndPoints> ("NoAttribute", DomainModelConstraintProviderStub);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithRealRelationEndPoints> ("NotNullable", DomainModelConstraintProviderStub);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NotNullable",
          actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}
