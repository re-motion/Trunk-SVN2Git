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
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class MemberChainingSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void SimpleSqlQuery_SimplePropertySelect ()
    {
      CheckQuery (
          from s in Cooks select s.FirstName,
          "SELECT [t0].[FirstName] AS [value] FROM [CookTable] AS [t0]",
          row => (object) row.GetValue<string> (new ColumnID ("value", 0)));
    }

    [Test]
    public void SimpleSqlQuery_EntityPropertySelect ()
    {
      CheckQuery (
          from k in Kitchens select k.Cook,
          "SELECT [t1].[ID],[t1].[FirstName],[t1].[Name],[t1].[IsStarredCook],[t1].[IsFullTimeCook],[t1].[SubstitutedID],[t1].[KitchenID] "
          + "FROM [KitchenTable] AS [t0] LEFT OUTER JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[KitchenID]",
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
    public void SimpleSqlQuery_ChainedPropertySelect_EndingWithSimpleProperty ()
    {
      CheckQuery (
          from k in Kitchens select k.Cook.FirstName,
          "SELECT [t1].[FirstName] AS [value] FROM [KitchenTable] AS [t0] LEFT OUTER JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[KitchenID]");
    }

    [Test]
    public void SelectQuery_ChainedPropertySelect_WithSameType ()
    {
      CheckQuery (
          from c in Cooks select c.Substitution.FirstName,
          "SELECT [t1].[FirstName] AS [value] FROM [CookTable] AS [t0] LEFT OUTER JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[SubstitutedID]");
    }

    [Test]
    public void SimpleSqlQuery_ChainedPropertySelect_EndingWithEntityProperty ()
    {
      CheckQuery (
          from k in Kitchens select k.Restaurant.SubKitchen.Cook,
          "SELECT [t3].[ID],[t3].[FirstName],[t3].[Name],[t3].[IsStarredCook],[t3].[IsFullTimeCook],[t3].[SubstitutedID],[t3].[KitchenID] "
          + "FROM [KitchenTable] AS [t0] "
          + "LEFT OUTER JOIN [RestaurantTable] AS [t1] ON [t0].[RestaurantID] = [t1].[ID] "
          + "LEFT OUTER JOIN [KitchenTable] AS [t2] ON [t1].[ID] = [t2].[RestaurantID] "
          + "LEFT OUTER JOIN [CookTable] AS [t3] ON [t2].[ID] = [t3].[KitchenID]");
    }

    [Test]
    public void ChainedPropertySelectAndWhere_SamePathTwice ()
    {
      CheckQuery (
          from k in Kitchens where k.Restaurant.SubKitchen.Cook != null select k.Restaurant.SubKitchen.Cook,
          "SELECT [t3].[ID],[t3].[FirstName],[t3].[Name],[t3].[IsStarredCook],[t3].[IsFullTimeCook],[t3].[SubstitutedID],[t3].[KitchenID] "
          + "FROM [KitchenTable] AS [t0] "
          + "LEFT OUTER JOIN [RestaurantTable] AS [t1] ON [t0].[RestaurantID] = [t1].[ID] "
          + "LEFT OUTER JOIN [KitchenTable] AS [t2] ON [t1].[ID] = [t2].[RestaurantID] "
          + "LEFT OUTER JOIN [CookTable] AS [t3] ON [t2].[ID] = [t3].[KitchenID] "
          + "WHERE ([t3].[ID] IS NOT NULL)");
    }

    [Test]
    public void ChainedPropertySelectAndWhere_PartialPathTwice ()
    {
      CheckQuery (
          from k in Kitchens where k.Restaurant.SubKitchen.Restaurant.ID == 0 select k.Restaurant.SubKitchen.Cook,
          "SELECT [t3].[ID],[t3].[FirstName],[t3].[Name],[t3].[IsStarredCook],[t3].[IsFullTimeCook],[t3].[SubstitutedID],[t3].[KitchenID] "+
          "FROM [KitchenTable] AS [t0] LEFT OUTER JOIN [RestaurantTable] AS [t1] ON [t0].[RestaurantID] = [t1].[ID] LEFT OUTER JOIN "+
          "[KitchenTable] AS [t2] ON [t1].[ID] = [t2].[RestaurantID] LEFT OUTER JOIN [CookTable] AS [t3] ON [t2].[ID] = [t3].[KitchenID] "+
          "LEFT OUTER JOIN [RestaurantTable] AS [t4] ON [t2].[RestaurantID] = [t4].[ID] WHERE ([t4].[ID] = @1)",
          new CommandParameter ("@1", 0));
    }

    [Test]
    public void PropertySelectsClassID ()
    {
      CheckQuery (
          from c in Cooks select c.MetaID.ClassID,
          "SELECT [t0].[ClassID] AS [value] FROM [CookTable] AS [t0]",
          row => (object) row.GetValue<string> (new ColumnID ("value", 0))
          );
    }

    [Test]
    public void MemberAccess_OnSubQuery_WithEntities ()
    {
      var query = Cooks.Select (c => (from a in c.Assistants select a.Substitution).First ().Name);
      CheckQuery (query,
        "SELECT [q3].[value] AS [value] FROM [CookTable] AS [t0] CROSS APPLY (SELECT TOP (1) " +
        "[t2].[Name] AS [value] FROM [CookTable] AS [t1] " +
        "LEFT OUTER JOIN [CookTable] AS [t2] ON [t1].[ID] = [t2].[SubstitutedID] WHERE ([t0].[ID] = [t1].[AssistedID])) AS [q3]");
    }

    [Test]
    public void MemberAccess_OnSubQuery_WithColumns ()
    {
      var query = Cooks.Select (c => (from a in c.Assistants select a.Name).First ().Length);
      CheckQuery (query,
        "SELECT [q2].[value] AS [value] FROM [CookTable] AS [t0] CROSS APPLY (SELECT TOP (1) " +
        "(LEN(([t1].[Name] + '#')) - 1) AS [value] FROM [CookTable] AS [t1] " +
        "WHERE ([t0].[ID] = [t1].[AssistedID])) AS [q2]");
    }

    [Test]
    public void MemberAccess_OnCoalesce_WithEntities_InSubQuery ()
    {
      var query = Cooks.Select (c => (from a in c.Assistants select a.Substitution ?? a).First ().Name);
      CheckQuery (query,
        "SELECT [q4].[value] AS [value] FROM [CookTable] AS [t0] CROSS APPLY (SELECT TOP (1) " +
        "CASE WHEN ([t2].[ID] IS NOT NULL) THEN [t2].[Name] ELSE [t1].[Name] END AS [value] FROM [CookTable] AS [t1] " +
        "LEFT OUTER JOIN [CookTable] AS [t2] ON [t1].[ID] = [t2].[SubstitutedID] WHERE ([t0].[ID] = [t1].[AssistedID])) AS [q4]");
    }

    [Test]
    public void MemberAccess_OnCoalesce_WithColumns_InSubQuery ()
    {
      var query = Cooks.Select (c => (from a in c.Assistants select a.Substitution.Name ?? a.Name).First ().Length);
      CheckQuery (query,
        "SELECT [q3].[value] AS [value] FROM [CookTable] AS [t0] "
        + "CROSS APPLY (SELECT TOP (1) CASE WHEN ([t2].[Name] IS NOT NULL) THEN (LEN(([t2].[Name] + '#')) - 1) "
        + "ELSE (LEN(([t1].[Name] + '#')) - 1) END AS [value] "
        + "FROM [CookTable] AS [t1] LEFT OUTER JOIN [CookTable] AS [t2] ON [t1].[ID] = [t2].[SubstitutedID] "
        + "WHERE ([t0].[ID] = [t1].[AssistedID])) AS [q3]");
    }

    [Test]
    public void MemberAccess_OnCoealesce_WithEntities_InSubQuery_InWhereClause ()
    {
      var query = Cooks.Where (c => (from a in c.Assistants select a.Substitution ?? a).First ().Name == "Hugo").Select (c => c.Name);
      CheckQuery (query, "SELECT [t0].[Name] AS [value] FROM [CookTable] AS [t0] WHERE ((SELECT TOP (1) "
        + "CASE WHEN ([t2].[ID] IS NOT NULL) THEN [t2].[Name] ELSE [t1].[Name] END AS [value] FROM [CookTable] AS [t1] "
        + "LEFT OUTER JOIN [CookTable] AS [t2] ON [t1].[ID] = [t2].[SubstitutedID] WHERE ([t0].[ID] = [t1].[AssistedID])) = @1)",
        new CommandParameter ("@1", "Hugo"));
    }

    [Test]
    public void MemberAccess_OnCoalesce_WithColumns ()
    {
      CheckQuery (Cooks.Select (c => (c.FirstName ?? c.Name).Length),
        "SELECT CASE WHEN ([t0].[FirstName] IS NOT NULL) THEN (LEN(([t0].[FirstName] + '#')) - 1) ELSE (LEN(([t0].[Name] + '#')) - 1) END AS [value] "
          +"FROM [CookTable] AS [t0]");
    }

    [Test]
    public void MemberAccess_OnCoalesce_WithEntities ()
    {
      CheckQuery (
          from c in Cooks
          select (c.Substitution ?? c).Name,
          "SELECT CASE WHEN ([t1].[ID] IS NOT NULL) THEN [t1].[Name] ELSE [t0].[Name] END AS [value] FROM [CookTable] AS [t0] "
            + "LEFT OUTER JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[SubstitutedID]");
    }

    [Test]
    public void MemberAccess_OnConditional_WithColumns ()
    {
      CheckQuery (Cooks.Select (c => (c.IsStarredCook ? c.Name : c.SpecificInformation).Length),
        "SELECT CASE WHEN ([t0].[IsStarredCook] = 1) THEN (LEN(([t0].[Name] + '#')) - 1) ELSE (LEN(([t0].[SpecificInformation] + '#')) - 1) END AS [value] "+
        "FROM [CookTable] AS [t0]");
    }

    [Test]
    public void MemberAccess_OnConditional_WithEntities ()
    {
      CheckQuery (
          from c in Cooks
          // ReSharper disable ConvertConditionalTernaryToNullCoalescing
          select (c.Substitution != null ? c.Substitution : c).Name,
        // ReSharper restore ConvertConditionalTernaryToNullCoalescing
          "SELECT CASE WHEN ([t1].[ID] IS NOT NULL) THEN [t1].[Name] ELSE [t0].[Name] END AS [value] FROM [CookTable] AS [t0] "
            + "LEFT OUTER JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[SubstitutedID]");
    }

    [Test]
    public void EntityAccess_WithNullableForeignKey ()
    {
      var myCompany = new Company { ID = 10 };
      CheckQuery (
          from r in Restaurants where r.CompanyIfAny == myCompany select r.ID,
          "SELECT [t0].[ID] AS [value] "
          + "FROM [RestaurantTable] AS [t0] "
          + "WHERE ([t0].[CompanyID] = @1)",
          new CommandParameter ("@1", 10));
    }

    [Test]
    public void EntityAccess_WithNullableForeignKey_Collection ()
    {
      CheckQuery (
          from c in Companies
          from r in c.AllRestaurants
          select r.ID,
          "SELECT [t1].[ID] AS [value] "
          + "FROM [CompanyTable] AS [t0] "
          + "CROSS JOIN [RestaurantTable] AS [t1] "
          + "WHERE ([t0].[ID] = [t1].[CompanyID])");
    }

    [Test]
    [Ignore ("TODO 3315")]
    public void EntityAccess_NoLeftJoin_OnIDAccess ()
    {
      CheckQuery (
          from k in Kitchens where k.Restaurant.ID == 10 select k.Restaurant.ID,
          "SELECT [t0].[RestaurantID] AS [value] "
          + "FROM [KitchenTable] AS [t0]  "
          + "WHERE ([t0].[RestaurantID] = @1)",
          new CommandParameter ("@1", 10));
    }

  }
}