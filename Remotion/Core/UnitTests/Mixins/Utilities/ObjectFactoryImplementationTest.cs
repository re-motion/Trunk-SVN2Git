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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.BridgeImplementations;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class ObjectFactoryImplementationTest
  {
    private ObjectFactoryImplementation _implementation;

    [SetUp]
    public void SetUp ()
    {
      _implementation = new ObjectFactoryImplementation();
    }

    [Test]
    public void ResolveType_ClassType ()
    {
      Type resolved = _implementation.ResolveType (typeof (object));
      Assert.That (resolved, Is.EqualTo (typeof (object)));
    }

    [Test]
    public void ResolveType_RegisteredInterfaceType ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (TestServiceProvider)).AddCompleteInterface (typeof (IServiceProvider)).EnterScope ())
      {
        Type resolved = _implementation.ResolveType (typeof (IServiceProvider));
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
        _implementation.ResolveType (typeof (IServiceProvider));
      }
    }

    [Test]
    public void CreateConstructorInvoker_ResolvesInterfaces ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (TestServiceProvider)).AddCompleteInterface (typeof (IServiceProvider)).EnterScope ())
      {
        object instance = _implementation.CreateInstance (false, typeof (IServiceProvider), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
        Assert.That (instance, Is.InstanceOfType (typeof (TestServiceProvider)));
      }
    }

    [Test]
    public void CreateConstructorInvoker_UsesConcreteType ()
    {
      object instance = _implementation.CreateInstance (false, typeof (BaseType1), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.That (instance, Is.Not.SameAs (typeof (BaseType1)));
      Assert.That (instance, Is.InstanceOfType (typeof (BaseType1)));
      Assert.That (instance, Is.InstanceOfType (TypeFactory.GetConcreteType (typeof (BaseType1))));
    }

    [Test]
    public void CreateConstructorInvoker_UsesTargetType_ForUnmixedType ()
    {
      object instance = _implementation.CreateInstance (false, typeof (object), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.That (instance.GetType(), Is.SameAs (typeof (object)));
    }

    [Test]
    public void CreateConstructorInvoker_PassesPreparedMixinInstances ()
    {
      BT1Mixin1 mixin = new BT1Mixin1();
      object instance = _implementation.CreateInstance (false, typeof (BaseType1), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured, mixin);

      Assert.That (Mixin.Get<BT1Mixin1> (instance), Is.SameAs (mixin));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type System.Object is not valid in the current " 
        + "configuration.\r\nParameter name: mixinInstances")]
    public void CreateConstructorInvoker_InvalidPreparedMixinInstances ()
    {
      object mixin = new object ();
      _implementation.CreateInstance (false, typeof (BaseType1), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured, mixin);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin " 
        + "instances must be specified.\r\nParameter name: preparedMixins")]
    public void CreateConstructorInvoker_InvalidPreparedMixinInstances_UnmixedObject ()
    {
      object mixin = new object ();
      _implementation.CreateInstance (false, typeof (object), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured, false, mixin);
    }

    public class TestServiceProvider : IServiceProvider
    {
      public object GetService (Type serviceType) { throw new NotImplementedException(); }
    }
  }
}
