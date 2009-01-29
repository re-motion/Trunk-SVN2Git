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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class ModuleManagerSerializationExtensionTest
  {
    private Func<Type, Type> _identityTransformer;
    private IModuleManager _moduleManager;

    [SetUp]
    public void SetUp ()
    {
      _identityTransformer = delegate (Type t) { return t; };
      _moduleManager = ConcreteTypeBuilder.Current.Scope;
    }

    [Test]
    public void BeginDeserialization_Transformer ()
    {
      NullTarget mixedObject = ObjectFactory.Create<NullTarget> (ParamList.Empty, GenerationPolicy.ForceGeneration);
      SerializationInfo info = new SerializationInfo (mixedObject.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ((ISerializable) mixedObject).GetObjectData (info, context);
      Type derivedType = TypeFactory.GetConcreteType (typeof (DerivedNullTarget), GenerationPolicy.ForceGeneration);

      Func<Type, Type> transformer = delegate (Type t)
      {
        Assert.AreEqual (mixedObject.GetType (), t);
        return derivedType;
      };

      IObjectReference objectReference = _moduleManager.BeginDeserialization (transformer, info, context);
      object deserializedObject = objectReference.GetRealObject (context);
      Assert.AreEqual (derivedType, deserializedObject.GetType ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "TypeTransformer returned type Remotion.UnitTests.Mixins.SampleTypes.BaseType2, "
        + "which is not compatible with the serialized mixin configuration. The configuration requires a type assignable to "
            + "Remotion.UnitTests.Mixins.SampleTypes.BaseType1.\r\nParameter name: typeTransformer")]
    public void BeginDeserialization_InvalidTransformer ()
    {
      BaseType1 mixedObject = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      SerializationInfo info = new SerializationInfo (mixedObject.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ((ISerializable) mixedObject).GetObjectData (info, context);

      Func<Type, Type> transformer = delegate (Type t)
      {
        Assert.AreEqual (mixedObject.GetType (), t);
        return typeof (BaseType2);
      };

      _moduleManager.BeginDeserialization (transformer, info, context);
    }

    [Test]
    public void BeginDeserialization_DoesNotFillUpData ()
    {
      BaseType1 mixedObject = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      mixedObject.I = 50;
      SerializationInfo info = new SerializationInfo (mixedObject.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ((ISerializable) mixedObject).GetObjectData (info, context);

      BaseType1 deserializedObject = (BaseType1) _moduleManager.BeginDeserialization (_identityTransformer, info, context).GetRealObject (context);
      Assert.AreNotEqual (50, deserializedObject.I);
    }

    class ClassNotImplementingISerializable
    {
      public bool CtorCalled = false;

      public ClassNotImplementingISerializable ()
      {
        CtorCalled = true;
      }
    }

    class ClassImplementingISerializable : ISerializable
    {
      public bool CtorCalled = false;

      public ClassImplementingISerializable ()
      {
      }

      private ClassImplementingISerializable (SerializationInfo info, StreamingContext context)
      {
        CtorCalled = true;
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        throw new NotImplementedException ();
      }
    }

    class ClassImplementingISerializableWithoutCtor : ISerializable
    {
      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        throw new NotImplementedException ();
      }
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Member '__configuration.ConfigurationContext.ClassType.AssemblyQualifiedName' was not found.")]
    public void BeginDeserialization_ThrowsOnUnmixedTypes ()
    {
      ClassImplementingISerializable instance = new ClassImplementingISerializable ();
      Assert.IsFalse (instance.CtorCalled);

      SerializationInfo info = new SerializationInfo (instance.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ClassImplementingISerializable deserializedObject = (ClassImplementingISerializable) _moduleManager.BeginDeserialization (_identityTransformer,
          info, context).GetRealObject (context);
      Assert.IsTrue (deserializedObject.CtorCalled);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Member '__configuration.ConfigurationContext.ClassType.AssemblyQualifiedName' was not found.")]
    
    public void BeginDeserialization_ThrowsOnTypeNotImplementingISerializable ()
    {
      ClassNotImplementingISerializable instance = new ClassNotImplementingISerializable ();

      SerializationInfo info = new SerializationInfo (instance.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      _moduleManager.BeginDeserialization (_identityTransformer, info, context);
    }

    [Test]
    public void FinishDeserialization_FillsUpDataAndMixins ()
    {
      BaseType1 mixedObject = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      mixedObject.I = 50;
      Mixin.Get<BT1Mixin1> (mixedObject).BackingField = "Data";

      SerializationInfo info = new SerializationInfo (mixedObject.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ((ISerializable) mixedObject).GetObjectData (info, context);

      IObjectReference objectReference = _moduleManager.BeginDeserialization (_identityTransformer, info, context);
      _moduleManager.FinishDeserialization (objectReference);

      BaseType1 deserializedObject = (BaseType1) objectReference.GetRealObject (context);
      Assert.AreEqual (50, deserializedObject.I);
      Assert.IsNotNull (Mixin.Get<BT1Mixin1> (deserializedObject));
      Assert.AreEqual ("Data", Mixin.Get<BT1Mixin1> (deserializedObject).BackingField);
    }

    class DummyReference : IObjectReference
    {
      private object _realObj;

      public DummyReference (object realObj)
      {
        _realObj = realObj;
      }

      public object GetRealObject (StreamingContext context)
      {
        return _realObj;
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument objectReference is a "
        + "Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.ModuleManagerSerializationExtensionTest+DummyReference, which cannot be assigned to type "
            + "System.Runtime.Serialization.IDeserializationCallback.\r\nParameter name: objectReference")]
    public void FinishDeserialization_ThrowsOnUnmixedObjects ()
    {
      ClassImplementingISerializable obj = new ClassImplementingISerializable ();
      IObjectReference dummyReference = new DummyReference (obj);

      _moduleManager.FinishDeserialization (dummyReference);
    }
  }
}
