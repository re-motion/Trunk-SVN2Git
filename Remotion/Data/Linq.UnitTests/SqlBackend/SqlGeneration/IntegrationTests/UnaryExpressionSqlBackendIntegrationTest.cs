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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class UnaryExpressionSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void UnaryPlus ()
    {
      var parameter = Expression.Parameter (typeof (Cook), "c");
      var weight = Expression.MakeMemberAccess (parameter, typeof (Cook).GetProperty ("Weight"));
      var selector = Expression.Lambda<Func<Cook, double>> (Expression.UnaryPlus (weight), parameter);
      var query = Cooks.Select (selector); // from c in Cooks select +c.Weight - C# compiler optimizes '+' away

      CheckQuery (
         query,
         "SELECT +[t0].[Weight] FROM [CookTable] AS [t0]"
         );
    }

    [Test]
    public void UnaryNegate ()
    {
      CheckQuery (
         from c in Cooks select -c.ID,
         "SELECT -[t0].[ID] FROM [CookTable] AS [t0]"
         );
    }

    [Test]
    public void UnaryNot ()
    {
      CheckQuery (
         from c in Cooks where !(c.FirstName == null) select c.ID,
         "SELECT [t0].[ID] FROM [CookTable] AS [t0] WHERE NOT ([t0].[FirstName] IS NULL)"
         );
    }

    [Test]
    [Ignore ("TODO 2362")]
    public void UnaryNot_OnColumn ()
    {
      CheckQuery (
         from c in Cooks where !c.IsStarredCook select c.ID,
         "SELECT [t0].[ID] FROM [CookTable] AS [t0] WHERE NOT ([t0].[IsStarredCool] = 1)"
         );
    }

    [Test]
    [Ignore ("TODO 2362")]
    public void UnaryNot_OnSelectedColumn ()
    {
      CheckQuery (
         from c in Cooks select !c.IsStarredCook,
         "SELECT CASE WHEN NOT ([t0].[IsStarredCook] = 1) THEN 1 ELSE 0 END FROM [CookTable] AS [t0]"
         );
    }

    [Test]
    [Ignore ("TODO 2362")]
    public void BitwiseNot ()
    {
      CheckQuery (
         from c in Cooks select ~c.ID,
         "SELECT ~[t0].[ID] FROM [CookTable] AS [t0]"
         );
    }
  }
}