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
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
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
    public void GetLastCreatedScriptContext ()
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter> ();
      var scriptContext1 = ScriptContext.CreateScriptContext ("GetLastCreatedScriptContextContext1", typeArbiterStub);
      Assert.That (GetLastCreatedScriptContextPrivateInvoke (), Is.SameAs (scriptContext1));
      var scriptContext2 = ScriptContext.CreateScriptContext ("GetLastCreatedScriptContextContext2", typeArbiterStub);
      Assert.That (GetLastCreatedScriptContextPrivateInvoke (), Is.SameAs (scriptContext2));
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

    public delegate void CreateScriptContextsDelegate ();

    [Test]
    public void CreateScriptContext_ThreadSafe ()
    {
      CreateScriptContextsDelegate thread0 = new ScriptContextCreator(true).CreateScriptContexts;
      CreateScriptContextsDelegate thread1 = new ScriptContextCreator (true).CreateScriptContexts;
      IAsyncResult iftAr0 = thread0.BeginInvoke (null, null);
      IAsyncResult iftAr1 = thread1.BeginInvoke (null, null);
      thread0.EndInvoke (iftAr0);
      thread1.EndInvoke (iftAr1);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (InvalidOperationException), ExpectedMessage = "ScriptContext inconsistent")]
    public void CreateScriptContextUnsafe_NotThreadSafe ()
    {
      CreateScriptContextsDelegate thread0 = new ScriptContextCreator (false).CreateScriptContexts;
      CreateScriptContextsDelegate thread1 = new ScriptContextCreator (false).CreateScriptContexts;
      IAsyncResult iftAr0 = thread0.BeginInvoke (null, null);
      IAsyncResult iftAr1 = thread1.BeginInvoke (null, null);
      thread0.EndInvoke (iftAr0);
      thread1.EndInvoke (iftAr1);
    }
    
 
    private class ScriptContextCreator
    {
      private readonly bool _useSafe;
      private static readonly ITypeArbiter s_typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter> ();

      public ScriptContextCreator (bool useSafe)
      {
        _useSafe = useSafe;
      }

      public void CreateScriptContexts ()
      {
        const string namePrefix = "CreateScriptContexts";
        //var scriptContexts = new System.Collections.Generic.List<ScriptContext>();
        var scriptContexts = new Dictionary<string,ScriptContext>();
        for (int i = 0; i < 1000; ++i)
        {
          try
          {
            string name = namePrefix + i;
            ScriptContext scriptContext;
            if(_useSafe)
            {
              scriptContext = ScriptContext.CreateScriptContext (name, s_typeArbiterStub);
            }
            else
            {
              scriptContext = (ScriptContext) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ScriptContext), "CreateScriptContextUnsafe", name, s_typeArbiterStub);
            }
            scriptContexts[name] = scriptContext;
            CheckGetScriptContextConsistency (name, scriptContext);
            Assert.That (ScriptContext.GetScriptContext (name), Is.SameAs (scriptContext));
          }
          catch (ArgumentException)
          {
            //To.ConsoleLine.s ("ArgumentException >>>>>>>>>>>>>"); //.e(Thread.CurrentThread.ManagedThreadId);
          }
        }

        foreach (var pair in scriptContexts)
        {
          //Assert.That (ScriptContext.GetScriptContext (pair.Key), Is.SameAs (pair.Value));
          CheckGetScriptContextConsistency (pair.Key, pair.Value);
          //To.ConsoleLine.e (pair.Value.Name).e(Thread.CurrentThread.Name);
        }
      }

      private void CheckGetScriptContextConsistency (string name, ScriptContext scriptContext)
      {
        if (!Object.ReferenceEquals (ScriptContext.GetScriptContext (name), scriptContext))
        {
          throw new InvalidOperationException ("ScriptContext inconsistent");
        }
      }
    }


    // TODO: Test thread safety of CreateScriptContext and GetScriptContext
    //private class ScriptContextCreator
    //{
    //  private int _start;
    //  private int _increment;
    //  private int _numberOfScriptContexts;

    //  public ScriptContextCreator (int start, int increment)
    //  {
    //    _start = start;
    //    _increment = increment;
    //  }

    //  public void CreateScriptContexts ()
    //  {
    //  }
    //}

    private ScriptContext CreateScriptContext (string name, ITypeArbiter typeArbiter)
    {
      return (ScriptContext) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptContext).Assembly, "Remotion.Scripting.ScriptContext",name,typeArbiter);
    }

    private ScriptContext GetLastCreatedScriptContextPrivateInvoke ()
    {
      return (ScriptContext) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ScriptContext), "GetLastCreatedScriptContext");
    }
  }
}