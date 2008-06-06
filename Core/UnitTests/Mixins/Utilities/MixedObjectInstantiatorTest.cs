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
using Remotion.Mixins;
using Remotion.Mixins.BridgeImplementations;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixedObjectInstantiatorTest
  {
    private MixedObjectInstantiator _instantiator;

    [SetUp]
    public void SetUp ()
    {
      _instantiator = new MixedObjectInstantiator();
    }

    [Test]
    public void ResolveType_ClassType ()
    {
      Type resolved = _instantiator.ResolveType (typeof (object));
      Assert.That (resolved, Is.EqualTo (typeof (object)));
    }

    [Test]
    public void ResolveType_RegisteredInterfaceType ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (TestServiceProvider)).AddCompleteInterface (typeof (IServiceProvider)).EnterScope ())
      {
        Type resolved = _instantiator.ResolveType (typeof (IServiceProvider));
        Assert.That (resolved, Is.EqualTo (typeof (TestServiceProvider)));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The interface 'System.IServiceProvider' has not been registered in " 
        + "the current configuration, no instances of the type can be created.")]
    public void ResolveType_NonRegisteredInterfaceType ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        _instantiator.ResolveType (typeof (IServiceProvider));
      }
    }

    [Test]
    public void CreateConstructorInvoker_ResolvesInterfaces ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (TestServiceProvider)).AddCompleteInterface (typeof (IServiceProvider)).EnterScope ())
      {
        object instance = _instantiator.CreateConstructorInvoker<object> (typeof (IServiceProvider), GenerationPolicy.GenerateOnlyIfConfigured, false)
            .With ();
        Assert.That (instance, Is.InstanceOfType (typeof (TestServiceProvider)));
      }
    }

    [Test]
    public void CreateConstructorInvoker_UsesConcreteType ()
    {
      object instance = _instantiator.CreateConstructorInvoker<object> (typeof (BaseType1), GenerationPolicy.GenerateOnlyIfConfigured, false).With ();
      Assert.That (instance, Is.Not.SameAs (typeof (BaseType1)));
      Assert.That (instance, Is.InstanceOfType (typeof (BaseType1)));
      Assert.That (instance, Is.InstanceOfType (TypeFactory.GetConcreteType (typeof (BaseType1))));
    }

    [Test]
    public void CreateConstructorInvoker_UsesTargetType_ForUnmixedType ()
    {
      object instance = _instantiator.CreateConstructorInvoker<object> (typeof (object), GenerationPolicy.GenerateOnlyIfConfigured, false).With ();
      Assert.That (instance.GetType(), Is.SameAs (typeof (object)));
    }

    [Test]
    public void CreateConstructorInvoker_PassesPreparedMixinInstances ()
    {
      BT1Mixin1 mixin = new BT1Mixin1();
      object instance = _instantiator.CreateConstructorInvoker<object> (typeof (BaseType1), GenerationPolicy.GenerateOnlyIfConfigured, false, mixin).With ();

      Assert.That (Mixin.Get<BT1Mixin1> (instance), Is.SameAs (mixin));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type System.Object is not valid in the current " 
        + "configuration.\r\nParameter name: mixinInstances")]
    public void CreateConstructorInvoker_InvalidPreparedMixinInstances ()
    {
      object mixin = new object ();
      _instantiator.CreateConstructorInvoker<object> (typeof (BaseType1), GenerationPolicy.GenerateOnlyIfConfigured, false, mixin).With ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin " 
        + "instances must be specified.\r\nParameter name: preparedMixins")]
    public void CreateConstructorInvoker_InvalidPreparedMixinInstances_UnmixedObject ()
    {
      object mixin = new object ();
      _instantiator.CreateConstructorInvoker<object> (typeof (object), GenerationPolicy.GenerateOnlyIfConfigured, false, mixin);
    }

    public class TestServiceProvider : IServiceProvider
    {
      public object GetService (Type serviceType) { throw new NotImplementedException(); }
    }
  }
}
