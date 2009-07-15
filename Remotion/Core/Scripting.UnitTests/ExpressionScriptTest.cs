// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ExpressionScriptTest
  {
    [SetUp]
    public void SetUp ()
    {
      ScriptContextTestHelper.ClearScriptContexts ();
    }

    [Test]
    public void Ctor ()
    {
      ScriptContext scriptContext = ScriptContextTestHelper.CreateTestScriptContext ();
      const ScriptingHost.ScriptLanguageType scriptLanguageType = ScriptingHost.ScriptLanguageType.Python;

      const string scriptText = 
"'ExpressionScriptCtorTest'";

      var scriptEnvironment = new ScriptEnvironment ();
      var script = new ExpressionScript<string> (scriptContext, scriptLanguageType, scriptText, scriptEnvironment);

      Assert.That (script.ScriptContext, Is.EqualTo (scriptContext));
      Assert.That (script.ScriptLanguageType, Is.EqualTo (scriptLanguageType));
      Assert.That (script.ScriptText, Is.EqualTo (scriptText));
      Assert.That (script.Execute (), Is.EqualTo ("ExpressionScriptCtorTest"));
    }



    [Test]
    public void Execute ()
    {
      const string scriptText =
"'Document Name: ' + rmDoc.DocumentName";

      var scriptEnvironment = new ScriptEnvironment ();
      var document = new Document ("Test Doc");
      scriptEnvironment.SetVariable ("rmDoc", document);

      var script = new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext (),
        ScriptingHost.ScriptLanguageType.Python, scriptText, scriptEnvironment);
      Assert.That (script.Execute (), Is.EqualTo ("Document Name: Test Doc"));
    }





    [Test]
    public void Execute_ImportedTypeIntoScriptScope ()
    {
      const string scriptText =
"Document('New ' + rmDoc.DocumentName)";

      var scriptEnvironment = new ScriptEnvironment ();
      //ImportFromAssemblyIntoScriptScope (scriptScope, "Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests.TestDomain", "Document");
      scriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests.TestDomain", "Document");
      var document = new Document ("Test Doc");
      scriptEnvironment.SetVariable ("rmDoc", document);

      var script = new ExpressionScript<Document> (ScriptContextTestHelper.CreateTestScriptContext (),
        ScriptingHost.ScriptLanguageType.Python, scriptText, scriptEnvironment);
      var result = script.Execute ();
      Assert.That (result.DocumentName, Is.EqualTo ("New Test Doc"));
    }



    [Test]
    public void Execute_SwitchesAndReleasesScriptContext ()
    {
      Assert.That (ScriptContext.Current, Is.Null);

      const string scriptText = 
"ScriptContext.Current";

      ScriptContext scriptContextForScript = ScriptContextTestHelper.CreateTestScriptContext ("Execute_SwitchesScriptContext_Script");
      var scriptEnvironment = new ScriptEnvironment ();
      scriptEnvironment.Import ("Remotion.Scripting", "Remotion.Scripting", "ScriptContext");
      var script = new ExpressionScript<ScriptContext> (scriptContextForScript, ScriptingHost.ScriptLanguageType.Python,
        scriptText, scriptEnvironment);
      Assert.That (script.Execute (), Is.SameAs (scriptContextForScript));

      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    public void Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows ()
    {
      Assert.That (ScriptContext.Current, Is.Null);

      const string scriptText = 
"RaiseCommandNotSupportedInIronPythonExpressioSoUsingUnkownSymbol";

      ScriptContext scriptContextForScript = ScriptContextTestHelper.CreateTestScriptContext ("Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows");
      var scriptEnvironment = new ScriptEnvironment ();
      var script = new ExpressionScript<ScriptContext> (scriptContextForScript, ScriptingHost.ScriptLanguageType.Python,
        scriptText, scriptEnvironment);

      try
      {
        script.Execute ();
      }
      catch (Exception e)
      {
        Assert.That (e.Message, Is.EqualTo ("name 'RaiseCommandNotSupportedInIronPythonExpressioSoUsingUnkownSymbol' is not defined"));
      }

      Assert.That (ScriptContext.Current, Is.Null);
    }



    public void ImportFromAssemblyIntoScriptScope (ScriptScope scriptScope, string assembly, string nameSpace, string symbol)
    {
      string scriptText = @"
import clr
clr.AddReferenceByPartialName('" + assembly + "')" +
 @"
from " + nameSpace + " import " + symbol;

      const ScriptingHost.ScriptLanguageType scriptLanguageType = ScriptingHost.ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (scriptScope);
    }

  }
}