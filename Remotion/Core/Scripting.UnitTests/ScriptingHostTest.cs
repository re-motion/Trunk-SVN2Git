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
    public void FindScriptEngines ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var scriptEngines = scriptingHost.FindScriptEngines ();
      Assert.That (scriptEngines, Is.Not.Null);
      Assert.That (scriptEngines.Count, Is.EqualTo(1));
      Assert.That (scriptEngines[ScriptingHost.ScriptLanguageType.Python], Is.Not.Null);
    }

    [Test]
    public void GetScriptEngines ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var scriptEngines = scriptingHost.GetScriptEngines ();
      Assert.That (scriptEngines, Is.EquivalentTo (scriptingHost.FindScriptEngines ()));
    }


    [Test]
    public void ScriptRuntime ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost();
      Assert.That (scriptingHost, Is.Not.Null);
      Assert.That (scriptingHost.GetScriptRuntime (), Is.Not.Null);
      Assert.That (scriptingHost.GetScriptRuntime ().Setup.LanguageSetups.Count, Is.EqualTo (1));
      Assert.That (scriptingHost.GetScriptRuntime ().Setup.LanguageSetups[0].TypeName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("IronPython.Runtime.PythonContext"));
    }

    [Test]
    public void ScriptingHost_Current ()
    {
      ScriptingHost scriptingHost = ScriptingHost.Current;
      Assert.That (scriptingHost, Is.Not.Null);
      ScriptingHost scriptingHost2 = ScriptingHost.Current;
      Assert.That (scriptingHost,Is.SameAs(scriptingHost2));
    }
 
    [Test]
    public void ScriptingHost_Current_ThreadStatic ()
    {
      ScriptingHost scriptingHostDifferentThread = null;
      var threadRunner = new ThreadRunner (delegate { scriptingHostDifferentThread = ScriptingHost.Current; });
      threadRunner.Run ();
      ScriptingHost scriptingHost = ScriptingHost.Current; 
      Assert.That (scriptingHost, Is.Not.Null);
      Assert.That (scriptingHostDifferentThread, Is.Not.Null);
      Assert.That (scriptingHost, Is.Not.SameAs (scriptingHostDifferentThread));
    }


    [Test]
    public void GetEngine ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var pythonEngine = scriptingHost.GetEngine (ScriptingHost.ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "ScriptEngine for ScriptLanguageType None cannot be supplied. Check App.config <microsoft.scripting>-section.")]
    public void GetEngine_Fails ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      scriptingHost.GetEngine (ScriptingHost.ScriptLanguageType.None);
    } 


    [Test]
    public void GetEngine_Static ()
    {
      var pythonEngine = ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "ScriptEngine for ScriptLanguageType None cannot be supplied. Check App.config <microsoft.scripting>-section.")]
    public void GetEngine_Static_Fails ()
    {
      ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType.None);
    } 


    private static ScriptingHost CreateScriptingHost ()
    {
      return (ScriptingHost) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptingHost).Assembly, "Remotion.Scripting.ScriptingHost");
    }
  }
}