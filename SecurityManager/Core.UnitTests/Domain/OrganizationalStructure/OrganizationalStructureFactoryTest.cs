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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class OrganizationalStructureFactoryTest : DomainTest
  {
    private IOrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new OrganizationalStructureFactory();
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void CreateTenant ()
    {
      Assert.That (_factory.CreateTenant (), Is.InstanceOfType (typeof (Tenant)));
    }

    [Test]
    public void CreateGroup ()
    {
      Assert.That (_factory.CreateGroup (), Is.InstanceOfType (typeof (Group)));
    }

    [Test]
    public void CreateUser ()
    {
      Assert.That (_factory.CreateUser (), Is.InstanceOfType (typeof (User)));
    }

    [Test]
    public void CreatePosition ()
    {
      Assert.That (_factory.CreatePosition (), Is.InstanceOfType (typeof (Position)));
    }

    [Test]
    public void GetTenantType ()
    {
      Assert.That (_factory.GetTenantType(), Is.SameAs (typeof (Tenant)));
    }

    [Test]
    public void GetGroupType ()
    {
      Assert.That (_factory.GetGroupType (), Is.SameAs (typeof (Group)));
    }

    [Test]
    public void GetUserType ()
    {
      Assert.That (_factory.GetUserType (), Is.SameAs (typeof (User)));
    }

    [Test]
    public void GetPositionType ()
    {
      Assert.That (_factory.GetPositionType (), Is.SameAs (typeof (Position)));
    }
  }
}
