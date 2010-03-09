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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlGeneration
{
  [TestFixture]
  public class SqlTableSourceVisitorTest
  {
    private SqlCommandBuilder _commandBuilder;

    [SetUp]
    public void SetUp ()
    {
      _commandBuilder = new SqlCommandBuilder ();
    }

    [Test]
    public void GenerateSql_ForSqlTableSource ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTableWithConstantTableSource ();
      sqlTable.TableSource = new SqlTableSource (typeof (int), "Table", "t");
      SqlTableSourceVisitor.GenerateSql (sqlTable, _commandBuilder);

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("[Table] AS [t]"));
    }

    [Test]
    public void GenerateSql_ForSqlJoinedTableSource ()
    {
      var originalTable = new SqlTable (new SqlTableSource (typeof (Kitchen), "KitchenTable", "k"));

      var kitchenCookMember = typeof (Kitchen).GetProperty ("Cook");
      var joinedTable = originalTable.GetOrAddJoin (kitchenCookMember, new JoinedTableSource (kitchenCookMember));

      var foreignTableSource = new SqlTableSource (typeof (Cook), "CookTable", "t2");
      var primaryColumn = new SqlColumnExpression (typeof (int), "t1", "ID");
      var foreignColumn = new SqlColumnExpression (typeof (int), "t2", "FK");

      joinedTable.JoinInfo = new SqlJoinedTableSource (foreignTableSource, primaryColumn, foreignColumn);

      SqlTableSourceVisitor.GenerateSql (originalTable, _commandBuilder);

      Assert.That (_commandBuilder.GetCommandText (), Is.EqualTo ("[KitchenTable] AS [k] JOIN [CookTable] AS [t2] ON [t1].[ID] = [t2].[FK]"));
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException))]
    public void TranslateTableSource_WithUnknownTableSource ()
    {
      var sqlTable = new SqlTable (new UnknownTableSource ());
      SqlTableSourceVisitor.GenerateSql (sqlTable, _commandBuilder);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "ConstantTableSource is not valid at this point.")]
    public void GenerateSql_WithConstantTableSource_RaisesException ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTableWithConstantTableSource ();
      SqlTableSourceVisitor.GenerateSql (sqlTable, _commandBuilder);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "JoinedTableSource is not valid at this point.")]
    public void GenerateSql_JoinedTableSource ()
    {
      var originalTable = new SqlTable (new SqlTableSource (typeof (Cook), "CookTable", "c"));

      var kitchenCookMember = typeof (Kitchen).GetProperty ("Cook");
      originalTable.GetOrAddJoin (kitchenCookMember, new JoinedTableSource (kitchenCookMember));

      SqlTableSourceVisitor.GenerateSql (originalTable, _commandBuilder);
    }
  }
}