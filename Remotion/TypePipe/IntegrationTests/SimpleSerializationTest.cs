﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.IntegrationTests.TypeAssembly;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.IntegrationTests
{
  [TestFixture]
  public class SimpleSerializationTest : ObjectFactoryIntegrationTestBase
  {
    [Test]
    public void Standard_NoModifications ()
    {
      var factory = CreateObjectFactory();
      var instance = factory.CreateObject<SerializableType>();
      instance.String = "abc";
      Assert.That (instance.ConstructorCalled, Is.True);
      Assert.That (instance.DeserializationConstructorCalled, Is.False);

      CheckInstanceIsSerializable (
          instance, deserializedInstance =>
          {
            Assert.That (deserializedInstance.String, Is.EqualTo ("abc"));
            Assert.That (deserializedInstance.ConstructorCalled, Is.False);
            Assert.That (deserializedInstance.DeserializationConstructorCalled, Is.False);
          });
    }

    [Test]
    public void Standard_AddedFields ()
    {
      var factory = CreateObjectFactory (CreateFieldAddingParticipant());
      var instance = factory.CreateObject<SerializableType>();
      instance.String = "abc";
      PrivateInvoke.SetPublicField (instance, "AddedIntField", 7);
      PrivateInvoke.SetPublicField (instance, "AddedSkippedIntField", 7);

      CheckInstanceIsSerializable (
          instance, deserializedInstance =>
          {
            Assert.That (deserializedInstance.String, Is.EqualTo ("abc"));
            Assert.That (PrivateInvoke.GetPublicField (deserializedInstance, "AddedIntField"), Is.EqualTo (7));
            Assert.That (PrivateInvoke.GetPublicField (deserializedInstance, "AddedSkippedIntField"), Is.EqualTo (0));
          });
    }

    [Test]
    public void TypeImplementingISerializable_AddedFields ()
    {
      var factory = CreateObjectFactory (CreateFieldAddingParticipant());
      var instance = factory.CreateObject<CustomSerializableType>();
      instance.String = "abc";
      PrivateInvoke.SetPublicField (instance, "AddedIntField", 7);
      PrivateInvoke.SetPublicField (instance, "AddedSkippedIntField", 7);
      Assert.That (instance.ConstructorCalled, Is.True);
      Assert.That (instance.DeserializationConstructorCalled, Is.False);

      CheckInstanceIsSerializable (
          instance, deserializedInstance =>
          {
            Assert.That (deserializedInstance.String, Is.EqualTo ("abc (custom deserialization ctor)"));
            Assert.That (PrivateInvoke.GetPublicField (deserializedInstance, "AddedIntField"), Is.EqualTo (7));
            Assert.That (PrivateInvoke.GetPublicField (deserializedInstance, "AddedSkippedIntField"), Is.EqualTo (0));
            
            Assert.That (deserializedInstance.ConstructorCalled, Is.False);
            Assert.That (deserializedInstance.DeserializationConstructorCalled, Is.True);
          });
    }

    [Test]
    public void InstanceInitialization ()
    {
      var factory = CreateObjectFactory (CreateInitializationAddingParticipant());
      var instance1 = factory.CreateObject<SerializableType>();
      instance1.String = "abc";
      var instance2 = factory.CreateObject<CustomSerializableType> ();
      instance2.String = "abc";

      CheckInstanceIsSerializable (
          instance1, deserializedInstance => Assert.That (deserializedInstance.String, Is.EqualTo ("abc valueFromInstanceInitialization")));
      CheckInstanceIsSerializable (
          instance2,
          deserializedInstance =>
          Assert.That (deserializedInstance.String, Is.EqualTo ("abc (custom deserialization ctor) valueFromInstanceInitialization")));
    }

    [Test]
    public void AddedCallback ()
    {
      var factory = CreateObjectFactory (CreateInitializationAddingParticipant(), CreateCallbackImplementingParticipant());
      var instance1 = factory.CreateObject<SerializableType>();
      instance1.String = "abc";
      var instance2 = factory.CreateObject<CustomSerializableType>();
      instance2.String = "abc";

      CheckInstanceIsSerializable (
          instance1, deserializedInstance => Assert.That (deserializedInstance.String, Is.EqualTo ("abc addedCallback valueFromInstanceInitialization")));
      CheckInstanceIsSerializable (
          instance2,
          deserializedInstance =>
          { 
            Assert.That (deserializedInstance.String, Is.EqualTo ("abc (custom deserialization ctor) addedCallback valueFromInstanceInitialization"));
            Assert.That (deserializedInstance.DeserializationConstructorCalled, Is.True);
          });
    }

    [Test]
    public void ExistingCallback ()
    {
      var factory = CreateObjectFactory (CreateInitializationAddingParticipant());
      var instance = factory.CreateObject<DeserializationCallbackType>();
      instance.String = "abc";

      CheckInstanceIsSerializable (
          instance,
          deserializedInstance => Assert.That (deserializedInstance.String, Is.EqualTo ("abc existingCallback valueFromInstanceInitialization")));
    }

    [Test]
    public void OnDeserializationMethodWithoutInterface ()
    {
      var factory = CreateObjectFactory (CreateInitializationAddingParticipant());
      var instance = factory.CreateObject<OnDeserializationMethodType>();
      instance.String = "abc";

      CheckInstanceIsSerializable (
          instance,
          deserializedInstance => Assert.That (deserializedInstance.String, Is.EqualTo ("abc valueFromInstanceInitialization")));
    }

    [Test]
    public void ISerializable_CannotModifyOrOverrideGetObjectData ()
    {
      SkipSavingAndPeVerification();
      var factory = CreateObjectFactory (CreateFieldAddingParticipant());

      var message = "The underlying type implements ISerializable but GetObjectData cannot be overridden. "
                    + "Make sure that GetObjectData is implemented implicitly (not explicitly) and virtual.";
      Assert.That (
          () => factory.GetAssembledType (typeof (ExplicitISerializableType)),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message));
      Assert.That (
          () => factory.GetAssembledType (typeof (DerivedExplicitISerializableType)),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The underlying type implements ISerializable but does not define a deserialization constructor.")]
    public void ISerializable_MissingDeserializationConstructor ()
    {
      SkipSavingAndPeVerification();
      var factory = CreateObjectFactory (CreateFieldAddingParticipant());
      factory.GetAssembledType (typeof (SerializableInterfaceTypeWithoutDeserializationConstructor));
    }

    [Test]
    public void IDeserializationCallback_CannotModifyOrOverrideOnDeserialization ()
    {
      SkipSavingAndPeVerification();
      var factory = CreateObjectFactory (CreateInitializationAddingParticipant());

      var message = "The underlying type implements IDeserializationCallback but OnDeserialization cannot be overridden. "
                    + "Make sure that OnDeserialization is implemented implicitly (not explicitly) and virtual.";
      Assert.That (
          () => factory.GetAssembledType (typeof (ExplicitIDeserializationCallbackType)),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message));
      Assert.That (
          () => factory.GetAssembledType (typeof (DerivedExplicitIDeserializationCallbackType)),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message));
    }

    private new IObjectFactory CreateObjectFactory (params IParticipant[] participants)
    {
      var factory = CreateObjectFactory (participants, stackFramesToSkip: 1);
      factory.CodeGenerator.SetAssemblyDirectory (AppDomain.CurrentDomain.BaseDirectory);

      return factory;
    }

    private void CheckInstanceIsSerializable (
        SerializableType instance,
        Action<SerializableType> assertions)
    {
      Assert.That (instance.GetType().IsSerializable, Is.True);

      var serializedData = Serializer.Serialize (instance);

      FlushAndTrackFilesForCleanup();
      AppDomainRunner.Run (
          args =>
          {
            var data = (byte[]) args[0];
            var expectedAssemblyQualifiedName = (string) args[1];
            var assertionsDelegate = (Action<SerializableType>) args[2];

            var deserializedInstance = (SerializableType) Serializer.Deserialize (data);

            Assert.That (deserializedInstance.GetType().AssemblyQualifiedName, Is.EqualTo (expectedAssemblyQualifiedName));
            assertionsDelegate (deserializedInstance);
          },
          serializedData,
          instance.GetType().AssemblyQualifiedName,
          assertions);
    }

    private IParticipant CreateFieldAddingParticipant ()
    {
      var attributeConstructor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new NonSerializedAttribute());
      return CreateParticipant (
          mutableType =>
          {
            mutableType.AddField ("AddedIntField", typeof (int), FieldAttributes.Public);
            mutableType.AddField ("AddedSkippedIntField", typeof (int), FieldAttributes.Public)
                       .AddCustomAttribute (new CustomAttributeDeclaration (attributeConstructor, new object[0]));
          });
    }

    private IParticipant CreateInitializationAddingParticipant ()
    {
      return CreateParticipant (
          mutableType =>
          {
            var stringField = mutableType.GetField ("String");

            mutableType.AddInstanceInitialization (
                ctx =>
                Expression.AddAssign (
                    Expression.Field (ctx.This, stringField),
                    Expression.Constant (" valueFromInstanceInitialization"),
                    ExpressionHelper.StringConcatMethod));
          });
    }

    private IParticipant CreateCallbackImplementingParticipant ()
    {
      return CreateParticipant (
          mutableType =>
          {
            var stringField = mutableType.GetField ("String");
            var callback = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDeserializationCallback obj) => obj.OnDeserialization (null));

            mutableType.AddInterface (typeof (IDeserializationCallback));
            mutableType.AddExplicitOverride (
                callback,
                ctx => Expression.AddAssign (
                    Expression.Field (ctx.This, stringField), Expression.Constant (" addedCallback"), ExpressionHelper.StringConcatMethod));
          });
    }

    [Serializable]
    public class SerializableType
    {
      public string String;

      [NonSerialized]
      public readonly bool ConstructorCalled;
      [NonSerialized]
      public readonly bool DeserializationConstructorCalled;

      public SerializableType ()
      {
        ConstructorCalled = true;
      }

      [UsedImplicitly]
      public SerializableType (SerializationInfo info, StreamingContext context)
      {
        DeserializationConstructorCalled = true;
      }
    }

    [Serializable]
    public class CustomSerializableType : SerializableType, ISerializable
    {
      public CustomSerializableType () { }

      public CustomSerializableType (SerializationInfo info, StreamingContext context) : base (info, context)
      {
        String = info.GetString ("key1") + " (custom deserialization ctor)";
      }

      public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        info.AddValue ("key1", String);
      }
    }

    [Serializable]
    public class DeserializationCallbackType : SerializableType, IDeserializationCallback
    {
      public virtual void OnDeserialization (object sender)
      {
        String += " existingCallback";
      }
    }

    [Serializable]
    public class OnDeserializationMethodType : SerializableType
    {
      [UsedImplicitly]
      public virtual void OnDeserialization (object sender)
      {
        String += " existingCallback (but does not implement IDeserializationCallback)";
      }
    }

    public class ExplicitISerializableType : ISerializable
    {
      void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context) { }
    }

    public class DerivedExplicitISerializableType : ExplicitISerializableType { }

    public class SerializableInterfaceTypeWithoutDeserializationConstructor : ISerializable
    {
      public virtual void GetObjectData (SerializationInfo info, StreamingContext context) { }
    }

    public class ExplicitIDeserializationCallbackType : SerializableType, IDeserializationCallback
    {
      void IDeserializationCallback.OnDeserialization (object sender) { }
    }

    [Serializable]
    public class DerivedExplicitIDeserializationCallbackType : ExplicitIDeserializationCallbackType { }
  }
}