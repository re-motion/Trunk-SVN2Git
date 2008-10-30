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
using Remotion.SecurityManager.Domain.AccessControl.TextWriterFactory;

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
        //var stringWriter = new StringWriter();
        var stringWriterFactory = new StringWriterFactory();
        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (stringWriterFactory, true);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        //var result = stringWriter.ToString();
        To.ConsoleLine.e (stringWriterFactory);
      }
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
        var stringWriterFactory = new StreamWriterFactory (Path.Combine("c:\\temp\\AclExpansions","AclExpansion_" + AclExpanderApplication.FileNameTimestampNow()));
        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter (stringWriterFactory, true);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansionAsHtml (aclExpansionEntryList);
        //var result = stringWriter.ToString();
        To.ConsoleLine.e (stringWriterFactory);
      }
    }
    


    [Test]
    public void ToFileNameTest ()
    {
      const string unityInput = "µabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      const string forbiddenInput = "!\"§$%&/()=?²³{[]}\\´`*+~'#;,:.-_";
      string forbiddenInputResult = new String ('_', forbiddenInput.Length);
      Assert.That (AclExpansionMultiFileHtmlWriter.ToFileName (unityInput), Is.EqualTo (unityInput));
      Assert.That (AclExpansionMultiFileHtmlWriter.ToFileName (forbiddenInput), Is.EqualTo (forbiddenInputResult));
      Assert.That (AclExpansionMultiFileHtmlWriter.ToFileName (forbiddenInput + unityInput + forbiddenInput + unityInput), Is.EqualTo (forbiddenInputResult + unityInput + forbiddenInputResult  + unityInput));
    }
    
  }
}