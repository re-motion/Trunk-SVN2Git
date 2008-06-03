/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Core;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
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
