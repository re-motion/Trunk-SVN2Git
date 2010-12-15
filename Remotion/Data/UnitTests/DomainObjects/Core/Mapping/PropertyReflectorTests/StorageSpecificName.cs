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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StorageSpecificName: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassHavingStorageSpecificIdentifierAttribute> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();
      actual.SetStorageProperty (new SimpleColumnDefinition ("NoAttribute", typeof (string), "varchar", true));

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute.NoAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NoAttribute", StorageModelTestHelper.GetColumnName(actual));
    }

    [Test]
    public void GetMetadata_WithStorageSpecificName()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassHavingStorageSpecificIdentifierAttribute> ("StorageSpecificName");

      PropertyDefinition actual = propertyReflector.GetMetadata();
      actual.SetStorageProperty (new SimpleColumnDefinition ("CustomName", typeof (string), "varchar", true));

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute.StorageSpecificName",
          actual.PropertyName);
      Assert.AreEqual ("CustomName", StorageModelTestHelper.GetColumnName(actual));
    }
  }
}
