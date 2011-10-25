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
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class TenantProxyTest : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Create ()
    {
      var tenant = _testHelper.CreateTenant ("TheTenant", "UID");
      var proxy = TenantProxy.Create (tenant);

      Assert.That (proxy.ID, Is.EqualTo (tenant.ID));
      Assert.That (proxy.UniqueIdentifier, Is.EqualTo (tenant.UniqueIdentifier));
      Assert.That (proxy.DisplayName, Is.EqualTo (tenant.DisplayName));
    }

    [Test]
    public void Serialization ()
    {
      var proxy = TenantProxy.Create (_testHelper.CreateTenant ("TheTenant", "UID"));

      var deserialized = Serializer.SerializeAndDeserialize (proxy);

      Assert.That (deserialized.ID, Is.EqualTo (proxy.ID));
      Assert.That (deserialized.UniqueIdentifier, Is.EqualTo (proxy.UniqueIdentifier));
      Assert.That (deserialized.DisplayName, Is.EqualTo (proxy.DisplayName));
    }
  }
}