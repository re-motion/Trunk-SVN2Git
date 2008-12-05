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
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionHtmlWriterImplementationTest
  {
    [Test]
    public void WriteTableDataForBooleanConditionTest ()
    {
      AssertAclExpansionHtmlWriterImplementationResult (x => x.WriteTableDataForBooleanCondition (true), "<td>X</td>");
      AssertAclExpansionHtmlWriterImplementationResult (x => x.WriteTableDataForBooleanCondition (false), "<td></td>");
    }


    public void AssertAclExpansionHtmlWriterImplementationResult (Action<AclExpansionHtmlWriterImplementation> testFunc, string resultingXmlExpected)
    {
      var stringWriter = new StringWriter ();
      var implementation = new AclExpansionHtmlWriterImplementation (stringWriter, false, new AclExpansionHtmlWriterSettings ());
      testFunc (implementation);
      implementation.HtmlTagWriter.Close ();
      var result = stringWriter.ToString ();
      Assert.That (result, Is.EqualTo (resultingXmlExpected));
    }
  }
}