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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework; 
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;
using System.Linq;
// TODO AE: Remove unused usings.

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  // TODO AE: Remove commented code. (Do not commit.)
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


    // TODO AE: When testing enumeration methods, prepare an expected collection and compare actual enumeration.ToArray() to expected collection.
    // TODO AE: This is usually easier to read.
    [Test]
    public void EnumeratorTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.ListMother.New (User, User2, User3);

      // TODO AE: Differentiate between mocks and stubs.
      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); 
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (users);

      var acls = Remotion.Development.UnitTesting.ObjectMother.ListMother.New<AccessControlList> (Acl,Acl2);

      var numberRoles = users.SelectMany (x => x.Roles).Count();
      Assert.That (numberRoles, Is.GreaterThanOrEqualTo (11));
      var numberAces = acls.SelectMany (x => x.AccessControlEntries).Count ();
      Assert.That (numberAces, Is.GreaterThanOrEqualTo (5));
     
      //To.ConsoleLine.e (() => numberRoles);
      //To.ConsoleLine.e (() => numberAces);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (acls);

      var userRoleAclAceCombinations = new UserRoleAclAceCombinationFinder (userFinderMock, aclFinderMock);
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
              //To.ConsoleLine.e (() => user);
              //To.ConsoleLine.e (enumerator.Current.User);
              Assert.That (enumerator.Current.User, Is.EqualTo (user));
              Assert.That (enumerator.Current.Role, Is.EqualTo (role));
              Assert.That (enumerator.Current.Acl, Is.EqualTo (acl));
              Assert.That (enumerator.Current.Ace, Is.EqualTo (ace));
            }
          }
        }
      }    
    
    }
  }
}
