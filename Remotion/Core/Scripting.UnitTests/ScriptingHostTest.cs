// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingHostTest
  {
    [Test]
    public void CollectScriptEnginesTest ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var scriptEngines = scriptingHost.FindScriptEngines ();
      Assert.That (scriptEngines, Is.Not.Null);
      Assert.That (scriptEngines.Count, Is.EqualTo(1));
      Assert.That (scriptEngines[ScriptingHost.ScriptLanguageType.Python], Is.Not.Null);
    }

    [Test]
    public void GetScriptEnginesTest ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var scriptEngines = scriptingHost.GetScriptEngines ();
      Assert.That (scriptEngines, Is.EquivalentTo (scriptingHost.FindScriptEngines ()));
    }


    [Test]
    public void ScriptRuntimeTest ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost();
      Assert.That (scriptingHost, Is.Not.Null);
      Assert.That (scriptingHost.GetScriptRuntime (), Is.Not.Null);
      Assert.That (scriptingHost.GetScriptRuntime ().Setup.LanguageSetups.Count, Is.EqualTo (1));
      Assert.That (scriptingHost.GetScriptRuntime ().Setup.LanguageSetups[0].TypeName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("IronPython.Runtime.PythonContext"));
    }

    [Test]
    public void GetScriptingHostTest ()
    {
      ScriptingHost scriptingHost = ScriptingHost.GetScriptingHost ();
      Assert.That (scriptingHost, Is.Not.Null);
      ScriptingHost scriptingHost2 = ScriptingHost.GetScriptingHost ();
      Assert.That (Object.ReferenceEquals(scriptingHost,scriptingHost2), Is.True);
    }
 
    [Test]
    public void ScriptingHostThreadStaticTest ()
    {
      ScriptingHost scriptingHostDifferentThread = null;
      var threadRunner = new ThreadRunner (delegate { scriptingHostDifferentThread = ScriptingHost.GetScriptingHost (); });
      threadRunner.Run ();
      ScriptingHost scriptingHost = ScriptingHost.GetScriptingHost (); 
      Assert.That (scriptingHost, Is.Not.Null);
      Assert.That (scriptingHostDifferentThread, Is.Not.Null);
      Assert.That (scriptingHost, Is.Not.EqualTo (scriptingHostDifferentThread));
    }


    [Test]
    public void ScriptRuntimeGetEngineTest ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var pythonEngine = scriptingHost.GetEngine (ScriptingHost.ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "ScriptEngine for ScriptLanguageType None cannot be supplied. Check App.config <microsoft.scripting>-section.")]
    public void ScriptRuntimeGetEngineFailsTest ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      scriptingHost.GetEngine (ScriptingHost.ScriptLanguageType.None);
    } 


    [Test]
    public void ScriptRuntimeGetEngineStaticTest ()
    {
      var pythonEngine = ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "ScriptEngine for ScriptLanguageType None cannot be supplied. Check App.config <microsoft.scripting>-section.")]
    public void ScriptRuntimeGetEngineStaticFailsTest ()
    {
      ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType.None);
    } 


    private static ScriptingHost CreateScriptingHost ()
    {
      return (ScriptingHost) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptingHost).Assembly, "Remotion.Scripting.ScriptingHost");
    }
  }
}