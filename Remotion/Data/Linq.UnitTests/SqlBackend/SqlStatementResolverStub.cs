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
using Remotion.Data.Linq.SqlBackend;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.SqlBackend
{
  public class SqlStatementResolverStub : ISqlStatementResolver
  {
    public virtual SqlTableSource ResolveConstantTableSource (ConstantTableSource tableSource)
    {
      var tableName = tableSource.ConstantExpression.Value.ToString();
      var tableAlias = tableName.Substring (0, 1).ToLower();
      return new SqlTableSource (typeof (string), tableName, tableAlias);
    }

    public virtual Expression ResolveTableReferenceExpression (SqlTableReferenceExpression tableReferenceExpression)
    {
      tableReferenceExpression.SqlTable.TableSource = new SqlTableSource (typeof (Cook), "Cook", "c");
      return new SqlColumnListExpression (
          tableReferenceExpression.Type,
          new[]
          {
              new SqlColumnExpression (typeof (Cook), "c", "ID"),
              new SqlColumnExpression (typeof (Cook), "c", "Name"),
              new SqlColumnExpression (typeof (Cook), "c", "City")
          });
    }

    public virtual Expression ResolveMemberExpression (SqlMemberExpression memberExpression, UniqueIdentifierGenerator generator)
    {
      var table = memberExpression.SqlTable.GetOrAddJoin (memberExpression.MemberInfo, memberExpression.SqlTable.TableSource);
      if (table.TableSource != memberExpression.SqlTable.TableSource)
      {
        memberExpression.SqlTable.TableSource = table.TableSource;
        return new SqlColumnExpression (typeof (Cook), generator.GetUniqueIdentifier ("t"), "FirstName");
      }
      else
      {
        memberExpression.SqlTable.TableSource = new SqlTableSource (typeof (Cook), "Cook", "c");
        return new SqlColumnExpression (typeof (Cook), "c", "FirstName");
      }
    }

    public SqlJoinedTableSource ResolveJoinedTableSource (SqlTable sourceSqlTable, SqlTable joinSqlTable)
    {
      return new SqlJoinedTableSource (
          (SqlTableSource) sourceSqlTable.TableSource, (SqlTableSource) joinSqlTable.TableSource, "ID", "KitchenID", joinSqlTable.TableSource.Type);
    }
  }
}