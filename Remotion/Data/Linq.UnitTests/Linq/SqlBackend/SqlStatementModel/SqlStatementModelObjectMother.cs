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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Data.Linq.Clauses;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel
{
  public class SqlStatementModelObjectMother
  {
    public static SqlStatement CreateSqlStatementWithCook ()
    {
      var sqlTable = CreateSqlTable_WithUnresolvedTableInfo (typeof (Cook));
      return new SqlStatement (new SqlTableReferenceExpression (sqlTable), new[] { sqlTable }, new Ordering[] { });
    }

    public static SqlStatement CreateSqlStatement ()
    {
      var sqlTable = CreateSqlTable_WithUnresolvedTableInfo();
      return new SqlStatement (new SqlTableReferenceExpression (sqlTable), new[] { sqlTable }, new Ordering[] { });
    }

    public static SqlStatement CreateSqlStatement_Resolved (Type type)
    {
      var sqlTable = CreateSqlTable_WithResolvedTableInfo (type);
      return new SqlStatement (CreateSqlEntityExpression (type), new[] { sqlTable }, new Ordering[] { });
    }

    public static SqlTable CreateSqlTable ()
    {
      return CreateSqlTable (typeof (Cook));
    }

    public static SqlTable CreateSqlTable (ITableInfo tableInfo)
    {
      var sqlTable = new SqlTable (tableInfo);
      return sqlTable;
    }

    public static SqlTable CreateSqlTable (Type type)
    {
      return new SqlTable (CreateUnresolvedTableInfo (type));
    }

    public static SqlTable CreateSqlTable_WithUnresolvedTableInfo ()
    {
      return CreateSqlTable_WithUnresolvedTableInfo (typeof (int));
    }

    public static SqlTable CreateSqlTable_WithUnresolvedTableInfo (Type type)
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (type);
      var sqlTable = new SqlTable (unresolvedTableInfo);
      return sqlTable;
    }

    public static SqlTable CreateSqlTable_WithResolvedTableInfo ()
    {
      return CreateSqlTable_WithResolvedTableInfo ("Table", "t");
    }

    public static SqlTable CreateSqlTable_WithResolvedTableInfo (Type type)
    {
      return CreateSqlTable_WithResolvedTableInfo (type, "Table", "t");
    }

    public static SqlTable CreateSqlTable_WithResolvedTableInfo (string tableName, string tableAlias)
    {
      var resolvedTableInfo = new ResolvedSimpleTableInfo (typeof (string), tableName, tableAlias);
      var sqlTable = new SqlTable (resolvedTableInfo);
      return sqlTable;
    }

    public static SqlTable CreateSqlTable_WithResolvedTableInfo (Type type, string tableName, string tableAlias)
    {
      var resolvedTableInfo = new ResolvedSimpleTableInfo (type, tableName, tableAlias);
      var sqlTable = new SqlTable (resolvedTableInfo);
      return sqlTable;
    }

    public static SqlJoinedTable CreateSqlJoinedTable_WithUnresolvedJoinInfo ()
    {
      var sqlTable = new SqlTable (new UnresolvedTableInfo (typeof (Cook)));
      var joinInfo = new UnresolvedJoinInfo (sqlTable, typeof (Cook).GetProperty ("FirstName"), JoinCardinality.One);
      return new SqlJoinedTable (joinInfo);
    }

    public static UnresolvedTableInfo CreateUnresolvedTableInfo ()
    {
      return CreateUnresolvedTableInfo (typeof (Cook));
    }

    public static UnresolvedTableInfo CreateUnresolvedTableInfo (Type type)
    {
      var array = Array.CreateInstance (type, 0);
      return new UnresolvedTableInfo (type);
    }

    public static UnresolvedJoinInfo CreateUnresolvedJoinInfo_KitchenCook ()
    {
      var sqlTable = new SqlTable (new UnresolvedTableInfo (typeof (Cook))); // TODO Review 2486: This is not correct, to join on Kitchen.Cook, the left side table must be of type Kitchen (we might add a type check in the future)
      return new UnresolvedJoinInfo (sqlTable, typeof (Kitchen).GetProperty ("Cook"), JoinCardinality.One);
    }

    public static ResolvedSimpleTableInfo CreateResolvedTableInfo ()
    {
      return CreateResolvedTableInfo (typeof (Cook));
    }

    public static ResolvedSimpleTableInfo CreateResolvedTableInfo (Type type)
    {
      return new ResolvedSimpleTableInfo (type, "table", "t");
    }

    public static ResolvedJoinInfo CreateResolvedJoinInfo ()
    {
      return CreateResolvedJoinInfo (typeof (Cook));
    }

    public static ResolvedJoinInfo CreateResolvedJoinInfo (Type type)
    {
      var primaryColumn = new SqlColumnExpression (typeof (int), "k", "ID");
      var foreignColumn = new SqlColumnExpression (typeof (int), "s", "ID");
      var foreignTableInfo = new ResolvedSimpleTableInfo (type, "Table", "s");
      return new ResolvedJoinInfo (foreignTableInfo, primaryColumn, foreignColumn);
    }

    public static SqlEntityExpression CreateSqlEntityExpression (Type type)
    {
      var primaryKeyColumn = new SqlColumnExpression (typeof (int), "t", "ID");
      return new SqlEntityExpression (
          type,
          primaryKeyColumn,
          new[]
          {
              primaryKeyColumn,
              new SqlColumnExpression (typeof (int), "t", "Name"),
              new SqlColumnExpression (typeof (int), "t", "City")
          });
    }
  }
}