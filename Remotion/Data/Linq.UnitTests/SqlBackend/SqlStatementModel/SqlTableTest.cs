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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.TestDomain;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel
{
  [TestFixture]
  public class SqlTableTest
  {
    [Test]
    public void SameType ()
    {
      var oldTableSource = new SqlTableSource (typeof (int), "table1", "t");
      var sqlTable = new SqlTable (oldTableSource);
      var newTableSource = new SqlTableSource (typeof (int), "table2", "s");

      sqlTable.TableSource = newTableSource;

      Assert.That (sqlTable.TableSource.ItemType, Is.EqualTo (newTableSource.ItemType));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void DifferentType ()
    {
      var oldTableSource = new SqlTableSource (typeof (int), "table1", "t");
      var sqlTable = new SqlTable (oldTableSource);
      var newTableSource = new SqlTableSource (typeof (string), "table2", "s");

      sqlTable.TableSource = newTableSource;
    }

    [Test]
    public void GetOrAddJoin_NewEntry ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable_TypeIsCook();

      var memberInfo = typeof (Cook).GetProperty ("FirstName");
      var joinedTableSource = new JoinedTableSource (memberInfo);

      var joinedTable = sqlTable.GetOrAddJoin (memberInfo, joinedTableSource);
      Assert.That (joinedTable.JoinInfo, Is.SameAs (joinedTableSource));
    }

    [Test]
    public void GetOrAddJoin_GetEntry_Twice ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable_TypeIsCook ();

      var memberInfo = typeof (Cook).GetProperty ("FirstName");
      var originalJoinedTableSource = new JoinedTableSource (memberInfo);

      var joinedTable1 = sqlTable.GetOrAddJoin (memberInfo, originalJoinedTableSource);
      var joinedTable2 = sqlTable.GetOrAddJoin (memberInfo, new JoinedTableSource (memberInfo));

      Assert.That (joinedTable2, Is.SameAs (joinedTable1));
      Assert.That (joinedTable2.JoinInfo, Is.SameAs (originalJoinedTableSource));
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Type mismatch between String and Int32.")]
    [Test]
    public void GetOrAddJoin_ThrowsException ()
    {
      var memberInfo = typeof (Cook).GetProperty ("FirstName");
      var tableSource = new JoinedTableSource (typeof (Cook).GetProperty ("ID"));

      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable_TypeIsCook();

      sqlTable.GetOrAddJoin (memberInfo, tableSource);
    }

  }
}