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
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Implements <see cref="IMappingResolver"/> to supply information from re-store to the re-linq SQL backend.
  /// </summary>
  public class MappingResolver : IMappingResolver
  {
    public IResolvedTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var classDefinition = GetClassDefinition (tableInfo.ItemType);
      if (classDefinition == null)
      {
        string message = string.Format ("The type '{0}' does not identify a queryable table.", tableInfo.ItemType.Name);
        throw new UnmappedItemException (message);
      }
      var viewName = classDefinition.GetViewName();
      return new ResolvedSimpleTableInfo (tableInfo.ItemType, viewName, generator.GetUniqueIdentifier ("t"));
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("joinInfo", joinInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var classDefinition = GetClassDefinition (joinInfo.OriginatingEntity.Type);
      if (classDefinition == null)
      {
        string message = string.Format (
            "The type '{0}' does not identify a queryable table.", joinInfo.OriginatingEntity.Type);
        throw new UnmappedItemException (message);
      }

      var leftEndPointDefinition = GetRelationEndPointDefinition (joinInfo.MemberInfo, classDefinition);
      if (leftEndPointDefinition == null)
      {
        string message =
            string.Format ("The member '{0}.{1}' does not identify a relation.", joinInfo.MemberInfo.DeclaringType.Name, joinInfo.MemberInfo.Name);
        throw new UnmappedItemException (message);
      }

      var rightEndPointDefinition = leftEndPointDefinition.GetOppositeEndPointDefinition ();
      var keyType = typeof (DomainObject).GetProperty ("ID").PropertyType;

      var alias = generator.GetUniqueIdentifier ("t");
      var resolvedSimpleTableInfo = new ResolvedSimpleTableInfo (
          rightEndPointDefinition.ClassDefinition.ClassType,
          rightEndPointDefinition.ClassDefinition.GetViewName (),
          alias);

      var leftKey = joinInfo.OriginatingEntity.GetColumn (keyType, GetJoinColumnName (leftEndPointDefinition), leftEndPointDefinition.IsVirtual);
      var rightKey = new SqlColumnDefinitionExpression (
          keyType,
          alias,
          GetJoinColumnName (rightEndPointDefinition),
          rightEndPointDefinition.IsVirtual);

      return new ResolvedJoinInfo (resolvedSimpleTableInfo, leftKey, rightKey);
    }

    public SqlEntityDefinitionExpression ResolveSimpleTableInfo (IResolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var tableAlias = tableInfo.TableAlias;
      
      var propertyInfo = typeof (DomainObject).GetProperty ("ID");
      // becomes SqlColumnDefinitionExpression
      var primaryKeyColumn = new SqlColumnDefinitionExpression (propertyInfo.PropertyType, tableAlias, propertyInfo.Name, true);

      // becomes SqlColumnDefinitionExpression
      var starColumn = new SqlColumnDefinitionExpression (tableInfo.ItemType, tableAlias, "*", false);

      return new SqlEntityDefinitionExpression (tableInfo.ItemType, tableAlias, null, primaryKeyColumn, starColumn);
    }

    public Expression ResolveMemberExpression (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      var property = memberInfo as PropertyInfo;
      if (property == null)
      {
        throw new UnmappedItemException (
            string.Format (
                "Field '{0}.{1}' cannot be used in a query because it is not a mapped member.",
                originatingEntity.Type.Name,
                memberInfo.Name));
      }

      if (property.Name == "ID" && property.DeclaringType == typeof (DomainObject))
        return originatingEntity.GetColumn (property.PropertyType, "ID", true);

      var classDefinition = GetClassDefinition (originatingEntity.Type);
      if (classDefinition == null)
      {
        string message = string.Format (
            "The type '{0}' does not identify a queryable table.", originatingEntity.Type);
        throw new UnmappedItemException (message);
      }

      var endPointDefinition = GetRelationEndPointDefinition (property, classDefinition);
      if (endPointDefinition != null)
        return new SqlEntityRefMemberExpression (originatingEntity, property);

      var potentiallyRedirectedProperty = LinqPropertyRedirectionAttribute.GetTargetProperty (property);
      var propertyDefinition = classDefinition.ResolveProperty (potentiallyRedirectedProperty);
      if (propertyDefinition == null)
      {
        string message = string.Format (
            "The member '{0}.{1}' does not have a queryable database mapping.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException (message);
      }

      return originatingEntity.GetColumn (property.PropertyType, propertyDefinition.StorageSpecificName, false);
    }

    public Expression ResolveMemberExpression (SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("sqlColumnExpression", sqlColumnExpression);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      if (memberInfo == typeof (ObjectID).GetProperty ("ClassID"))
        return sqlColumnExpression.Update (typeof (string), sqlColumnExpression.OwningTableAlias, "ClassID", false);

      throw new UnmappedItemException (
          string.Format ("The member '{0}.{1}' does not identify a mapped property.", memberInfo.ReflectedType.Name, memberInfo.Name));
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      ArgumentUtility.CheckNotNull ("constantExpression", constantExpression);

      if (constantExpression.Value is DomainObject)
        return new SqlEntityConstantExpression (constantExpression.Type, constantExpression.Value, ((DomainObject) constantExpression.Value).ID);
      else
        return constantExpression;
    }

    public Expression ResolveTypeCheck (Expression checkedExpression, Type desiredType)
    {
      ArgumentUtility.CheckNotNull ("checkedExpression", checkedExpression);
      ArgumentUtility.CheckNotNull ("desiredType", desiredType);

      if (desiredType.IsAssignableFrom (checkedExpression.Type))
        return Expression.Constant (true);
      else if (checkedExpression.Type.IsAssignableFrom (desiredType))
      {
        if (!typeof (DomainObject).IsAssignableFrom (checkedExpression.Type))
        {
          var message = string.Format (
              "No database-level type check can be added for the expression '{0}'." +
              "Only the types of DomainObjects can be checked in database queries.",
              FormattingExpressionTreeVisitor.Format (checkedExpression));
          throw new UnmappedItemException (message);
        }

        var classDefinition = GetClassDefinition (desiredType);
        if (classDefinition == null)
        {
          string message = string.Format (
              "The type '{0}' does not identify a queryable table.", desiredType.Name);
          throw new UnmappedItemException (message);
        }

        var idExpression = Expression.MakeMemberAccess (checkedExpression, typeof (DomainObject).GetProperty ("ID"));
        var classIDExpression = Expression.MakeMemberAccess (idExpression, typeof (ObjectID).GetProperty ("ClassID"));
        return Expression.Equal (classIDExpression, new SqlLiteralExpression (classDefinition.ID));
      }
      else
        return Expression.Constant (false);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (MemberInfo relationMember, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationMember", relationMember);
      var property = relationMember as PropertyInfo;
      if (property == null)
        return null;

      var potentiallyRedirectedProperty = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      return classDefinition.ResolveRelationEndPoint (potentiallyRedirectedProperty);
    }

    private ClassDefinition GetClassDefinition (Type type)
    {
      return MappingConfiguration.Current.ClassDefinitions[type];
    }

    private string GetJoinColumnName (IRelationEndPointDefinition endPoint)
    {
      return endPoint.IsVirtual ? "ID" : endPoint.ClassDefinition.GetMandatoryPropertyDefinition (endPoint.PropertyName).StorageSpecificName;
    }
  }
}