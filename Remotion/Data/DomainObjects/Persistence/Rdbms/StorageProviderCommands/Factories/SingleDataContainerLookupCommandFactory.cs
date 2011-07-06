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
    private readonly IDataContainerReader _dataContainerReader;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public SingleDataContainerLookupCommandFactory (
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IDataContainerReader dataContainerReader,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _dataContainerReader = dataContainerReader;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext> CreateCommand (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      return InlineEntityDefinitionVisitor.Visit<IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext>> (
          _rdbmsPersistenceModelProvider.GetEntityDefinition(id.ClassDefinition),
          (table, continuation) => CreateSingleDataContainerLoadCommand (table, id, _dataContainerReader),
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition."); },
          (nullEntity, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a NullEntityDefinition."); });
    }

    private IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext> CreateSingleDataContainerLoadCommand (
        TableDefinition tableDefinition, ObjectID id, IDataContainerReader dataContainerReader)
    {
      var commandBuilder = _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (
          tableDefinition,
          AllSelectedColumnsSpecification.Instance,
          id);
      return new SingleDataContainerLoadCommand (commandBuilder, dataContainerReader);
    }
  }
}