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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework; 
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
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

      var userRoleAclAceCombinations = new UserRoleAclAceCombinations (userFinderMock,aclFinderMock);
      Assert.That (Remotion.Development.UnitTesting.PrivateInvoke.GetNonPublicField (userRoleAclAceCombinations, "_userFinder"), Is.EqualTo(userFinderMock));
      Assert.That (Remotion.Development.UnitTesting.PrivateInvoke.GetNonPublicField (userRoleAclAceCombinations, "_accessControlListFinder"), Is.EqualTo (aclFinderMock));
    }


    [Test]
    public void EnumeratorTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User, User2, User3);

      var userFinderMock = MockRepository.GenerateMock<IAclExpanderUserFinder> (); 
      userFinderMock.Expect (mock => mock.FindUsers ()).Return (users);

      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl,Acl2);

      var numberRoles = users.SelectMany (x => x.Roles).Count();
      Assert.That (numberRoles, Is.GreaterThanOrEqualTo (11));
      var numberAces = acls.SelectMany (x => x.AccessControlEntries).Count ();
      Assert.That (numberAces, Is.GreaterThanOrEqualTo (5));
     
      //To.ConsoleLine.e (() => numberRoles);
      //To.ConsoleLine.e (() => numberAces);

      var aclFinderMock = MockRepository.GenerateMock<IAclExpanderAclFinder> ();
      aclFinderMock.Expect (mock => mock.FindAccessControlLists ()).Return (acls);

      var userRoleAclAceCombinations = new UserRoleAclAceCombinations (userFinderMock, aclFinderMock);
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