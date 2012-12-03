// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlGeneration.IntegrationTests.ResultOperators
{
  [TestFixture]
  public class AnyResultOperatorSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void Any_OnTopLevel_WithoutPredicate ()
    {
      CheckQuery (
          () => Cooks.Any(),
          "SELECT CASE WHEN EXISTS((SELECT [t0].[ID] FROM [CookTable] AS [t0])) THEN 1 ELSE 0 END AS [value]",
          row => (object) ConvertExpressionMarker (Convert.ToBoolean (row.GetValue<int> (new ColumnID ("value", 0)))));
    }

    [Test]
    public void Any_OnTopLevel_WithPredicate ()
    {
      CheckQuery (
          () => Cooks.Any (c => c.FirstName == "Hugo"),
          "SELECT CASE WHEN EXISTS((SELECT [t0].[ID] FROM [CookTable] AS [t0] WHERE ([t0].[FirstName] = @1))) THEN 1 ELSE 0 END AS [value]",
          row => (object) ConvertExpressionMarker (Convert.ToBoolean (row.GetValue<int> (new ColumnID ("value", 0)))),
          new CommandParameter("@1", "Hugo"));
    }

    [Test]
    public void Any_InSubquery ()
    {
      CheckQuery (
          from s in Cooks where (from s2 in Cooks select s2).Any () select s.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] WHERE EXISTS((SELECT [t1].[ID] FROM [CookTable] AS [t1]))");
    }

    [Test]
    public void Any_OrderingsRemoved ()
    {
      CheckQuery (
          () => Cooks.OrderBy (c => c.FirstName).Any(),
          "SELECT CASE WHEN EXISTS((SELECT [t0].[ID] FROM [CookTable] AS [t0])) THEN 1 ELSE 0 END AS [value]",
          row => (object) ConvertExpressionMarker (Convert.ToBoolean (row.GetValue<int> (new ColumnID ("value", 0))))
          );
    }
  }
}