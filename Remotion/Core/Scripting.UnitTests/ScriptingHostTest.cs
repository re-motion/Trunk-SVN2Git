// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remtion.Scripting;
using System.Linq;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingHostTest
  {
    [Test]
    public void ScriptRuntimeTest ()
    {
      Assert.That (ScriptingHost.ScriptRuntime, Is.Not.Null);
      //To.ConsoleLine.e (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Select (ls => ls.TypeName));
      //To.ConsoleLine.e (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Select (ls => ls.DisplayName));
      //To.ConsoleLine.e (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Select (ls => ls.Names));
      Assert.That (ScriptingHost.ScriptRuntime.Setup.LanguageSetups.Count, Is.EqualTo (1));
      Assert.That (ScriptingHost.ScriptRuntime.Setup.LanguageSetups[0].TypeName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("IronPython.Runtime.PythonContext"));
    }

    [Test]
    public void ScriptRuntimeThreadStaticTest ()
    {
      ScriptRuntime scriptRuntimeDifferentThread = null;
      var threadRunner = new ThreadRunner (delegate { scriptRuntimeDifferentThread = ScriptingHost.ScriptRuntime; });
      threadRunner.Run ();
      Assert.That (ScriptingHost.ScriptRuntime, Is.Not.EqualTo (scriptRuntimeDifferentThread));
    }
  }
}