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
using System.IO;
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
using NUnitList = NUnit.Framework.SyntaxHelpers.List;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTreeTest : AclToolsTestBase
  {
    [Test]
    [Explicit]
    public void ExpansionTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User2, User3, User);

        // Create stateless-only ACL
        //SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
        //var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
        //TestHelper.AttachAces (statlessAcl, Ace);

        var statelessAcl = CreateStatelessAcl (Ace);

        var acls = List.New<AccessControlList> (Acl, statelessAcl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

        using (var textWriter = new StreamWriter ("c:\\temp\\aaa.html"))
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (textWriter, true);
          aclExpansionHtmlWriter.Settings.OutputRowCount = true;
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
          //string result = textWriter.ToString ();
          //To.ConsoleLine.e (() => result);
          //Clipboard.SetText (result); 
        }


        var aclExpansionTree = new AclExpansionTree (aclExpansionEntryList);

        foreach (var userNode in aclExpansionTree.Tree)
        {
          To.ConsoleLine.sb().e (userNode.NumberLeafNodes).e (userNode.Key).se();
        }

        To.Console.IndentationString = "  ";
        To.Console.AllowNewline = true;
        To.ConsoleLine.nl (2).e (aclExpansionTree.Tree);

      }
    }

    [Test]
    public void SingleAclSingleUserExpansionTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);

        var acls = List.New<AccessControlList> (Acl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

        using (var textWriter = new StreamWriter ("c:\\temp\\aaa.html"))
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (textWriter, true);
          aclExpansionHtmlWriter.Settings.OutputRowCount = true;
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        }


        var aclExpansionTree = new AclExpansionTree (aclExpansionEntryList);

        foreach (var userNode in aclExpansionTree.Tree)
        {
          To.ConsoleLine.sb ().e (userNode.NumberLeafNodes).e (userNode.Key).se ();
        }

        To.Console.IndentationString = "  ";
        To.Console.AllowNewline = true;
        To.ConsoleLine.nl (2).e (aclExpansionTree.Tree);

        var userNodes = aclExpansionTree.Tree;
        Assert.That (userNodes.Count, Is.EqualTo (1)); // # users
        Assert.That (userNodes[0].Key, Is.EqualTo (User));
        
        var roleNodes = userNodes[0].Children;
        Assert.That (roleNodes.Count, Is.EqualTo (1)); // # roles
        Assert.That (roleNodes[0].Key, Is.EqualTo (User.Roles[0]));
        
        var classNodes = roleNodes[0].Children;
        Assert.That (classNodes.Count, Is.EqualTo (1)); // # classes
        Assert.That (classNodes[0].Key.StatefulAccessControlLists, NUnitList.Contains (Acl));

        var stateNodes = classNodes[0].Children;
        Assert.That (stateNodes.Count, Is.EqualTo (3)); // # states
        //Assert.That (stateNodes[0].StateCombinations, Is.SubsetOf (Acl.StateCombinations));
        foreach (AclExpansionEntry aee in stateNodes)
        {
          Assert.That (aee.StateCombinations, Is.SubsetOf (Acl.StateCombinations));
        }
      }
    }


    [Test]
    public void StatelessAccessControlListSortOrderTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
        
        var statelessAcl = CreateStatelessAcl (Ace);
        var acls = List.New (Acl, statelessAcl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

        var aclExpansionTreeInverseSorted = new AclExpansionTree (
            aclExpansionEntryList,
            (classEntry => (classEntry.AccessControlList is StatefulAccessControlList) ? "A" : "B"));
        LogAclExpansionTree (aclExpansionTreeInverseSorted);
        Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children.Count, Is.EqualTo (2));
        Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (Acl));

        var aclExpansionTreeDefaultSorted = new AclExpansionTree (aclExpansionEntryList);
        LogAclExpansionTree (aclExpansionTreeDefaultSorted);
        Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children.Count, Is.EqualTo (2));
        Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (statelessAcl));

      }
    }

    private static void LogAclExpansionTree (AclExpansionTree aclExpansionTree)
    {
      To.Console.IndentationString = "  ";
      To.Console.AllowNewline = true;
      To.ConsoleLine.nl (2).e (aclExpansionTree.Tree);
    }


    private AccessControlList CreateStatelessAcl (params AccessControlEntry[] aces)
    {
      // Create stateless-only ACL
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
      var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
      foreach (AccessControlEntry ace in aces)
      {
        TestHelper.AttachAces (statlessAcl, ace);
      }
      return statlessAcl;
    }
  }
}