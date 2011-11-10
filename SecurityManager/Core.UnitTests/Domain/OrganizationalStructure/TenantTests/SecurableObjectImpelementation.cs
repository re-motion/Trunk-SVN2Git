// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class SecurableObjectImpelementation : TenantTestBase
  {
    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");

      IObjectSecurityStrategy objectSecurityStrategy = tenant.GetSecurityStrategy();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOf (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");

      Assert.AreSame (tenant.GetSecurityStrategy(), tenant.GetSecurityStrategy());
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");

      Assert.AreSame (typeof (Tenant), tenant.GetSecurableType());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");
      IDomainObjectSecurityContextFactory factory = tenant;

      Assert.IsFalse (factory.IsInvalid);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      tenant.Delete();

      Assert.IsTrue (factory.IsInvalid);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Tenant", "UID: Tenant");

      ISecurityContext securityContext = ((ISecurityContextFactory) tenant).CreateSecurityContext();
      Assert.AreEqual (tenant.GetPublicDomainObjectType(), Type.GetType (securityContext.Class));
      Assert.IsNull (securityContext.Owner);
      Assert.IsNull (securityContext.OwnerGroup);
      Assert.AreEqual (tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.That (securityContext.AbstractRoles, Is.Empty);
      Assert.IsFalse (securityContext.IsStateless);
    }
  }
}