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
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_EmptyResult ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.Find (new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidAbstractRole ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.Find (new EnumWrapper[] { new EnumWrapper (ProjectRoles.QualityManager) });

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual ("QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests", ((AbstractRoleDefinition) result[0]).Name);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.FindAll ();

      Assert.AreEqual (2, result.Count);
      for (int i = 0; i < result.Count; i++)
      {
        AbstractRoleDefinition abstractRole = (AbstractRoleDefinition) result[i];
        Assert.AreEqual (i, abstractRole.Index, "Wrong Index.");
      }
    }
  }
}
