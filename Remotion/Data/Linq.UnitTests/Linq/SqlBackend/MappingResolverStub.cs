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
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend
{
  public class MappingResolverStub : IMappingResolver
  {
    public virtual IResolvedTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      switch (tableInfo.ItemType.Name)
      {
        case "Cook":
        case "Kitchen":
        case "Restaurant":
        case "Compyany":
        case "Chef":
          return CreateResolvedTableInfo (tableInfo.ItemType, generator);
      }

      throw new UnmappedItemException ("The type " + tableInfo.ItemType + " cannot be queried from the stub provider.");
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      if (joinInfo.MemberInfo.DeclaringType == typeof (Cook))
      {
        switch (joinInfo.MemberInfo.Name)
        {
          case "Substitution":
            return CreateResolvedJoinInfo (
                (ResolvedSimpleTableInfo) joinInfo.OriginatingTable.GetResolvedTableInfo(),
                "ID",
                CreateResolvedTableInfo (joinInfo.ItemType, generator),
                "SubstitutedID");
          case "Assistants":
            return CreateResolvedJoinInfo (
                (ResolvedSimpleTableInfo) joinInfo.OriginatingTable.GetResolvedTableInfo(),
                "ID",
                CreateResolvedTableInfo (joinInfo.ItemType, generator),
                "AssistedID");
        }
      }
      else if (joinInfo.MemberInfo.DeclaringType == typeof (Kitchen))
      {
        switch (joinInfo.MemberInfo.Name)
        {
          case "Cook":
            return CreateResolvedJoinInfo (
                (ResolvedSimpleTableInfo) joinInfo.OriginatingTable.GetResolvedTableInfo(),
                "ID",
                CreateResolvedTableInfo (joinInfo.ItemType, generator),
                "KitchenID");
          case "Restaurant":
            return CreateResolvedJoinInfo (
                (ResolvedSimpleTableInfo) joinInfo.OriginatingTable.GetResolvedTableInfo(),
                "RestaurantID",
                CreateResolvedTableInfo (joinInfo.ItemType, generator),
                "ID");
        }
      }
      else if (joinInfo.MemberInfo.DeclaringType == typeof (Restaurant))
      {
        switch (joinInfo.MemberInfo.Name)
        {
          case "SubKitchen":
            return CreateResolvedJoinInfo (
                (ResolvedSimpleTableInfo) joinInfo.OriginatingTable.GetResolvedTableInfo(),
                "ID",
                CreateResolvedTableInfo (joinInfo.ItemType, generator),
                "RestaurantID");
          case "Cooks":
            return CreateResolvedJoinInfo (
                (ResolvedSimpleTableInfo) joinInfo.OriginatingTable.GetResolvedTableInfo(),
                "ID",
                CreateResolvedTableInfo (joinInfo.ItemType, generator),
                "RestaurantID");
        }
      }

      throw new UnmappedItemException ("Member " + joinInfo.MemberInfo + " is not a valid join member.");
    }

    public virtual Expression ResolveTableReferenceExpression (
        SqlTableReferenceExpression tableReferenceExpression, UniqueIdentifierGenerator generator)
    {
      var resolvedTableInfo = tableReferenceExpression.SqlTable.GetResolvedTableInfo();
      if (resolvedTableInfo is ResolvedSubStatementTableInfo &&
          (tableReferenceExpression.Type == typeof (string) || (tableReferenceExpression.Type == typeof (int))
           || (tableReferenceExpression.Type == typeof (object))))
        return new SqlValueTableReferenceExpression (new SqlTable (resolvedTableInfo));
      else
        return CreateEntityExpression (tableReferenceExpression.SqlTable, resolvedTableInfo);
    }

    public virtual Expression ResolveMemberExpression (SqlTableBase sqlTable, MemberInfo memberInfo, UniqueIdentifierGenerator generator)
    {
      var memberType = ReflectionUtility.GetFieldOrPropertyType (memberInfo);
      if (memberInfo.DeclaringType == typeof (Cook))
      {
        switch (memberInfo.Name)
        {
          case "ID":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo (), memberInfo.Name, true);
          case "FirstName":
          case "Name":
          case "IsFullTimeCook":
          case "IsStarredCook":
          case "Weight":
          case "MetaID":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo(), memberInfo.Name, false);
          case "Substitution":
            return new SqlEntityRefMemberExpression (sqlTable, memberInfo);
          
        }
      }
      else if (memberInfo.DeclaringType == typeof (ISpecificCook))
      {
        switch (memberInfo.Name)
        {
          case "SpecificInformation":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo (), memberInfo.Name, false);
        }
      }
      else if (memberInfo.DeclaringType == typeof (Chef))
      {
        switch (memberInfo.Name)
        {
          case "LetterOfRecommendation":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo (), memberInfo.Name, false);
        }
      }
      else if (memberInfo.DeclaringType == typeof (Kitchen))
      {
        switch (memberInfo.Name)
        {
          case "ID":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo (), memberInfo.Name, true);
          case "Name":
          case "RoomNumber":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo (), memberInfo.Name, false);
          case "Cook":
          case "Restaurant":
            return new SqlEntityRefMemberExpression (sqlTable, memberInfo);
        }
      }
      else if (memberInfo.DeclaringType == typeof (Restaurant))
      {
        switch (memberInfo.Name)
        {
          case "ID":
            return CreateColumn (memberType, sqlTable.GetResolvedTableInfo (), memberInfo.Name, true);
          case "SubKitchen":
            return new SqlEntityRefMemberExpression (sqlTable, memberInfo);
        }
      }

      throw new UnmappedItemException ("Cannot resolve member: " + memberInfo);
    }

    public Expression ResolveMemberExpression (SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo)
    {
      if (memberInfo.DeclaringType == typeof (MetaID))
      {
        if (memberInfo.Name == "ClassID")
          return new SqlColumnExpression (typeof (string), sqlColumnExpression.OwningTableAlias, "ClassID", false);
      }
      throw new UnmappedItemException ("Cannot resolve member for: " + memberInfo.Name);
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      if (constantExpression.Value is Cook)
        return new SqlEntityConstantExpression (typeof (Cook), constantExpression.Value, ((Cook) constantExpression.Value).ID);
      else
        return constantExpression;
    }

    public Expression ResolveTypeCheck (Expression innerExpression, Type desiredType)
    {
       if (desiredType.IsAssignableFrom (innerExpression.Type))
         return Expression.Constant (true);
       else if (desiredType == typeof (Chef) && innerExpression.Type == typeof (Cook))
         return Expression.MakeMemberAccess (innerExpression, typeof (Cook).GetProperty ("IsStarredCook"));
       else
         throw new UnmappedItemException ("Cannot resolve type for innerExpression: " + innerExpression.Type.Name);
    }

    private SqlColumnExpression CreateColumn (Type columnType, IResolvedTableInfo resolvedSimpleTableInfo, string columnName, bool isPriamryKey)
    {
      return new SqlColumnExpression (columnType, resolvedSimpleTableInfo.TableAlias, columnName, isPriamryKey);
    }

    private SqlEntityExpression CreateEntityExpression (SqlTableBase sqlTable, IResolvedTableInfo tableInfo)
    {
      Type type = tableInfo.ItemType;
      if (type == typeof (Cook))
      {
        var primaryKeyColumn = CreateColumn (typeof (int), tableInfo, "ID", true);
        return new SqlEntityExpression (
            sqlTable,
            primaryKeyColumn,
            new[]
            {
                primaryKeyColumn,
                CreateColumn (typeof (string), tableInfo, "FirstName", false),
                CreateColumn (typeof (string), tableInfo, "Name", false),
                CreateColumn (typeof (bool), tableInfo, "IsStarredCook", false),
                CreateColumn (typeof (bool), tableInfo, "IsFullTimeCook", false),
                CreateColumn (typeof (int), tableInfo, "SubstitutedID", false),
                CreateColumn (typeof (int), tableInfo, "KitchenID", false)
            });
      }
      else if (type == typeof (Kitchen))
      {
        var primaryKeyColumn = CreateColumn (typeof (int), tableInfo, "ID", true);
        return new SqlEntityExpression (
            sqlTable,
            primaryKeyColumn,
            new[]
            {
                primaryKeyColumn,
                CreateColumn (typeof (int), tableInfo, "CookID", false),
                CreateColumn (typeof (string), tableInfo, "Name", false),
                CreateColumn (typeof (int), tableInfo, "RestaurantID", false),
                CreateColumn (typeof (int), tableInfo, "SubKitchenID", false),
            });
      }
      else if (type == typeof (Restaurant))
      {
        var primaryKeyColumn = CreateColumn (typeof (int), tableInfo, "ID", true);
        return new SqlEntityExpression (
            sqlTable,
            primaryKeyColumn,
            new[]
            {
                primaryKeyColumn,
                CreateColumn (typeof (int), tableInfo, "CookID", false),
                CreateColumn (typeof (string), tableInfo, "Name", false),
            });
      }
      else if (type == typeof (Chef))
      {
        var primaryKeyColumn = CreateColumn (typeof (int), tableInfo, "ID", true);
        return new SqlEntityExpression (
            sqlTable,
            primaryKeyColumn,
            new[]
            {
                primaryKeyColumn,
                CreateColumn (typeof (string), tableInfo, "FirstName", false),
                CreateColumn (typeof (string), tableInfo, "Name", false),
                CreateColumn (typeof (bool), tableInfo, "IsStarredCook", false),
                CreateColumn (typeof (bool), tableInfo, "IsFullTimeCook", false),
                CreateColumn (typeof (int), tableInfo, "SubstitutedID", false),
                CreateColumn (typeof (int), tableInfo, "KitchenID", false),
                CreateColumn (typeof (string), tableInfo, "LetterOfRecommendation", false)
            });
      }
      return null;
    }

    private ResolvedSimpleTableInfo CreateResolvedTableInfo (Type entityType, UniqueIdentifierGenerator generator)
    {
      return new ResolvedSimpleTableInfo (entityType, entityType.Name + "Table", generator.GetUniqueIdentifier ("t"));
    }

    private ResolvedJoinInfo CreateResolvedJoinInfo (
        ResolvedSimpleTableInfo originatingTableInfo, string primaryKeyName, ResolvedSimpleTableInfo foreignTableInfo, string foreignKeyName)
    {
      var primaryColumn = CreateColumn (typeof (int), originatingTableInfo, primaryKeyName, true);
      var foreignColumn = CreateColumn (typeof (int), foreignTableInfo, foreignKeyName, false);

      return new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);
    }
  }
}