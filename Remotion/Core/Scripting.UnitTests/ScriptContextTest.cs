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
    [SetUp]
    public void SetUp ()
    {
      ScriptContextTestHelper.ClearScriptContexts ();
    }


    [Test]
    public void Name_And_TypeArbiter_Properties ()
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter>();
      var scriptContext = ScriptContextTestHelper.CreateTestScriptContext ("Context0", typeArbiterStub);
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
      const string name = "Context2";
      ScriptContext scriptContext = CreateScriptContext(name);
      Assert.That (ScriptContext.GetScriptContext (name),Is.SameAs(scriptContext));
    }

    private ScriptContext CreateScriptContext (string name)
    {
      var typeArbiterStub = MockRepository.GenerateStub<ITypeArbiter> ();
      return ScriptContext.CreateScriptContext (name, typeArbiterStub);
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

    public delegate void CreateScriptContextsDelegate ();

    [Test]
    [Explicit] // Note: Race condition does only occur when test is executed standalone.
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
    public void ScriptContext_Current_ThreadStatic ()
    {
      ScriptContext scriptContext = CreateScriptContext ("ScriptContext_Current_ThreadStatic");
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
      Assert.That (ScriptContext.Current, Is.SameAs (scriptContext));

      var threadRunner = new ThreadRunner (delegate {
        var scriptContext2 = CreateScriptContext ("ScriptContext_Current_ThreadStatic_DifferentThread");
        ScriptContext.SwitchAndHoldScriptContext (scriptContext2);
        Assert.That (ScriptContext.Current, Is.SameAs (scriptContext2));
        // Note that we do not call ReleaseScriptContext(scriptContext2) here, so the  ScriptContext stays active on this thread
      });
      threadRunner.Run ();
      Assert.That (ScriptContext.Current, Is.SameAs (scriptContext));
    }


    [Test]
    public void SwitchAndHoldScriptContext ()
    {
      ScriptContext scriptContext = CreateScriptContext ("SwitchAndHoldScriptContext");
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
      Assert.That (ScriptContext.Current, Is.SameAs (scriptContext));
      ScriptContext.ReleaseScriptContext (scriptContext);
      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (Remotion.Utilities.AssertionException), ExpectedMessage = "ReleaseScriptContext: There is already an active script context ('SwitchAndHoldScriptContext_Fails_DueToSameAlreadyActiveScriptContext') on this thread.")]
    public void SwitchAndHoldScriptContext_Fails_DueToSameAlreadyActiveScriptContext ()
    {
      ScriptContext scriptContext = CreateScriptContext ("SwitchAndHoldScriptContext_Fails_DueToSameAlreadyActiveScriptContext");
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (Remotion.Utilities.AssertionException), ExpectedMessage = "ReleaseScriptContext: There is already an active script context ('SwitchAndHoldScriptContext') on this thread.")]
    public void SwitchAndHoldScriptContext_Fails_DueToDifferentAlreadyActiveScriptContext ()
    {
      ScriptContext scriptContext0 = CreateScriptContext ("SwitchAndHoldScriptContext");
      ScriptContext scriptContext1 = CreateScriptContext ("SwitchAndHoldScriptContext_Fails_DueToAlreadyActiveScriptContext");
      ScriptContext.SwitchAndHoldScriptContext (scriptContext0);
      ScriptContext.SwitchAndHoldScriptContext (scriptContext1);
    }

    [Test]
    public void ReleaseScriptContext ()
    {
      ScriptContext scriptContext = CreateScriptContext ("ReleaseScriptContext");
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);
      Assert.That (ScriptContext.Current, Is.SameAs (scriptContext));
      ScriptContext.ReleaseScriptContext (scriptContext);
      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (InvalidOperationException), ExpectedMessage = "Tried to release script context 'ReleaseScriptContext_Fails_DueToTryingToReleaseNonActiveScriptContext' while active script context is 'Remotion.Scripting.ScriptContext'.")]
    public void ReleaseScriptContext_Fails_DueToTryingToReleaseNonActiveScriptContext ()
    {
      ScriptContext scriptContext0 = CreateScriptContext ("SwitchAndHoldScriptContext");
      ScriptContext scriptContext1 = CreateScriptContext ("ReleaseScriptContext_Fails_DueToTryingToReleaseNonActiveScriptContext");
      ScriptContext.SwitchAndHoldScriptContext (scriptContext0);
      ScriptContext.ReleaseScriptContext (scriptContext1);
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
            //To.ConsoleLine.e (() => name).e (scriptContext);
            CheckGetScriptContextConsistency (name, scriptContext);
          }
          catch (ArgumentException)
          {
            // Exception intentionally ignored; threads are expected to try to create same ScriptContext|s.
            //To.ConsoleLine.s ("ArgumentException");
          }
        }

        foreach (var pair in scriptContexts)
        {
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


 

  }
}