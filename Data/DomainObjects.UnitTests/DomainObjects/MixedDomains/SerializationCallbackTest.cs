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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.MixedDomains
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Serializable]
    public class MixinWithSerializationCallbacks : ClassWithSerializationCallbacksBase
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
    public void SerializationEvents_OnTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter(), instance, ClassWithSerializationCallbacks.SetReceiver)
            .Test_SerializationCallbacks();
      }
    }

    [Test]
    public void DeserializationEvents_OnTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
            .Test_DeserializationCallbacks();
      }
    }

    [Test]
    public void SerializationEvents_OnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, MixinWithSerializationCallbacks.SetReceiver)
            .Test_SerializationCallbacks ();
      }
    }

    [Test]
    public void DeserializationEvents_OnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, MixinWithSerializationCallbacks.SetReceiver)
            .Test_DeserializationCallbacks ();
      }
    }
  }
}
