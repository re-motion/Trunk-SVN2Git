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
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using List = Remotion.Development.UnitTesting.ObjectMother.List;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTreeTest : AclToolsTestBase
  {
    [Test]
    [Explicit]
    public void SeperateGroupingClassTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User2, User3, User);

        // Create stateless-only ACL
        SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
        var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
        TestHelper.AttachAces (statlessAcl, Ace);

        var acls = List.New<AccessControlList> (Acl, statlessAcl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

        var aclExpansionTree = new AclExpansionTree (aclExpansionEntryList);

        foreach (var userNode in aclExpansionTree.Tree)
        {
          To.ConsoleLine.sb().e (userNode.NumberLeafNodes).e (userNode.Key).se();
        } 

      }
    }
  }
}