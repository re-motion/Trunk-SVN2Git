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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class TestWithTwoTenants : DomainTest
  {
    private DatabaseFixtures _dbFixtures;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      _dbFixtures = new DatabaseFixtures ();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_EmptyResult ()
    {
      var result = AbstractRoleDefinition.Find (new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidOneAbstractRole ()
    {
      var abstractRoles = new[] { new EnumWrapper (ProjectRoles.QualityManager) };
      var result = AbstractRoleDefinition.Find (abstractRoles);

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual (abstractRoles[0].Name, result[0].Name);
    }

    [Test]
    public void Find_ValidTwoAbstractRoles ()
    {
      var abstractRoles = new[] { new EnumWrapper (ProjectRoles.QualityManager), new EnumWrapper (ProjectRoles.Developer) };
      var result = AbstractRoleDefinition.Find (abstractRoles);

      Assert.AreEqual (2, result.Count);
      Assert.AreEqual (abstractRoles[1].Name, result[0].Name);
      Assert.AreEqual (abstractRoles[0].Name, result[1].Name);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      var result = AbstractRoleDefinition.FindAll ();

      Assert.AreEqual (2, result.Count);
      for (int i = 0; i < result.Count; i++)
      {
        AbstractRoleDefinition abstractRole = result[i];
        Assert.AreEqual (i, abstractRole.Index, "Wrong Index.");
      }
    }
  }
}
