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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using NUnitText = NUnit.Framework.SyntaxHelpers.Text;
using System.Windows;
using List = Remotion.Development.UnitTesting.ObjectMother.List;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionHtmlWriterTest : AclToolsTestBase
  {
    [Test]
    [Explicit]
    public void XmlWriterSpikeTest ()
    {
      var stringWriter = new StringWriter ();
      var xmlWriter = CreateXmlWriter (stringWriter, false);

      //xmlWriter.WriteStartDocument();
      // DOCTYPE
      xmlWriter.WriteDocType ("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN",null,null);
      // HTML
      xmlWriter.WriteStartElement ("html");
        // HEAD
        xmlWriter.WriteStartElement ("head");
          // TITLE
          xmlWriter.WriteStartElement ("title");
            xmlWriter.WriteValue ("re-motion ACL Expansion");
          xmlWriter.WriteEndElement (); // title

          // STYLE
          xmlWriter.WriteStartElement ("style");
            xmlWriter.WriteValue ("@import \"AclExpansion.css\";");
          xmlWriter.WriteEndElement (); // style
        xmlWriter.WriteEndElement (); // head

        // BODY
        xmlWriter.WriteStartElement ("body");
            xmlWriter.WriteValue ("re-motion ACL Expansion body");
            
            // TABLE
            xmlWriter.WriteStartElement ("table");
              xmlWriter.WriteAttributeString ("style", "width: 100%;");
              xmlWriter.WriteAttributeString ("class", "aclExpansionTable");
              xmlWriter.WriteAttributeString ("id", "remotion-ACL-expansion-table");
              
              // TR
              xmlWriter.WriteStartElement ("tr");
                // TD
                xmlWriter.WriteStartElement ("td");
                  xmlWriter.WriteAttributeString ("class", "header");
                  xmlWriter.WriteValue ("User");
                  xmlWriter.WriteEndElement (); // td
              xmlWriter.WriteEndElement (); // tr

            xmlWriter.WriteEndElement (); // table
        xmlWriter.WriteEndElement (); // body
      xmlWriter.WriteEndElement (); // html

      xmlWriter.Close();

      //To.ConsoleLine.s (stringWriter.ToString());
    }


    [Test]
    [Explicit]
    public void WriteAclExpansionAsHtmlSpikeTest ()
    {
      using (new CultureScope ("de-AT","de-AT"))
      {
        var aclExpander = new AclExpander();
        var aclExpansion = aclExpander.GetAclExpansionEntryList ();
        var stringWriter = new StringWriter();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
        //To.ConsoleLine.s (stringWriter.ToString());
      }
    }


    [Test]
    public void FullNameTest ()
    {
      using (new CultureScope ("")) // Invariant culture
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

        var stringWriter = new StringWriter();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
        aclExpansionHtmlWriter.Settings.ShortenNames = false;
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
        string result = stringWriter.ToString();
        //To.ConsoleLine.e (() => result);
        Assert.That (result, NUnitText.Contains ("Dhl|Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests"));
        Assert.That (result, NUnitText.Contains ("Remotion.SecurityManager.UnitTests.TestDomain.Order"));
      }
    }

    [Test]
    public void ShortNameTest ()
    {
      using (new CultureScope ("")) // Invariant culture
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

        var stringWriter = new StringWriter ();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
        aclExpansionHtmlWriter.Settings.ShortenNames = true;
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
        string result = stringWriter.ToString ();
        //To.ConsoleLine.e (() => result);
        Assert.That (result, NUnitText.Contains ("Dhl"));
        Assert.That (
            result,
            NUnitText.DoesNotContain (
                "Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests"));
        Assert.That (result, NUnitText.Contains ("Order"));
        Assert.That (result, NUnitText.DoesNotContain ("Remotion.SecurityManager.UnitTests.TestDomain.Order"));
      }
    }


    [Test]
    public void OutputRowCountTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
      aclExpansionHtmlWriter.Settings.OutputRowCount = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result);
      Assert.That (result, NUnitText.Contains ("Usa Da, Dr. (3)"));
      Assert.That (result, NUnitText.Contains ("Da Group, Supreme Being (3)"));
      Assert.That (result, NUnitText.Contains ("Order (3)"));
    }

    [Test]
    public void DontOutputRowCountTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
      aclExpansionHtmlWriter.Settings.OutputRowCount = false;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result);
      Assert.That (result, NUnitText.DoesNotContain ("(2)"));
    }


    [Test]
    public void UserSortOrderTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User, User2, User3);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl, Acl2);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
      aclExpansionHtmlWriter.Settings.ShortenNames = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result);

      Assert.That (result, NUnitText.Contains ("James Ryan"));
      Assert.That (result.IndexOf ("James Ryan"), Is.LessThan (result.IndexOf ("Smith, Mr.")));
    }

    [Test]
    public void SortOrderTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User3);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionTree = new AclExpansionTree (aclExpansion);
      To.ConsoleLine.e (() => aclExpansionTree).nl();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansionTree, stringWriter, true);
      aclExpansionHtmlWriter.Settings.ShortenNames = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result); Clipboard.SetText (result);

      // Roles
      const string firstRoleText = "Anotha Group, Supreme Being";
      const string secondRoleText = "Anotha Group, Working Drone";
      const string thirdRoleText = "Da 3rd Group, Combatant";
      Assert.That (result, NUnitText.Contains (firstRoleText));
      Assert.That (result.IndexOf (firstRoleText), Is.LessThan (result.IndexOf (secondRoleText)));
      Assert.That (result.IndexOf (secondRoleText), Is.LessThan (result.IndexOf (thirdRoleText)));

      Assert.That (result, NUnitText.Contains ("Dhl, None, Received"));
      Assert.That (result, NUnitText.Contains ("Delete, FirstAccessType, Read, Write"));
    }


    [Test]
    public void ResultTest ()
    {
      //using (CultureScope_en_US ())
      using (CultureScope_de_DE())
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User, User2, User3);
        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl2, Acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

        using (var textWriter = new StringWriter())
        //using (var textWriter = new StreamWriter ("c:\\temp\\aaa.html"))
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
          string result = textWriter.ToString();
          //To.ConsoleLine.e (() => result);
          //Clipboard.SetText (result); 
          
          const string resultExpected = 
            #region
 @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"" """">
<html>
  <head>
    <title>re-motion ACL Expansion</title>
    <style>@import ""AclExpansion.css"";</style>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  </head>
  <body>
    <table style=""width: 100%;"" class=""aclExpansionTable"" id=""remotion-ACL-expansion-table"">
      <tr>
        <th class=""header"">User</th>
        <th class=""header"">Role</th>
        <th class=""header"">Class</th>
        <th class=""header"">States</th>
        <th class=""header"">User Must Own</th>
        <th class=""header"">Group Must Own</th>
        <th class=""header"">Tenant Must Own</th>
        <th class=""header"">User Must Have Abstract Role</th>
        <th class=""header"">Access Rights</th>
      </tr>
      <tr>
        <td rowspan=""21"">James Ryan</td>
        <td rowspan=""3"">Anotha Group, Supreme Being</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Anotha Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da 3rd Group, Combatant</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da 3rd Group, Combatant</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da Group, Combatant</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da Group, Supreme Being</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""12"">Smith, Mr.</td>
        <td rowspan=""3"">Anotha Group, Supreme Being</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Anotha Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da 3rd Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Usa Da, Dr.</td>
        <td rowspan=""3"">Da Group, Supreme Being</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
    </table>
  </body>
</html>";
          #endregion
          Assert.That (result, Is.EqualTo (resultExpected));
        }
      }
    }


    [Test]
    public void StatelessAclTest ()
    {
      using (CultureScope_de_DE ())
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);

        // Create stateless-only ACL
        SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
        var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
        TestHelper.AttachAces (statlessAcl, Ace);

        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (statlessAcl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

        using (var textWriter = new StringWriter ())
        {
          const string stateLessAclStateHtmlText = "@*?stateless state?!?!?";
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
          aclExpansionHtmlWriter.StatelessAclStateHtmlText = stateLessAclStateHtmlText;
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
          string result = textWriter.ToString ();
          //To.ConsoleLine.e (() => result);

          Assert.That (result, NUnitText.Contains("<td>"+ stateLessAclStateHtmlText + @"</td>"));
        }
      }
    }



    [Test]
    [Explicit]
    public void SeperateGroupingClassTest ()
    {
      using (CultureScope_de_DE ())
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User2,User3,User);

        // Create stateless-only ACL
        SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
        var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
        TestHelper.AttachAces (statlessAcl, Ace);

        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl, statlessAcl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

        //var aclExpansionUserGrouping = (from aee in aclExpansionEntryList
        //                               orderby aee.User.DisplayName
        //                               group aee by aee.User)
        //                               group ;

        //var aclExpansionUserGrouping = 
        //    from aeeUser in aclExpansionEntryList
        //    select
        //        new {User = aeeUser.User, 
        //             UserGroup = 
        //                 from aeeRole in aeeUser.User.Roles
        //                 group aeeUser by aeeUser.User into aeeRoles
        //                 select
        //                     new {Role = aeeRoles.Key,
        //                          RoleGroup = aeeRoles
        //                         }
        //            };


        //var aclExpansionUserGrouping =
        //    (from aeeUser in aclExpansionEntryList
        //     group aeeUser by aeeUser.User into aeeUsers
        //     select
        //        (from aeeRole in aeeUsers
        //         group aeeRole by aeeRole.Role into aeeRoles 
        //        select aeeRoles).ToList()).ToList();

        //var aclExpansionUserGrouping =
        //    from aeeUser in aclExpansionEntryList
        //     group aeeUser by aeeUser.User into aeeUsers
        //     select
        //        (from aeeRole in aeeUsers
        //         group aeeRole by aeeRole.Role into aeeRoles
        //         select 
        //          (from aeeClass in aeeRoles
        //          group aeeClass by aeeClass.Class into aeeClasses
        //          select aeeClasses.ToList()).ToList()).ToList ();


        //var aclExpansionUserGrouping =
        //    from aeeUser in aclExpansionEntryList
        //    group aeeUser by aeeUser.User
        //    into aeeUsers
        //        select AclExpansionTreeNode.New (
        //        aeeUsers.Key,
        //        (from aeeRole in aeeUsers
        //         group aeeRole by aeeRole.Role
        //           into aeeRoles
        //           select aeeRoles).ToList (),
        //        //aeeUsers.ToList(),
        //        aeeUsers.Count());
        
        
        //var aclExpansionUserGrouping =
        //    aclExpansionEntryList.GroupBy (aeeUser => aeeUser.User).Select (
        //        aeeUsers => AclExpansionTreeNode.New (
        //                        aeeUsers.Key,
        //                        (from aeeRole in aeeUsers
        //                         group aeeRole by aeeRole.Role
        //                         into aeeRoles
        //                             select aeeRoles).ToList(),
        //                        //aeeUsers.ToList(),
        //                        aeeUsers.Count()));

        
            

        //var aclExpansionUserGrouping = aclExpansionEntryList.OrderBy (aee => aee.User.DisplayName).GroupBy (aee => aee.User);
        //var aclExpansionRoleGrouping = aclExpansionUserGrouping.OrderBy (aee => aee.Key.R).GroupBy (aee => aee.User);
        //To.Console.SetOutputComplexityToFull();
        //To.ToTextProvider.Settings.
 
        //foreach (var entries in aclExpansionUserGrouping)
        //{
        //  To.ConsoleLine.e (entries.);
        //  To.ConsoleLine.e (entries[0][0]);
        //  To.ConsoleLine.e (entries[0][0][0]);
        //}
      }
    }


    [Test]
    public void StatelessAclOutputFirstTest ()
    {
      using (CultureScope_de_DE ())
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);

        // Create stateless-only ACL
        SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
        var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
        TestHelper.AttachAces (statlessAcl, Ace);

        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl,statlessAcl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

        using (var textWriter = new StringWriter ())
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
          string result = textWriter.ToString ();
          To.ConsoleLine.e (() => result);

          //Assert.That (result, NUnitText.Contains ("<td>" + stateLessAclStateHtmlText + @"</td>"));
        }
      }
    }


    [Test]
    public void AclExpansionTreeCtorEquivalenceTest ()
    {
      var aclExpansionEntry = new AclExpansionEntry (
          User, Role, Acl, new AclExpansionAccessConditions (), AccessTypeDefinitions, AccessTypeDefinitions2);
      var aclExpansionEntryList = List.New (aclExpansionEntry);
      var textWriterAclExpansionEntryList = new StringWriter ();
      //{
      //  var aclExpansionHtmlWriterAclExpansionEntryList = new AclExpansionHtmlWriter (aclExpansionEntryList, textWriterAclExpansionEntryList, false);
      //  aclExpansionHtmlWriterAclExpansionEntryList.WriteAclExpansionAsHtml();
        new AclExpansionHtmlWriter (aclExpansionEntryList, textWriterAclExpansionEntryList, false).WriteAclExpansionAsHtml();
      var aclExpansionTree = new AclExpansionTree (aclExpansionEntryList);
      var textWriterAclExpansionTree = new StringWriter ();
      new AclExpansionHtmlWriter (aclExpansionTree, textWriterAclExpansionTree, false).WriteAclExpansionAsHtml();

      string resultAclExpansionEntryList = textWriterAclExpansionEntryList.ToString();
      string resultAclExpansionTree = textWriterAclExpansionTree.ToString ();

      Assert.That (resultAclExpansionEntryList, Is.EqualTo (resultAclExpansionTree));
    }




    [Test]
    [Ignore]
    // TODO: Complete & add functionality 
    public void DenyRightsOptionalOutputTest ()
    {
      using (CultureScope_de_DE ())
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl2);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

        //using (var textWriter = new StringWriter ())
        using (var textWriter = new StreamWriter (Path.Combine("c:\\temp","DenyRightsOptionalOutputTest.html")))
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, textWriter, true);
          aclExpansionHtmlWriter.WriteAclExpansionAsHtml ();
          string result = textWriter.ToString ();
          //To.ConsoleLine.e (() => result);
          const string resultExpected =
          #region
 @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"" """">
<html>
  <head>
    <title>re-motion ACL Expansion</title>
    <style>@import ""AclExpansion.css"";</style>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  </head>
  <body>
    <table style=""width: 100%;"" class=""aclExpansionTable"" id=""remotion-ACL-expansion-table"">
      <tr>
        <th class=""header"">User</th>
        <th class=""header"">Role</th>
        <th class=""header"">Class</th>
        <th class=""header"">States</th>
        <th class=""header"">User Must Own</th>
        <th class=""header"">Group Must Own</th>
        <th class=""header"">Tenant Must Own</th>
        <th class=""header"">User Must Have Abstract Role</th>
        <th class=""header"">Access Rights</th>
      </tr>
      <tr>
        <td rowspan=""24"">James Ryan</td>
        <td rowspan=""4"">Anotha Group, Supreme Being</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Anotha Group, Working Drone</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da 3rd Group, Combatant</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da 3rd Group, Combatant</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da Group, Combatant</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""4"">Da Group, Supreme Being</td>
        <td rowspan=""4"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""12"">Smith, Mr.</td>
        <td rowspan=""3"">Anotha Group, Supreme Being</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Anotha Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da 3rd Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da Group, Working Drone</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Usa Da, Dr.</td>
        <td rowspan=""3"">Da Group, Supreme Being</td>
        <td rowspan=""3"">Bestellung</td>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>X</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
    </table>
  </body>
</html>";
          #endregion
          //Assert.That (result, Is.EqualTo (resultExpected));
        }
      }
    }



    private CultureScope CultureScope_de_DE ()
    {
      return new CultureScope ("de-DE");
    }
 
    private CultureScope CultureScope_en_US ()
    {
      return new CultureScope ("en-US");
    }

    public static XmlWriter CreateXmlWriter (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings ();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      //settings.ConformanceLevel = ConformanceLevel.Fragment;

      return XmlWriter.Create (textWriter, settings);
    }
  }


}