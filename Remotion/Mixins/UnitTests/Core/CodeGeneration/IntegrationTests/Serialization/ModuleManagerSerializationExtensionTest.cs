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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization
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
      _moduleManager = ConcreteTypeBuilderTestHelper.GetCurrentModuleManager ();
    }

    [Test]
    public void BeginDeserialization_Transformer ()
    {
      NullTarget mixedObject = TypeGenerationHelper.ForceTypeGenerationAndCreateInstance<NullTarget> ();
      SerializationInfo info = new SerializationInfo (mixedObject.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ((ISerializable) mixedObject).GetObjectData (info, context);
      Type derivedType = TypeGenerationHelper.ForceTypeGeneration (typeof (DerivedNullTarget));

      Func<Type, Type> transformer = delegate (Type t)
      {
        Assert.That (t, Is.EqualTo (mixedObject.GetType ()));
        return derivedType;
      };

      IObjectReference objectReference = _moduleManager.BeginDeserialization (transformer, info, context);
      object deserializedObject = objectReference.GetRealObject (context);
      Assert.That (deserializedObject.GetType (), Is.EqualTo (derivedType));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void BeginDeserialization_InvalidTransformer ()
    {
      BaseType1 mixedObject = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      SerializationInfo info = new SerializationInfo (mixedObject.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ((ISerializable) mixedObject).GetObjectData (info, context);

      Func<Type, Type> transformer = delegate (Type t)
      {
        Assert.That (t, Is.EqualTo (mixedObject.GetType ()));
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
      Assert.That (deserializedObject.I, Is.Not.EqualTo (50));
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
      Assert.That (instance.CtorCalled, Is.False);

      SerializationInfo info = new SerializationInfo (instance.GetType (), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      ClassImplementingISerializable deserializedObject = (ClassImplementingISerializable) _moduleManager.BeginDeserialization (_identityTransformer,
          info, context).GetRealObject (context);
      Assert.That (deserializedObject.CtorCalled, Is.True);
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
      Assert.That (deserializedObject.I, Is.EqualTo (50));
      Assert.That (Mixin.Get<BT1Mixin1> (deserializedObject), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin1> (deserializedObject).BackingField, Is.EqualTo ("Data"));
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
        + "Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization.ModuleManagerSerializationExtensionTest+DummyReference, which cannot be assigned to type "
            + "System.Runtime.Serialization.IDeserializationCallback.\r\nParameter name: objectReference")]
    public void FinishDeserialization_ThrowsOnUnmixedObjects ()
    {
      ClassImplementingISerializable obj = new ClassImplementingISerializable ();
      IObjectReference dummyReference = new DummyReference (obj);

      _moduleManager.FinishDeserialization (dummyReference);
    }
  }
}
