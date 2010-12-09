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
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The <see cref="StorageSpecificExpressionResolver"/> is responsible to resolve expressions for a specific storage system.
  /// </summary>
  public class StorageSpecificExpressionResolver : IStorageSpecificExpressionResolver
  {
    public SqlEntityDefinitionExpression ResolveEntity (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var propertyInfo = typeof (DomainObject).GetProperty ("ID");
      var primaryKeyColumn = new SqlColumnDefinitionExpression (propertyInfo.PropertyType, tableAlias, propertyInfo.Name, true);
      var starColumn = new SqlColumnDefinitionExpression (classDefinition.ClassType, tableAlias, "*", false);

      return new SqlEntityDefinitionExpression (classDefinition.ClassType, tableAlias, null, primaryKeyColumn, starColumn);
    }

    public Expression ResolveColumn (SqlEntityExpression originatingEntity, PropertyDefinition propertyDefinition, bool isPrimaryKeyColumn)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return originatingEntity.GetColumn (propertyDefinition.PropertyType, propertyDefinition.StoragePropertyDefinition.Name, isPrimaryKeyColumn);
    }

    public IResolvedTableInfo ResolveTable (ClassDefinition classDefinition, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var viewName = classDefinition.StorageEntityDefinition.LegacyViewName;
      return new ResolvedSimpleTableInfo (classDefinition.ClassType, viewName, tableAlias);
    }

    public ResolvedJoinInfo ResolveJoin (SqlEntityExpression originatingEntity, IRelationEndPointDefinition leftEndPoint, string tableAlias)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("leftEndPoint", leftEndPoint);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var rightEndPointDefinition = leftEndPoint.GetOppositeEndPointDefinition();
      var keyType = typeof (DomainObject).GetProperty ("ID").PropertyType;

      var resolvedSimpleTableInfo = ResolveTable (rightEndPointDefinition.ClassDefinition, tableAlias);

      var leftKey = originatingEntity.GetColumn (keyType, GetJoinColumnName (leftEndPoint), leftEndPoint.IsVirtual);
      var rightKey = new SqlColumnDefinitionExpression (
          keyType,
          tableAlias,
          GetJoinColumnName (rightEndPointDefinition),
          rightEndPointDefinition.IsVirtual);

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