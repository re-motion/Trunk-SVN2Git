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
using System.Runtime.Serialization;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.UnitTesting
{
  [TestFixture]
  public class SerializationCallbackTesterTest
  {
    [Serializable]
    class OrdinaryClass : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }
    }

    [Test]
    public void TestSerializationCallbacks_ViaOrdinaryInstance ()
    {
      new SerializationCallbackTester<OrdinaryClass> (new RhinoMocksRepositoryAdapter(), new OrdinaryClass(), OrdinaryClass.SetReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void TestDeserializationCallbacks_ViaOrdinaryInstance ()
    {
      new SerializationCallbackTester<OrdinaryClass> (new RhinoMocksRepositoryAdapter (), new OrdinaryClass (), OrdinaryClass.SetReceiver)
          .Test_DeserializationCallbacks ();
    }

    [Serializable]
    class BrokenClass : ClassWithSerializationCallbacksBase, IDeserializationCallback
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }

      void IDeserializationCallback.OnDeserialization (object sender)
      {
        // suppresses receiver event
      }
    }

    [Test]
    [ExpectedException (typeof (ExpectationViolationException), ExpectedMessage = "ISerializationEventReceiver.OnDeserialization(any); Expected #1, Actual #0.")]
    public void TestDeserializationCallbacks_ViaBrokenInstance ()
    {
      new SerializationCallbackTester<BrokenClass> (new RhinoMocksRepositoryAdapter (), new BrokenClass (), BrokenClass.SetReceiver)
          .Test_DeserializationCallbacks ();
    }
  }
}
