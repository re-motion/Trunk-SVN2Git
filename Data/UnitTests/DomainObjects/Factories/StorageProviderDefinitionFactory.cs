// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public static class StorageProviderDefinitionFactory
  {
    public static ProviderCollection<StorageProviderDefinition> Create()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.DefaultStorageProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (DatabaseTest.c_unitTestStorageProviderStubID, typeof (UnitTestStorageProviderStub)));

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      return storageProviderDefinitionCollection;
    }
  }
}
