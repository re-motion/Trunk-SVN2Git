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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The <see cref="StorageSpecificExpressionResolver"/> is responsible to resolve expressions for a specific storage system.
  /// </summary>
  public class StorageSpecificExpressionResolver : IStorageSpecificExpressionResolver
  {
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public StorageSpecificExpressionResolver (IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public SqlEntityDefinitionExpression ResolveEntity (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var entityDefinition = _rdbmsPersistenceModelProvider.GetEntityDefinition (classDefinition);
      var idColumnDefinition = GetSingleColumnForLookup (entityDefinition.ObjectIDProperty);

      var tableColumns = (from storageProperty in entityDefinition.GetAllProperties ()
                          from sqlColumnDefinition in CreateSqlColumnDefinitions (storageProperty, tableAlias)
                          select sqlColumnDefinition
                         ).ToArray();

      return new SqlEntityDefinitionExpression (
          classDefinition.ClassType,
          tableAlias,
          null,
          e => GetColumnFromEntity (idColumnDefinition, e),
          tableColumns);
    }

    public Expression ResolveProperty (SqlEntityExpression originatingEntity, PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
      return ResolveStorageProperty (originatingEntity, storagePropertyDefinition);
    }

    public Expression ResolveIDProperty (SqlEntityExpression originatingEntity, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var entityDefinition = _rdbmsPersistenceModelProvider.GetEntityDefinition (classDefinition);
      var valueExpression = Expression.Convert (ResolveStorageProperty (originatingEntity, entityDefinition.ObjectIDProperty.ValueProperty), typeof (object));
      var classIDExpression = ResolveStorageProperty (originatingEntity, entityDefinition.ObjectIDProperty.ClassIDProperty);

      var objectIDCtor = typeof (ObjectID).GetConstructor (new[] { typeof (string), typeof (object) });
      Assertion.IsNotNull (objectIDCtor);
      var valueMember = typeof (ObjectID).GetProperty ("Value");
      var classIDMember = typeof (ObjectID).GetProperty ("ClassID");
      var objectIDConstruction = Expression.New (objectIDCtor, new[] { classIDExpression, valueExpression }, new[] { classIDMember, valueMember });
      
      return NamedExpression.CreateNewExpressionWithNamedArguments (objectIDConstruction);
    }

    public IResolvedTableInfo ResolveTable (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var viewName = InlineRdbmsStorageEntityDefinitionVisitor.Visit<string> (
          _rdbmsPersistenceModelProvider.GetEntityDefinition (classDefinition),
          (table, continuation) => GetFullyQualifiedEntityName (table.ViewName),
          (filterView, continuation) => GetFullyQualifiedEntityName (filterView.ViewName),
          (unionView, continuation) => GetFullyQualifiedEntityName (unionView.ViewName),
          (emptyView, continuation) => GetFullyQualifiedEntityName (emptyView.ViewName));

      return new ResolvedSimpleTableInfo (classDefinition.ClassType, viewName, tableAlias);
    }

    public ResolvedJoinInfo ResolveJoin (
        SqlEntityExpression originatingEntity, IRelationEndPointDefinition leftEndPoint, IRelationEndPointDefinition rightEndPoint, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("leftEndPoint", leftEndPoint);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var leftKey = GetJoinColumn (leftEndPoint, originatingEntity);

      var resolvedSimpleTableInfo = ResolveTable (rightEndPoint.ClassDefinition, tableAlias);
      var rightEntity = ResolveEntity (rightEndPoint.ClassDefinition, tableAlias);

      Expression rightKey = GetJoinColumn (rightEndPoint, rightEntity);

      return new ResolvedJoinInfo (resolvedSimpleTableInfo, Expression.Equal (leftKey, rightKey));
    }

    public Expression ResolveEntityIdentityViaForeignKey (SqlEntityExpression originatingEntity, RelationEndPointDefinition foreignKeyEndPoint)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("foreignKeyEndPoint", foreignKeyEndPoint);

      return GetJoinColumn (foreignKeyEndPoint, originatingEntity);
    }

    private IEnumerable<SqlColumnDefinitionExpression> CreateSqlColumnDefinitions (IRdbmsStoragePropertyDefinition storageProperty, string tableAlias)
    {
      return storageProperty.GetColumns().Select (
          (cd, i) => new SqlColumnDefinitionExpression (
                         cd.StorageTypeInfo.DotNetType,
                         tableAlias,
                         cd.Name,
                         cd.IsPartOfPrimaryKey));
    }

    private string GetFullyQualifiedEntityName (EntityNameDefinition entityNameDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityNameDefinition", entityNameDefinition);

      return entityNameDefinition.SchemaName != null
                 ? entityNameDefinition.SchemaName + "." + entityNameDefinition.EntityName
                 : entityNameDefinition.EntityName;
    }

    private Expression GetJoinColumn (IRelationEndPointDefinition endPoint, SqlEntityExpression entityDefinition)
    {
      if (endPoint.IsVirtual)
      {
        // In ResolveEntity above, we defined that we take the ID column as the primary key.
        return entityDefinition.GetIdentityExpression();
      }
      else
      {
        var propertyDefinition = ((RelationEndPointDefinition) endPoint).PropertyDefinition;
        var storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
        var column = GetSingleColumnForLookup (storagePropertyDefinition);
        return GetColumnFromEntity (column, entityDefinition);
      }
    }

    private SqlColumnExpression GetColumnFromEntity (ColumnDefinition columnDefinition, SqlEntityExpression originatingEntity)
    {
      return originatingEntity.GetColumn (columnDefinition.StorageTypeInfo.DotNetType, columnDefinition.Name, columnDefinition.IsPartOfPrimaryKey);
    }

    private ColumnDefinition GetSingleColumnForLookup (IRdbmsStoragePropertyDefinition storagePropertyDefinition)
    {
      var columns = storagePropertyDefinition.GetColumnsForComparison().ToList();
      if (columns.Count > 1)
        throw new NotSupportedException ("Compound-column IDs are not supported by this LINQ provider.");

      return columns.Single();
    }

    private Expression ResolveStorageProperty (SqlEntityExpression originatingEntity, IRdbmsStoragePropertyDefinition storagePropertyDefinition)
    {
      var columns = storagePropertyDefinition.GetColumns ().ToList ();
      if (columns.Count > 1)
        throw new NotSupportedException ("Compound-column properties are not supported by this LINQ provider.");

      var column = columns.Single ();
      return GetColumnFromEntity (column, originatingEntity);
    }

  }
}