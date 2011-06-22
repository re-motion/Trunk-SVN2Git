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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  public class RelatedDataContainerLookupCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;
    private readonly IDataContainerReader _dataContainerReader;
    private readonly IObjectIDReader _objectIDReader;

    public RelatedDataContainerLookupCommandFactory (
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory,
        IDataContainerReader dataContainerReader,
        IObjectIDReader objectIDReader)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);
      ArgumentUtility.CheckNotNull ("objectIDReader", objectIDReader);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _storageProviderCommandFactory = storageProviderCommandFactory;
      _dataContainerReader = dataContainerReader;
      _objectIDReader = objectIDReader;
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateCommand (
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyEndPoint", foreignKeyEndPoint);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);

      return InlineEntityDefinitionVisitor.Visit<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>> (
          (IEntityDefinition) foreignKeyEndPoint.ClassDefinition.StorageEntityDefinition,
          (table, continuation) => CreateDirectDataContainerLoadCommand (
              table,
              foreignKeyEndPoint,
              foreignKeyValue,
              sortExpression,
              _dataContainerReader),
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => CreateIndirectDataContainerLoadCommand (
              unionView,
              foreignKeyEndPoint,
              foreignKeyValue,
              sortExpression,
              _objectIDReader),
          (nullEntity, continuation) => { throw new InvalidOperationException ("The ClassDefinition must not have a NullEntityDefinition."); });
    }

    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateDirectDataContainerLoadCommand (
        TableDefinition tableDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression,
        IDataContainerReader dataContainerReader)
    {
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromTable (
          tableDefinition,
          AllSelectedColumnsSpecification.Instance,
          ((IDColumnDefinition) foreignKeyEndPoint.PropertyDefinition.StoragePropertyDefinition),
          foreignKeyValue,
          GetOrderedColumns (sortExpression));
      return new MultiDataContainerLoadCommand (new[] { dbCommandBuilder }, false, dataContainerReader);
    }

    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateIndirectDataContainerLoadCommand (
        UnionViewDefinition unionViewDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression,
        IObjectIDReader objectIDReader)
    {
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromUnionView (
          unionViewDefinition,
          new SelectedColumnsSpecification (new[] { unionViewDefinition.ObjectIDColumn, unionViewDefinition.ClassIDColumn }),
          (IDColumnDefinition) foreignKeyEndPoint.PropertyDefinition.StoragePropertyDefinition,
          foreignKeyValue,
          GetOrderedColumns (sortExpression));

      var objectIDLoadCommand = new MultiObjectIDLoadCommand (new[] { dbCommandBuilder }, objectIDReader);
      return new IndirectDataContainerLoadCommand (objectIDLoadCommand, _storageProviderCommandFactory);
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

      return new OrderedColumnsSpecification (columns);
    }
  }
}