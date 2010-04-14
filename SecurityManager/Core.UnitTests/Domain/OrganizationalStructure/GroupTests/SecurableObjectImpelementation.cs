// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class SecurableObjectImpelementation : GroupTestBase
  {
    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject group = CreateGroup();

      IObjectSecurityStrategy objectSecurityStrategy = group.GetSecurityStrategy();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject group = CreateGroup();

      Assert.AreSame (group.GetSecurityStrategy(), group.GetSecurityStrategy());
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject group = CreateGroup();

      Assert.AreSame (typeof (Group), group.GetSecurableType());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Group group = CreateGroup();
      IDomainObjectSecurityContextFactory factory = group;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      group.Delete();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Group group = CreateGroup();

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext();
      Assert.AreEqual (group.GetPublicDomainObjectType(), Type.GetType (securityContext.Class));
      Assert.IsNull (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.AreEqual (group.Tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.That (securityContext.AbstractRoles, Is.Empty);
      Assert.IsFalse (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoTenant ()
    {
      Group group = CreateGroup();
      group.Tenant = null;

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext();
      Assert.AreEqual (group.GetPublicDomainObjectType(), Type.GetType (securityContext.Class));
      Assert.IsNull (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsNull (securityContext.OwnerTenant);
      Assert.That (securityContext.AbstractRoles, Is.Empty);
      Assert.IsFalse (securityContext.IsStateless);
    }
  }
}