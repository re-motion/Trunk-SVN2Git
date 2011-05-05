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
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Database;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Utilities;
using DomainObjectIDs = Remotion.Data.UnitTests.DomainObjects.Factories.DomainObjectIDs;

namespace Remotion.Data.UnitTests.DomainObjects
{
  public class StandardMappingTest : DatabaseTest
  {
    public const string CreateTestDataFileName = "DataDomainObjects_CreateTestData.sql";

    protected StandardMappingTest ()
        : base (new StandardMappingDatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
    }

    public override void SetUp ()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return StandardConfiguration.Instance.GetDomainObjectIDs(); }
    }

    protected MappingConfiguration Configuration
    {
      get { return MappingConfiguration.Current; }
    }

    protected StorageProviderDefinition TestDomainStorageProviderDefinition
    {
      get { return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID]; }
    }

    protected StorageProviderDefinition UnitTestStorageProviderDefinition
    {
      get { return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_unitTestStorageProviderStubID]; }
    }

    protected IRelationEndPointDefinition GetEndPointDefinition (Type declaringType, string shortPropertyName)
    {
      var endPointDefinition = Configuration
          .GetTypeDefinition (declaringType)
          .PropertyAccessorDataCache
          .GetMandatoryPropertyAccessorData (declaringType, shortPropertyName)
          .RelationEndPointDefinition;
      Assertion.IsNotNull (endPointDefinition, "Property '{0}.{1}' is not a relation end-point.", declaringType, shortPropertyName);
      return endPointDefinition;
    }
  }
}
