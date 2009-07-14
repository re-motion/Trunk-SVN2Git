// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptEnvironmentTest
  {
    [Test]
    public void Ctor ()
    {
      ScriptScope scriptScope = ScriptingHelper.CreateScriptScope (ScriptingHost.ScriptLanguageType.Python);
      var scriptEnvironment = new ScriptEnvironment (scriptScope);
      Assert.That (scriptEnvironment.ScriptScope, Is.EqualTo (scriptScope));
    }
  }
}