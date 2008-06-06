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

    [Serializable]
    [Uses (typeof (MixinWithSerializationCallbacks))]
    public class TargetTypeWithSerializationCallbacks : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetStaticReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }
    }

    [Serializable]
    public class MixinWithSerializationCallbacks : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetStaticReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }
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

    [Serializable]
    public abstract class AbstractMixinWithSerializationCallbacks : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetStaticReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }

      [OverrideTarget]
      protected new string ToString () // protected overrider to force class to be derived
      {
        return "";
      }
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
