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
using System.Text;
using NUnit.Framework;
using Remotion.Data.Linq.Backend;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Data.Linq.Backend.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.Linq.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.Backend.SqlGeneration.SqlServer
{
  [TestFixture]
  public class FromBuilderTest
  {
    [Test]
    public void CombineTables_SelectsJoinsPerTable()
    {
      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry());
      var fromBuilder = new FromBuilder (commandBuilder);

      var table1 = new Table ("s1", "s1_alias");
      var table2 = new Table ("s2", "s2_alias");
      var column1 = new Column (table1, "c1");
      var column2 = new Column (table2, "c2");

      var tables = new List<IColumnSource> { table1 }; // this table does not have a join associated with it
      var joins = new JoinCollection ();
      var join = new SingleJoin (column2, column1);
      joins.AddPath (new FieldSourcePath(table2, new [] { join }));
      fromBuilder.BuildFromPart (tables, joins);

      Assert.AreEqual ("FROM [s1] [s1_alias]", commandBuilder.GetCommandText());
    }

    [Test]
    public void CombineTables_WithJoin ()
    {
      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry());
      var fromBuilder = new FromBuilder (commandBuilder);

      var table1 = new Table ("s1", "s1_alias");
      var table2 = new Table ("s2", "s2_alias");
      var column1 = new Column (table1, "c1");
      var column2 = new Column (table2, "c2");

      var tables = new List<IColumnSource> { table2 };
      var joins = new JoinCollection();
      var join = new SingleJoin (column2, column1);
      joins.AddPath (new FieldSourcePath (table2, new[] { join }));
      fromBuilder.BuildFromPart (tables, joins);

      Assert.AreEqual ("FROM [s2] [s2_alias] LEFT OUTER JOIN [s1] [s1_alias] ON [s2_alias].[c2] = [s1_alias].[c1]", commandBuilder.GetCommandText());
    }

    [Test]
    public void CombineTables_WithNestedJoin ()
    {
      var commandBuilder = new CommandBuilder (
          new StringBuilder(), new List<CommandParameter>(), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry());
      var fromBuilder = new FromBuilder (commandBuilder);

      // table2.table1.table3

      var table1 = new Table ("s1", "s1_alias");
      var table2 = new Table ("s2", "s2_alias");
      var table3 = new Table ("s3", "s3_alias");
      var column1 = new Column (table1, "c1");
      var column2 = new Column (table2, "c2");
      var column3 = new Column (table1, "c1'");
      var column4 = new Column (table3, "c3");

      var tables = new List<IColumnSource> { table2 };

      var joins = new JoinCollection();

      var join1 = new SingleJoin (column2, column1);
      var join2 = new SingleJoin (column3, column4);
      joins.AddPath (new FieldSourcePath (table2, new[] { join1, join2 }));

      fromBuilder.BuildFromPart (tables, joins);

      Assert.AreEqual (
          "FROM [s2] [s2_alias] "
          + "LEFT OUTER JOIN [s1] [s1_alias] ON [s2_alias].[c2] = [s1_alias].[c1] "
          + "LEFT OUTER JOIN [s3] [s3_alias] ON [s1_alias].[c1'] = [s3_alias].[c3]",
          commandBuilder.GetCommandText());
    }

    [Test]
    public void CombineTables_WithSubqueries ()
    {
      var mockRepository = new MockRepository();

      var subQuery = new SubQuery (ExpressionHelper.CreateQueryModel_Student(), ParseMode.SubQueryInFrom, "sub_alias");
      var tables = new List<IColumnSource> { new Table ("s1", "s1_alias"), subQuery };

      var commandBuilderMock = mockRepository.StrictMock<ICommandBuilder> ();

      commandBuilderMock.Expect (mock => mock.Append ("FROM "));
      commandBuilderMock.Expect (mock => mock.Append ("[s1] [s1_alias]"));
      commandBuilderMock.Expect (mock => mock.Append (" CROSS APPLY "));
      commandBuilderMock.Expect (mock => mock.AppendEvaluation (subQuery));

      commandBuilderMock.Replay ();

      var fromBuilder = new FromBuilder (commandBuilderMock);
      fromBuilder.BuildFromPart (tables, new JoinCollection ());
      commandBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void CombineTables_WithSubqueryInFirstFrom ()
    {
      var mockRepository = new MockRepository ();

      var subQuery = new SubQuery (ExpressionHelper.CreateQueryModel_Student (), ParseMode.SubQueryInFrom, "sub_alias");
      var tables = new List<IColumnSource> { subQuery };

      var commandBuilderMock = mockRepository.StrictMock<ICommandBuilder>();

      commandBuilderMock.Expect (mock => mock.Append ("FROM "));
      commandBuilderMock.Expect (mock => mock.AppendEvaluation (subQuery));

      commandBuilderMock.Replay ();

      var fromBuilder = new FromBuilder (commandBuilderMock);
      fromBuilder.BuildFromPart (tables, new JoinCollection ());
      commandBuilderMock.VerifyAllExpectations ();
    }
  }
}
