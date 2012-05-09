// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  public static class RdbmsProviderObjectMother
  {
    public static RdbmsProvider CreateForIntegrationTest (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        Func<RdbmsProviderDefinition, IPersistenceExtension, IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>, RdbmsProvider> ctorCall)
    {
      var storageNameProvider = new ReflectionBasedStorageNameProvider ();
      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider ();
      var infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider (
          storageTypeInformationProvider, storageNameProvider);
      var dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          new StorageEntityBasedStorageProviderDefinitionFinder());
      var commandFactory = new RdbmsProviderCommandFactory (
          storageProviderDefinition,
          dbCommandBuilderFactory,
          rdbmsPersistenceModelProvider,
          new ObjectReaderFactory (rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider, storageTypeInformationProvider),
          new TableDefinitionFinder (rdbmsPersistenceModelProvider),
          dataStoragePropertyDefinitionFactory);

      return ctorCall (storageProviderDefinition, NullPersistenceExtension.Instance, commandFactory);
    }
  }
}