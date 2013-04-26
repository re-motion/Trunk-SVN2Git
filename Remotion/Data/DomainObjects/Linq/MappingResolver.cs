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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Linq;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Reflection;
using Remotion.Utilities;
using ArgumentUtility = Remotion.Linq.Utilities.ArgumentUtility;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Implements <see cref="IMappingResolver"/> to supply information from re-store to the re-linq SQL backend.
  /// </summary>
  public class MappingResolver : IMappingResolver
  {
    private readonly IStorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private static readonly PropertyInfo s_classIDPropertyInfo = typeof (ObjectID).GetProperty ("ClassID");
    private static readonly PropertyInfo s_idPropertyInfo = typeof (DomainObject).GetProperty ("ID");

    public MappingResolver (IStorageSpecificExpressionResolver storageSpecificExpressionResolver)
    {
      ArgumentUtility.CheckNotNull ("storageSpecificExpressionResolver", storageSpecificExpressionResolver);

      _storageSpecificExpressionResolver = storageSpecificExpressionResolver;
    }

    public IResolvedTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var classDefinition = GetClassDefinition (tableInfo.ItemType);
      return _storageSpecificExpressionResolver.ResolveTable (classDefinition, generator.GetUniqueIdentifier ("t"));
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("joinInfo", joinInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var leftEndPointDefinition = GetEndPointDefinition (joinInfo.OriginatingEntity, joinInfo.MemberInfo);

      return _storageSpecificExpressionResolver.ResolveJoin (
          joinInfo.OriginatingEntity,
          leftEndPointDefinition,
          leftEndPointDefinition.GetOppositeEndPointDefinition(),
          generator.GetUniqueIdentifier ("t"));
    }

    public SqlEntityDefinitionExpression ResolveSimpleTableInfo (IResolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var classDefinition = GetClassDefinition (tableInfo.ItemType);
      return _storageSpecificExpressionResolver.ResolveEntity (classDefinition, tableInfo.TableAlias);
    }

    public Expression ResolveMemberExpression (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      var property = GetMemberAsPropertyInfo (originatingEntity, memberInfo);
      var entityClassDefinition = GetClassDefinition (originatingEntity.Type);

      if (property.Name == "ID" && property.DeclaringType == typeof (DomainObject))
        return _storageSpecificExpressionResolver.ResolveIDProperty (originatingEntity, entityClassDefinition);

      var propertyInfoAdapter = PropertyInfoAdapter.Create (property);
      var allClassDefinitions = new[] { entityClassDefinition }.Concat (entityClassDefinition.GetAllDerivedClasses ());
      var resolvedMember = allClassDefinitions
          .Select (cd => ResolveMemberInClassDefinition (originatingEntity, propertyInfoAdapter, cd)).
           FirstOrDefault (e => e != null);

      if (resolvedMember == null)
      {
        Assertion.IsNotNull (property.DeclaringType);
        string message = string.Format (
            "The member '{0}.{1}' does not have a queryable database mapping.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException (message);
      }

      return resolvedMember;
    }

    public Expression ResolveMemberExpression (SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("sqlColumnExpression", sqlColumnExpression);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      throw new UnmappedItemException (
          string.Format ("The member '{0}.{1}' does not identify a mapped property.", memberInfo.ReflectedType.Name, memberInfo.Name));
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      ArgumentUtility.CheckNotNull ("constantExpression", constantExpression);

      var domainObject = constantExpression.Value as DomainObject;
      if (domainObject != null)
      {
        var primaryKeyExpression = Expression.Constant (domainObject.ID);
        return new SqlEntityConstantExpression (constantExpression.Type, constantExpression.Value, primaryKeyExpression);
      }
      
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
        if (!ReflectionUtility.IsDomainObject (checkedExpression.Type))
        {
          var message = string.Format (
              "No database-level type check can be added for the expression '{0}'." +
              "Only the types of DomainObjects can be checked in database queries.",
              FormattingExpressionTreeVisitor.Format (checkedExpression));
          throw new UnmappedItemException (message);
        }

        var classDefinition = GetClassDefinition (desiredType);
        var idExpression = Expression.MakeMemberAccess (checkedExpression, s_idPropertyInfo);
        var classIDExpression = Expression.MakeMemberAccess (idExpression, s_classIDPropertyInfo);
        return Expression.Equal (classIDExpression, new SqlLiteralExpression (classDefinition.ID));
      }
      else
        return Expression.Constant (false);
    }

    public Expression TryResolveOptimizedIdentity (SqlEntityRefMemberExpression entityRefMemberExpression)
    {
      ArgumentUtility.CheckNotNull ("entityRefMemberExpression", entityRefMemberExpression);

      var endPointDefinition = GetEndPointDefinition (entityRefMemberExpression.OriginatingEntity, entityRefMemberExpression.MemberInfo);
      var foreignKeyEndPoint = endPointDefinition as RelationEndPointDefinition;
      if (foreignKeyEndPoint == null)
        return null;

      return _storageSpecificExpressionResolver.ResolveEntityIdentityViaForeignKey (entityRefMemberExpression.OriginatingEntity, foreignKeyEndPoint);
    }

    public Expression TryResolveOptimizedMemberExpression (SqlEntityRefMemberExpression entityRefMemberExpression, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("entityRefMemberExpression", entityRefMemberExpression);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      if (memberInfo.DeclaringType != typeof (DomainObject) || memberInfo.Name != "ID")
        return null;

      var endPointDefinition = GetEndPointDefinition (entityRefMemberExpression.OriginatingEntity, entityRefMemberExpression.MemberInfo);
      var foreignKeyEndPoint = endPointDefinition as RelationEndPointDefinition;
      if (foreignKeyEndPoint == null)
        return null;

      return _storageSpecificExpressionResolver.ResolveIDPropertyViaForeignKey (entityRefMemberExpression.OriginatingEntity, foreignKeyEndPoint);
    }

    private ClassDefinition GetClassDefinition (Type type)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (
          type,
          t => new UnmappedItemException (string.Format ("The type '{0}' does not identify a queryable table.", t)));
      return classDefinition;
    }

    private static PropertyInfo GetMemberAsPropertyInfo (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      var property = memberInfo as PropertyInfo;
      if (property == null)
      {
        throw new UnmappedItemException (
            string.Format (
                "Field '{0}.{1}' cannot be used in a query because it is not a mapped member.",
                originatingEntity.Type.Name,
                memberInfo.Name));
      }
      return property;
    }

    private Expression ResolveMemberInClassDefinition (
        SqlEntityExpression originatingEntity, PropertyInfoAdapter propertyInfoAdapter, ClassDefinition classDefinition)
    {
      var endPointDefinition = classDefinition.ResolveRelationEndPoint (propertyInfoAdapter);
      if (endPointDefinition != null)
        return new SqlEntityRefMemberExpression (originatingEntity, propertyInfoAdapter.PropertyInfo);

      var propertyDefinition = classDefinition.ResolveProperty (propertyInfoAdapter);
      if (propertyDefinition != null)
        return _storageSpecificExpressionResolver.ResolveProperty (originatingEntity, propertyDefinition);

      return null;
    }

    private IRelationEndPointDefinition GetEndPointDefinition (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      var property = GetMemberAsPropertyInfo (originatingEntity, memberInfo);
      var entityClassDefinition = GetClassDefinition (originatingEntity.Type);

      var propertyInfoAdapter = PropertyInfoAdapter.Create (property);
      var allClassDefinitions = new[] { entityClassDefinition }.Concat (entityClassDefinition.GetAllDerivedClasses());
      var leftEndPointDefinition = allClassDefinitions
          .Select (cd => cd.ResolveRelationEndPoint (propertyInfoAdapter))
          .FirstOrDefault (e => e != null);

      if (leftEndPointDefinition == null)
      {
        Assertion.IsNotNull (memberInfo.DeclaringType);
        string message =
            string.Format ("The member '{0}.{1}' does not identify a relation.", memberInfo.DeclaringType.Name, memberInfo.Name);
        throw new UnmappedItemException (message);
      }
      return leftEndPointDefinition;
    }
  }
}