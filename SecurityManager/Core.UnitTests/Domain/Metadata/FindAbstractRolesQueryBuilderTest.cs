/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Development.Data.UnitTesting.DomainObjects.Linq;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class FindAbstractRolesQueryBuilderTest : DomainTest
  {
    private FindAbstractRolesQueryBuilder _queryBuilder;
    private ExpressionTreeComparer _expressionTreeComparer;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _expressionTreeComparer = new ExpressionTreeComparer ((actual, exptected) => Assert.That (actual, Is.EqualTo (exptected)));
    }

    public override void SetUp ()
    {
      base.SetUp();
      _queryBuilder = new FindAbstractRolesQueryBuilder();
    }

    [Test]
    public void CreateQuery_ZeroRoles ()
    {
      var abstractRoles = new EnumWrapper[0];
      var expected = new AbstractRoleDefinition[0].AsQueryable();

      var actual = _queryBuilder.CreateQuery (abstractRoles);

      Assert.That (actual, Is.TypeOf (expected.GetType()));
    }

    [Test]
    public void CreateQuery_OneRole ()
    {
      var abstractRoles = new[] { new EnumWrapper (ProjectRoles.QualityManager) };
      var expected = from ar in DataContext.Entity<AbstractRoleDefinition>()
                     where ar.Name == abstractRoles[0].Name
                     select ar;

      var actual = _queryBuilder.CreateQuery (abstractRoles);

      _expressionTreeComparer.Compare (expected, actual);
    }

    [Test]
    public void CreateQuery_TwoRoles ()
    {
      var abstractRoles = new[] { new EnumWrapper (ProjectRoles.QualityManager), new EnumWrapper (ProjectRoles.Developer) };
      var expected = from ar in DataContext.Entity<AbstractRoleDefinition>()
                     where ar.Name == abstractRoles[0].Name || ar.Name == abstractRoles[1].Name
                     select ar;

      var actual = _queryBuilder.CreateQuery (abstractRoles);

      _expressionTreeComparer.Compare (expected, actual);
    }

    [Test]
    public void CreateQuery_ThreeRoles ()
    {
      var abstractRoles = new[]
                          {
                              new EnumWrapper (ProjectRoles.QualityManager), new EnumWrapper (ProjectRoles.Developer),
                              new EnumWrapper (UndefinedAbstractRoles.Undefined)
                          };

      var expected = from ar in DataContext.Entity<AbstractRoleDefinition>()
                     where ar.Name == abstractRoles[0].Name || ar.Name == abstractRoles[1].Name || ar.Name == abstractRoles[2].Name
                     select ar;

      var actual = _queryBuilder.CreateQuery (abstractRoles);

      _expressionTreeComparer.Compare (expected, actual);
    }
  }
}