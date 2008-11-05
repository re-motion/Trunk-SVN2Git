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
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests;
using Remotion.Utilities;

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

      To.ConsoleLine.s (stringWriter.ToString());
    }


    [Test]
    [Explicit]
    public void WriteAclExpansionAsHtmlSpikeTest ()
    {
      using (new CultureScope ("de-AT","de-AT"))
      {
        var aclExpander = new AclExpander();
        var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList ();
        var stringWriter = new StringWriter();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        To.ConsoleLine.s (stringWriter.ToString());
      }
    }


    [Test]
    public void FullNameTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.Settings.UseShortNames = false;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Dhl|Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Remotion.SecurityManager.UnitTests.TestDomain.Order"));
    }

    [Test]
    //[Ignore]
    [Explicit]
    public void ShortNameTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.Settings.UseShortNames = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
      string result = stringWriter.ToString ();
      To.ConsoleLine.e (() => result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Dhl"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.DoesNotContain ("Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Order"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.DoesNotContain ("Remotion.SecurityManager.UnitTests.TestDomain.Order"));
    }


    [Test]
    public void OutputRowCountTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.Settings.OutputRowCount = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
      string result = stringWriter.ToString ();
      To.ConsoleLine.e (() => result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Usa Da, Dr. (2)"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Da Group, Supreme Being (2)"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Order (2)"));
    }

    [Test]
    public void DontOutputRowCountTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.Settings.OutputRowCount = false;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
      string result = stringWriter.ToString ();
      To.ConsoleLine.e (() => result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.DoesNotContain ("(2)"));
    }


    [Test]
    public void UserSortOrderTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User, User2, User3);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl, Acl2);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.Settings.UseShortNames = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result);

      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("James Ryan"));
      Assert.That (result.IndexOf ("James Ryan"), Is.LessThan (result.IndexOf ("Smith, Mr.")));
    }

    [Test]
    [Ignore]
    public void SortOrderTest ()
    {
      var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User3);
      var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.Settings.UseShortNames = true;
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
      string result = stringWriter.ToString ();
      //To.ConsoleLine.e (() => result);

      // Roles
      const string firstRoleText = "Anotha Group, Supreme Being";
      const string secondRoleText = "Anotha Group, Working Drone";
      const string thirdRoleText = "Da 3rd Group, Combatant";
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (firstRoleText));
      Assert.That (result.IndexOf (firstRoleText), Is.LessThan (result.IndexOf (secondRoleText)));
      Assert.That (result.IndexOf (secondRoleText), Is.LessThan (result.IndexOf (thirdRoleText)));

      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Dhl, None, Received"));
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("Delete, Read, Write"));
    }


    [Test]
    [Ignore]
    public void ResultTest ()
    {
      //using (CultureScope_en_US ())
      //using (new CultureScope ("en-US", "en-US"))
      using (CultureScope_de_DE())
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User, User2, User3);
        var acls = Remotion.Development.UnitTesting.ObjectMother.List.New (Acl2, Acl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList_UserList_AceList (users, acls);

        var stringWriter = new StringWriter();
        //var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, false);
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
        aclExpansionHtmlWriter.Settings.UseShortNames = true;
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        string result = stringWriter.ToString();
        To.ConsoleLine.e (() => result);
        const string resultExpected =
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"18\">James Ryan</td><td rowspan=\"3\">Anotha Group, Supreme Being</td><td rowspan=\"3\">Bestellung</td><td>Dhl, Paid, Received</td><td></td><td></td><td></td><td></td><td>Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"3\">Anotha Group, Working Drone</td><td rowspan=\"3\">Order</td><td>Dhl, Paid, Received</td><td></td><td></td><td></td><td></td><td>Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"3\">Da 3rd Group, Combatant</td><td rowspan=\"3\">Order</td><td>Dhl, Paid, Received</td><td></td><td></td><td></td><td></td><td>Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"3\">Da 3rd Group, Combatant</td><td rowspan=\"3\">Order</td><td>Dhl, Paid, Received</td><td></td><td></td><td></td><td></td><td>Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"3\">Da Group, Combatant</td><td rowspan=\"3\">Order</td><td>Dhl, Paid, Received</td><td></td><td></td><td></td><td></td><td>Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"3\">Da Group, Supreme Being</td><td rowspan=\"3\">Order</td><td>Dhl, Paid, Received</td><td></td><td></td><td></td><td></td><td>Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"8\">Smith, Mr.</td><td rowspan=\"2\">Anotha Group, Supreme Being</td><td rowspan=\"2\">Order</td><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"2\">Anotha Group, Working Drone</td><td rowspan=\"2\">Order</td><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"2\">Da 3rd Group, Working Drone</td><td rowspan=\"2\">Order</td><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"2\">Da Group, Working Drone</td><td rowspan=\"2\">Order</td><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr><tr><td rowspan=\"2\">Usa Da, Dr.</td><td rowspan=\"2\">Da Group, Supreme Being</td><td rowspan=\"2\">Order</td><td>Dhl, None, Received</td><td></td><td></td><td>X</td><td></td><td>Delete, Read, Write</td></tr><tr><td>Dhl, None, Received</td><td></td><td></td><td></td><td></td><td>Delete, Read, Write</td></tr></table></body></html>";
        Assert.That (result, Is.EqualTo (resultExpected));
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