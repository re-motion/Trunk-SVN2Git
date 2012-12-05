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
using System;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
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
    public void CreateInstance_UsesConcreteType ()
    {
      object instance = _implementation.CreateInstance (false, typeof (BaseType1), ParamList.Empty);
      Assert.That (instance, Is.Not.SameAs (typeof (BaseType1)));
      Assert.That (instance, Is.InstanceOf (typeof (BaseType1)));
      Assert.That (instance, Is.InstanceOf (TypeFactory.GetConcreteType (typeof (BaseType1))));
    }

    [Test]
    public void CreateInstance_UsesTargetType_ForUnmixedType ()
    {
      object instance = _implementation.CreateInstance (false, typeof (object), ParamList.Empty);
      Assert.That (instance.GetType(), Is.SameAs (typeof (object)));
    }

    [Test]
    public void CreateInstance_PassesPreparedMixinInstances ()
    {
      var mixin = new BT1Mixin1();
      object instance = _implementation.CreateInstance (false, typeof (BaseType1), ParamList.Empty, mixin);

      Assert.That (Mixin.Get<BT1Mixin1> (instance), Is.SameAs (mixin));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void CreateInstance_InvalidPreparedMixinInstances ()
    {
      var mixin = new object ();
      _implementation.CreateInstance (false, typeof (BaseType1), ParamList.Empty, mixin);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin " 
        + "instances must be specified.\r\nParameter name: preparedMixins")]
    public void CreateInstance_InvalidPreparedMixinInstances_UnmixedObject ()
    {
      var mixin = new object ();
      _implementation.CreateInstance (false, typeof (object), ParamList.Empty, false, mixin);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type 'System.IServiceProvider', it's an interface.\r\nParameter name: targetOrConcreteType")]
    public void CreateInstance_Interface ()
    {
      _implementation.CreateInstance (false, typeof (IServiceProvider), ParamList.Empty, false);
    }

    public class TestServiceProvider : IServiceProvider
    {
      public object GetService (Type serviceType) { throw new NotImplementedException(); }
    }
  }
}
