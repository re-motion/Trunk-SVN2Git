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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class MixinSerializationTest : CodeGenerationBaseTest
  {
    [Test]
    public void SerializationOfMixinThisWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2), typeof (BT3Mixin2B));
      var mixin = Mixin.Get<BT3Mixin2> (bt3);
      Assert.AreSame (bt3, mixin.This);

      var mixin2 = Mixin.Get<BT3Mixin2B> (bt3);
      Assert.AreSame (bt3, mixin2.This);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      var mixinA = Mixin.Get<BT3Mixin2> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.AreSame (bt3A, mixinA.This);

      var mixin2A = Mixin.Get<BT3Mixin2B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.AreSame (bt3A, mixin2A.This);
    }

    [Test]
    public void SerializationOfMixinBaseWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin1), typeof (BT3Mixin1B));
      var mixin = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin.Base);
      Assert.AreSame (bt3.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType, mixin.Base.GetType ());

      var mixin2 = Mixin.Get<BT3Mixin1B> (bt3);
      Assert.IsNotNull (mixin2.Base);
      Assert.AreSame (bt3.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType, mixin2.Base.GetType ());

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      var mixinA = Mixin.Get<BT3Mixin1> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.IsNotNull (mixinA.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType, mixinA.Base.GetType ());

      var mixin2A = Mixin.Get<BT3Mixin1B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.IsNotNull (mixin2A.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType, mixin2A.Base.GetType ());
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);
      Assert.IsTrue (mixin.GetType ().IsSerializable);
      Serializer.Serialize (targetInstance);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);

      mixin.I = 13;

      MixinWithAbstractMembers mixinA = Serializer.SerializeAndDeserialize (mixin);
      Assert.AreEqual (mixin.I, mixinA.I);
      Assert.AreNotSame (mixin, mixinA);
    }

    [Test]
    public void GeneratedTypeCorrectlySerializesThisAndBase()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers> (targetInstance);

      Assert.AreEqual (targetInstance, MixinReflector.GetTargetProperty (mixin.GetType()).GetValue (mixin, null));
      Assert.AreEqual (MixinReflector.GetBaseCallProxyType(targetInstance),
          MixinReflector.GetBaseProperty (mixin.GetType ()).GetValue (mixin, null).GetType());

      ClassOverridingMixinMembers targetInstanceA = Serializer.SerializeAndDeserialize (targetInstance);
      var mixinA = Mixin.Get<MixinWithAbstractMembers> (targetInstanceA);

      Assert.AreEqual (targetInstanceA, MixinReflector.GetTargetProperty (mixinA.GetType ()).GetValue (mixinA, null));
      Assert.AreEqual (MixinReflector.GetBaseCallProxyType (targetInstanceA),
          MixinReflector.GetBaseProperty (mixinA.GetType ()).GetValue (mixinA, null).GetType ());
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers> (typeof (AbstractMixinImplementingISerializable));
      var mixin = Mixin.Get<AbstractMixinImplementingISerializable> (targetInstance);

      mixin.I = 15;
      Assert.AreEqual (15, mixin.I);

      AbstractMixinImplementingISerializable mixinA = Serializer.SerializeAndDeserialize (mixin);
      Assert.AreEqual (32, mixinA.I);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "is not marked as serializable", MatchType = MessageMatch.Contains)]
    public void ThrowsIfAbstractMixinTypeNotSerializable()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers> (typeof (NotSerializableMixin));

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void AllowsAbstractMixinTypeNotSerializableWithISerializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (NotSerializableMixinWithISerializable));
      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void SerializationOfGeneratedMixinWorks ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod> (typeof (MixinOverridingClassMethod));
      var comAsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> (com));

      Assert.IsNotNull (comAsIfc);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", comAsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));

      ClassOverridingSingleMixinMethod com2 = Serializer.SerializeAndDeserialize (com);
      var com2AsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> (com2));
      Assert.AreNotSame (Mixin.Get<MixinOverridingClassMethod> (com), Mixin.Get<MixinOverridingClassMethod> (com2));

      Assert.IsNotNull (com2AsIfc);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", com2AsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com2.OverridableMethod (13));
    }
  }
}
