// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework; 
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class UserRoleAclAceCombinationsTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> ();
      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();

      var userRoleAclAceCombinations = new UserRoleAclAceCombinationFinder (userFinderMock,aclFinderMock);
      Assert.That (Remotion.Development.UnitTesting.PrivateInvoke.GetNonPublicField (userRoleAclAceCombinations, "_userFinder"), Is.EqualTo(userFinderMock));
      Assert.That (Remotion.Development.UnitTesting.PrivateInvoke.GetNonPublicField (userRoleAclAceCombinations, "_accessControlListFinder"), Is.EqualTo (aclFinderMock));
    }

    // TODO AE (MGi): Remove this test in favor of its Linq based variant
    [Test]
    public void EnumeratorTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.ListMother.New (User, User2, User3);

      var userFinderStub = MockRepository.GenerateStub<IAclExpanderUserFinder> ();
      userFinderStub.Expect (stub => stub.FindUsers ()).Return (users);

      var acls = Remotion.Development.UnitTesting.ObjectMother.ListMother.New<AccessControlList> (Acl,Acl2);

      var numberRoles = users.SelectMany (x => x.Roles).Count();
      Assert.That (numberRoles, Is.GreaterThanOrEqualTo (11));
      var numberAces = acls.SelectMany (x => x.AccessControlEntries).Count ();
      Assert.That (numberAces, Is.GreaterThanOrEqualTo (5));
     
      var aclFinderStub = MockRepository.GenerateStub<IAclExpanderAclFinder> ();
      aclFinderStub.Expect (stub => stub.FindAccessControlLists ()).Return (acls);

      var userRoleAclAceCombinations = new UserRoleAclAceCombinationFinder (userFinderStub, aclFinderStub);
      var enumerator = userRoleAclAceCombinations.GetEnumerator();

      foreach (var user in users)
      {
        foreach (var role in user.Roles)
        {
          foreach (var acl in acls)
          {
            foreach (var ace in acl.AccessControlEntries)
            {
              enumerator.MoveNext();
              Assert.That (enumerator.Current.User, Is.EqualTo (user));
              Assert.That (enumerator.Current.Role, Is.EqualTo (role));
              Assert.That (enumerator.Current.Acl, Is.EqualTo (acl));
              Assert.That (enumerator.Current.Ace, Is.EqualTo (ace));
            }
          }
        }
      }    
    
    }

    
    [Test]
    public void EnumeratorTest2 ()
    {
      // Prepare to serve some User|s
      var users = Remotion.Development.UnitTesting.ObjectMother.ListMother.New (User, User2, User3);
      var userFinderStub = MockRepository.GenerateStub<IAclExpanderUserFinder> ();
      userFinderStub.Expect (stub => stub.FindUsers ()).Return (users);

      // Prepare to serve some Acl|s
      var acls = Remotion.Development.UnitTesting.ObjectMother.ListMother.New<AccessControlList> (Acl, Acl2);
      var aclFinderStub = MockRepository.GenerateStub<IAclExpanderAclFinder> ();
      aclFinderStub.Expect (stub => stub.FindAccessControlLists ()).Return (acls);

      // Assert that our test set is not too small.
      var numberRoles = users.SelectMany (x => x.Roles).Count ();
      Assert.That (numberRoles, Is.GreaterThanOrEqualTo (11));
      var numberAces = acls.SelectMany (x => x.AccessControlEntries).Count ();
      Assert.That (numberAces, Is.GreaterThanOrEqualTo (5));


      // Assert that the result set is the outer product of the participation sets.
      var userRoleAclAceCombinationsExpected = from user in users
                                               from role in user.Roles
                                               from acl in acls
                                               from ace in acl.AccessControlEntries
                                               select new UserRoleAclAceCombination (role, ace);

      var userRoleAclAceCombinations = new UserRoleAclAceCombinationFinder (userFinderStub, aclFinderStub);
      Assert.That (userRoleAclAceCombinations.ToArray (), Is.EquivalentTo (userRoleAclAceCombinationsExpected.ToArray ()));
    }


    [Test]
    public void CompoundValueEqualityComparerTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination (Role, Ace);
      Assert.That (UserRoleAclAceCombination.Comparer.GetEqualityParticipatingObjects (userRoleAclAceCombination),
        Is.EqualTo(new object[] { Role, Ace }));
    }

    [Test]
    public void EqualityTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination (Role, Ace);
      var userRoleAclAceCombinationSame = new UserRoleAclAceCombination (Role, Ace);
      Assert.That (userRoleAclAceCombination,Is.EqualTo (userRoleAclAceCombination));
      Assert.That (userRoleAclAceCombination, Is.EqualTo (userRoleAclAceCombinationSame));
      Assert.That (userRoleAclAceCombinationSame, Is.EqualTo (userRoleAclAceCombination));
    }

    [Test]
    public void InEqualityTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination (Role2, Ace3);
      var userRoleAclAceCombinationDifferent0 = new UserRoleAclAceCombination (Role2, Ace);
      var userRoleAclAceCombinationDifferent1 = new UserRoleAclAceCombination (Role, Ace3);
      Assert.That (userRoleAclAceCombination, Is.Not.EqualTo (userRoleAclAceCombinationDifferent0));
      Assert.That (userRoleAclAceCombination, Is.Not.EqualTo (userRoleAclAceCombinationDifferent1));
      Assert.That (userRoleAclAceCombinationDifferent0, Is.Not.EqualTo (userRoleAclAceCombination));
      Assert.That (userRoleAclAceCombinationDifferent1, Is.Not.EqualTo (userRoleAclAceCombination));
    }

    [Test]
    public void GetHashCodeTest ()
    {
      Assert.That ((new UserRoleAclAceCombination (Role3, Ace)).GetHashCode (), Is.EqualTo ((new UserRoleAclAceCombination (Role3, Ace)).GetHashCode ()));
      Assert.That ((new UserRoleAclAceCombination (Role3, Ace2)).GetHashCode (), Is.EqualTo ((new UserRoleAclAceCombination (Role3, Ace2)).GetHashCode ()));
      Assert.That ((new UserRoleAclAceCombination (Role, Ace3)).GetHashCode (), Is.EqualTo ((new UserRoleAclAceCombination (Role, Ace3)).GetHashCode ()));
    }

  }
}
