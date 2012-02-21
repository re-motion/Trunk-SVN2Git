// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

// TODO 4650: Move to 'Common' unit test assembly
// ReSharper disable CheckNamespace
namespace Remotion.Mixins.UnitTests.Core
// ReSharper restore CheckNamespace
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
        Assert.That (ObjectFactory.Create<SafeContext>(ParamList.Empty).GetDefaultInstance(), Is.SameAs (TestSafeContextMixin.NewDefaultInstance));
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
      MixinConfiguration.SetActiveConfiguration (null);

      Assert.That (MixinConfiguration.HasActiveConfiguration, Is.False);
      Dev.Null = SafeContext.Instance;
      Assert.That (MixinConfiguration.HasActiveConfiguration, Is.False);
    }
  }
}
