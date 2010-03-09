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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.UnitTests.TestDomain;
using Remotion.Data.Linq.UnitTests.TestQueryGenerators;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlGeneration
{
  [TestFixture]
  public class SqlBackendIntegrationTest
  {
    private IQueryable<Cook> _cooks;

    [SetUp]
    public void SetUp ()
    {
      _cooks = ExpressionHelper.CreateCookQueryable();
    }

    [Test]
    public void SimpleSqlQuery_SelectAllFromQueryable ()
    {
      var queryable = SelectTestQueryGenerator.CreateSimpleQuery (_cooks);
      var result = GenerateSql (queryable);

      var expected = 
          "SELECT [c].[ID],[c].[FirstName],[c].[Name],[c].[IsStarredCook],[c].[IsFullTimeCook],[c].[SubstitutionID] "
          + "FROM [CookTable] AS [c]";
      Assert.That (result.CommandText, Is.EqualTo (expected));
    }

    [Test]
    public void SimpleSqlQuery_SelctPropertyFromQueryable ()
    {
      IQueryable<string> queryable = SelectTestQueryGenerator.CreateSimpleQueryWithProjection (_cooks);
      var result = GenerateSql (queryable);

      Assert.That (result.CommandText, Is.EqualTo ("SELECT [c].[FirstName] FROM [CookTable] AS [c]"));
    }

    [Test]
    [Ignore ("TODO: Fix stub implementation to correctly create the joins")]
    public void SelectQuery_WithJoin ()
    {
      var queryable = from c in _cooks select c.Substitution.FirstName;
      var result = GenerateSql (queryable);

      Assert.That (
          result.CommandText,
          Is.EqualTo (
              "SELECT [c].[FirstName] FROM [CookTable] AS [c] JOIN [SubstitutionTable] AS [s] ON [c].[ID] = [s].[SubstitutionID]"));
    }

    //[Test]
    //public void SelectQuuery_WithWhereCondition ()
    //{
    //  var queryable = from c in _cooks where c.Name == "Huber" select c.FirstName;
    //  //var queryable = from c in _cooks select c.FirstName + c.Name;
    //  var result = GenerateSql (queryable);

    //  Assert.That (result.CommandText, Is.EqualTo (""));
    //}

    //TODO: add several integration tests

    // Integration test: select cook.Substitution; select cook.Substitution.FirstName; select cook.Substitution.Substitution.FirstName

    //where conditions
    //from c in _cooks where c.Name = "Huber" select c.FirstName
    //from c in _cooks where c.Name = "Huber" && c.FirstName = "Sepp" select c;

    //result operators
    //(from c in _cooks select c).Count()
    //(from c in _cooks select c).Distinct()
    //(from c in _cooks select c).Take(5)
    //(from c in _cooks select c).Take(c.ID)
    //from k in _kitchen from c in k.Restaurant.Cooks.Take(k.RoomNumber) select c
    //(from c in _cooks select c).Single()
    //(from c in _cooks select c).First()

    //constant expressions
    //(from c in _cooks where c.IsFullTimeCook select c)
    //(from c in _cooks where c.IsFullTimeCook == true select c)
    //add tests for replacing true/false with 1/0

    //binary expression
    //(from c in _cooks where c.Name == null select c)
    //(from c in _cooks where c.ID + c.ID select c)
    // see SqlGeneratingExpressionVisitor.VisitBinaryExpressions for further tests

    //unary expressions (unary plus, unary negate, unary not)
    //(from c in _cooks where (-c.ID) == -1 select c)
    //(from c in _cooks where !c.IsStarredCook == true select c)
    //(from c in _cooks where (+c.ID) == -1 select c)

    //method calls (review method)
    //SqlStatementTextGenerator.GenerateSqlGeneratorRegistry

    private SqlCommand GenerateSql<T> (IQueryable<T> queryable)
    {
      var queryModel = ExpressionHelper.ParseQuery (queryable.Expression);

      var sqlStatement = SqlPreparationQueryModelVisitor.TransformQueryModel (queryModel, new SqlPreparationContext());

      ResolvingSqlStatementVisitor.ResolveExpressions (sqlStatement, new SqlStatementResolverStub(), new UniqueIdentifierGenerator());

      var sqlTextGenerator = new SqlStatementTextGenerator ();
      return sqlTextGenerator.Build (sqlStatement);
    }
  }


}