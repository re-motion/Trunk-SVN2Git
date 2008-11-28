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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionEntryTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      var accessConditions = new AclExpansionAccessConditions();
      var aclExpansionEntry = new AclExpansionEntry (User, Role, Acl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2);
      Assert.That (aclExpansionEntry.User, Is.EqualTo (User));
      Assert.That (aclExpansionEntry.Role, Is.EqualTo (Role));
      Assert.That (aclExpansionEntry.Class, Is.EqualTo (Acl.Class));
      Assert.That (aclExpansionEntry.StateCombinations, Is.EqualTo (Acl.StateCombinations));
      Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (accessConditions));
      Assert.That (aclExpansionEntry.AllowedAccessTypes, Is.EqualTo (AccessTypeDefinitions));
      Assert.That (aclExpansionEntry.DeniedAccessTypes, Is.EqualTo (AccessTypeDefinitions2));
    }


    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = @"StateCombinations not defined for StatelessAccessControlList. Test for ""is StatefulAccessControlList"" in calling code.") ]
    public void StateCombinationsForStatelessAclThrowsTest ()
    {
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
      var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);

      var accessConditions = new AclExpansionAccessConditions ();
      var aclExpansionEntry = new AclExpansionEntry (User, Role, statlessAcl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2);
      To.ConsoleLine.e(aclExpansionEntry.StateCombinations); // TODO AE: To.Console is never executed. Replace by Dev.Null = ...;
    }

    // TODO AE: Remaining TDD-style unit tests are missing.


  }
}