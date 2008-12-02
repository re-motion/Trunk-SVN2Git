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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderAclFinderTest : AclToolsTestBase
  {
    [Test]
    public void AclFinderTest ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var aclExpanderAclFinder = new AclExpanderAclFinder();
        var acls = aclExpanderAclFinder.FindAccessControlLists().ToList();

        Assert.That (new[] { Acl, Acl2 }, Is.SubsetOf (acls));
      }
    }
  }
}