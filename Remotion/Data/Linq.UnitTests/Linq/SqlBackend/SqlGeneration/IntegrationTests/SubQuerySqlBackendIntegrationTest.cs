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
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class SubQuerySqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void InWhereCondition_First ()
    {
      CheckQuery (
          from c in Cooks where c.Name == (from a in Cooks select a.FirstName).First () select c.Name,
          "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] WHERE ([t0].[Name] = (SELECT TOP (1) [t1].[FirstName] AS [value] FROM [CookTable] AS [t1]))");
    }

    [Test]
    public void InWhereCondition_Single ()
    {
      CheckQuery (
          from c in Cooks where c.Name == (from k in Kitchens select k.Name).Single () select c.Name,
          "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] WHERE ([t0].[Name] = (SELECT TOP (2) [t1].[Name] AS [value] FROM [KitchenTable] AS [t1]))");
    }

    [Test]
    public void SingleInSubQuery_Top1IsUsedInsteadOfTop2 ()
    {
      CheckQuery (from c in Cooks select c.Assistants.Single (), 
        "SELECT [q2].[ID],[q2].[FirstName],[q2].[Name],[q2].[IsStarredCook],[q2].[IsFullTimeCook],[q2].[SubstitutedID],[q2].[KitchenID] "+
        "FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[ID],[t1].[FirstName],[t1].[Name],[t1].[IsStarredCook],"
        +"[t1].[IsFullTimeCook],[t1].[SubstitutedID],[t1].[KitchenID] FROM [CookTable] AS [t1] WHERE ([t0].[ID] = [t1].[AssistedID])) AS [q2]");
    }

    [Test]
    public void InWhereCondition_Count ()
    {
      CheckQuery (
          from c in Cooks where c.ID == (from k in Kitchens select k).Count () select c.Name,
          "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] WHERE ([t0].[ID] = (SELECT COUNT(*) AS [value] FROM [KitchenTable] AS [t1]))");
    }

    [Test]
    public void SubQueryInMainFromClause ()
    {
      CheckQuery (
          from s in (from s2 in Cooks select s2).Take (1) select s,
          "SELECT [q0].[ID],[q0].[FirstName],[q0].[Name],[q0].[IsStarredCook],[q0].[IsFullTimeCook],[q0].[SubstitutedID],[q0].[KitchenID] "
          + "FROM "
          +
          "(SELECT TOP (@1) [t1].[ID],[t1].[FirstName],[t1].[Name],[t1].[IsStarredCook],[t1].[IsFullTimeCook],[t1].[SubstitutedID],[t1].[KitchenID] "
          + "FROM [CookTable] AS [t1]) AS [q0]",
          new CommandParameter ("@1", 1));
    }

    [Test]
    public void SubQueryInAdditionalFromClause ()
    {
      CheckQuery (
          from s in Cooks from s2 in (from s3 in Cooks select s3) select s.FirstName,
          "SELECT [t1].[FirstName] AS [value] FROM [CookTable] AS [t1] CROSS APPLY "
          + "(SELECT [t2].[ID],[t2].[FirstName],[t2].[Name],[t2].[IsStarredCook],[t2].[IsFullTimeCook],[t2].[SubstitutedID],[t2].[KitchenID] "
          + "FROM [CookTable] AS [t2]) AS [q0]");
    }

    [Test]
    public void SubQuery_SelectingEntityMember_InAdditionalFromClause ()
    {
      CheckQuery (
          from s in Cooks from s2 in (from s3 in Cooks select s3.Substitution) from s4 in s2.Assistants select s.FirstName,
          "SELECT [t1].[FirstName] AS [value] FROM [CookTable] AS [t1] "
          + "CROSS APPLY ("
          + "SELECT [t3].[ID],[t3].[FirstName],[t3].[Name],[t3].[IsStarredCook],[t3].[IsFullTimeCook],[t3].[SubstitutedID],[t3].[KitchenID] "
          + "FROM [CookTable] AS [t2] LEFT OUTER JOIN [CookTable] AS [t3] ON [t2].[ID] = [t3].[SubstitutedID]) "
          + "AS [q0] "
          + "CROSS JOIN [CookTable] AS [t4] WHERE ([q0].[ID] = [t4].[AssistedID])");
    }

    [Test]
    public void ComplexSubQueryInAdditionalFromClause ()
    {
      CheckQuery (
          from s in Cooks from s2 in (from s3 in Cooks where s3.ID == s.ID && s3.ID > 3 select s3) select s2.FirstName,
          "SELECT [q0].[FirstName] AS [value] FROM [CookTable] AS [t1] CROSS APPLY "
          + "(SELECT [t2].[ID],[t2].[FirstName],[t2].[Name],[t2].[IsStarredCook],[t2].[IsFullTimeCook],[t2].[SubstitutedID],[t2].[KitchenID] "
          + "FROM [CookTable] AS [t2] "
          + "WHERE (([t2].[ID] = [t1].[ID]) AND ([t2].[ID] > @1))) AS [q0]",
          new CommandParameter ("@1", 3));
    }

    [Test]
    public void ComplexSubQuery_WithEntityComparison_InAdditionalFromClause ()
    {
      CheckQuery (
          from s in Cooks from s2 in (from s3 in Cooks where s3 == s select s3) select s2.FirstName,
          "SELECT [q0].[FirstName] AS [value] FROM [CookTable] AS [t1] CROSS APPLY "
          + "(SELECT [t2].[ID],[t2].[FirstName],[t2].[Name],[t2].[IsStarredCook],[t2].[IsFullTimeCook],[t2].[SubstitutedID],[t2].[KitchenID] "
          + "FROM [CookTable] AS [t2] "
          + "WHERE ([t2].[ID] = [t1].[ID])) AS [q0]");
    }

    [Test]
    public void SubQueryInFromClause_SelectingColumn ()
    {
      CheckQuery (
          from s in (from s2 in Cooks select s2.FirstName).Take (1) select s,
          "SELECT [q0].[value] AS [value] FROM (SELECT TOP (@1) [t1].[FirstName] AS [value] FROM [CookTable] AS [t1]) AS [q0]",
          new CommandParameter ("@1", 1));
    }

    [Test]
    public void SubQueryInFromClause_SelectingSingleValue ()
    {
      CheckQuery (
          from s in (from s2 in Cooks select s2.ID + s2.ID).Take (1) select s,
          "SELECT [q0].[value] AS [value] FROM (SELECT TOP (@1) ([t1].[ID] + [t1].[ID]) AS [value] FROM [CookTable] AS [t1]) AS [q0]",
          new CommandParameter ("@1", 1));
    }

    [Test]
    public void ResolveNonEntity ()
    {
      CheckQuery (
          (from r in Restaurants
           from name in r.Cooks.Select (n => n.FirstName)
           select name),
          "SELECT [q0].[value] AS [value] FROM [RestaurantTable] AS [t1] CROSS APPLY "
          + "(SELECT [t2].[FirstName] AS [value] FROM [CookTable] AS [t2] WHERE ([t1].[ID] = [t2].[RestaurantID])) AS [q0]");
    }

    [Test]
    public void InOrderByClause ()
    {
      CheckQuery (
          from c in Cooks orderby (from k in Kitchens select k).Count () select c.Name,
          "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] ORDER BY (SELECT COUNT(*) AS [value] FROM [KitchenTable] AS [t1]) ASC");
    }

    [Test]
    public void SubQueryInSubQuery ()
    {
      CheckQuery (
          from c in Cooks where c.ID == (from k in Kitchens where k.ID == (from r in Restaurants select r).Count () select k).Count () select c.Name,
          "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] "
          + "WHERE ([t0].[ID] = (SELECT COUNT(*) AS [value] FROM [KitchenTable] AS [t1] "
          + "WHERE ([t1].[ID] = (SELECT COUNT(*) AS [value] FROM [RestaurantTable] AS [t2]))))");
    }

    [Test]
    public void DependentSubQueryInSubQuery ()
    {
      CheckQuery (
          from r in Restaurants where r.ID == (from c in r.Cooks where c.ID == (from a in c.Assistants select a).Count () select c).Count () select r.ID,
          "SELECT [t0].[ID] AS [value] "
          + "FROM [RestaurantTable] AS [t0] "
          + "WHERE ([t0].[ID] = "
          + "(SELECT COUNT(*) AS [value] FROM [CookTable] AS [t1] "
          + "WHERE "
          + "(([t0].[ID] = [t1].[RestaurantID]) AND "
          + "([t1].[ID] = (SELECT COUNT(*) AS [value] FROM [CookTable] AS [t2] "
          + "WHERE ([t1].[ID] = [t2].[AssistedID]))))))");
    }

    [Test]
    public void Contains ()
    {
      CheckQuery (
          from c in Cooks where (from k in Cooks select k.Substitution).Contains (c) select c.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0] "
          + "WHERE [t0].[ID] IN (SELECT [t2].[ID] FROM [CookTable] AS [t1] LEFT OUTER JOIN [CookTable] AS [t2] ON [t1].[ID] = [t2].[SubstitutedID])");
    }

    [Test]
    public void OrderingsInSubQuery_WithDistinct ()
    {
      CheckQuery (
        from s in (from s2 in Cooks orderby s2.Name select s2.ID).Distinct () select s,
          "SELECT [q0].[value] AS [value] FROM (SELECT DISTINCT [t1].[ID] AS [value] FROM [CookTable] AS [t1]) AS [q0]");
    }

    [Test]
    public void OrderingsInSubQuery_WithoutDistinct ()
    {
      CheckQuery (
        // => from s in Cooks from s2 in (from s2 in Cooks select new { Key = s2.ID, Value = new { Key = s2.Name, Value = null } }) orderby s.Value.Key select s.Key  
        from s in Cooks from s2 in (from s2 in Cooks orderby s2.Name select s2.ID) select s2,
          "SELECT [q0].[Key] AS [value] FROM [CookTable] AS [t1] CROSS APPLY (SELECT [t2].[ID] AS [Key],[t2].[Name] AS [Value] " +
          "FROM [CookTable] AS [t2]) AS [q0] ORDER BY [q0].[Value] ASC");

      CheckQuery (
        // => from s in Cooks from s2 in (from s2 in Cooks select new { Key = s2, Value = new { Key = s2.Name, Value = null } }) orderby s.Value.Key select s.Key  
        from s in Cooks from s2 in (from s2 in Cooks orderby s2.Name select s2) select s.FirstName,
          "SELECT [t1].[FirstName] AS [value] FROM [CookTable] AS [t1] CROSS APPLY (SELECT [t2].[ID] AS [Key_ID]," +
          "[t2].[FirstName] AS [Key_FirstName],[t2].[Name] AS [Key_Name],[t2].[IsStarredCook] AS [Key_IsStarredCook]," +
          "[t2].[IsFullTimeCook] AS [Key_IsFullTimeCook],[t2].[SubstitutedID] AS [Key_SubstitutedID]," +
          "[t2].[KitchenID] AS [Key_KitchenID],[t2].[Name] AS [Value] " +
          "FROM [CookTable] AS [t2]) AS [q0] ORDER BY [q0].[Value] ASC");
    }

    [Test]
    public void OrderingsInSubQuery_WithTopExpression ()
    {
      CheckQuery (
        from s in Cooks from s2 in (from s2 in Cooks orderby s2.Name select s2.ID).Take (10) select s2,
          "SELECT [q0].[Key] AS [value] FROM [CookTable] AS [t1] CROSS APPLY ("
          + "SELECT TOP (@1) [t2].[ID] AS [Key],[t2].[Name] AS [Value] FROM [CookTable] AS [t2] "
          + "ORDER BY [t2].[Name] ASC) AS [q0] "
          + "ORDER BY [q0].[Value] ASC",
          new CommandParameter ("@1", 10));
    }

    [Test]
    public void InSelectProjection_Count ()
    {
      CheckQuery (
          from c in Cooks select (from k in Kitchens select k.Name).Count (),
          "SELECT (SELECT COUNT(*) AS [value] FROM [KitchenTable] AS [t1]) AS [value] FROM [CookTable] AS [t0]",
          row => (object) row.GetValue<int> (new ColumnID ("value", 0)));
    }

    [Test]
    public void InSelectProjection_Entity ()
    {
      CheckQuery (from c in Cooks select (from c2 in Cooks select c2).First (),
        "SELECT [q2].[ID],[q2].[FirstName],[q2].[Name],[q2].[IsStarredCook],[q2].[IsFullTimeCook],[q2].[SubstitutedID],[q2].[KitchenID] " +
        "FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[ID],[t1].[FirstName],[t1].[Name],[t1].[IsStarredCook],[t1].[IsFullTimeCook]," +
        "[t1].[SubstitutedID],[t1].[KitchenID] FROM [CookTable] AS [t1]) AS [q2]",
        row => (object) row.GetEntity<Cook> (
            new ColumnID ("ID", 0),
            new ColumnID ("FirstName", 1),
            new ColumnID ("Name", 2),
            new ColumnID ("IsStarredCook", 3),
            new ColumnID ("IsFullTimeCook", 4),
            new ColumnID ("SubstitutedID", 5),
            new ColumnID ("KitchenID", 6)));

      CheckQuery (from c in Cooks select (from c2 in Cooks select c2).Single (),
        "SELECT [q2].[ID],[q2].[FirstName],[q2].[Name],[q2].[IsStarredCook],[q2].[IsFullTimeCook],[q2].[SubstitutedID],[q2].[KitchenID] " +
        "FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[ID],[t1].[FirstName],[t1].[Name],[t1].[IsStarredCook],[t1].[IsFullTimeCook]," +
        "[t1].[SubstitutedID],[t1].[KitchenID] FROM [CookTable] AS [t1]) AS [q2]",
        row => (object) row.GetEntity<Cook> (
            new ColumnID ("ID", 0),
            new ColumnID ("FirstName", 1),
            new ColumnID ("Name", 2),
            new ColumnID ("IsStarredCook", 3),
            new ColumnID ("IsFullTimeCook", 4),
            new ColumnID ("SubstitutedID", 5),
            new ColumnID ("KitchenID", 6)));
    }

    [Test]
    public void InSelectProjection_Column ()
    {
      CheckQuery (
          from c in Cooks select (from k in Kitchens select k.Name).First(),
          "SELECT [q2].[value] AS [value] FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[Name] AS [value] FROM [KitchenTable] AS [t1]) AS [q2]",
          row => (object) row.GetValue<string> (new ColumnID ("value", 0)));

      CheckQuery (
          from c in Cooks select (from k in Kitchens select k.Name).Single (),
          "SELECT [q2].[value] AS [value] FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[Name] AS [value] FROM [KitchenTable] AS [t1]) AS [q2]",
          row => (object) row.GetValue<string> (new ColumnID ("value", 0)));
    }

    [Test]
    public void InSelectProjection_NewExpression ()
    {
      CheckQuery (
          from c in Cooks select (from k in Kitchens select new { k.Name }).First (),
          "SELECT [q2].[Name] AS [Name] FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[Name] AS [Name] FROM [KitchenTable] AS [t1]) AS [q2]",
          row => (object) new { Name = row.GetValue<string> (new ColumnID ("Name", 0)) });

      CheckQuery (
        from c in Cooks select (from k in Kitchens select new { k.Name }).Single (),
        "SELECT [q2].[Name] AS [Name] FROM [CookTable] AS [t0] OUTER APPLY (SELECT TOP (1) [t1].[Name] AS [Name] FROM [KitchenTable] AS [t1]) AS [q2]",
          row => (object) new { Name = row.GetValue<string> (new ColumnID ("Name", 0)) });
    }
  }
}