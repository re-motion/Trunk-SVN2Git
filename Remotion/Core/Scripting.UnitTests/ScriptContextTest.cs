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
  public class ScriptContextTest
  {
    [Test]
    public void Name_And_TypeArbiter_Properties ()
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter>();
      var scriptContext = CreateScriptContext ("Context0", typeArbiterStub);
      Assert.That (scriptContext.Name, Is.EqualTo ("Context0"));
      Assert.That (scriptContext.TypeArbiter, Is.SameAs (typeArbiterStub));
    }

    [Test]
    public void CreateScriptContext ()
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter>();
      var scriptContext = ScriptContext.CreateScriptContext ("Context1", typeArbiterStub);
      Assert.That (scriptContext.Name, Is.EqualTo ("Context1"));
      Assert.That (scriptContext.TypeArbiter, Is.SameAs (typeArbiterStub));
    }

    [Test]
    public void GetScriptContext ()
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter> ();
      const string name = "Context2";
      var scriptContext = ScriptContext.CreateScriptContext (name, typeArbiterStub);
      Assert.That (ScriptContext.GetScriptContext (name),Is.SameAs(scriptContext));
    }

    [Test]
    public void GetScriptContext_NonExistingContext ()
    {
      const string name = "NonExistingContext";
      Assert.That (ScriptContext.GetScriptContext (name), Is.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (ArgumentException), ExpectedMessage = "ScriptContext named \"DuplicateContext\" already exists.")]
    public void CreateScriptContext_CreatingSameNamedContextFails ()
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter> ();
      const string name = "DuplicateContext";
      var scriptContext = ScriptContext.CreateScriptContext (name, typeArbiterStub);
      Assert.That (scriptContext, Is.Not.Null);
      ScriptContext.CreateScriptContext (name, typeArbiterStub);
    }


    // TODO: Test thread safety of CreateScriptContext and GetScriptContext


    private ScriptContext CreateScriptContext (string name, ITypeArbiter typeArbiter)
    {
      return (ScriptContext) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptContext).Assembly, "Remotion.Scripting.ScriptContext",name,typeArbiter);
    }

 
  }
}