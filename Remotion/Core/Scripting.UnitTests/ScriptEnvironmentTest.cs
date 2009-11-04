// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Microsoft.Scripting.Runtime;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptEnvironmentTest
  {
    [TearDown]
    public void TearDown ()
    {
      ScriptContextTestHelper.ClearScriptContexts ();
    }

    [Test]
    public void Create ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      var scriptEnvironment2 = ScriptEnvironment.Create ();
      Assert.That (scriptEnvironment.ScriptScope, Is.Not.Null);
      Assert.That (scriptEnvironment2.ScriptScope, Is.Not.Null);
      Assert.That (scriptEnvironment.ScriptScope, Is.Not.SameAs (scriptEnvironment2.ScriptScope));
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (MissingMemberException), ExpectedMessage = "'str' object has no attribute 'Substring'")]
    public void NotImportClr ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      AssertSubstringMethodExists(scriptEnvironment);
    }

    [Test]
    public void ImportClr ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportClr ();
      AssertSubstringMethodExists (scriptEnvironment);
    }

    private void AssertSubstringMethodExists (ScriptEnvironment scriptEnvironment)
    {
      const string scriptText = "'ABcd'.Substring(1,2)";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext ("ImportClr"), ScriptLanguageType.Python, scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("Bc"));
    }


    [Test]
    public void Import ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.Import ("Remotion", "Remotion.Diagnostics.ToText", "To", "ToTextBuilder");
      Assert.That (scriptEnvironment.ScriptScope.GetVariable("To"), Is.Not.Null);
      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ToTextBuilder"), Is.Not.Null);
    }

    [Test]
    public void SetVariable ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      var doc = new Document ("ScriptEnvironmentTest_SetVariable");
      scriptEnvironment.SetVariable ("ScriptEnvironmentTest_SetVariable", doc);
      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ScriptEnvironmentTest_SetVariable"), Is.SameAs (doc));
    }

    [Test]
    public void GetVariable ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      const string variableName = "ScriptEnvironmentTest_GetVariable";
      var variableInvalid = scriptEnvironment.GetVariable<Document> (variableName);
      Assert.That (variableInvalid.IsValid , Is.False);
      var doc = new Document ("GetVariable");
      scriptEnvironment.SetVariable (variableName, doc);
      var variableValid = scriptEnvironment.GetVariable<Document> (variableName);
      Assert.That (variableValid.IsValid, Is.True);
      Assert.That (variableValid.Value, Is.SameAs (doc));
    }

    [Test]
    public void ImportHelperFunctions_IIf ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportHelperFunctions();
      scriptEnvironment.SetVariable ("x", 100000);
      const string scriptText = "IIf(x > 1000,'big','small')";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext ("ImportHelperFunctions"), ScriptLanguageType.Python, 
            scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("big"));
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (UnboundNameException), ExpectedMessage = "name 'NonExistingSymbol' is not defined")]
    public void ImportHelperFunctions_IIfIsNotLazy ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportHelperFunctions ();
      scriptEnvironment.SetVariable ("x", 100000);
      const string scriptText = "IIf(x > 1000,'big',NonExistingSymbol)";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext ("ImportHelperFunctions"), ScriptLanguageType.Python,
            scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("big"));
    }


    [Test]
    public void ImportHelperFunctions_LazyIIf ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportHelperFunctions ();
      scriptEnvironment.SetVariable ("x", 100000);
      const string scriptText = "LazyIIf(x > 1000,lambda:'big',lambda:NonExistingSymbol)";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext ("ImportHelperFunctions"), ScriptLanguageType.Python,
            scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("big"));
    }

  }
}
