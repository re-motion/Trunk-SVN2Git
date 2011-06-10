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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  public class SingleDataContainerLookupCommandFactory
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
        _tableDefinition = tableDefinition;
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition.");
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        filterViewDefinition.BaseEntity.Accept (this);
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        //Nothing to do here
      }
    }

    public SingleDataContainerLookupCommandFactory (IDbCommandBuilderFactory dbCommandBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
    }

    public IStorageProviderCommand<DataContainer> CreateCommand (
        ObjectID id, IDbCommandFactory factory, IDbCommandExecutor executor, ValueConverter valueConverter)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("valueConverter", valueConverter);

      var visitor = new EntityDefinitionVisitor();
      ((IEntityDefinition) id.ClassDefinition.StorageEntityDefinition).Accept (visitor);

      var commandBuilder = _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (
          visitor.TableDefinition, AllSelectedColumnsSpecification.Instance, id);
      return new SingleDataContainerLoadCommand (commandBuilder, factory, executor, valueConverter);
    }
  }
}