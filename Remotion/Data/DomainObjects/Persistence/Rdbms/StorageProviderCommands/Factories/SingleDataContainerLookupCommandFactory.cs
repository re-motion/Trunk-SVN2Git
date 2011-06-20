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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// Takes an <see cref="ObjectID"/> and creates the <see cref="SingleIDLookupSelectDbCommandBuilder"/> and 
  /// <see cref="SingleDataContainerLoadCommand"/> required to load the respective <see cref="DataContainer"/>.
  /// </summary>
  public class SingleDataContainerLookupCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    
    public SingleDataContainerLookupCommandFactory (IDbCommandBuilderFactory dbCommandBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
    }

    // TODO Review 4069: Move the dataContainerReader to the ctor of this class (same in other factories)
    public IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext> CreateCommand (
        ObjectID id, IRdbmsProviderCommandExecutionContext commandExecutionContext, IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      return InlineEntityDefinitionVisitor.Visit<IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext>> (
          (IEntityDefinition) id.ClassDefinition.StorageEntityDefinition,
          (table, continuation) => CreateSingleDataContainerLoadCommand (table, id, commandExecutionContext, dataContainerReader),
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition."); },
          (nullEntity, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a NullEntityDefinition."); });
    }

    private IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext> CreateSingleDataContainerLoadCommand (
        TableDefinition tableDefinition,
        ObjectID id,
        IRdbmsProviderCommandExecutionContext commandExecutionContext,
        IDataContainerReader dataContainerReader)
    {
      var commandBuilder = _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (
          tableDefinition, 
          AllSelectedColumnsSpecification.Instance,
          id);
      return new SingleDataContainerLoadCommand (commandBuilder, commandExecutionContext, dataContainerReader);
    }
  }
}