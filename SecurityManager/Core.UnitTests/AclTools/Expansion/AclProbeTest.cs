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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclProbeTest : AclToolsTestBase
  {

    [Test]
    public void CreateAclProbe_User_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.User, Is.EqualTo (User));
    }

    [Test]
    public void CreateAclProbe_SpecificTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithSpecficTenant (Tenant);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningTenant, Is.EqualTo (ace.SpecificTenant));
    }

    [Test]
    [Ignore]
    public void CreateAclProbe_OwningTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant ();
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningTenant, Is.EqualTo (ace.SpecificTenant));
    }

    //[Test]
    //public void EmptyToken_EmptyAce_Matches ()
    //{
    //  AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
    //  AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
    //}

  }
}