// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.TestDomain;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
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
      var classContextField = t.GetField ("__classContext", BindingFlags.NonPublic | BindingFlags.Static);
      Assert.That (classContextField, Is.Not.Null);
      Assert.That (classContextField.IsStatic, Is.True);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);

      var classContextField = bt1.GetType().GetField ("__classContext", BindingFlags.NonPublic | BindingFlags.Static);
      Assert.That (classContextField.GetValue (null), Is.Not.Null);

      var expectedClassContext = TargetClassDefinitionUtility.GetContext (
          typeof (BaseType1), 
          MixinConfiguration.ActiveConfiguration, 
          GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.That (classContextField.GetValue (null), Is.EqualTo (expectedClassContext));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      Assert.That (bt1.GetType ().IsSerializable, Is.True);

      bt1.I = 25;
      Serializer.Serialize (bt1);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      Assert.That (bt1.GetType ().IsSerializable, Is.True);

      bt1.I = 25;
      Serializer.SerializeAndDeserialize (bt1);
      Assert.That (bt1.I, Is.EqualTo (25));
    }

    [Test]
    public void GeneratedTypeWithReferenceToMixinBaseIsDeserializable ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<OverridableBaseType> ().Clear().AddMixins (typeof (MixinOverridingClassMethod)).EnterScope())
      {
        var instance = ObjectFactory.Create<OverridableBaseType> (ParamList.Empty);
        Assert.That (instance.GetType ().IsSerializable, Is.True);

        Assert.That (instance.OverridableMethod (85), Is.EqualTo ("MixinOverridingClassMethod.OverridableMethod-85"));

        OverridableBaseType deserialiedInstance = Serializer.SerializeAndDeserialize (instance);

        Assert.That (deserialiedInstance.OverridableMethod (85), Is.EqualTo ("MixinOverridingClassMethod.OverridableMethod-85"));
        Assert.That (Mixin.Get<MixinOverridingClassMethod> (deserialiedInstance).This, Is.SameAs (deserialiedInstance));

        Assert.That (Mixin.Get<MixinOverridingClassMethod> (deserialiedInstance).Base, Is.Not.Null);
        Assert.That (
                      ((MixinOverridingClassMethod.IRequirements) Mixin.Get<MixinOverridingClassMethod> (deserialiedInstance).Base).OverridableMethod (84), Is.EqualTo ("OverridableBaseType.OverridableMethod(84)"));
      }
    }

    [Test]
    public void DeserializedMembersFit ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      Assert.That (bt1.GetType ().IsSerializable, Is.True);

      bt1.I = 25;
      BaseType1 bt1a = Serializer.SerializeAndDeserialize (bt1);
      Assert.That (bt1a, Is.Not.SameAs (bt1));
      Assert.That (bt1a.I, Is.EqualTo (bt1.I));

      var bt2 = CreateMixedObject<BaseType2> (typeof(BT2Mixin1));
      Assert.That (bt2.GetType ().IsSerializable, Is.True);

      bt2.S = "Bla";
      BaseType2 bt2a = Serializer.SerializeAndDeserialize (bt2);
      Assert.That (bt2a, Is.Not.SameAs (bt2));
      Assert.That (bt2a.S, Is.EqualTo (bt2.S));
    }

    [Test]
    public void ExtensionsAndConfigurationSerialized ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      var mixinTarget = (IMixinTarget) bt1;

      BaseType1 deserializedBT1 = Serializer.SerializeAndDeserialize (bt1);
      var deserializedMixinTarget = (IMixinTarget) deserializedBT1;

      Assert.That (deserializedMixinTarget.ClassContext, Is.EqualTo (mixinTarget.ClassContext));

      Assert.That (deserializedMixinTarget.Mixins, Is.Not.Null);
      Assert.That (deserializedMixinTarget.Mixins.Length, Is.EqualTo (mixinTarget.Mixins.Length));
      Assert.That (deserializedMixinTarget.Mixins[0].GetType (), Is.EqualTo (mixinTarget.Mixins[0].GetType ()));

      Assert.That (deserializedMixinTarget.FirstBaseCallProxy, Is.Not.Null);
      Assert.That (deserializedMixinTarget.FirstBaseCallProxy, Is.Not.EqualTo (mixinTarget.FirstBaseCallProxy));
      Assert.That (deserializedMixinTarget.FirstBaseCallProxy.GetType (), Is.EqualTo (deserializedMixinTarget.GetType ().GetNestedType ("BaseCallProxy")));
      Assert.That (deserializedMixinTarget.FirstBaseCallProxy.GetType ().GetField ("__depth").GetValue (deserializedMixinTarget.FirstBaseCallProxy), Is.EqualTo (0));
      Assert.That (deserializedMixinTarget.FirstBaseCallProxy.GetType ().GetField ("__this").GetValue (deserializedMixinTarget.FirstBaseCallProxy), Is.SameAs (deserializedMixinTarget));
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassImplementingISerializable c = ObjectFactory.Create<ClassImplementingISerializable> (ParamList.Empty);
      Assert.That (c.GetType (), Is.Not.EqualTo (typeof (ClassImplementingISerializable)));

      c.I = 15;
      Assert.That (c.I, Is.EqualTo (15));
      
      ClassImplementingISerializable c2 = Serializer.SerializeAndDeserialize (c);
      Assert.That (c2, Is.Not.EqualTo (c));
      Assert.That (c2.I, Is.EqualTo (28));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "is not marked as serializable", MatchType = MessageMatch.Contains)]
    public void ThrowsIfClassNotSerializable ()
    {
      NotSerializableClass targetInstance = CreateMixedObject<NotSerializableClass> ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void AllowsClassNotSerializableWithISerializable ()
    {
      NotSerializableClassWithISerializable targetInstance = CreateMixedObject<NotSerializableClassWithISerializable> ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void WorksIfNoDefaultCtor ()
    {
      ClassWithoutDefaultCtor c = ObjectFactory.Create<ClassWithoutDefaultCtor> (ParamList.Create (35));
      Assert.That (c.GetType (), Is.Not.EqualTo (typeof (ClassImplementingISerializable)));

      Assert.That (c.S, Is.EqualTo ("35"));

      ClassWithoutDefaultCtor c2 = Serializer.SerializeAndDeserialize (c);
      Assert.That (c2, Is.Not.EqualTo (c));
      Assert.That (c2.S, Is.EqualTo ("35"));
    }

    [Test]
    public void OnInitializedNotCalledOnDeserialization ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithOnInitializedAndOnDeserialized)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        Assert.That (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (instance).OnInitializedCalled, Is.True);

        NullTarget deserializedInstance = Serializer.SerializeAndDeserialize (instance);
        Assert.That (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (deserializedInstance).OnInitializedCalled, Is.False);
      }
    }

    [Test]
    public void OnDeserializedCalledOnDeserialization ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithOnInitializedAndOnDeserialized)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        Assert.That (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (instance).OnDeserializedCalled, Is.False);

        NullTarget deserializedInstance = Serializer.SerializeAndDeserialize (instance);
        Assert.That (Mixin.Get<MixinWithOnInitializedAndOnDeserialized> (deserializedInstance).OnDeserializedCalled, Is.True);
      }
    }

    [Test]
    public void MixinConfigurationCanDifferAtDeserializationTime ()
    {
      byte[] serializedData;
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        Assert.That (Mixin.Get<NullMixin> (instance), Is.Not.Null);
        serializedData = Serializer.Serialize (instance);
      }

      var deserializedInstance = (NullTarget) Serializer.Deserialize (serializedData);
      Assert.That (Mixin.Get<NullMixin> (deserializedInstance), Is.Not.Null);
    }
  }
}
