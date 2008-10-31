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
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionMultiFileHtmlWriterTest : AclToolsTestBase
  {
    [Test]
    [Explicit]
    public void WriteUserStringTest ()
    {
      using (new CultureScope ("de-AT", "de-AT"))
      {
        var aclExpander = new AclExpander();
        var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList ();
        var stringWriterFactory = new StringWriterFactory();
        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (stringWriterFactory, false);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        To.ConsoleLine.e (stringWriterFactory);

        Assert.That (stringWriterFactory.Count, Is.EqualTo (6));


        AssertTextWriterFactoryMemberEquals(stringWriterFactory,"AclExpansionMain","<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion - User Master Table</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-user-table\"><tr><th class=\"header\">User</th><th class=\"header\">First Name</th><th class=\"header\">Last Name</th><th class=\"header\">Access Rights</th></tr><tr><td>test.user</td><td>test</td><td>user</td><td><a href=\".\\test.user\" target=\"_blank\">.\\test.user</a></td></tr><tr><td>group0/user1</td><td></td><td>user1</td><td><a href=\".\\group0_user1\" target=\"_blank\">.\\group0_user1</a></td></tr><tr><td>group1/user1</td><td></td><td>user1</td><td><a href=\".\\group1_user1\" target=\"_blank\">.\\group1_user1</a></td></tr><tr><td>group0/user2</td><td></td><td>user2</td><td><a href=\".\\group0_user2\" target=\"_blank\">.\\group0_user2</a></td></tr><tr><td>group1/user2</td><td></td><td>user2</td><td><a href=\".\\group1_user2\" target=\"_blank\">.\\group1_user2</a></td></tr></table></body></html>");

        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "test.user", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"4\">user test, Dipl.Ing.(FH)</td><td rowspan=\"1\">testOwningGroup, Manager</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan=\"1\">testGroup, Official</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan=\"1\">testGroup, Manager</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan=\"1\">testRootGroup, Official</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>");

        /*
\"AclExpansionMain\"
\"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion - User Master Table</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-user-table\"><tr><th class=\"header\">User</th><th class=\"header\">First Name</th><th class=\"header\">Last Name</th><th class=\"header\">Access Rights</th></tr><tr><td>test.user</td><td>test</td><td>user</td><td><a href=\".\\test.user\" target=\"_blank\">.\\test.user</a></td></tr><tr><td>group0/user1</td><td></td><td>user1</td><td><a href=\".\\group0_user1\" target=\"_blank\">.\\group0_user1</a></td></tr><tr><td>group1/user1</td><td></td><td>user1</td><td><a href=\".\\group1_user1\" target=\"_blank\">.\\group1_user1</a></td></tr><tr><td>group0/user2</td><td></td><td>user2</td><td><a href=\".\\group0_user2\" target=\"_blank\">.\\group0_user2</a></td></tr><tr><td>group1/user2</td><td></td><td>user2</td><td><a href=\".\\group1_user2\" target=\"_blank\">.\\group1_user2</a></td></tr></table></body></html>\"

\"test.user\"
\"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"4\">user test, Dipl.Ing.(FH)</td><td rowspan=\"1\">testOwningGroup, Manager</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan=\"1\">testGroup, Official</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan=\"1\">testGroup, Manager</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan=\"1\">testRootGroup, Official</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>\"

\"group0_user1\"
\"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"1\">user1</td><td rowspan=\"1\">parentGroup0, Manager</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>\"

\"group1_user1\"
\"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"1\">user1</td><td rowspan=\"1\">parentGroup1, Manager</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>\"

\"group0_user2\"
\"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"1\">user2</td><td rowspan=\"1\">parentGroup0, Official</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>\"

\"group1_user2\"
\"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Group Must Own</th><th class=\"header\">Tenant Must Own</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"1\">user2</td><td rowspan=\"1\">parentGroup1, Official</td><td rowspan=\"1\">Order</td><td>None, Dhl, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>\"
         
         * "AclExpansionMain"

         "<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ""><html><head><title>re-motion ACL Expansion - User Master Table</title><style>@import "AclExpansion.css";</style></head><body><table style="width: 100%;" class="aclExpansionTable" id="remotion-user-table"><tr><th class="header">User</th><th class="header">First Name</th><th class="header">Last Name</th><th class="header">Access Rights</th></tr><tr><td>test.user</td><td>test</td><td>user</td><td><a href=".\test.user" target="_blank">.\test.user</a></td></tr><tr><td>group0/user1</td><td></td><td>user1</td><td><a href=".\group0_user1" target="_blank">.\group0_user1</a></td></tr><tr><td>group1/user1</td><td></td><td>user1</td><td><a href=".\group1_user1" target="_blank">.\group1_user1</a></td></tr><tr><td>group1/user2</td><td></td><td>user2</td><td><a href=".\group1_user2" target="_blank">.\group1_user2</a></td></tr><tr><td>group0/user2</td><td></td><td>user2</td><td><a href=".\group0_user2" target="_blank">.\group0_user2</a></td></tr></table></body></html>"

          "test.user"
          "<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ""><html><head><title>re-motion ACL Expansion</title><style>@import "AclExpansion.css";</style></head><body><table style="width: 100%;" class="aclExpansionTable" id="remotion-ACL-expansion-table"><tr><th class="header">User</th><th class="header">Role</th><th class="header">Class</th><th class="header">States</th><th class="header">User Must Own</th><th class="header">Group Must Own</th><th class="header">Tenant Must Own</th><th class="header">User Must Have Abstract Role</th><th class="header">Access Rights</th></tr><tr><td rowspan="4">user test, Dipl.Ing.(FH)</td><td rowspan="1">testGroup, Official</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan="1">testGroup, Manager</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan="1">testRootGroup, Official</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td rowspan="1">testOwningGroup, Manager</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>"

          "group0_user1"
          "<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ""><html><head><title>re-motion ACL Expansion</title><style>@import "AclExpansion.css";</style></head><body><table style="width: 100%;" class="aclExpansionTable" id="remotion-ACL-expansion-table"><tr><th class="header">User</th><th class="header">Role</th><th class="header">Class</th><th class="header">States</th><th class="header">User Must Own</th><th class="header">Group Must Own</th><th class="header">Tenant Must Own</th><th class="header">User Must Have Abstract Role</th><th class="header">Access Rights</th></tr><tr><td rowspan="1">user1</td><td rowspan="1">parentGroup0, Manager</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>"

          "group1_user1"
          "<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ""><html><head><title>re-motion ACL Expansion</title><style>@import "AclExpansion.css";</style></head><body><table style="width: 100%;" class="aclExpansionTable" id="remotion-ACL-expansion-table"><tr><th class="header">User</th><th class="header">Role</th><th class="header">Class</th><th class="header">States</th><th class="header">User Must Own</th><th class="header">Group Must Own</th><th class="header">Tenant Must Own</th><th class="header">User Must Have Abstract Role</th><th class="header">Access Rights</th></tr><tr><td rowspan="1">user1</td><td rowspan="1">parentGroup1, Manager</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>"

          "group1_user2"
          "<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ""><html><head><title>re-motion ACL Expansion</title><style>@import "AclExpansion.css";</style></head><body><table style="width: 100%;" class="aclExpansionTable" id="remotion-ACL-expansion-table"><tr><th class="header">User</th><th class="header">Role</th><th class="header">Class</th><th class="header">States</th><th class="header">User Must Own</th><th class="header">Group Must Own</th><th class="header">Tenant Must Own</th><th class="header">User Must Have Abstract Role</th><th class="header">Access Rights</th></tr><tr><td rowspan="1">user2</td><td rowspan="1">parentGroup1, Official</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>"

          "group0_user2"
          "<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ""><html><head><title>re-motion ACL Expansion</title><style>@import "AclExpansion.css";</style></head><body><table style="width: 100%;" class="aclExpansionTable" id="remotion-ACL-expansion-table"><tr><th class="header">User</th><th class="header">Role</th><th class="header">Class</th><th class="header">States</th><th class="header">User Must Own</th><th class="header">Group Must Own</th><th class="header">Tenant Must Own</th><th class="header">User Must Have Abstract Role</th><th class="header">Access Rights</th></tr><tr><td rowspan="1">user2</td><td rowspan="1">parentGroup0, Official</td><td rowspan="1">Order</td><td>Received, None, Dhl</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr></table></body></html>"
        */
      }
    }

    private void AssertTextWriterFactoryMemberEquals (StringWriterFactory stringWriterFactory, string name, string resultExpected)
    {
      var textWriterData = stringWriterFactory.GetTextWriterData (name);
      string result = textWriterData.TextWriter.ToString();
      Assert.That (result, Is.EqualTo (resultExpected));
    }

    private void AssertTextWriterFactoryMemberContains (StringWriterFactory stringWriterFactory, string name, string resultExpected)
    {
      var textWriterData = stringWriterFactory.GetTextWriterData (name);
      string result = textWriterData.TextWriter.ToString ();
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (resultExpected));
    }


    [Test]
    [Explicit]
    public void WriteUserFileTest ()
    {
      using (new CultureScope ("de-AT", "de-AT"))
      {
        var aclExpander = new AclExpander();
        var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList ();
        //var stringWriter = new StringWriter();
        var streamWriterFactory = new StreamWriterFactory ();
        streamWriterFactory.Directory = Path.Combine ("c:\\temp\\AclExpansions", "AclExpansion_" + AclExpanderApplication.FileNameTimestampNow ());
        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (streamWriterFactory, true);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        //var result = stringWriter.ToString();
        To.ConsoleLine.e (streamWriterFactory);
      }
    }
    


    [Test]
    public void ToFileNameTest ()
    {
      const string unityInput = "µabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      const string forbiddenInput =  "\"?/\\*:"; 
      string forbiddenInputResult = new String ('_', forbiddenInput.Length);
      Assert.That (AclExpansionMultiFileHtmlWriter.ToValidFileName (unityInput), Is.EqualTo (unityInput));
      Assert.That (AclExpansionMultiFileHtmlWriter.ToValidFileName (forbiddenInput), Is.EqualTo (forbiddenInputResult));
      Assert.That (AclExpansionMultiFileHtmlWriter.ToValidFileName (forbiddenInput + unityInput + forbiddenInput + unityInput), Is.EqualTo (forbiddenInputResult + unityInput + forbiddenInputResult  + unityInput));
    }
    
  }
}