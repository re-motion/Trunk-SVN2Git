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
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

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
      AccessControlList expectedAccessControlList;
      using (_currentClassDefinitionTransaction.EnterNonDiscardingScope ())
      {
        expectedAccessControlList = _currentClassDefinition.StatelessAccessControlList;
      }
      SecurityContext context = SecurityContext.CreateStateless(typeof (Order));
     
      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (ClientTransaction.CreateRootTransaction (), context);

      Assert.That (foundAcl.ID, Is.EqualTo (expectedAccessControlList.ID));
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = 
        "The securable class 'Remotion.SecurityManager.UnitTests.TestDomain.PremiumOrder, Remotion.SecurityManager.UnitTests' cannot be found.")]
    public void Fail_WithUnkownSecurableClassDefinition ()
    {
      SecurityContext context = SecurityContext.CreateStateless(typeof (PremiumOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (ClientTransaction.CreateRootTransaction (), context);
    }
  }
}
