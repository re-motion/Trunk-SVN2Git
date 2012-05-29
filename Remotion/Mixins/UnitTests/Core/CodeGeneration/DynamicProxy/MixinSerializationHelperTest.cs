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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class MixinSerializationHelperTest
  {
    private SerializationInfo _serializationInfo;
    private StreamingContext _context;
    private FakeConcreteMixinType _concreteMixin;
    private ConcreteMixinTypeIdentifier _identifier;

    [SetUp]
    public void SetUp ()
    {
      _serializationInfo = new SerializationInfo (typeof (object), new FormatterConverter ());
      _context = new StreamingContext ();
      _concreteMixin = new FakeConcreteMixinType ();
      var classContext = ClassContextObjectMother.Create(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      _identifier = DefinitionObjectMother.GetTargetClassDefinition (classContext).Mixins[0].GetConcreteMixinTypeIdentifier ();
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SetsType ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);

      Assert.That (_serializationInfo.FullTypeName, Is.EqualTo (typeof (MixinSerializationHelper).FullName));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializesIdentifier ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);
      var identifierDeserializer = new SerializationInfoConcreteMixinTypeIdentifierDeserializer (_serializationInfo, "__identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifier.Deserialize (identifierDeserializer);
      Assert.That (deserializedIdentifier, Is.EqualTo (_identifier));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializeBaseMembers_True ()
    {
      _concreteMixin.I = 7431;
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);
      Assert.That (_serializationInfo.GetValue ("__baseMemberValues", typeof (object[])), Has.Member(7431));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializeBaseMembers_False ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, false);
      Assert.That (_serializationInfo.GetValue ("__baseMemberValues", typeof (object[])), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "TypeTransformer returned type 'System.Object', which is not compatible "
        + "with the serialized mixin configuration. The configuration requires a type assignable to "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.MixinWithAbstractMembers'.")]
    public void Initialization_WithTypeTransformer_InvalidTransformedType_NonAssignable ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);
      new MixinSerializationHelper (_serializationInfo, _context, t => typeof (object));
    }

    [Test]
    public void Initialization_SignalsOnDeserializing ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);

      var helper = new MixinSerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixinType));
      var deserializedObject = (FakeConcreteMixinType) helper.GetRealObject (_context);

      Assert.That (deserializedObject.OnDeserializingCalled, Is.True);
    }

    [Test]
    public void Initialization_BaseMembersSerialized_CallsNoCtor ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);

      var helper = new MixinSerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixinType));
      var deserializedObject = (FakeConcreteMixinType) helper.GetRealObject (_context);

      Assert.That (deserializedObject.CtorCalled, Is.False);
    }

    [Test]
    public void Initialization_BaseMembersNotSerialized_CallsSerializationCtor ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, false);

      var helper = new MixinSerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixinType));
      var deserializedObject = (FakeConcreteMixinType) helper.GetRealObject (_context);

      Assert.That (deserializedObject.CtorCalled, Is.True);
      Assert.That (deserializedObject.SerializationCtorCalled, Is.True);
    }

    [Test]
    public void GetRealObject_ReturnsConcreteMixinTypeInstance ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);

      var helper = new MixinSerializationHelper (_serializationInfo, _context);

      var realObject = helper.GetRealObject (_context);
      Assert.That (realObject.GetType (), Is.SameAs (ConcreteTypeBuilder.Current.GetConcreteMixinType (_identifier).GeneratedType));
    }

    [Test]
    public void GetRealObject_WithTypeTransformer ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);

      var helper = new MixinSerializationHelper (
          _serializationInfo,
          _context,
          delegate (Type t)
          {
            Assert.That (t, Is.SameAs (ConcreteTypeBuilder.Current.GetConcreteMixinType (_identifier).GeneratedType));
            return typeof (FakeConcreteMixinType);
          });

      var realObject = helper.GetRealObject (_context);
      Assert.That (realObject.GetType (), Is.SameAs (typeof (FakeConcreteMixinType)));
    }

    [Test]
    public void OnDeserialization_RestoresBaseMembers ()
    {
      _concreteMixin.I = 4711;
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);
      var helper = new MixinSerializationHelper (_serializationInfo, _context);

      var deserializedObject = (MixinWithAbstractMembers) helper.GetRealObject (_context);
      Assert.That (deserializedObject.I, Is.EqualTo (0));

      helper.OnDeserialization (null);
      Assert.That (deserializedObject.I, Is.EqualTo (4711));
    }

    [Test]
    public void OnDeserialization_RestoresBaseMembers_NotWhenNoBaseMembers ()
    {
      _concreteMixin.I = 4711;
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, false);
      var helper = new MixinSerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixinType));

      var deserializedObject = (MixinWithAbstractMembers) helper.GetRealObject (_context);
      Assert.That (deserializedObject.I, Is.EqualTo (0));

      helper.OnDeserialization (null);
      Assert.That (deserializedObject.I, Is.EqualTo (0));
    }

    [Test]
    public void OnDeserialization_RaisesEvents ()
    {
      MixinSerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteMixin, _identifier, true);
      var helper = new MixinSerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixinType));

      var deserializedObject = (FakeConcreteMixinType) helper.GetRealObject (_context);
      Assert.That (deserializedObject.OnDeserializedCalled, Is.False);
      Assert.That (deserializedObject.OnDeserializationCalled, Is.False);

      helper.OnDeserialization (null);

      Assert.That (deserializedObject.OnDeserializedCalled, Is.True);
      Assert.That (deserializedObject.OnDeserializationCalled, Is.True);
    }
  }
}
