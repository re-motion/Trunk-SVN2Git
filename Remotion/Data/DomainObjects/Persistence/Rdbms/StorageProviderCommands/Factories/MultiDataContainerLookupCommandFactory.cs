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
using Remotion.Collections;
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

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private TableDefinition _tableDefinition;

      public TableDefinition TableDefinition
      {
        get { return _tableDefinition; }
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

        _tableDefinition = tableDefinition;
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

        throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition.");
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

        filterViewDefinition.BaseEntity.Accept (this);
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        //Nothing to do here
      }
    }

    public MultiDataContainerLookupCommandFactory (IDbCommandBuilderFactory dbCommandBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateCommand (
        IEnumerable<ObjectID> ids, IRdbmsProviderCommandExecutionContext commandExecutionContext, IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("ids", ids);
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      // TODO 4090: Replace visitor with InlineEntityDefinitionVisitor
      // TODO 4090: Remove nested EntityDefinitionVisitor
      var visitor = new EntityDefinitionVisitor();
      
      var dbCommandBuilders = from id in ids
                              let tableDefinition = GetTableDefinition (id, visitor)
                              group id by tableDefinition
                              into idsByTable
                              select CreateDbCommandBuilder (idsByTable.Key, idsByTable.ToArray());
      return new MultiDataContainerLoadCommand (dbCommandBuilders, false, commandExecutionContext, dataContainerReader);
    }

    private TableDefinition GetTableDefinition (ObjectID objectID, EntityDefinitionVisitor visitor)
    {
      ((IEntityDefinition) objectID.ClassDefinition.StorageEntityDefinition).Accept (visitor);
      return visitor.TableDefinition;
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