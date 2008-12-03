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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Text.StringExtensions;
using Remotion.Utilities;
using NUnitText = NUnit.Framework.SyntaxHelpers.Text;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  // TODO AE: Remove commented code. (Do not commit.)
  // TODO AE: Dedicated test for GetUsers and GetAccessControlEntriesForUser should be added. (Non-TDD code?)
  public class AclExpansionMultiFileHtmlWriterTest : AclToolsTestBase
  {
    [Test]
    public void TextWriterFactoryResultTest ()
    {
      using (new CultureScope ("en-US"))
      {
        var aclExpander = new AclExpander();
        var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList ();
        var stringWriterFactory = new StringWriterFactory();
        
        stringWriterFactory.Directory = "";
        stringWriterFactory.Extension = "xYz";

        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (stringWriterFactory, false);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansion (aclExpansionEntryList);
        //To.ConsoleLine.e (stringWriterFactory);

        Assert.That (stringWriterFactory.Count, Is.EqualTo (6));

        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "_AclExpansionMain_", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion - User Master Table</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-user-table\"><tr><th class=\"header\">User</th><th class=\"header\">First Name</th><th class=\"header\">Last Name</th><th class=\"header\">Access Rights</th></tr><tr><td>test.user</td><td>test</td><td>user</td><td><a href=\".\\test.user.xYz\" target=\"_blank\">.\\test.user.xYz</a></td></tr><tr><td>group0/user1</td><td></td><td>user1</td><td><a href=\".\\group0_user1.xYz\" target=\"_blank\">.\\group0_user1.xYz</a></td></tr><tr><td>group1/user1</td><td></td><td>user1</td><td><a href=\".\\group1_user1.xYz\" target=\"_blank\">.\\group1_user1.xYz</a></td></tr><tr><td>group0/user2</td><td></td><td>user2</td><td><a href=\".\\group0_user2.xYz\" target=\"_blank\">.\\group0_user2.xYz</a></td></tr><tr><td>group1/user2</td><td></td><td>user2</td><td><a href=\".\\group1_user2.xYz\" target=\"_blank\">.\\group1_user2.xYz</a></td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "test.user", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Owning Group Equals</th><th class=\"header\">Owning Tenant Equals</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"8\">user test, Dipl.Ing.(FH)</td><td rowspan=\"2\">testGroup, Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr><tr><td rowspan=\"2\">testGroup, Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr><tr><td rowspan=\"2\">testOwningGroup, Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr><tr><td rowspan=\"2\">testRootGroup, Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "group0_user1", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Owning Group Equals</th><th class=\"header\">Owning Tenant Equals</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"2\">user1</td><td rowspan=\"2\">parentGroup0, Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "group1_user1", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Owning Group Equals</th><th class=\"header\">Owning Tenant Equals</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"2\">user1</td><td rowspan=\"2\">parentGroup1, Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "group0_user2", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Owning Group Equals</th><th class=\"header\">Owning Tenant Equals</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"2\">user2</td><td rowspan=\"2\">parentGroup0, Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals (stringWriterFactory, "group1_user2", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">States</th><th class=\"header\">User Must Own</th><th class=\"header\">Owning Group Equals</th><th class=\"header\">Owning Tenant Equals</th><th class=\"header\">User Must Have Abstract Role</th><th class=\"header\">Access Rights</th></tr><tr><td rowspan=\"2\">user2</td><td rowspan=\"2\">parentGroup1, Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
      }
    }


    // TODO AE: Move to bottom.
    private void AssertTextWriterFactoryMemberEquals (StringWriterFactory stringWriterFactory, string name, string resultExpected)
    {
      var textWriterData = stringWriterFactory.GetTextWriterData (name);
      string result = textWriterData.TextWriter.ToString();

      //To.ConsoleLine.e ("resultExpected", resultExpected);
      //To.ConsoleLine.e ("result        ", result);

      Assert.That (result, Is.EqualTo (resultExpected));
    }


    // TODO AE: Remove unused method.
    public string WriteStringWriterFactory (StringWriterFactory stringWriterFactory) //, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("stringWriterFactory", stringWriterFactory);
      //ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      var toTextBuilderString = To.String;
      foreach (KeyValuePair<string, TextWriterData> pair in stringWriterFactory.NameToTextWriterData)
      {
        toTextBuilderString.nl ().s ("AssertTextWriterFactoryMemberEquals(stringWriterFactory,").e (pair.Key).s (", ").e (pair.Value.TextWriter.ToString ().Escape ()).s (");");
      }
      return toTextBuilderString.CheckAndConvertToString();
    }


    // TODO AE: Consider naming this integration test and making it automatically executable. Or remove it.
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
        streamWriterFactory.Directory = Path.Combine ("c:\\temp\\AclExpansions", "AclExpansion_" + StringUtility.GetFileNameTimestampNow ());
        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (streamWriterFactory, true);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansion (aclExpansionEntryList);
        //var result = stringWriter.ToString();
        //To.ConsoleLine.e (streamWriterFactory);
      }
    }
    


    [Test]
    // TODO AE: Rename test to match method being tested (ToValidFileNameTest). Consider moving to AclExpansionHtmlWriterBaseTest.
    public void ToFileNameTest ()
    {
      const string unityInput = "µabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      const string forbiddenInput =  "\"?/\\*:";
      string forbiddenInputResult = new String ('_', forbiddenInput.Length);
      Assert.That (AclExpansionHtmlWriterImplementationBase.ToValidFileName (unityInput), Is.EqualTo (unityInput));
      Assert.That (AclExpansionHtmlWriterImplementationBase.ToValidFileName (forbiddenInput), Is.EqualTo (forbiddenInputResult));
      Assert.That (AclExpansionHtmlWriterImplementationBase.ToValidFileName (forbiddenInput + unityInput + forbiddenInput + unityInput), Is.EqualTo (forbiddenInputResult + unityInput + forbiddenInputResult + unityInput));
    }



    //[Test]
    //public void OutputRowCountTest ()
    //{
    //  var users = Remotion.Development.UnitTesting.ObjectMother.List.New (User);
    //  var acls = Remotion.Development.UnitTesting.ObjectMother.List.New<AccessControlList> (Acl);

    //  List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList_UserList_AceList (users, acls);

    //  var stringWriter = new StringWriter ();
    //  var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansion, stringWriter, true);
    //  aclExpansionHtmlWriter.Settings.OutputRowCount = true;
    //  aclExpansionHtmlWriter.WriteAclExpansion ();
    //  string result = stringWriter.ToString ();
    //  //To.ConsoleLine.e (() => result);
    //  Assert.That (result, NUnitText.Contains ("Usa Da, Dr. (3)"));
    //  Assert.That (result, NUnitText.Contains ("Da Group, Supreme Being (3)"));
    //  Assert.That (result, NUnitText.Contains ("Order (3)"));
    //}
    


  }
}