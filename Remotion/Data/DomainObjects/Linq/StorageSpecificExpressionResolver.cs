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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
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
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;

    public StorageSpecificExpressionResolver (
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IStorageNameProvider storageNameProvider,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _storageNameProvider = storageNameProvider;
      _storageTypeInformationProvider = storageTypeInformationProvider;
    }

    public SqlEntityDefinitionExpression ResolveEntity (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var entityDefinition = _rdbmsPersistenceModelProvider.GetEntityDefinition (classDefinition);
      var tableColumns = (from storageProperty in entityDefinition.GetAllProperties()
                          from sqlColumnDefinition in CreateSqlColumnDefinitions (storageProperty, tableAlias)
                          select sqlColumnDefinition
                         ).ToArray();


      return new SqlEntityDefinitionExpression (
          classDefinition.ClassType,
          tableAlias,
          null,
          tableColumns.Where (c => c.IsPrimaryKey).First(),
          tableColumns);
    }

    public Expression ResolveColumn (SqlEntityExpression originatingEntity, PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
      var columns = storagePropertyDefinition.GetColumns().ToList();
      if (columns.Count > 1)
        throw new NotSupportedException ("Compound-column properties are not supported by this LINQ provider.");

      var column = columns.Single();
      return GetColumnFromEntity (storagePropertyDefinition.PropertyType, column, originatingEntity);
    }

    public SqlColumnExpression ResolveIDColumn (SqlEntityExpression originatingEntity, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var entityDefinition = _rdbmsPersistenceModelProvider.GetEntityDefinition (classDefinition);
      var idColumn = GetSingleColumnForLookup (entityDefinition.ObjectIDProperty);

      Assertion.IsTrue (idColumn.IsPartOfPrimaryKey);
      return GetColumnFromEntity (entityDefinition.ObjectIDProperty.PropertyType, idColumn, originatingEntity);
    }

    public Expression ResolveValueColumn (SqlColumnExpression idColumn)
    {
      ArgumentUtility.CheckNotNull ("idColumn", idColumn);

      var storageTypeInfo = _storageTypeInformationProvider.GetStorageTypeForID (true);
      var sqlColumnExpression = idColumn.Update (storageTypeInfo.DotNetType, idColumn.OwningTableAlias, _storageNameProvider.GetIDColumnName (), false);
      return Expression.Convert (sqlColumnExpression, typeof (object));
    }

    public Expression ResolveClassIDColumn (SqlColumnExpression idColumn)
    {
      ArgumentUtility.CheckNotNull ("idColumn", idColumn);

      return idColumn.Update (typeof (string), idColumn.OwningTableAlias, _storageNameProvider.GetClassIDColumnName(), false);
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

      return new ResolvedJoinInfo (resolvedSimpleTableInfo, leftKey, rightKey);
    }

    private IEnumerable<SqlColumnDefinitionExpression> CreateSqlColumnDefinitions (IRdbmsStoragePropertyDefinition storageProperty, string tableAlias)
    {
      // HACK: re-linq currently doesn't support compound columns. Therefore, we can't represent compound re-store columns (e.g., ObjectIDs) as 
      // compound columns with a common parent expression with the correct expression type. Instead, we represent compound columns as multiple unrelated 
      // columns (e.g., ID and ClassID) and assign the outer expression type to the first column.
      // This should be changed as soon as re-linq gets compound column support.

      return storageProperty.GetColumns().Select (
          (cd, i) => new SqlColumnDefinitionExpression (
                         i == 0 ? storageProperty.PropertyType : cd.StorageTypeInfo.DotNetType,
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
        return ResolveIDColumn (entityDefinition, endPoint.ClassDefinition);
      else
      {
        var propertyDefinition = ((RelationEndPointDefinition) endPoint).PropertyDefinition;
        var storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition);
        var column = GetSingleColumnForLookup (storagePropertyDefinition);
        return GetColumnFromEntity (storagePropertyDefinition.PropertyType, column, entityDefinition);
      }
    }

    private SqlColumnExpression GetColumnFromEntity (Type propertyType, ColumnDefinition columnDefinition, SqlEntityExpression originatingEntity)
    {
      return originatingEntity.GetColumn (propertyType, columnDefinition.Name, columnDefinition.IsPartOfPrimaryKey);
    }

    private ColumnDefinition GetSingleColumnForLookup (IRdbmsStoragePropertyDefinition storagePropertyDefinition)
    {
      var columns = storagePropertyDefinition.GetColumnsForComparison().ToList();
      if (columns.Count > 1)
        throw new NotSupportedException ("Compound-column IDs are not supported by this LINQ provider.");

      return columns.Single();
    }
  }
}