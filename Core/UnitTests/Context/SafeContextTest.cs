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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class SafeContextTest
  {
    [SetUp]
    public void SetUp ()
    {
      SafeContext.SetInstance (null);
    }

    [TearDown]
    public void TearDown ()
    {
      SafeContext.SetInstance (null);
    }

    [Test]
    public void Instance_AutoInitialization ()
    {
      ISafeContextStorageProvider instance = SafeContext.Instance;
      Assert.That (instance, Is.Not.Null);
      Assert.That (SafeContext.Instance, Is.SameAs (instance));
    }

    [Test]
    public void SetInstance ()
    {
      ISafeContextStorageProvider myInstance = new CallContextStorageProvider();
      SafeContext.SetInstance (myInstance);
      Assert.That (SafeContext.Instance, Is.SameAs (myInstance));
    }

    [Test]
    public void SetInstance_Null ()
    {
      ISafeContextStorageProvider myInstance = new CallContextStorageProvider ();
      SafeContext.SetInstance (myInstance);
      Assert.That (SafeContext.Instance, Is.SameAs (myInstance));
      SafeContext.SetInstance (null);
      Assert.That (SafeContext.Instance, Is.Not.SameAs (myInstance));
      Assert.That (SafeContext.Instance, Is.Not.Null);
    }

    [Test]
    public void DefaultInstanceMixable ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<SafeContext> ().AddMixin<TestSafeContextMixin> ().EnterScope ())
      {
        Assert.That (ObjectFactory.Create<SafeContext>().With().GetDefaultInstance(), Is.SameAs (TestSafeContextMixin.NewDefaultInstance));
      }
    }

    public class TestSafeContextMixin : Mixin<SafeContext>
    {
      public static ISafeContextStorageProvider NewDefaultInstance = new CallContextStorageProvider();

      [OverrideTarget]
      public ISafeContextStorageProvider GetDefaultInstance ()
      {
        return NewDefaultInstance;
      }
    }

    [Test]
    public void AutoInitialization_LeavesMixinConfigurationEmpty ()
    {
      Dev.Null = SafeContext.Instance;
      Assert.That (MixinConfiguration.HasActiveConfiguration, Is.False);
    }
  }
}