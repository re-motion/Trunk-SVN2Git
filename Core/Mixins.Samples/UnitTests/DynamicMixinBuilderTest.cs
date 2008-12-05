// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.IO;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Samples.DynamicMixinBuilding;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.UnitTests
{
  [TestFixture]
  public class DynamicMixinBuilderTest
  {
    public class SampleTarget
    {
      public bool VoidMethodCalled = false;

      public virtual string StringMethod (int intArg)
      {
        return "SampleTarget.StringMethod (" + intArg + ")";
      }

      public virtual void VoidMethod ()
      {
        VoidMethodCalled = true;
      }
    }

    private readonly List<Tuple<object, MethodInfo, object[], object>> _calls = new List<Tuple<object, MethodInfo, object[], object>> ();
    private MethodInvocationHandler _invocationHandler;
    private DynamicMixinBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      string directory = PrepareDirectory();

      DynamicMixinBuilder.Scope = new ModuleScope (true, "DynamicMixinBuilder.Signed", Path.Combine (directory, "DynamicMixinBuilder.Signed.dll"),
        "DynamicMixinBuilder.Unsigned", Path.Combine (directory, "DynamicMixinBuilder.Unsigned.dll"));

      ConcreteTypeBuilder.SetCurrent (null);

      _invocationHandler = delegate (object instance, MethodInfo method, object[] args, BaseMethodInvoker baseMethod)
      {
        object result = baseMethod (args);
        _calls.Add (Tuple.NewTuple (instance, method, args, result));
        return "Intercepted: " + result;
      };
      
      _calls.Clear ();

      _builder = new DynamicMixinBuilder (typeof (SampleTarget));
    }

    private string PrepareDirectory ()
    {
      string directory = Path.Combine (Environment.CurrentDirectory, "DynamicMixinBuilder.Generated");
      if (Directory.Exists (directory))
        Directory.Delete (directory, true);
      Directory.CreateDirectory (directory);

      CopyFile (typeof (Mixin<,>).Assembly.ManifestModule.FullyQualifiedName, directory); // Core/Mixins assembly
      CopyFile (typeof (MethodInvocationHandler).Assembly.ManifestModule.FullyQualifiedName, directory); // Samples assembly
      return directory;
    }

    private void CopyFile (string sourcePath, string targetDirectory)
    {
      Assert.IsTrue (Directory.Exists (targetDirectory));
      File.Copy (sourcePath, Path.Combine (targetDirectory, Path.GetFileName (sourcePath)));
    }

    [TearDown]
    public void TearDown ()
    {
      if (DynamicMixinBuilder.Scope.StrongNamedModule != null)
      {
        DynamicMixinBuilder.Scope.SaveAssembly (true);
        PEVerifier.VerifyPEFile (DynamicMixinBuilder.Scope.StrongNamedModule.FullyQualifiedName);
      }
      if (DynamicMixinBuilder.Scope.WeakNamedModule != null)
      {
        DynamicMixinBuilder.Scope.SaveAssembly (false);
        PEVerifier.VerifyPEFile (DynamicMixinBuilder.Scope.WeakNamedModule.FullyQualifiedName);
      }
    }

    [Test]
    public void BuildMixinType_CreatesType ()
    {
      Type t = new DynamicMixinBuilder (typeof (object)).BuildMixinType (_invocationHandler);
      Assert.IsNotNull (t);
    }

    [Test]
    public void BuildMixinType_CreatesTypeDerivedFromMixin ()
    {
      Type t = new DynamicMixinBuilder (typeof (object)).BuildMixinType (_invocationHandler);
      Assert.IsTrue (ReflectionUtility.CanAscribe (t, typeof (Mixin<,>)));
    }

    [Test]
    public void BuildMixinType_AddsMethodsWithOverrideAttribute ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      MethodInfo overriderMethod = t.GetMethod ("StringMethod");
      Assert.IsNotNull (overriderMethod);
      Assert.IsTrue (overriderMethod.IsDefined (typeof (OverrideTargetAttribute), false));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The declaring type of the method must be the "
        + "target type.\r\nParameter name: method")]
    public void BuildMixinType_OverrideMethod_FromWrongType ()
    {
      _builder.OverrideMethod (typeof (object).GetMethod ("ToString"));
    }

    [Test]
    public void GeneratedTypeHoldsHandler ()
    {
      Type t = _builder.BuildMixinType (_invocationHandler);
      FieldInfo handlerField = t.GetField ("InvocationHandler");
      Assert.IsNotNull (handlerField);
      Assert.AreSame (_invocationHandler, handlerField.GetValue (null));
    }

    [Test]
    public void GeneratedMethodIsIntercepted ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass (typeof (SampleTarget)).Clear().AddMixins (t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With();
        target.StringMethod (4);
        Assert.IsTrue (_calls.Count == 1);
      }
    }

    [Test]
    public void GeneratedMethodIsIntercepted_WithRightParameters ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass (typeof (SampleTarget)).Clear().AddMixins (t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With ();
        target.StringMethod (4);

        Tuple<object, MethodInfo, object[], object> callInfo = _calls[0];
        Assert.AreSame (target, callInfo.A);
        Assert.AreEqual (typeof (SampleTarget).GetMethod ("StringMethod"), callInfo.B);
        Assert.That (callInfo.C, Is.EquivalentTo (new object[] { 4 } ));
      }
    }

    [Test]
    public void GeneratedMethodIsIntercepted_WithRightReturnValue ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass (typeof (SampleTarget)).Clear().AddMixins (t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With ();
        target.StringMethod (4);

        Tuple<object, MethodInfo, object[], object> callInfo = _calls[0];
        Assert.AreEqual ("SampleTarget.StringMethod (4)", callInfo.D);
      }
    }

    [Test]
    public void GeneratedMethodIsIntercepted_WithCorrectBase ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass (typeof (SampleTarget)).Clear().AddMixins (t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With ();
        string result = target.StringMethod (4);
        Assert.AreEqual ("Intercepted: SampleTarget.StringMethod (4)", result);
      }
    }

    [Test]
    public void InterceptVoidMethod ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("VoidMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.BuildFromActive().ForClass (typeof (SampleTarget)).Clear().AddMixins (t).EnterScope())
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With ();
        target.VoidMethod ();
        Assert.IsTrue (target.VoidMethodCalled);

        Tuple<object, MethodInfo, object[], object> callInfo = _calls[0];
        Assert.AreEqual (null, callInfo.D);
      }
    }
  }
}
