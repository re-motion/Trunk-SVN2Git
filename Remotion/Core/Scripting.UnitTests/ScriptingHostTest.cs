// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;
using Remotion.Scripting;
using System.Linq;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingHostTest
  {
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
    public void ScriptingHostThreadStaticTest ()
    {
      ScriptingHost scriptingHostDifferentThread = null;
      var threadRunner = new ThreadRunner (delegate { scriptingHostDifferentThread = ScriptingHost.GetCurrentScriptingHost (); });
      threadRunner.Run ();
      ScriptingHost scriptingHost = ScriptingHost.GetCurrentScriptingHost (); 
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

    //[Test]
    //public void ScriptRuntimeGetEngineFailsTest ()
    //{
    //  var pythonEngine = ScriptingHost.GetEngine (ScriptingHost.ScriptLanguageType (3));
    //  Assert.That (pythonEngine, Is.Not.Null);
    //} 


    [Test]
    public void ScriptRuntimeGetEngineStaticTest ()
    {
      var pythonEngine = ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    //[Test]
    //public void ScriptRuntimeGetEngineStaticFailsTest ()
    //{
    //  var pythonEngine = ScriptingHost.GetScriptEngine (ScriptingHost.ScriptLanguageType (3));
    //  Assert.That (pythonEngine, Is.Not.Null);
    //} 



    private static ScriptingHost CreateScriptingHost ()
    {
      return (ScriptingHost) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptingHost).Assembly, "Remotion.Scripting.ScriptingHost");
    }
  }
}