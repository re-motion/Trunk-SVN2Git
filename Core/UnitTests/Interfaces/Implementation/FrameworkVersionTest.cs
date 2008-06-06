/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class FrameworkVersionTest
  {
    [SetUp]
    public void SetUp ()
    {
      FrameworkVersion.Reset();
    }

    [TearDown]
    public void TearDown ()
    {
      FrameworkVersion.Reset ();
    }

    [Test]
    public void SetAndAccessValue ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      Assert.That (FrameworkVersion.Value, Is.EqualTo (new Version (1, 2, 3, 4)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The framework version has already been set to 1.2.3.4. It can "
        + "only be set once.")]
    public void SetValue_Twice ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void SetValue_Null ()
    {
      FrameworkVersion.Value = null;
    }

    [Test]
    public void AutomaticVersionDiscovery ()
    {
      Assert.AreEqual (typeof (INullObject).Assembly.GetName().Version, FrameworkVersion.Value);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The framework version could not be determined automatically. "
        + "Manually set Remotion.Implementation.FrameworkVersion.Value to specify which version should be used. The automatic discovery error was: " 
        + "There is no version of Remotion currently loaded or referenced.")]
    public void FailedAutomaticDiscovery ()
    {
      
      ModuleScope scope = new ModuleScope (true, "VersionAccessorAssembly", "VersionAccessorAssembly.dll", "x", "x");
      CustomClassEmitter versionAccessorTypeBuilder = new CustomClassEmitter (scope, "VersionAccessor", typeof (object));
      CustomMethodEmitter versionAccessorMethod =
          versionAccessorTypeBuilder.CreateMethod ("AccessVersion", MethodAttributes.Public | MethodAttributes.Static);
      versionAccessorMethod.AddStatement (
          new ExpressionStatement (new MethodInvocationExpression (null, typeof (FrameworkVersion).GetProperty ("Value").GetGetMethod())));
      versionAccessorMethod.AddStatement (new PopStatement());
      versionAccessorMethod.ImplementByReturningVoid();

      Type versionAccessorType = versionAccessorTypeBuilder.BuildType();
      AppDomain newDomain = AppDomain.CreateDomain ("Test", null, AppDomain.CurrentDomain.SetupInformation);
      scope.SaveAssembly (true);

      try
      {
        CrossAppDomainDelegate action =
            (CrossAppDomainDelegate) Delegate.CreateDelegate (typeof (CrossAppDomainDelegate), versionAccessorType, "AccessVersion");
        newDomain.DoCallBack (action);
      }
      finally
      {
        AppDomain.Unload (newDomain);
        FileUtility.DeleteAndWaitForCompletion (scope.StrongNamedModule.FullyQualifiedName);
      }
    }

    [Test]
    public void Reset ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      FrameworkVersion.Reset();
      FrameworkVersion.Value = new Version (4, 3, 2, 1);
      Assert.That (FrameworkVersion.Value, Is.EqualTo (new Version (4, 3, 2, 1)));
    }
  }
}
