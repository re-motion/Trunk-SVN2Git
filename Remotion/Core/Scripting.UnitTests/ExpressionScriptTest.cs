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
using System.Runtime.Remoting;
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
@"'ExpressionScriptCtorTest'";

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptScope = engine.CreateScope ();

      var script = new ExpressionScript<string> (scriptContext, scriptLanguageType, scriptText, scriptScope);

      Assert.That (script.ScriptContext, Is.EqualTo (scriptContext));
      Assert.That (script.ScriptLanguageType, Is.EqualTo (scriptLanguageType));
      Assert.That (script.ScriptText, Is.EqualTo (scriptText));
      Assert.That (script.Execute (), Is.EqualTo ("ExpressionScriptCtorTest"));
    }



    [Test]
    public void Execute ()
    {
      const string scriptText =
@"'Document Name: ' + rmDoc.DocumentName";

      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var document = new Document ("Test Doc");
      scriptScope.SetVariable ("rmDoc", document);

      var script = new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext (), ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope);
      Assert.That (script.Execute (), Is.EqualTo ("Document Name: Test Doc"));
    }





//    [Test]
//    public void Execute2 ()
//    {
//      const string scriptText =
//@"Document('New ' + rmDoc.DocumentName)";

//      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
//      var document = new Document ("Test Doc");
//      scriptScope.SetVariable ("rmDoc", document);

//      var script = new ExpressionScript<string> (ScriptContextTestHelper.CreateTestScriptContext (), ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope);
//      Assert.That (script.Execute (), Is.EqualTo ("Document Name: Test Doc"));
//    }



//    [Test]
//    public void Execute_SwitchesAndReleasesScriptContext ()
//    {
//      Assert.That (ScriptContext.Current, Is.Null);

//      const string scriptText = @"
//import clr
//clr.AddReferenceByPartialName('Remotion.Scripting')
//from Remotion.Scripting import *
//def Test() :
//  return ScriptContext.Current
//";

//      ScriptContext scriptContextForScript = ScriptContextTestHelper.CreateTestScriptContext ("Execute_SwitchesScriptContext_Script");
//      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
//      var script = new Script<ScriptContext> (scriptContextForScript, ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");
//      Assert.That (script.Execute (), Is.SameAs (scriptContextForScript));

//      Assert.That (ScriptContext.Current, Is.Null);
//    }

//    [Test]
//    public void Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows ()
//    {
//      Assert.That (ScriptContext.Current, Is.Null);

//      const string scriptText = @"
//import clr
//clr.AddReferenceByPartialName('Remotion.Scripting')
//from Remotion.Scripting import *
//def Test() :
//  raise Exception('IntentionallyRaisedIronPythonException') 
//";

//      ScriptContext scriptContextForScript = ScriptContextTestHelper.CreateTestScriptContext ("Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows");
//      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
//      var script = new Script<Object> (scriptContextForScript, ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");

//      try
//      {
//        script.Execute ();
//      }
//      catch (Exception e)
//      {
//        Assert.That (e.Message, Is.EqualTo ("IntentionallyRaisedIronPythonException"));
//      }

//      Assert.That (ScriptContext.Current, Is.Null);
//    }






    private ScriptScope CreateScriptScope (ScriptingHost.ScriptLanguageType scriptLanguageType)
    {
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      return engine.CreateScope ();
    }
  }
}