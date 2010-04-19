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

      Tuple<RelationDefinition, ClassDefinition, string> relationData = GetRelationData (joinInfo.MemberInfo);
      if (relationData == null)
      {
        string message =
            string.Format ("The member '{0}.{1}' does not identify a relation.", joinInfo.MemberInfo.DeclaringType.Name, joinInfo.MemberInfo.Name);
        throw new UnmappedItemException (message);
      }

      RelationDefinition relationDefinition = relationData.Item1;
      ClassDefinition classDefinition = relationData.Item2;
      string propertyIdentifier = relationData.Item3;

      var leftEndPoint = relationDefinition.GetEndPointDefinition (classDefinition.ID, propertyIdentifier);
      var rightEndPoint = relationDefinition.GetOppositeEndPointDefinition (leftEndPoint);

      var keyType = typeof (DomainObject).GetProperty ("ID").PropertyType;

      var alias = generator.GetUniqueIdentifier ("t");
      var resolvedSimpleTableInfo = new ResolvedSimpleTableInfo (
          rightEndPoint.ClassDefinition.ClassType,
          rightEndPoint.ClassDefinition.GetViewName(),
          alias);

      var leftKey = new SqlColumnExpression (keyType, joinInfo.OriginatingTable.GetResolvedTableInfo().TableAlias, GetJoinColumnName (leftEndPoint));
      var rightKey = new SqlColumnExpression (keyType, alias, GetJoinColumnName (rightEndPoint));

      return new ResolvedJoinInfo (resolvedSimpleTableInfo, leftKey, rightKey);
    }

    public Expression ResolveTableReferenceExpression (SqlTableReferenceExpression tableReferenceExpression, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("tableReferenceExpression", tableReferenceExpression);
      ArgumentUtility.CheckNotNull ("generator", generator);

      if (!typeof (DomainObject).IsAssignableFrom (tableReferenceExpression.Type))
        return new SqlValueTableReferenceExpression (tableReferenceExpression.SqlTable);
      
      var tableAlias = tableReferenceExpression.SqlTable.GetResolvedTableInfo().TableAlias;

      var propertyInfo = typeof (DomainObject).GetProperty ("ID");
      var primaryKeyColumn = new SqlColumnExpression (propertyInfo.PropertyType, tableAlias, propertyInfo.Name);

      var starColumn = new SqlColumnExpression (tableReferenceExpression.Type, tableAlias, "*");

      return new SqlEntityExpression (tableReferenceExpression.SqlTable, primaryKeyColumn, starColumn);
    }

    public Expression ResolveMemberExpression (SqlTableBase sqlTable, MemberInfo memberInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("sqlTable", sqlTable);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);
      
      var tableAlias = sqlTable.GetResolvedTableInfo().TableAlias;
      var property = (PropertyInfo) memberInfo; // TODO Review 2562: Cast using "as". If result is null, throw an UnmappedItemException: "Field '{0}.{1}' cannot be used in a query because it is not a mapped member.".

      if (property.Name == "ID" && property.DeclaringType == typeof (DomainObject))
        return new SqlColumnExpression (property.PropertyType, tableAlias, "ID");

      var potentiallyRedirectedProperty = LinqPropertyRedirectionAttribute.GetTargetProperty (property);
      Tuple<RelationDefinition, ClassDefinition, string> relationData = GetRelationData (property);
      if (relationData != null)
        return new SqlEntityRefMemberExpression (sqlTable, property);

      var classDefinition = GetClassDefinition (property.DeclaringType);
      if (classDefinition == null)
      {
        string message = string.Format (
            "The type '{0}' declaring member '{1}' does not identify a queryable table.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException (message);
      }

      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (potentiallyRedirectedProperty);
      var propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);

      if (propertyDefinition == null)
      {
        string message = string.Format (
            "The member '{0}.{1}' does not have a queryable database mapping.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException (message);
      }

      return new SqlColumnExpression (propertyDefinition.PropertyType, tableAlias, propertyDefinition.StorageSpecificName);
    }

    public Expression ResolveMemberExpression (SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("sqlColumnExpression", sqlColumnExpression);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      if (memberInfo == typeof (ObjectID).GetProperty ("ClassID"))
        return new SqlColumnExpression (typeof (string), sqlColumnExpression.OwningTableAlias, "ClassID");

      throw new UnmappedItemException (string.Format ("The member '{0}.{1}' does not identify a mapped property.",memberInfo.ReflectedType.Name, memberInfo.Name));
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      ArgumentUtility.CheckNotNull ("constantExpression", constantExpression);

      if (constantExpression.Value is DomainObject)
        return new SqlEntityConstantExpression (constantExpression.Type, constantExpression.Value, ((DomainObject) constantExpression.Value).ID);
      else
        return constantExpression;
    }

    public Expression ResolveTypeCheck (Expression innerExpression, Type desiredType)
    {
      ArgumentUtility.CheckNotNull ("innerExpression", innerExpression);
      ArgumentUtility.CheckNotNull ("desiredType", desiredType);

      if (desiredType.IsAssignableFrom (innerExpression.Type))
        return Expression.Constant (true);
      else if(innerExpression.Type.IsAssignableFrom(desiredType))
      {
        if (!typeof (DomainObject).IsAssignableFrom (innerExpression.Type))
        {
          var message = string.Format("No database-level type check can be added for the expression '{0}'."+ 
                                      "Only the types of DomainObjects can be checked in the database query.", innerExpression);
          throw new UnmappedItemException (message);
        }

        var classDefinition = GetClassDefinition (desiredType);
        if(classDefinition==null)
        {
          string message = string.Format (
            "The type '{0}' does not identify a queryable table.", desiredType.Name);
          throw new UnmappedItemException (message);
        }

        var entityExpression = innerExpression as SqlEntityExpression;
        if (entityExpression == null)
          throw new UnmappedItemException(string.Format("The queried expression '{0}' has to be an entity.", innerExpression));

        var sqlColumnExpression = new SqlColumnExpression (typeof (string), entityExpression.PrimaryKeyColumn.OwningTableAlias, "ClassID");
        return Expression.Equal (sqlColumnExpression, new SqlLiteralExpression (classDefinition.ID));
      }
      else
      {
        return Expression.Constant (false);
      }
    }

    private Tuple<RelationDefinition, ClassDefinition, string> GetRelationData (MemberInfo relationMember)
    {
      ArgumentUtility.CheckNotNull ("relationMember", relationMember);
      var property = relationMember as PropertyInfo;
      if (property == null)
        return null;

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[property.DeclaringType];
      if (classDefinition == null)
        return null;

      var potentiallyRedirectedProperty = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (potentiallyRedirectedProperty);
      RelationDefinition relationDefinition = classDefinition.GetRelationDefinition (propertyIdentifier);
      if (relationDefinition == null)
        return null;
      else
        return Tuple.Create (relationDefinition, classDefinition, propertyIdentifier);
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