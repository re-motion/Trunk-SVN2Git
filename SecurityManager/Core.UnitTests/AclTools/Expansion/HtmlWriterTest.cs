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
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.Diagnostics.ToText;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class HtmlWriterTest
  {
    [Test]
    public void WriteTagTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new HtmlWriter (stringWriter, false))
      {
        htmlWriter.Tag ("div").Value ("xxx").TagEnd ("div");
      }
      var result = stringWriter.ToString ();
      To.ConsoleLine.e (() => result);
      Assert.That (result, Is.EqualTo ("<div>xxx</div>"));
    }

    [Test]
    public void WritePageHeaderTest ()
    {
      var stringWriter = new StringWriter ();
      using (var htmlWriter = new HtmlWriter (stringWriter, false))
      {
        htmlWriter.WritePageHeader("Page Header Test","pageHeaderTest.css");
      }
      var result = stringWriter.ToString ();
      To.ConsoleLine.e (() => result);
      Assert.That (result, Is.EqualTo ("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>Page Header Test</title><style>@import \"pageHeaderTest.css\";</style></head></html>"));
    }

  }
}