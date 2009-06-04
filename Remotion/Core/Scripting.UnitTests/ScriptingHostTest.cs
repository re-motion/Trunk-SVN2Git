// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
      var scriptingHost = new ScriptingHost();
      Assert.That (scriptingHost.ScriptRuntime, Is.Not.Null);
      //To.ConsoleLine.e (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Select (ls => ls.TypeName));
      //To.ConsoleLine.e (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Select (ls => ls.DisplayName));
      //To.ConsoleLine.e (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Select (ls => ls.Names));
      Assert.That (scriptingHost.ScriptRuntime.Setup.LanguageSetups.Count, Is.EqualTo(1));
      Assert.That (scriptingHost.ScriptRuntime.Setup.LanguageSetups[0].TypeName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("IronPython.Runtime.PythonContext"));
    } 
  }
}