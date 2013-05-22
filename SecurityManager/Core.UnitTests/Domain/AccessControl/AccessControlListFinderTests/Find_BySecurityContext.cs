// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlListFinderTests
{
  [TestFixture]
  public class Find_BySecurityContext : DomainTest
  {
    private SecurableClassDefinition _currentClassDefinition;
    private ClientTransaction _currentClassDefinitionTransaction;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
 
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      _currentClassDefinitionTransaction = ClientTransaction.CreateRootTransaction ();
      _currentClassDefinition = dbFixtures.CreateAndCommitSecurableClassDefinitionWithAccessControlLists (2, _currentClassDefinitionTransaction);
    }

    public override void SetUp ()
    {
      base.SetUp ();
      _currentClassDefinitionTransaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Succeed_WithValidSecurityContext ()
    {
      var expectedAccessControlList = _currentClassDefinition.StatelessAccessControlList;
      SecurityContext context = SecurityContext.CreateStateless(typeof (Order));
     
      var aclFinder = CreateAccessControlListFinder();
      var foundAcl = aclFinder.Find (context);

      Assert.That (foundAcl, Is.EqualTo (expectedAccessControlList).Using (DomainObjectHandleComparer.Instance));
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = 
        "The securable class 'Remotion.SecurityManager.UnitTests.TestDomain.PremiumOrder, Remotion.SecurityManager.UnitTests' could not be found.")]
    public void Fail_WithUnkownSecurableClassDefinition ()
    {
      SecurityContext context = SecurityContext.CreateStateless(typeof (PremiumOrder));

      var aclFinder = CreateAccessControlListFinder();
      aclFinder.Find (context);
    }

    private AccessControlListFinder CreateAccessControlListFinder ()
    {
      return new AccessControlListFinder (new SecurityContextRepository(new RevisionProvider()));
    }
  }
}
