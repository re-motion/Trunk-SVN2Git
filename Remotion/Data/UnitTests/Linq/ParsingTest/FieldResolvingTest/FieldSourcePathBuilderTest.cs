// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing.FieldResolving;

namespace Remotion.Data.UnitTests.Linq.ParsingTest.FieldResolvingTest
{
  [TestFixture]
  public class FieldSourcePathBuilderTest
  {
    private JoinedTableContext _context;
    private Table _initialTable;
    private PropertyInfo _studentDetailMember;
    private PropertyInfo _studentMember;

    [SetUp]
    public void SetUp()
    {
      _context = new JoinedTableContext();
      _initialTable = new Table ("initial", "i");

      _studentDetailMember = typeof (Student_Detail_Detail).GetProperty ("Student_Detail");
      _studentMember = typeof (Student_Detail).GetProperty ("Student");
    }

    [Test]
    public void BuildFieldSourcePath_NoJoin ()
    {
      MemberInfo[] joinMembers = new MemberInfo[] { };
      FieldSourcePath result =
          new FieldSourcePathBuilder ().BuildFieldSourcePath (StubDatabaseInfo.Instance, _context, _initialTable, joinMembers);

      FieldSourcePath expected = new FieldSourcePath(_initialTable, new SingleJoin[0]);
      Assert.AreEqual (expected, result);
    }

    [Test]
    public void BuildFieldSourcePath_SimpleJoin ()
    {
      MemberInfo[] joinMembers = new MemberInfo[] { _studentMember };
      FieldSourcePath result =
          new FieldSourcePathBuilder ().BuildFieldSourcePath (StubDatabaseInfo.Instance, _context, _initialTable, joinMembers);

      Table relatedTable = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, _studentMember);
      Tuple<string, string> joinColumns = DatabaseInfoUtility.GetJoinColumnNames (StubDatabaseInfo.Instance, _studentMember);

      SingleJoin singleJoin = new SingleJoin (new Column (_initialTable, joinColumns.A), new Column (relatedTable, joinColumns.B));
      FieldSourcePath expected = new FieldSourcePath (_initialTable, new[] { singleJoin });
      Assert.AreEqual (expected, result);
    }

    [Test]
    public void BuildFieldSourcePath_NestedJoin ()
    {
      MemberInfo[] joinMembers = new MemberInfo[] { _studentDetailMember, _studentMember };
      FieldSourcePath result =
          new FieldSourcePathBuilder ().BuildFieldSourcePath (StubDatabaseInfo.Instance, _context, _initialTable, joinMembers);

      Table relatedTable1 = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, _studentDetailMember);
      Tuple<string, string> joinColumns1 = DatabaseInfoUtility.GetJoinColumnNames (StubDatabaseInfo.Instance, _studentDetailMember);

      Table relatedTable2 = DatabaseInfoUtility.GetRelatedTable (StubDatabaseInfo.Instance, _studentMember);
      Tuple<string, string> joinColumns2 = DatabaseInfoUtility.GetJoinColumnNames (StubDatabaseInfo.Instance, _studentMember);

      SingleJoin singleJoin1 = new SingleJoin (new Column (_initialTable, joinColumns1.A), new Column (relatedTable1, joinColumns1.B));
      SingleJoin singleJoin2 = new SingleJoin (new Column (relatedTable1, joinColumns2.A), new Column (relatedTable2, joinColumns2.B));
      FieldSourcePath expected = new FieldSourcePath (_initialTable, new[] { singleJoin1, singleJoin2 });
      Assert.AreEqual (expected, result);
    }

    [Test]
    public void BuildFieldSourcePath_UsesContext ()
    {
      Assert.AreEqual (0, _context.Count);
      BuildFieldSourcePath_SimpleJoin ();
      Assert.AreEqual (1, _context.Count);
    }

  }
}
