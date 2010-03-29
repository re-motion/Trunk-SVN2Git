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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
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

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[tableInfo.ItemType];
      if (classDefinition == null)
      {
        string message = string.Format ("The item type '{0}' does not identify a queryable table.", tableInfo.ItemType.Name);
        throw new UnmappedItemException (message);
      }
      else
      {
        var viewName = classDefinition.GetViewName();
        return new ResolvedSimpleTableInfo (tableInfo.ItemType, viewName, generator.GetUniqueIdentifier ("t"));
      }
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

      var leftKey = new SqlColumnExpression (keyType, joinInfo.SqlTable.GetResolvedTableInfo().TableAlias, GetJoinColumnName (leftEndPoint));
      var rightKey = new SqlColumnExpression (keyType, alias, GetJoinColumnName (rightEndPoint));

      return new ResolvedJoinInfo (resolvedSimpleTableInfo, leftKey, rightKey);
    }

    public Expression ResolveTableReferenceExpression (SqlTableReferenceExpression tableReferenceExpression, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("tableReferenceExpression", tableReferenceExpression);
      ArgumentUtility.CheckNotNull ("generator", generator);

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[tableReferenceExpression.SqlTable.ItemType];

      if (classDefinition == null)
      {
        string message = string.Format ("The type '{0}' does not identify a queryable table.", tableReferenceExpression.SqlTable.ItemType.Name);
        throw new UnmappedItemException (message);
      }

      var tableAlias = tableReferenceExpression.SqlTable.GetResolvedTableInfo().TableAlias;

      var propertyInfo = typeof (DomainObject).GetProperty ("ID");
      var primaryKeyColumn = new SqlColumnExpression (propertyInfo.PropertyType, tableAlias, propertyInfo.Name);

      var classIDPropertyInfo = typeof (ObjectID).GetProperty ("ClassID");
      var classIDColumn = new SqlColumnExpression (classIDPropertyInfo.PropertyType, tableAlias, classIDPropertyInfo.Name);

      var timestampPropertyInfo = typeof (DomainObject).GetProperty ("Timestamp");
      var timestampColumn = new SqlColumnExpression (timestampPropertyInfo.PropertyType, tableAlias, timestampPropertyInfo.Name);

      var columns = new List<SqlColumnExpression>();

      columns.Add (primaryKeyColumn);
      columns.Add (classIDColumn);
      columns.Add (timestampColumn);

      // TODO Review 2439: Use a LINQ expression to get the sql columns, use AddRange to add them to the list of columns

      foreach (PropertyDefinition propertyDefinition in classDefinition.GetPropertyDefinitions())
      {
        //TODO 2439 test
        // TODO Review 2439: To test this, use a Computer table reference.
        if (propertyDefinition.StorageClass == StorageClass.Persistent)
        {
          var storageSpecificName = propertyDefinition.StorageSpecificName;
          if (!string.IsNullOrEmpty (storageSpecificName))
            columns.Add (new SqlColumnExpression (propertyDefinition.PropertyType, tableAlias, storageSpecificName));
        }
      }

      // TODO Review 2439: Use tableReferenceExpression.Type instead of tableReferenceExpression.SqlTable.ItemType, it's the same, but faster
      return new SqlEntityExpression (tableReferenceExpression.SqlTable.ItemType, primaryKeyColumn, columns.ToArray());
    }

    public Expression ResolveMemberExpression (SqlMemberExpression memberExpression, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("memberExpression", memberExpression);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var tableAlias = memberExpression.SqlTable.GetResolvedTableInfo().TableAlias;
      var property = (PropertyInfo) memberExpression.MemberInfo;

      if (property.Name == "ID" && property.DeclaringType == typeof (DomainObject))
        return new SqlColumnExpression (property.GetType(), tableAlias, "ID");

      // TODO Review 2439: Extract a method for these lookups, they occur in nearly every member of this class
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[property.DeclaringType];
      if (classDefinition == null)
      {
        string message = string.Format (
            "The type '{0}' declaring member '{1}' does not identify a queryable table.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException (message);
      }

      var potentiallyRedirectedProperty = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      // TODO Review 2439: This does not work for 1:1 properties when the propery is the non-foreign key side. For example: Order.OrderTicket.
      // TODO Review 2439: At the non-key side, there is no property definition. However, there's a relation definition.
      // TODO Review 2439: Add a test with Employee.Computer - it should fail. Then, change code to first try to get the relation data (GetRelationData). If this succeeds, return a SqlEntityRefMemberExpression.
      // TODO Review 2439: If it doesn't, try to get the property definition. If there is no property definition at this point, throw the exception. Otherwise, return a SqlColumnExpression.

      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (potentiallyRedirectedProperty);
      var propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);

      if (propertyDefinition == null)
      {
        string message = string.Format (
            "The member '{0}.{1}' does not have a queryable database mapping.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException (message);
      }

      if (propertyDefinition.IsObjectID)
        return new SqlEntityRefMemberExpression (memberExpression.SqlTable, property);
      else
        return new SqlColumnExpression (propertyDefinition.PropertyType, tableAlias, propertyDefinition.StorageSpecificName);
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      ArgumentUtility.CheckNotNull ("constantExpression", constantExpression);

      if (constantExpression.Value is DomainObject)
        return new SqlEntityConstantExpression (constantExpression.Type, constantExpression.Value, ((DomainObject) constantExpression.Value).ID);
      else
        return constantExpression;
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

    private string GetJoinColumnName (IRelationEndPointDefinition endPoint)
    {
      return endPoint.IsVirtual ? "ID" : endPoint.ClassDefinition.GetMandatoryPropertyDefinition (endPoint.PropertyName).StorageSpecificName;
    }
  }
}