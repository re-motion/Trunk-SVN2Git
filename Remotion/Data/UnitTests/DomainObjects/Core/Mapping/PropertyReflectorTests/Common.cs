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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class Common : BaseTest
  {
    [Test]
    public void GetMetadata_ForSingleProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty", DomainModelConstraintProviderStub);

      PropertyDefinition actual = propertyReflector.GetMetadata();
      actual.SetStorageProperty (
          ColumnDefinitionObjectMother.CreateTypedColumn ("Boolean", typeof (bool), new StorageTypeInformation ("bit", DbType.Boolean)));

      Assert.IsNotNull (actual);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreEqual ("Boolean", StorageModelTestHelper.GetColumnName (actual));
      Assert.AreSame (typeof (bool), actual.PropertyType);
    }
  }
}