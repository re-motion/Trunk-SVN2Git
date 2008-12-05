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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class SerializationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeHasConfigurationField ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.IsNotNull (t.GetField ("__configuration", BindingFlags.NonPublic | BindingFlags.Static));
      Assert.IsTrue (t.GetField ("__configuration", BindingFlags.NonPublic | BindingFlags.Static).IsStatic);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.IsNotNull (bt1.GetType ().GetField ("__configuration", BindingFlags.NonPublic | BindingFlags.Static));
      Assert.AreSame (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)), bt1.GetType ().GetField ("__configuration", BindingFlags.NonPublic | BindingFlags.Static).GetValue (bt1));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      Serializer.Serialize (bt1);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      Serializer.SerializeAndDeserialize (bt1);
      Assert.AreEqual (25, bt1.I);
    }

    [Test]
    public void GeneratedTypeWithReferenceToMixinBaseIsDeserializable ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<OverridableBaseType> ().Clear().AddMixins (typeof (MixinOverridingClassMethod)).EnterScope())
      {
        OverridableBaseType instance = ObjectFactory.Create<OverridableBaseType> ().With ();
        Assert.IsTrue (instance.GetType().IsSerializable);

        Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-85", instance.OverridableMethod (85));

        OverridableBaseType deserialiedInstance = Serializer.SerializeAndDeserialize (instance);

        Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-85", deserialiedInstance.OverridableMethod (85));
        Assert.AreSame (deserialiedInstance, Mixin.Get<MixinOverridingClassMethod> (deserialiedInstance).This);

        Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> (deserialiedInstance).Base);
        Assert.AreEqual ("OverridableBaseType.OverridableMethod(84)",
            ((MixinOverridingClassMethod.IRequirements) Mixin.Get<MixinOverridingClassMethod> (deserialiedInstance).Base).OverridableMethod(84));
      }
    }

    [Test]
    public void DeserializedMembersFit ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      BaseType1 bt1a = Serializer.SerializeAndDeserialize (bt1);
      Assert.AreNotSame (bt1, bt1a);
      Assert.AreEqual (bt1.I, bt1a.I);

      BaseType2 bt2 = CreateMixedObject<BaseType2> (typeof(BT2Mixin1)).With();
      Assert.IsTrue (bt2.GetType ().IsSerializable);

      bt2.S = "Bla";
      BaseType2 bt2a = Serializer.SerializeAndDeserialize (bt2);
      Assert.AreNotSame (bt2, bt2a);
      Assert.AreEqual (bt2.S, bt2a.S);
    }

    [Test]
    public void ExtensionsAndConfigurationSerializedFirstFits ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      var mixinTarget = (IMixinTarget) bt1;

      BaseType1 bt1a = Serializer.SerializeAndDeserialize (bt1);
      var mixinTargetA = (IMixinTarget) bt1a;

      Assert.IsNotNull (mixinTargetA.Configuration);
      Assert.AreEqual (mixinTarget.Configuration.Type, mixinTargetA.Configuration.Type);

      Assert.IsNotNull (mixinTargetA.Mixins);
      Assert.AreEqual (mixinTarget.Mixins.Length, mixinTargetA.Mixins.Length);
      Assert.AreEqual (mixinTarget.Mixins[0].GetType(), mixinTargetA.Mixins[0].GetType());

      Assert.IsNotNull (mixinTargetA.FirstBaseCallProxy);
      Assert.AreNotEqual (mixinTarget.FirstBaseCallProxy, mixinTargetA.FirstBaseCallProxy);
      Assert.AreEqual (mixinTargetA.GetType ().GetNestedType ("BaseCallProxy"), mixinTargetA.FirstBaseCallProxy.GetType ());
      Assert.AreEqual (0, mixinTargetA.FirstBaseCallProxy.GetType ().GetField ("__depth").GetValue (mixinTargetA.FirstBaseCallProxy));
      Assert.AreSame (mixinTargetA, mixinTargetA.FirstBaseCallProxy.GetType ().GetField ("__this").GetValue(mixinTargetA.FirstBaseCallProxy));
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassImplementingISerializable c = ObjectFactory.Create<ClassImplementingISerializable> ().With();
      Assert.AreNotEqual (typeof (ClassImplementingISerializable), c.GetType ());

      c.I = 15;
      Assert.AreEqual (15, c.I);
      
      ClassImplementingISerializable c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreNotEqual (c, c2);
      Assert.AreEqual (28, c2.I);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "is not marked as serializable", MatchType = MessageMatch.Contains)]
    public void ThrowsIfClassNotSerializable ()
    {
      NotSerializableClass targetInstance = CreateMixedObject<NotSerializableClass> ().With ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void AllowsClassNotSerializableWithISerializable ()
    {
      NotSerializableClassWithISerializable targetInstance = CreateMixedObject<NotSerializableClassWithISerializable> ().With ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void WorksIfNoDefaultCtor ()
    {
      ClassWithoutDefaultCtor c = ObjectFactory.Create<ClassWithoutDefaultCtor> ().With (35);
      Assert.AreNotEqual (typeof (ClassImplementingISerializable), c.GetType ());

      Assert.AreEqual ("35", c.S);

      ClassWithoutDefaultCtor c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreNotEqual (c, c2);
      Assert.AreEqual ("35", c2.S);
    }

    [Test]
    public void OnInitializedNotCalledOnDeserialization ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithOnInitializedAndOnDeserialized)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget> ().With ();
        Assert.IsTrue (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (instance).OnInitializedCalled);

        NullTarget deserializedInstance = Serializer.SerializeAndDeserialize (instance);
        Assert.IsFalse (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (deserializedInstance).OnInitializedCalled);
      }
    }

    [Test]
    public void OnDeserializedCalledOnDeserialization ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithOnInitializedAndOnDeserialized)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget> ().With ();
        Assert.IsFalse (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (instance).OnDeserializedCalled);

        NullTarget deserializedInstance = Serializer.SerializeAndDeserialize (instance);
        Assert.IsTrue (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (deserializedInstance).OnDeserializedCalled);
      }
    }

    [Test]
    public void MixinConfigurationCanDifferAtDeserializationTime ()
    {
      byte[] serializedData;
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget> ().With ();
        Assert.IsNotNull (Mixin.Get<NullMixin> (instance));
        serializedData = Serializer.Serialize (instance);
      }

      var deserializedInstance = (NullTarget) Serializer.Deserialize (serializedData);
      Assert.IsNotNull (Mixin.Get<NullMixin> (deserializedInstance));
    }
  }
}
