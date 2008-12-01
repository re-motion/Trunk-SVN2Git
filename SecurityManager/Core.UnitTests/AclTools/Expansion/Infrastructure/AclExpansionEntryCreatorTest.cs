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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.Infrastructure
{
  [TestFixture]
  public class AclExpansionEntryCreatorTest : AclToolsTestBase
  {
    [Test]
    public void GetAccessTypesTest ()
    {
      var ace = TestHelper.CreateAceWithNoMatchingRestrictions ();
      AttachAccessTypeReadWriteDelete (ace, true, false, true);
      Assert.That (ace.Validate ().IsValid);
      TestHelper.CreateStatefulAcl (ace);

      var aclExpansionEntryCreator = new AclExpansionEntryCreator();
      //AclProbe aclProbe;
      //AccessTypeStatistics accessTypeStatistics;
      var accessTypesResult = 
        aclExpansionEntryCreator.GetAccessTypes (new UserRoleAclAceCombination (Role2, ace)); //, out aclProbe, out accessTypeStatistics);

      //To.ConsoleLine.e (accessInformation.AllowedAccessTypes);
      //To.ConsoleLine.e (accessInformation.DeniedAccessTypes);

      Assert.That (accessTypesResult.AccessInformation.AllowedAccessTypes, Is.EquivalentTo (new[] { ReadAccessType, DeleteAccessType }));
      Assert.That (accessTypesResult.AccessInformation.DeniedAccessTypes, Is.EquivalentTo (new[] { WriteAccessType }));
    }

    [Test]
    public void GetAccessTypesUsesDiscardingScopeTest ()
    {
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var aclExpansionEntryCreator = new AclExpansionEntryCreator ();
        //AclProbe aclProbe;
        //AccessTypeStatistics accessTypeStatistics;
        User.Roles.Add (Role2);
        //To.ConsoleLine.e (User.Roles);
        var userRoleAclAceCombination = new UserRoleAclAceCombination (Role, Ace);
        var accessTypesResult =
          aclExpansionEntryCreator.GetAccessTypes (userRoleAclAceCombination); //, out aclProbe, out accessTypeStatistics);
        Assert.That (User.Roles, Is.EquivalentTo (new[] { Role, Role2 }));
        accessTypesResult.AclProbe.SecurityToken.Principal.Roles.Clear ();
        accessTypesResult.AclProbe.SecurityToken.Principal.Roles.Add (userRoleAclAceCombination.Role);
        Assert.That (User.Roles, Is.Not.EquivalentTo (new[] { Role, Role2 }));
      }
    }


    //[Test]
    //public void CreateAclExpansionEntryTest ()
    //{
    //  var userRoleAclAce = new UserRoleAclAceCombination (Role, Ace);
    //  var allowedAccessTypes = new[] { WriteAccessType, DeleteAccessType };
    //  var deniedAccessTypes = new[] { ReadAccessType };
    //  AccessInformation accessInformation = new AccessInformation (allowedAccessTypes, deniedAccessTypes);
    //  AclProbe aclProbe = null;
    //  AccessTypeStatistics accessTypeStatistics;
    //  var aclExpansionEntryCreatorMock = MockRepository.GenerateMock<AclExpansionEntryCreator> ();
    //  aclExpansionEntryCreatorMock.Expect (x => x.GetAccessTypes (userRoleAclAce, Arg<AclProbe>.Is.Anything, out accessTypeStatistics)).Return (accessInformation);
      
    //  aclExpansionEntryCreatorMock.Replay();

    //  //var accessTypes = aclExpansionEntryCreatorMock.GetAccessTypes (userRoleAclAceCombination, out aclProbe, out accessTypeStatistics);
    //  var aclExpansionEntry = aclExpansionEntryCreatorMock.CreateAclExpansionEntry (userRoleAclAce);

    //  aclExpansionEntryCreatorMock.VerifyAllExpectations ();


    //  //Assert.That (aclExpansionEntry.User, Is.EqualTo (userRoleAclAce.User));
    //  //Assert.That (aclExpansionEntry.Role, Is.EqualTo (userRoleAclAce.Role));
    //  //Assert.That (aclExpansionEntry.AccessControlList, Is.EqualTo (userRoleAclAce.Acl));
    //  ////Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (aclProbe.AccessConditions));
    //  //Assert.That (aclExpansionEntry.AllowedAccessTypes, Is.EqualTo (allowedAccessTypes));
    //  //Assert.That (aclExpansionEntry.DeniedAccessTypes, Is.EqualTo (deniedAccessTypes));

    //}
  }
}