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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class SerializationCallbackTest
  {
    private TargetTypeWithSerializationCallbacks _instance;

    [SetUp]
    public void SetUp ()
    {
      _instance = ObjectFactory.Create<TargetTypeWithSerializationCallbacks> ().With ();
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnTargetClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter(), _instance, TargetTypeWithSerializationCallbacks.SetStaticReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnTargetClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), _instance, TargetTypeWithSerializationCallbacks.SetStaticReceiver)
          .Test_DeserializationCallbacks ();
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnMixinClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), _instance, MixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnMixinClass ()
    {
      new SerializationCallbackTester<TargetTypeWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), _instance, MixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_DeserializationCallbacks ();
    }

    [Uses (typeof (AbstractMixinWithSerializationCallbacks))]
    [Serializable]
    public class TargetClassForAbstractMixinWithSerializationCallbacks
    {
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnGeneratedMixinClass ()
    {
      TargetClassForAbstractMixinWithSerializationCallbacks instance =
          ObjectFactory.Create<TargetClassForAbstractMixinWithSerializationCallbacks> ().With ();
      new SerializationCallbackTester<TargetClassForAbstractMixinWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, AbstractMixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnGeneratedMixinClass ()
    {
      TargetClassForAbstractMixinWithSerializationCallbacks instance =
          ObjectFactory.Create<TargetClassForAbstractMixinWithSerializationCallbacks> ().With ();
      new SerializationCallbackTester<TargetClassForAbstractMixinWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, AbstractMixinWithSerializationCallbacks.SetStaticReceiver)
          .Test_DeserializationCallbacks ();
    }
  }
}
