// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.Metadata;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionHtmlWriterImplementationTest : AclToolsTestBase
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

    [Test]
    public void WriteTableDataForAbstractRoleConditionNoAbstractRoleTest ()
    {
      AclExpansionAccessConditions accessConditions = new AclExpansionAccessConditions { AbstractRole = null };
      AssertAclExpansionHtmlWriterImplementationResult (x => x.WriteTableDataForAbstractRoleCondition (accessConditions), "<td></td>");
    }

    [Test]
    public void WriteTableDataForAbstractRoleConditionAbstractRoleTest ()
    {
      var abstractRole = TestHelper.CreateTestAbstractRole();
      AclExpansionAccessConditions accessConditions = new AclExpansionAccessConditions { AbstractRole = abstractRole };
      AssertAclExpansionHtmlWriterImplementationResult (x => x.WriteTableDataForAbstractRoleCondition (accessConditions), "<td>Test</td>");
    }
  }
}
