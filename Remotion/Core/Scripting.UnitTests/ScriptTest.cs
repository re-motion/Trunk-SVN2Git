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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptTest
  {
    //[Test]
    //public void Ctor ()
    //{
    //  ScriptContext scriptContext = ScriptContextTestHelper.GetTestScriptContext ();
    //  const ScriptingHost.ScriptLanguageType scriptLanguageType = ScriptingHost.ScriptLanguageType.Python;
    //  const string scriptText = "text";

    //  var script = new ScriptBase (scriptContext, scriptLanguageType, scriptText);
    //  Assert.That (script.ScriptContext, Is.EqualTo (scriptContext));
    //  Assert.That (script.ScriptLanguageType, Is.EqualTo (scriptLanguageType));
    //  Assert.That (script.ScriptText, Is.EqualTo (scriptText));
    //}


    //private ScriptContext GetScriptContext ()
    //{
    //  var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter> ();
    //  return CreateScriptContext("DummyScriptContext", typeArbiterStub);
    //}

    //private ScriptContext CreateScriptContext (string name, ITypeArbiter typeArbiter)
    //{
    //  return (ScriptContext) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptContext).Assembly, "Remotion.Scripting.ScriptContext", name, typeArbiter);
    //}
  }
}