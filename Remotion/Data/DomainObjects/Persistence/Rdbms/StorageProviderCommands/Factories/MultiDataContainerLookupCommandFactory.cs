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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  public class MultiDataContainerLookupCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IDataContainerReader _dataContainerReader;
    
    public MultiDataContainerLookupCommandFactory (IDbCommandBuilderFactory dbCommandBuilderFactory, IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _dataContainerReader = dataContainerReader;
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateCommand (IEnumerable<ObjectID> ids)
    {
      ArgumentUtility.CheckNotNull ("ids", ids);

      var objectIDs = ids.ToList();
      var dbCommandBuilders = from id in objectIDs
                              let tableDefinition = GetTableDefinition (id)
                              group id by tableDefinition
                              into idsByTable
                              select CreateDbCommandBuilder (idsByTable.Key, idsByTable.ToArray());
      var multiDataContainerLoadCommand = new MultiDataContainerLoadCommand (dbCommandBuilders, false, _dataContainerReader);
      return new MultiDataContainerSortCommand (objectIDs, multiDataContainerLoadCommand);
    }

    private TableDefinition GetTableDefinition (ObjectID objectID)
    {
      return InlineEntityDefinitionVisitor.Visit<TableDefinition> (
          (IEntityDefinition) objectID.ClassDefinition.StorageEntityDefinition,
          (table, continuation) => table,
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition."); },
          (nullEntity, continuation) => { throw new InvalidOperationException ("The ClassDefinition must not have a NullEntityDefinition."); });
    }  

    private IDbCommandBuilder CreateDbCommandBuilder (TableDefinition tableDefinition, ObjectID[] objectIDs)
    {
      if (objectIDs.Length > 1)
        return _dbCommandBuilderFactory.CreateForMultiIDLookupFromTable (tableDefinition, AllSelectedColumnsSpecification.Instance, objectIDs);
      else
        return _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (tableDefinition, AllSelectedColumnsSpecification.Instance, objectIDs[0]);
    }
  }
}