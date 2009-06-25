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
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptTest
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
      const string scriptFunctionName = "Test";

      const string scriptText =
@"def Test() :
  return 'CtorTest'";

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptScope = engine.CreateScope ();

      var script = new Script<string> (scriptContext, scriptLanguageType, scriptText, scriptScope, scriptFunctionName);

      Assert.That (script.ScriptContext, Is.EqualTo (scriptContext));
      Assert.That (script.ScriptLanguageType, Is.EqualTo (scriptLanguageType));
      Assert.That (script.ScriptText, Is.EqualTo (scriptText));
      Assert.That (script.Execute (), Is.EqualTo ("CtorTest"));
    }


    [Test]
    public void Ctor_NumberTemplateArguments_1 ()
    {
      const string scriptText =
@"def Test(s) :
  return 'Test: ' + s";

      ScriptScope scriptScope = CreateScriptScope(ScriptingHost.ScriptLanguageType.Python);
      var script = new Script<string, string> (ScriptContextTestHelper.CreateTestScriptContext (), ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");
      Assert.That (script.Execute ("works"), Is.EqualTo ("Test: works"));
    }


    [Test]
    public void Ctor_NumberTemplateArguments_2 ()
    {
      const string scriptText =
@"def Test(s0,s1) :
  return 'Test: ' + s0 + ' ' + s1";

      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var script = new Script<string, string, string> (ScriptContextTestHelper.CreateTestScriptContext (), ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");
      Assert.That (script.Execute ("really","works"), Is.EqualTo ("Test: really works"));
    }


    [Test]
    public void Ctor_NumberTemplateArguments_9 ()
    {
      const string scriptText =
@"def Test(s1,s2,s3,s4,s5,s6,s7,s8,s9) :
  return 'Test: '+s1+s2+s3+s4+s5+s6+s7+s8+s9";

      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var script = new Script<string, string, string, string, string, string, string, string, string, string> (ScriptContextTestHelper.CreateTestScriptContext (), ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");
      Assert.That (script.Execute ("1","2","3","4","5","6","7","8","9"), Is.EqualTo ("Test: 123456789"));
    }

    [Test]
    public void Execute_SwitchesAndReleasesScriptContext ()
    {
      Assert.That (ScriptContext.Current , Is.Null);

      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting')
from Remotion.Scripting import *
def Test() :
  return ScriptContext.Current
";

      ScriptContext scriptContextForScript = ScriptContextTestHelper.CreateTestScriptContext ("Execute_SwitchesScriptContext_Script");
      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var script = new Script<ScriptContext> (scriptContextForScript, ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");
      Assert.That (script.Execute (), Is.SameAs (scriptContextForScript));
      
      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    public void Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows ()
    {
      Assert.That (ScriptContext.Current, Is.Null);

      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting')
from Remotion.Scripting import *
def Test() :
  raise Exception('IntentionallyRaisedIronPythonException') 
";

      ScriptContext scriptContextForScript = ScriptContextTestHelper.CreateTestScriptContext ("Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows");
      ScriptScope scriptScope = CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var script = new Script<Object> (scriptContextForScript, ScriptingHost.ScriptLanguageType.Python, scriptText, scriptScope, "Test");

      try
      {
        script.Execute ();
      }
      catch (Exception e)
      {
        Assert.That (e.Message, Is.EqualTo ("IntentionallyRaisedIronPythonException"));
      }

      Assert.That (ScriptContext.Current, Is.Null);
    }


    private ScriptScope CreateScriptScope (ScriptingHost.ScriptLanguageType scriptLanguageType)
    {
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      return engine.CreateScope ();
    }
  }
}