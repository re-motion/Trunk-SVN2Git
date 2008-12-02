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
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using List = Remotion.Development.UnitTesting.ObjectMother.List;
using NUnitList = NUnit.Framework.SyntaxHelpers.List;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  // TODO AE: Remove commented code. (Do not commit.)
  // TODO AE: Refactory AclExpansionTree to be able to test in a more fine-grained way.
  [TestFixture]
  public class AclExpansionTreeTest : AclToolsTestBase
  {
    // TODO AE: Remove explicit test - make automatically executable or remove.
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

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls, false);

        using (var textWriter = new StreamWriter ("c:\\temp\\aaa.html"))
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
          aclExpansionHtmlWriter.Settings.OutputRowCount = true;
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
          //string result = textWriter.ToString ();
          //To.ConsoleLine.e (() => result);
          //Clipboard.SetText (result); 
        }


        var aclExpansionTree = new AclExpansionTree (aclExpansion);

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

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls, false);

        //WriteAclExpansionAsHtmlToDisk(aclExpansion, "c:\\temp\\aaa.html");

        var aclExpansionTree = new AclExpansionTree (aclExpansion);

        //WriteAclExpansionTreeToConsole(aclExpansionTree);

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
        Assert.That (stateNodes.Count, Is.EqualTo (2)); // # states

        Assert.That (stateNodes[0].Children.Count, Is.EqualTo (2)); // # states in group with same AclExpansionEntry ignoring StateCombinations
        Assert.That (stateNodes[1].Children.Count, Is.EqualTo (1)); // # states in group with same AclExpansionEntry ignoring StateCombinations

        foreach (var aclExpansionEntryTreeNode in stateNodes)
        {
          foreach (AclExpansionEntry aee in aclExpansionEntryTreeNode.Children)
          {
            Assert.That (aee.StateCombinations, Is.SubsetOf (Acl.StateCombinations));
          }
        }
      }
    }

    private void WriteAclExpansionTreeToConsole (AclExpansionTree aclExpansionTree)
    {
      To.Console.IndentationString = "  ";
      To.Console.AllowNewline = true;
      To.ConsoleLine.nl (2).e (aclExpansionTree.Tree);
    }

    private void WriteAclExpansionAsHtmlToDisk (List<AclExpansionEntry> aclExpansion, string fileName)
    {
      using (var textWriter = new StreamWriter (fileName))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
        aclExpansionHtmlWriter.Settings.OutputRowCount = true;
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
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

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls, false);

        var aclExpansionTreeInverseSorted = new AclExpansionTree (
            aclExpansionEntryList,
            (classEntry => (classEntry.AccessControlList is StatefulAccessControlList) ? "A" : "B")); // sort stateful before stateless
        //LogAclExpansionTree (aclExpansionTreeInverseSorted);
        Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children.Count, Is.EqualTo (2));
        //Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (Acl));
        Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (Acl));

        var aclExpansionTreeDefaultSorted = new AclExpansionTree (aclExpansionEntryList);
        //LogAclExpansionTree (aclExpansionTreeDefaultSorted);
        Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children.Count, Is.EqualTo (2));
        //Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (statelessAcl));
        Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (statelessAcl));

      }
    }


    [Test]
    public void AclExpansionEntryIgnoreStateEqualityComparerTest ()
    {
      var comparer = AclExpansionTree.AclExpansionEntryIgnoreStateEqualityComparer;
      var aclExpansionEntry1 =
          new AclExpansionEntry (
              User, Role, Acl, new AclExpansionAccessConditions (), new[] { WriteAccessType }, new[] { ReadAccessType, DeleteAccessType });
      var aclExpansionEntry2 =
          new AclExpansionEntry (
              User, Role, Acl2, new AclExpansionAccessConditions (), new[] { WriteAccessType }, new[] { ReadAccessType, DeleteAccessType });
      var aclExpansionEntry3 =
         new AclExpansionEntry (
             User, Role, Acl, new AclExpansionAccessConditions (), new[] { ReadAccessType, WriteAccessType }, new[] { DeleteAccessType });
      //aclExpansionEntry2.StateCombinations;
      Assert.That (comparer.Equals (aclExpansionEntry1, aclExpansionEntry1), Is.True);
      Assert.That (comparer.Equals (aclExpansionEntry1, aclExpansionEntry2), Is.True);
      Assert.That (comparer.Equals (aclExpansionEntry1, aclExpansionEntry3), Is.False);
    }


    [Test]
    public void AclExpansionEntryIgnoreStateEqualityComparerTest2 ()
    {
      var comparer = AclExpansionTree.AclExpansionEntryIgnoreStateEqualityComparer;

      var aclSameClassDiffernenStates = TestHelper.CreateStatefulAcl (Acl.Class, new StateDefinition[] { });
      var aclDifferentClass = TestHelper.CreateStatefulAcl (TestHelper.CreateClassDefinition ("2008-11-26, 16:41"), new StateDefinition[] { });

      var a =
          new AclExpansionEntry (
              User, Role, Acl, new AclExpansionAccessConditions (), new[] { WriteAccessType }, new[] { ReadAccessType, DeleteAccessType });
      var aDifferent =
          new AclExpansionEntry (
              User2, Role2, aclDifferentClass, new AclExpansionAccessConditions { OwningTenant = Tenant }, new[] { ReadAccessType }, new[] { DeleteAccessType });

      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.True);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, aclSameClassDiffernenStates, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.True);

      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (aDifferent.User, a.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, aDifferent.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, aDifferent.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, aDifferent.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, a.AccessConditions, aDifferent.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, aDifferent.DeniedAccessTypes)), Is.False);
    }



    // TODO AE: Do not commit console output in unit tests. (Ok for debugging reasons, but slows down build process and clutters the build log.)
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