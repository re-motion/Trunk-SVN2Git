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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Backend;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Backend.FieldResolving
{
  [TestFixture]
  public class JoinedTableContextTest
  {
    private QueryModel _queryModel;
    private JoinedTableContext _context;

    [SetUp]
    public void SetUp ()
    {
      _queryModel = ExpressionHelper.CreateQueryModel_Student ();
      _context = new JoinedTableContext (StubDatabaseInfo.Instance);
    }

    [Test]
    public void GetJoinedTable()
    {
      FieldSourcePath fieldSourcePath = ExpressionHelper.GetPathForNewTable();
      MemberInfo member = typeof (Kitchen).GetProperty ("Cook");
      Assert.That (_context.Count, Is.EqualTo (0));
      Table table = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member);
      Assert.That (_context.Count, Is.EqualTo (1));
      Assert.That (table, Is.Not.Null);
      Assert.That (table.Alias, Is.Null);
      Assert.That (table.Name, Is.EqualTo ("cookTable"));
    }

    [Test]
    public void GetJoinedTable_Twice ()
    {
      var table = new Table();
      FieldSourcePath fieldSourcePath1 = ExpressionHelper.GetPathForTable (table);
      FieldSourcePath fieldSourcePath2 = ExpressionHelper.GetPathForTable (table);
      MemberInfo member = typeof (Kitchen).GetProperty ("Cook");
      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath1, member);
      Table table2 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath2, member);
      Assert.That (_context.Count, Is.EqualTo (1));
      Assert.That (table2, Is.SameAs (table1));
    }

    [Test]
    public void GetJoinedTable_TwiceWithDifferentMembers ()
    {
      FieldSourcePath fieldSourcePath = ExpressionHelper.GetPathForNewTable ();
      MemberInfo member1 = typeof (Kitchen).GetProperty ("Restaurant");
      MemberInfo member2 = typeof (Kitchen).GetProperty ("Cook");
      
      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member1);
      Table table2 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member2);
      Assert.That (_context.Count, Is.EqualTo (2));
      Assert.That (table2, Is.Not.SameAs (table1));
      Assert.That (table1.Name, Is.EqualTo ("restaurantTable"));
      Assert.That (table2.Name, Is.EqualTo ("cookTable"));
    }

    [Test]
    public void GetJoinedTable_TwiceWithDifferentFieldPath ()
    {
      FieldSourcePath fieldSourcePath1 = ExpressionHelper.GetPathForNewTable ("x", "y");
      FieldSourcePath fieldSourcePath2 = ExpressionHelper.GetPathForNewTable ("z", "i");

      MemberInfo member = typeof (Kitchen).GetProperty ("Restaurant");
      
      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath1, member);
      Table table2 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath2, member);

      Assert.That (table2, Is.Not.SameAs (table1));
      Assert.That (table2, Is.EqualTo (table1));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = 
        "'Remotion.Data.Linq.UnitTests.TestDomain.Cook.FirstName' is not a relation member.")]
    public void GetJoinedTable_InvalidMember ()
    {
      FieldSourcePath fieldSourcePath = ExpressionHelper.GetPathForNewTable ();
      MemberInfo member = typeof (Cook).GetProperty ("FirstName");
      _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member);
    }

    [Test]
    public void CreateAliases()
    {
      FieldSourcePath fieldSourcePath = ExpressionHelper.GetPathForNewTable ();
      
      MemberInfo member1 = typeof (Kitchen).GetProperty ("Cook");
      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member1);

      Assert.That (table1.Alias, Is.Null);
      _context.CreateAliases (_queryModel);
      Assert.That (table1.Alias, Is.EqualTo ("#j0"));
    }

    [Test]
    public void CreateAliases_DoesntOverwriteAliases ()
    {
      FieldSourcePath fieldSourcePath = ExpressionHelper.GetPathForNewTable ();

      MemberInfo member1 = typeof (Kitchen).GetProperty ("Cook");
      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member1);
      table1.SetAlias ("Franz");

      _context.CreateAliases (_queryModel);
      Assert.That (table1.Alias, Is.EqualTo ("Franz"));
    }

    [Test]
    public void CreateAliases_MultipleTimes ()
    {
      FieldSourcePath fieldSourcePath = ExpressionHelper.GetPathForNewTable ();

      MemberInfo member1 = typeof (Kitchen).GetProperty ("Cook");
      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member1);

      Assert.That (table1.Alias, Is.Null);
      _context.CreateAliases (_queryModel);
      Assert.That (table1.Alias, Is.EqualTo ("#j0"));

      MemberInfo member2 = typeof (Kitchen).GetProperty ("Restaurant");
      Table table2 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath, member2);

      Assert.That (table2.Alias, Is.Null);
      _context.CreateAliases (_queryModel);
      Assert.That (table2.Alias, Is.EqualTo ("#j1"));
    }

    [Test]
    public void CreateAliases_MultipleTablesAndOrdering ()
    {
      FieldSourcePath fieldSourcePath1 = ExpressionHelper.GetPathForNewTable ("1", null);
      FieldSourcePath fieldSourcePath2 = ExpressionHelper.GetPathForNewTable ("2", null);
      FieldSourcePath fieldSourcePath3 = ExpressionHelper.GetPathForNewTable ("3", null);

      MemberInfo member = typeof (Kitchen).GetProperty ("Cook");

      Table table1 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath1, member);
      Table table2 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath2, member);
      Table table3 = _context.GetJoinedTable (StubDatabaseInfo.Instance, fieldSourcePath3, member);

      Assert.That (table1.Alias, Is.Null);
      Assert.That (table2.Alias, Is.Null);
      Assert.That (table3.Alias, Is.Null);

      _context.CreateAliases (_queryModel);

      Assert.That (table1.Alias, Is.EqualTo ("#j0"));
      Assert.That (table2.Alias, Is.EqualTo ("#j1"));
      Assert.That (table3.Alias, Is.EqualTo ("#j2"));
    }

    [Test]
    public void GetColumnSource ()
    {
      IQueryable querySource = ExpressionHelper.CreateStudentQueryable ();

      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause_Int ("s1", typeof (Cook), querySource);
      Assert.That (_context.GetColumnSource (fromClause), Is.EqualTo (new Table ("cookTable", "s1")));
    }

    [Test]
    public void GetColumnSource_CachesInstance ()
    {
      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause_Student ();
      IColumnSource t1 = _context.GetColumnSource (fromClause);
      IColumnSource t2 = _context.GetColumnSource (fromClause);
      Assert.That (t2, Is.SameAs (t1));
    }

    [Test]
    public void GetColumnSource_SubQuery ()
    {
      var subQueryExpression = new SubQueryExpression (ExpressionHelper.CreateQueryModel_Student ());
      var fromClause = new AdditionalFromClause ("s", typeof (Cook), subQueryExpression);

      IColumnSource columnSource = _context.GetColumnSource (fromClause);
      Assert.That (columnSource, Is.EqualTo (new SubQuery (subQueryExpression.QueryModel, ParseMode.SubQueryInFrom, "s")));
    }
  }
}
