// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class SerializationHelperTest
  {
    private SerializationInfo _serializationInfo;
    private StreamingContext _context;
    private BaseType1 _concreteObject;
    private ClassContext _classContext;
    private object[] _extensions;

    [SetUp]
    public void SetUp ()
    {
      _serializationInfo = new SerializationInfo (typeof (object), new FormatterConverter ());
      _context = new StreamingContext ();
      _concreteObject = new BaseType1();
      _classContext = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1));
      _extensions = new object[] { new BT1Mixin1 () };
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SetsType ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, false);
      Assert.That (_serializationInfo.FullTypeName, Is.EqualTo (typeof (SerializationHelper).FullName));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializesClassContext ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, false);
      var classContextDeserializer = new SerializationInfoClassContextDeserializer (_serializationInfo, "__configuration.ConfigurationContext");

      var deserializedClassContext = ClassContext.Deserialize (classContextDeserializer);
      Assert.That (deserializedClassContext, Is.EqualTo (_classContext));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_AddsExtensions ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, false);
      Assert.That (_serializationInfo.GetValue ("__extensions", typeof (object[])), Is.SameAs (_extensions));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializeBaseMembers_True ()
    {
      _concreteObject.I = 7431;
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);
      Assert.That (_serializationInfo.GetValue ("__baseMemberValues", typeof (object[])), List.Contains (7431));
    }

    [Test]
    public void GetObjectDataForGeneratedTypes_SerializeBaseMembers_False ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, false);
      Assert.That (_serializationInfo.GetValue ("__baseMemberValues", typeof (object[])), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "TypeTransformer returned type 'System.Object', which is not compatible "
        + "with the serialized mixin configuration. The configuration requires a type assignable to 'Remotion.UnitTests.Mixins.SampleTypes.BaseType1'.")]
    public void Initialization_WithTypeTransformer_InvalidTransformedType_NonAssignable ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);

      new SerializationHelper (_serializationInfo, _context, t => typeof (object));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "TypeTransformer returned type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.BaseType1', which does not implement IMixinTarget.")]
    public void Initialization_WithTypeTransformer_InvalidTransformedType_NonIMixinTarget ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);

      new SerializationHelper (_serializationInfo, _context, t => typeof (BaseType1));
    }

    [Test]
    public void Initialization_SignalsOnDeserializing ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);

      var helper = new SerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixedType));
      var deserializedObject = (FakeConcreteMixedType) helper.GetRealObject (_context);

      Assert.That (deserializedObject.OnDeserializingCalled, Is.True);
    }

    [Test]
    public void Initialization_BaseMembersSerialized_CallsNoCtor ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);

      var helper = new SerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixedType));
      var deserializedObject = (FakeConcreteMixedType) helper.GetRealObject (_context);

      Assert.That (deserializedObject.CtorCalled, Is.False);
    }

    [Test]
    public void Initialization_BaseMembersNotSerialized_CallsSerializationCtor ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, false);

      var helper = new SerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixedType));
      var deserializedObject = (FakeConcreteMixedType) helper.GetRealObject (_context);

      Assert.That (deserializedObject.CtorCalled, Is.True);
      Assert.That (deserializedObject.SerializationCtorCalled, Is.True);
    }

    [Test]
    public void GetRealObject_ReturnsConcreteTypeInstance ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);
      
      var helper = new SerializationHelper (_serializationInfo, _context);

      var realObject = helper.GetRealObject (_context);
      Assert.That (realObject.GetType (), Is.SameAs (ConcreteTypeBuilder.Current.GetConcreteType (_classContext)));
    }

    [Test]
    public void GetRealObject_WithTypeTransformer ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);

      var helper = new SerializationHelper (
          _serializationInfo, 
          _context, 
          delegate (Type t) 
          { 
              Assert.That (t, Is.SameAs (ConcreteTypeBuilder.Current.GetConcreteType (_classContext))); 
              return typeof (FakeConcreteMixedType); 
          });

      var realObject = helper.GetRealObject (_context);
      Assert.That (realObject.GetType (), Is.SameAs (typeof (FakeConcreteMixedType)));
    }

    [Test]
    public void OnDeserialization_RestoresBaseMembers ()
    {
      _concreteObject.I = 4711;
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);
      var helper = new SerializationHelper (_serializationInfo, _context);
      
      var deserializedObject = (BaseType1) helper.GetRealObject (_context);
      Assert.That (deserializedObject.I, Is.EqualTo (0));

      helper.OnDeserialization (null);
      Assert.That (deserializedObject.I, Is.EqualTo (4711));
    }

    [Test]
    public void OnDeserialization_RestoresBaseMembers_NotWhenNoBaseMembers ()
    {
      _concreteObject.I = 4711;
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, false);
      var helper = new SerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixedType));

      var deserializedObject = (BaseType1) helper.GetRealObject (_context);
      Assert.That (deserializedObject.I, Is.EqualTo (0));

      helper.OnDeserialization (null);
      Assert.That (deserializedObject.I, Is.EqualTo (0));
    }

    [Test]
    public void OnDeserialization_RestoresExtensions ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);
      var helper = new SerializationHelper (_serializationInfo, _context);
      helper.OnDeserialization (null);

      Assert.That (((IMixinTarget) helper.GetRealObject (_context)).Mixins, Is.SameAs (_extensions));
    }

    [Test]
    public void OnDeserialization_RaisesEvents ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_serializationInfo, _context, _concreteObject, _classContext, _extensions, true);
      var helper = new SerializationHelper (_serializationInfo, _context, t => typeof (FakeConcreteMixedType));

      var deserializedObject = (FakeConcreteMixedType) helper.GetRealObject (_context);
      Assert.That (deserializedObject.OnDeserializedCalled, Is.False);
      Assert.That (deserializedObject.OnDeserializationCalled, Is.False);
      
      helper.OnDeserialization (null);

      Assert.That (deserializedObject.OnDeserializedCalled, Is.True);
      Assert.That (deserializedObject.OnDeserializationCalled, Is.True);
    }
  }
}
