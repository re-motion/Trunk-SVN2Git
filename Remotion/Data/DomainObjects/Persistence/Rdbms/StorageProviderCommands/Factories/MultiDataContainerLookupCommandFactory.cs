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

      var objectIDsPerTableDefinition = new MultiDictionary<TableDefinition, ObjectID>();
      var visitor = new EntityDefinitionVisitor();
      foreach (var objectID in ids)
      {
        ((IEntityDefinition) objectID.ClassDefinition.StorageEntityDefinition).Accept (visitor);
        objectIDsPerTableDefinition[visitor.TableDefinition].Add (objectID);
      }

      var commandBuilders = new List<IDbCommandBuilder>();
      foreach (var group in objectIDsPerTableDefinition)
      {
        if (group.Value.Count > 1)
        {
          commandBuilders.Add (
              _dbCommandBuilderFactory.CreateForMultiIDLookupFromTable (group.Key, AllSelectedColumnsSpecification.Instance, group.Value.ToArray()));
        }
        else
          commandBuilders.Add (
              _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (group.Key, AllSelectedColumnsSpecification.Instance, group.Value[0]));
      }

      return new MultiDataContainerLoadCommand (commandBuilders, false, commandExecutionContext, dataContainerReader);
    }
  }
}