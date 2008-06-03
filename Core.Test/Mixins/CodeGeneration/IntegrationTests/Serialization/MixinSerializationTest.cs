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
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class MixinSerializationTest : CodeGenerationBaseTest
  {
    [Test]
    public void SerializationOfMixinThisWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2), typeof (BT3Mixin2B)).With ();
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (bt3);
      Assert.AreSame (bt3, mixin.This);

      BT3Mixin2B mixin2 = Mixin.Get<BT3Mixin2B> (bt3);
      Assert.AreSame (bt3, mixin2.This);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin2 mixinA = Mixin.Get<BT3Mixin2> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.AreSame (bt3A, mixinA.This);

      BT3Mixin2B mixin2A = Mixin.Get<BT3Mixin2B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.AreSame (bt3A, mixin2A.This);
    }

    [Test]
    public void SerializationOfMixinBaseWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin1), typeof (BT3Mixin1B)).With ();
      BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin.Base);
      Assert.AreSame (bt3.GetType ().GetField ("__first").FieldType, mixin.Base.GetType ());

      BT3Mixin1B mixin2 = Mixin.Get<BT3Mixin1B> (bt3);
      Assert.IsNotNull (mixin2.Base);
      Assert.AreSame (bt3.GetType ().GetField ("__first").FieldType, mixin2.Base.GetType ());

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin1 mixinA = Mixin.Get<BT3Mixin1> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.IsNotNull (mixinA.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first").FieldType, mixinA.Base.GetType ());

      BT3Mixin1B mixin2A = Mixin.Get<BT3Mixin1B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.IsNotNull (mixin2A.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first").FieldType, mixin2A.Base.GetType ());
    }

    [Test]
    public void GeneratedTypeHasConfigurationField ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();

      Type t = Mixin.Get<MixinWithAbstractMembers> (targetInstance).GetType();
      Assert.IsNotNull (t.GetField ("__configuration"));
      Assert.IsTrue (t.GetField ("__configuration").IsStatic);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      MixinWithAbstractMembers mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);

      Assert.IsNotNull (mixin.GetType ().GetField ("__configuration"));
      Assert.AreSame (((IMixinTarget)targetInstance).Configuration.Mixins[typeof(MixinWithAbstractMembers)],
          mixin.GetType ().GetField ("__configuration").GetValue (mixin));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      MixinWithAbstractMembers mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);
      Assert.IsTrue (mixin.GetType ().IsSerializable);
      Serializer.Serialize ((object) targetInstance);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      MixinWithAbstractMembers mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);

      mixin.I = 13;

      MixinWithAbstractMembers mixinA = Serializer.SerializeAndDeserialize (mixin);
      Assert.AreEqual (mixin.I, mixinA.I);
      Assert.AreNotSame (mixin, mixinA);
    }

    [Test]
    public void GeneratedTypeCorrectlySerializesThisAndBase()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      MixinWithAbstractMembers mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);

      Assert.AreEqual (targetInstance, MixinReflector.GetTargetProperty (mixin.GetType()).GetValue (mixin, null));
      Assert.AreEqual (MixinReflector.GetBaseCallProxyType(targetInstance),
          MixinReflector.GetBaseProperty (mixin.GetType ()).GetValue (mixin, null).GetType());

      ClassOverridingMixinMembers targetInstanceA = Serializer.SerializeAndDeserialize (targetInstance);
      MixinWithAbstractMembers mixinA = Mixin.Get<MixinWithAbstractMembers> (targetInstanceA);

      Assert.AreEqual (targetInstanceA, MixinReflector.GetTargetProperty (mixinA.GetType ()).GetValue (mixinA, null));
      Assert.AreEqual (MixinReflector.GetBaseCallProxyType (targetInstanceA),
          MixinReflector.GetBaseProperty (mixinA.GetType ()).GetValue (mixinA, null).GetType ());
    }

    [Serializable]
    public abstract class AbstractMixinImplementingISerializable : MixinWithAbstractMembers, ISerializable
    {
      public AbstractMixinImplementingISerializable ()
      {
      }

      public AbstractMixinImplementingISerializable (SerializationInfo info, StreamingContext context)
      {
        I = info.GetInt32 ("I") + 13;
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        info.AddValue ("I", I + 4);
      }
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers> (typeof (AbstractMixinImplementingISerializable)).With ();
      AbstractMixinImplementingISerializable mixin = Mixin.Get<AbstractMixinImplementingISerializable> (targetInstance);

      mixin.I = 15;
      Assert.AreEqual (15, mixin.I);

      AbstractMixinImplementingISerializable mixinA = Serializer.SerializeAndDeserialize (mixin);
      Assert.AreEqual (32, mixinA.I);
    }

    public abstract class NotSerializableMixin : MixinWithAbstractMembers
    {
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "is not marked as serializable", MatchType = MessageMatch.Contains)]
    public void ThrowsIfAbstractMixinTypeNotSerializable()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers> (typeof (NotSerializableMixin)).With ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    public abstract class NotSerializableMixinWithISerializable : MixinWithAbstractMembers, ISerializable
    {
      public NotSerializableMixinWithISerializable ()
      {
      }

      public NotSerializableMixinWithISerializable (SerializationInfo info, StreamingContext context)
      {
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
      }
    }

    [Test]
    public void AllowsAbstractMixinTypeNotSerializableWithISerializable ()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers> (typeof (NotSerializableMixinWithISerializable)).With ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void SerializationOfGeneratedMixinWorks ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod> (typeof (MixinOverridingClassMethod)).With ();
      IMixinOverridingClassMethod comAsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> ((object) com));

      Assert.IsNotNull (comAsIfc);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", comAsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));

      ClassOverridingSingleMixinMethod com2 = Serializer.SerializeAndDeserialize (com);
      IMixinOverridingClassMethod com2AsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> ((object) com2));
      Assert.AreNotSame (Mixin.Get<MixinOverridingClassMethod> ((object) com),
          Mixin.Get<MixinOverridingClassMethod> ((object) com2));

      Assert.IsNotNull (com2AsIfc);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", com2AsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com2.OverridableMethod (13));
    }
  }
}
