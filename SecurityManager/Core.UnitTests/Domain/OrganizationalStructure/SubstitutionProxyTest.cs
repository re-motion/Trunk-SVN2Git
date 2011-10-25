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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class SubstitutionProxyTest : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
    }

    [Test]
    public void Create ()
    {
      var substitution = CreateSubstitution();
      var proxy = SubstitutionProxy.Create (substitution);

      Assert.That (proxy.ID, Is.EqualTo (substitution.ID));
      Assert.That (proxy.UniqueIdentifier, Is.EqualTo (((IBusinessObjectWithIdentity) substitution).UniqueIdentifier));
      Assert.That (proxy.DisplayName, Is.EqualTo (((IBusinessObjectWithIdentity) substitution).DisplayName));
    }

    [Test]
    public void Serialization ()
    {
      var substitution = CreateSubstitution ();
      var proxy = SubstitutionProxy.Create (substitution);

      var deserialized = Serializer.SerializeAndDeserialize (proxy);

      Assert.That (deserialized.ID, Is.EqualTo (proxy.ID));
      Assert.That (deserialized.UniqueIdentifier, Is.EqualTo (proxy.UniqueIdentifier));
      Assert.That (deserialized.DisplayName, Is.EqualTo (proxy.DisplayName));
    }

    private Substitution CreateSubstitution ()
    {
      var tenant = _testHelper.CreateTenant ("TheTenant", "UID");
      var group = _testHelper.CreateGroup ("TheGroup", null, tenant);
      var user = _testHelper.CreateUser ("UserName", "FN", "LN", null, group, tenant);
      var substituton = Substitution.NewObject();
      substituton.SubstitutedUser = user;
      substituton.SubstitutingUser = _testHelper.CreateUser ("SubstitutingUser", "SFN", "SLN", null, group, tenant);

      return substituton;
    }
  }
}