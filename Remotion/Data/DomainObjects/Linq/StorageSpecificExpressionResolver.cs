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
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The <see cref="StorageSpecificExpressionResolver"/> is responsible to resolve expressions for a specific storage system.
  /// </summary>
  public class StorageSpecificExpressionResolver : IStorageSpecificExpressionResolver
  {
    //TODO 3572: move visitor to a new class file and make it public !?
    private class SqlColumnListColumnDefinitionVisitor : IColumnDefinitionVisitor
    {
      private readonly IList<SimpleColumnDefinition> columnDefinitions;
      private readonly string _tableAlias;

      public SqlColumnListColumnDefinitionVisitor (string tableAlias)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);
        
        _tableAlias = tableAlias;
        columnDefinitions = new List<SimpleColumnDefinition>();
      }

      public void VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
      {
        ArgumentUtility.CheckNotNull ("simpleColumnDefinition", simpleColumnDefinition);

        columnDefinitions.Add (simpleColumnDefinition);
      }

      public void VisitIDColumnDefinition (IDColumnDefinition idColumnDefinition)
      {
        ArgumentUtility.CheckNotNull ("idColumnDefinition", idColumnDefinition);

        idColumnDefinition.ObjectIDColumn.Accept (this);
        if (idColumnDefinition.HasClassIDColumn)
          idColumnDefinition.ClassIDColumn.Accept (this);
      }

      public void VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
      {
        ArgumentUtility.CheckNotNull ("nullColumnDefinition", nullColumnDefinition);

      }

      public IEnumerable<SqlColumnDefinitionExpression> GetSqlColumns ()
      {
        return columnDefinitions.Select (cd => new SqlColumnDefinitionExpression (cd.PropertyType, _tableAlias, cd.Name, false));
      }
    }

    public SqlEntityDefinitionExpression ResolveEntity (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var propertyInfo = typeof (DomainObject).GetProperty ("ID");
      var primaryKeyColumn = new SqlColumnDefinitionExpression (propertyInfo.PropertyType, tableAlias, propertyInfo.Name, true);

      var visitor = new SqlColumnListColumnDefinitionVisitor(tableAlias);
      foreach (var columnDefinition in ((IEntityDefinition) classDefinition.StorageEntityDefinition).GetColumns ())
        columnDefinition.Accept (visitor);
      var tableColumns = visitor.GetSqlColumns().ToArray();

      return new SqlEntityDefinitionExpression (classDefinition.ClassType, tableAlias, null, primaryKeyColumn, tableColumns);
    }

    public Expression ResolveColumn (SqlEntityExpression originatingEntity, PropertyDefinition propertyDefinition, bool isPrimaryKeyColumn)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return originatingEntity.GetColumn (propertyDefinition.PropertyType, propertyDefinition.StoragePropertyDefinition.Name, isPrimaryKeyColumn);
    }

    public SqlColumnExpression ResolveIDColumn (SqlEntityExpression originatingEntity, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return originatingEntity.GetColumn (typeof (ObjectID), "ID", true);
    }

    public IResolvedTableInfo ResolveTable (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var viewName = classDefinition.StorageEntityDefinition.LegacyViewName;
      return new ResolvedSimpleTableInfo (classDefinition.ClassType, viewName, tableAlias);
    }

    public ResolvedJoinInfo ResolveJoin (
        SqlEntityExpression originatingEntity, IRelationEndPointDefinition leftEndPoint, IRelationEndPointDefinition rightEndPoint, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("leftEndPoint", leftEndPoint);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var keyType = typeof (DomainObject).GetProperty ("ID").PropertyType;

      var resolvedSimpleTableInfo = ResolveTable (rightEndPoint.ClassDefinition, tableAlias);

      var leftKey = originatingEntity.GetColumn (keyType, GetJoinColumnName (leftEndPoint), leftEndPoint.IsVirtual);
      var rightKey = new SqlColumnDefinitionExpression (
          keyType,
          tableAlias,
          GetJoinColumnName (rightEndPoint),
          rightEndPoint.IsVirtual);

      return new ResolvedJoinInfo (resolvedSimpleTableInfo, leftKey, rightKey);
    }

    private string GetJoinColumnName (IRelationEndPointDefinition endPoint)
    {
      return endPoint.IsVirtual
                 ? "ID"
                 : endPoint.ClassDefinition.GetMandatoryPropertyDefinition (endPoint.PropertyName).StoragePropertyDefinition.Name;
    }
  }
}