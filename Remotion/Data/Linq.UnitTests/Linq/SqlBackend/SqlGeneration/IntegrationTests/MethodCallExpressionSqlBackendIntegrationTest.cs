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
using NUnit.Framework;
using Remotion.Data.Linq.SqlBackend;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class MethodCallExpressionSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void String_Length_Property ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName != null && c.FirstName.Length > 0 select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE (([t0].[FirstName] IS NOT NULL) AND (LEN([t0].[FirstName]) > @1))",
          new CommandParameter ("@1", 0)
          );
    }

    [Test]
    public void String_IsNullOrEmpty ()
    {
      CheckQuery (
          from c in Cooks where string.IsNullOrEmpty (c.FirstName) select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE (([t0].[FirstName] IS NULL) OR (LEN([t0].[FirstName]) = 0))");
    }

    [Test]
    public void Contains ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.Contains ("abc") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", "%abc%")
          );
      CheckQuery (
          from c in Cooks where c.FirstName.Contains ("a%b_c[a] [^]") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", @"%a\%b\_c\[a] \[^]%")
          );
      CheckQuery (
          from c in Cooks where c.FirstName.Contains (null) select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 0));
    }

    [Test]
    public void Contains_WithNonConstantValue ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.Contains (c.Name) select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE (('%' + REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([t0].[Name], '\', '\\'), '%', '\%'), '_', '\_'), '[', '\['), ']', '\]')) + '%') ESCAPE '\'"
        );
    }

    [Test]
    public void StartsWith ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.StartsWith ("abc") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", "abc%")
          );
      CheckQuery (
          from c in Cooks where c.FirstName.StartsWith ("a%b_c[a] [^]") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", @"a\%b\_c\[a] \[^]%")
          );
      CheckQuery (
          from c in Cooks where c.FirstName.StartsWith (null) select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 0)
          );
    }

    [Test]
    public void StartsWith_WithNonConstantValue()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.StartsWith (c.Name) select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE (REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([t0].[Name], '\', '\\'), '%', '\%'), '_', '\_'), '[', '\['), ']', '\]') + '%') ESCAPE '\'"
        );
    }


    [Test]
    public void EndsWith ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.EndsWith ("abc") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", "%abc")
          );
      CheckQuery (
          from c in Cooks where c.FirstName.EndsWith ("a%b_c[a] [^]") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", @"%a\%b\_c\[a] \[^]")
          );
      CheckQuery (
          from c in Cooks where c.FirstName.EndsWith (null) select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 0)
          );
    }

    [Test]
    public void EndsWith_WithNonConstantValue ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.EndsWith(c.Name) select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE ('%' + REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([t0].[Name], '\', '\\'), '%', '\%'), '_', '\_'), '[', '\['), ']', '\]')) ESCAPE '\'"
        );
    }

    [Test]
    public void Convert ()
    {
      CheckQuery (
          from c in Cooks select c.ID.ToString(),
          "SELECT CONVERT(NVARCHAR, [t0].[ID]) AS [value] FROM [CookTable] AS [t0]"
          );

      CheckQuery (
          from c in Cooks select System.Convert.ToInt32 (c.FirstName),
          "SELECT CONVERT(INT, [t0].[FirstName]) AS [value] FROM [CookTable] AS [t0]"
          );

      CheckQuery (
          from c in Cooks select System.Convert.ToString (c.ID),
          "SELECT CONVERT(NVARCHAR, [t0].[ID]) AS [value] FROM [CookTable] AS [t0]"
          );
    }

    [Test]
    public void ToLower_ToUpper ()
    {
      CheckQuery (
          from c in Cooks select c.FirstName.ToLower(),
          "SELECT LOWER([t0].[FirstName]) AS [value] FROM [CookTable] AS [t0]"
          );
      CheckQuery (
          from c in Cooks select c.FirstName.ToUpper(),
          "SELECT UPPER([t0].[FirstName]) AS [value] FROM [CookTable] AS [t0]"
          );
    }

    [Test]
    public void Remove ()
    {
      CheckQuery (
          from c in Cooks select c.FirstName.Remove (3),
          "SELECT STUFF([t0].[FirstName], (@1 + 1), LEN([t0].[FirstName]), '') AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 3)
          );
      CheckQuery (
          from c in Cooks select c.FirstName.Remove (3, 5),
          "SELECT STUFF([t0].[FirstName], (@1 + 1), @2, '') AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 3),
          new CommandParameter ("@2", 5)
          );
    }

    [Test]
    public void Substring ()
    {
      CheckQuery (
          from c in Cooks select c.FirstName.Substring (3),
          "SELECT SUBSTRING([t0].[FirstName], (@1 + 1), LEN([t0].[FirstName])) AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 3)
          );
      CheckQuery (
          from c in Cooks select c.FirstName.Substring (3, 5),
          "SELECT SUBSTRING([t0].[FirstName], (@1 + 1), @2) AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 3),
          new CommandParameter ("@2", 5)
          );
    }

    [Test]
    public void IndexOf ()
    {
      CheckQuery (
          from c in Cooks select c.FirstName.IndexOf ("test"),
          "SELECT CASE WHEN (LEN(@1) = 0) THEN 0 ELSE (CHARINDEX(@2, [t0].[FirstName]) - 1) END AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", "test"),
          new CommandParameter ("@2", "test")
          );

      CheckQuery (
          from c in Cooks select c.FirstName.IndexOf ('t'),
          "SELECT CASE WHEN (LEN(@1) = 0) THEN 0 ELSE (CHARINDEX(@2, [t0].[FirstName]) - 1) END AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 't'),
          new CommandParameter ("@2", 't')
          );

      CheckQuery (
          from c in Cooks select c.FirstName.IndexOf ("test", 2),
          "SELECT CASE WHEN ((LEN(@1) = 0) AND ((@2 + 1) <= LEN([t0].[FirstName]))) THEN @3 ELSE (CHARINDEX(@4, [t0].[FirstName], (@5 + 1)) - 1) END AS [value] "
          + "FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", "test"),
          new CommandParameter ("@2", 2),
          new CommandParameter ("@3", 2),
          new CommandParameter ("@4", "test"),
          new CommandParameter ("@5", 2)
          );

      CheckQuery (
          from c in Cooks select c.FirstName.IndexOf ('t', 2),
          "SELECT CASE WHEN ((LEN(@1) = 0) AND ((@2 + 1) <= LEN([t0].[FirstName]))) THEN @3 ELSE (CHARINDEX(@4, [t0].[FirstName], (@5 + 1)) - 1) END AS [value] "
          + "FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 't'),
          new CommandParameter ("@2", 2),
          new CommandParameter ("@3", 2),
          new CommandParameter ("@4", 't'),
          new CommandParameter ("@5", 2)
          );

      CheckQuery (
          from c in Cooks select c.FirstName.IndexOf ("test", 2, 5),
          "SELECT CASE WHEN ((LEN(@1) = 0) AND ((@2 + 1) <= LEN([t0].[FirstName]))) THEN @3 "
          + "ELSE (CHARINDEX(@4, SUBSTRING([t0].[FirstName], 1, (@5 + @6)), (@7 + 1)) - 1) END AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", "test"),
          new CommandParameter ("@2", 2),
          new CommandParameter ("@3", 2),
          new CommandParameter ("@4", "test"),
          new CommandParameter ("@5", 2),
          new CommandParameter ("@6", 5),
          new CommandParameter ("@7", 2)
          );

      CheckQuery (
          from c in Cooks select c.FirstName.IndexOf ('t', 2, 5),
          "SELECT CASE WHEN ((LEN(@1) = 0) AND ((@2 + 1) <= LEN([t0].[FirstName]))) THEN @3 "
          + "ELSE (CHARINDEX(@4, SUBSTRING([t0].[FirstName], 1, (@5 + @6)), (@7 + 1)) - 1) END AS [value] FROM [CookTable] AS [t0]",
          new CommandParameter ("@1", 't'),
          new CommandParameter ("@2", 2),
          new CommandParameter ("@3", 2),
          new CommandParameter ("@4", 't'),
          new CommandParameter ("@5", 2),
          new CommandParameter ("@6", 5),
          new CommandParameter ("@7", 2)
          );
    }

    [Test]
    public void Like ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.SqlLike ("%ab%c") select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE @1 ESCAPE '\'",
          new CommandParameter ("@1", "%ab%c")
          );

      CheckQuery (
          from c in Cooks where c.FirstName.SqlLike (c.Name) select c.ID,
          @"SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE [t0].[FirstName] LIKE [t0].[Name] ESCAPE '\'");
    }

    [Test]
    public void ContainsFulltext ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.SqlContainsFulltext ("%ab%c") select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE CONTAINS([t0].[FirstName], @1)",
          new CommandParameter ("@1", "%ab%c")
          );

      CheckQuery (
          from c in Cooks where c.FirstName.SqlContainsFulltext ("%ab%c", "de") select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE CONTAINS([t0].[FirstName], @1, LANGUAGE @2)",
          new CommandParameter ("@1", "%ab%c"),
          new CommandParameter ("@2", "de")
          );

      CheckQuery (
          from c in Cooks where c.FirstName.SqlContainsFulltext (c.Name) select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE CONTAINS([t0].[FirstName], [t0].[Name])");
    }

    [Test]
    public void ContainsFreetext ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName.SqlContainsFreetext ("%ab%c") select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE FREETEXT([t0].[FirstName], @1)",
          new CommandParameter ("@1", "%ab%c")
          );

      CheckQuery (
          from c in Cooks where c.FirstName.SqlContainsFreetext ("%ab%c", "de") select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE FREETEXT([t0].[FirstName], @1, LANGUAGE @2)",
          new CommandParameter ("@1", "%ab%c"),
          new CommandParameter ("@2", "de")
          );

      CheckQuery (
          from c in Cooks where c.FirstName.SqlContainsFreetext (c.Name) select c.ID,
          "SELECT [t0].[ID] AS [value] FROM [CookTable] AS [t0] WHERE FREETEXT([t0].[FirstName], [t0].[Name])");
    }

    [Test]
    public void Equal ()
    {
      CheckQuery (from c in Cooks where c.Name.Equals ("abc") select c.Name, 
        "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] WHERE ([t0].[Name] = @1)",
        new CommandParameter("@1", "abc"));
    }
  }
}