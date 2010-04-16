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
using Remotion.Data.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class WhereConditionSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void BooleanColumn ()
    {
      CheckQuery (
          from c in Cooks where c.IsFullTimeCook select c.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] WHERE ([t0].[IsFullTimeCook] = 1)"
          );
    }

    [Test]
    public void True ()
    {
      CheckQuery (
          from c in Cooks where true select c.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 1)
          );
    }

    [Test]
    public void False ()
    {
      CheckQuery (
          from c in Cooks where false select c.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 0)
          );
    }

    [Test]
    public void BinaryExpression ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName == "hugo" select c.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] WHERE ([t0].[FirstName] = @1)",
          new CommandParameter ("@1", "hugo")
          );
    }

    // TODO Review 2589: Conditinal expressions have nothing to do with binary expressions, so there should be a separate integration test fixture only for conditional expressions; test conditional in Where expressions and Select expressions (as indicated in the task description)
    [Test]
    public void BinaryExpression_WithConditionalExpression ()
    {
      CheckQuery (
          from c in Cooks where c.FirstName == (c.FirstName == "Hugo" ? "test1" : "test2") select c.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] WHERE "
          + "([t0].[FirstName] = CASE WHEN ([t0].[FirstName] = @1) THEN @2 ELSE @3 END)",
          new CommandParameter ("@1", "Hugo"),
          new CommandParameter ("@2", "test1"),
          new CommandParameter ("@3", "test2"));
    }
  }
}