/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
      using (new CultureScope ("de","de"))
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
      //To.ConsoleLine.e (() => result);
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