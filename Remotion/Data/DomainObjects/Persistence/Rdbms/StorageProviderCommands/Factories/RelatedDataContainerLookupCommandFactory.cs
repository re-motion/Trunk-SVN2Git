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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  public class RelatedDataContainerLookupCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IStorageProviderCommandFactory _storageProviderCommandFactory;

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private TableDefinition _tableDefinition;
      private UnionViewDefinition _unionViewDefinition;

      public TableDefinition TableDefinition
      {
        get { return _tableDefinition; }
      }

      public UnionViewDefinition UnionViewDefinition
      {
        get { return _unionViewDefinition; }
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

        _tableDefinition = tableDefinition;
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

        _unionViewDefinition = unionViewDefinition;
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);

        filterViewDefinition.BaseEntity.Accept (this);
      }

      public void VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        ArgumentUtility.CheckNotNull ("nullEntityDefinition", nullEntityDefinition);

        //Nothing to do here
      }
    }

    public RelatedDataContainerLookupCommandFactory (
        IDbCommandBuilderFactory dbCommandBuilderFactory, IStorageProviderCommandFactory storageProviderCommandFactory)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _storageProviderCommandFactory = storageProviderCommandFactory;
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>> CreateCommand (
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression,
        IDbCommandFactory factory,
        IDbCommandExecutor executor,
        IDataContainerReader dataContainerReader,
        IObjectIDFactory objectIDFactory)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyEndPoint", foreignKeyEndPoint);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);
      ArgumentUtility.CheckNotNull ("objectIDFactory", objectIDFactory);

      var visitor = new EntityDefinitionVisitor();
      ((IEntityDefinition) foreignKeyEndPoint.ClassDefinition.StorageEntityDefinition).Accept (visitor);

      if (visitor.TableDefinition != null)
      {
        var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromTable (
            visitor.TableDefinition,
            AllSelectedColumnsSpecification.Instance,
            ((SimpleColumnDefinition) foreignKeyEndPoint.PropertyDefinition.StoragePropertyDefinition),
            foreignKeyValue,
            GetOrderedColumns (sortExpression));
        return new MultiDataContainerLoadCommand (new[] { dbCommandBuilder }, false, factory, executor, dataContainerReader); //TODO 4074: AllowNulls False/True ?
      }
      else
      {
        var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromUnionView (
            visitor.UnionViewDefinition,
            new SelectedColumnsSpecification (new[] { visitor.UnionViewDefinition.ObjectIDColumn, visitor.UnionViewDefinition.ClassIDColumn }),
            (SimpleColumnDefinition) foreignKeyEndPoint.PropertyDefinition.StoragePropertyDefinition,
            foreignKeyValue,
            GetOrderedColumns (sortExpression));

        var objectIDLoadCommand = new MultiObjectIDLoadCommand (new[] { dbCommandBuilder }, factory, executor, objectIDFactory);
        return new IndirectDataContainerLookupCommand (objectIDLoadCommand, _storageProviderCommandFactory);
      }
    }

    private IOrderedColumnsSpecification GetOrderedColumns (SortExpressionDefinition sortExpression)
    {
      if (sortExpression == null)
        return EmptyOrderedColumnsSpecification.Instance;

      Assertion.IsTrue (sortExpression.SortedProperties.Count > 0, "The sort-epression must have at least one sorted property.");

      var columns = from spec in sortExpression.SortedProperties
                    let column = (IColumnDefinition) spec.PropertyDefinition.StoragePropertyDefinition 
                    from simpleColumn in SimpleColumnDefinitionFindingVisitor.FindSimpleColumnDefinitions (new[] { column })
                    select Tuple.Create (simpleColumn, spec.Order);

      return new NonEmptyOrderedColumnsSpecification (columns);
    }
  }
}