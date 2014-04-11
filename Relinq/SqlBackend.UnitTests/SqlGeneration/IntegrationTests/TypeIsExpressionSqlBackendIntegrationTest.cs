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
using Remotion.Linq.SqlBackend.UnitTests.TestDomain;

namespace Remotion.Linq.SqlBackend.UnitTests.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class TypeIsExpressionSqlBackendIntegrationTest :SqlBackendIntegrationTestBase
  {
    [Test]
    public void Is ()
    {
      CheckQuery (
          Cooks.Where (c => c is Chef),
          "SELECT [t0].[ID],[t0].[FirstName],[t0].[Name],[t0].[IsStarredCook],[t0].[IsFullTimeCook],[t0].[SubstitutedID],[t0].[KitchenID],"
          + "[t0].[KnifeID],[t0].[KnifeClassID] "
          + "FROM [CookTable] AS [t0] WHERE ([t0].[IsStarredCook] = 1)"
          );

      CheckQuery (
#pragma warning disable 183
          Cooks.Where (c => c is Cook),
#pragma warning restore 183
          "SELECT [t0].[ID],[t0].[FirstName],[t0].[Name],[t0].[IsStarredCook],[t0].[IsFullTimeCook],[t0].[SubstitutedID],[t0].[KitchenID],"
          + "[t0].[KnifeID],[t0].[KnifeClassID] "
          + "FROM [CookTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 1)
          );

      CheckQuery (
#pragma warning disable 183
          Chefs.Where (c => c is Cook),
#pragma warning restore 183
          "SELECT [t0].[ID],[t0].[FirstName],[t0].[Name],[t0].[IsStarredCook],[t0].[IsFullTimeCook],[t0].[SubstitutedID],[t0].[KitchenID],"
          + "[t0].[KnifeID],[t0].[KnifeClassID],"
          + "[t0].[LetterOfRecommendation] FROM [dbo].[ChefTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 1));

      CheckQuery (
#pragma warning disable 183
          Chefs.Where (c => c is Chef),
#pragma warning restore 183
          "SELECT [t0].[ID],[t0].[FirstName],[t0].[Name],[t0].[IsStarredCook],[t0].[IsFullTimeCook],[t0].[SubstitutedID],[t0].[KitchenID],"
          + "[t0].[KnifeID],[t0].[KnifeClassID],"
          + "[t0].[LetterOfRecommendation] FROM [dbo].[ChefTable] AS [t0] WHERE (@1 = 1)",
          new CommandParameter ("@1", 1));
    }
  }
}