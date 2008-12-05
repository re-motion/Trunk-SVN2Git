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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class SerializationImplementerTest : CodeGenerationBaseTest
  {
    [Test]
    public void ImplementGetObjectDataByDelegationBaseNonSerializable ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationBaseNonSerializable", typeof (object),
          new[] {typeof (ISerializable)}, TypeAttributes.Public, false );

      FieldReference delegationTargetCalled = classEmitter.CreateField ("DelegationTargetCalled", typeof (bool));

      CustomMethodEmitter delegationTarget = classEmitter.CreateMethod ("DelegationTarget", MethodAttributes.Public)
          .SetParameterTypes (typeof (SerializationInfo), typeof (StreamingContext));

      delegationTarget
          .AddStatement (new AssignStatement (delegationTargetCalled, new ConstReference (true).ToExpression()))
          .AddStatement (new ReturnStatement());

      CustomMethodEmitter implementedMethod = SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter,
          delegate (CustomMethodEmitter getObjectDataMethod, bool baseIsISerializeable)
          {
            Assert.IsNotNull (getObjectDataMethod);
            Assert.IsFalse (baseIsISerializeable);

            return
                new MethodInvocationExpression (delegationTarget.MethodBuilder, getObjectDataMethod.ArgumentReferences[0].ToExpression(),
                getObjectDataMethod.ArgumentReferences[1].ToExpression());
          });

      Type t = classEmitter.BuildType ();
      object instance = Activator.CreateInstance (t);

      Assert.IsFalse ((bool) PrivateInvoke.GetPublicField (instance, "DelegationTargetCalled"));
      PrivateInvoke.InvokePublicMethod (instance, implementedMethod.Name, null, new StreamingContext());
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (instance, "DelegationTargetCalled"));
    }

    [Test]
    public void ImplementGetObjectDataByDelegationBaseSerializable ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationBaseSerializable", typeof (SerializableClass));
      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter,
          delegate (CustomMethodEmitter getObjectDataMethod, bool baseIsISerializeable)
          {
            Assert.IsNotNull (getObjectDataMethod);
            Assert.IsTrue (baseIsISerializeable);

            return new MethodInvocationExpression (typeof (SerializableClass).GetMethod ("GetObjectData"),
              getObjectDataMethod.ArgumentReferences[0].ToExpression(),
              getObjectDataMethod.ArgumentReferences[1].ToExpression());
          });

      var instance = (SerializableClass) Activator.CreateInstance (classEmitter.BuildType ());
      var info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter());
      var context = new StreamingContext();

      instance.GetObjectData (info, context);

      Assert.AreEqual (info, instance.Info);
      Assert.AreEqual (context, instance.Context);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No public or protected deserialization constructor in type "
        + "Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes.SerializableClassWithoutCtor - serialization is not supported.")]
    public void ImplementGetObjectDataByDelegationThrowsIfBaseHasNoDeserializationCtor ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationThrowsIfBaseHasNoDeserializationCtor", typeof (SerializableClassWithoutCtor),
          new[] {typeof (ISerializable)}, TypeAttributes.Public | TypeAttributes.Serializable, false);
      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter, delegate { return null; });
      var instance = (SerializableClassWithoutCtor) Activator.CreateInstance (classEmitter.BuildType ());

      Serializer.Serialize (instance);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No public or protected GetObjectData in type "
        + "Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes.SerializableClassWithPrivateGetObjectData - serialization is not supported.")]
    public void ImplementGetObjectDataByDelegationThrowsIfBaseHasPrivateGetObjectData ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationThrowsIfBaseHasPrivateGetObjectData", typeof (SerializableClassWithPrivateGetObjectData),
          new[] { typeof (ISerializable) }, TypeAttributes.Public | TypeAttributes.Serializable, false);

      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter, delegate { return null; });
      var instance = (SerializableClassWithPrivateGetObjectData) Activator.CreateInstance (classEmitter.BuildType ());

      Serializer.Serialize (instance);
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException),
        ExpectedMessage = "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers.")]
    public void ImplementDeserializationConstructorByThrowingWhenBaseHasNoCtor ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowingWhenBaseHasNoCtor", typeof (object));
      ConstructorEmitter emitter = SerializationImplementer.ImplementDeserializationConstructorByThrowing (classEmitter);
      Assert.IsNotNull (emitter);

      try
      {
        Activator.CreateInstance (classEmitter.BuildType (), new object[] { null, new StreamingContext () });
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException),
        ExpectedMessage = "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers.")]
    public void ImplementDeserializationConstructorByThrowingWhenBaseHasCtor ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowing", typeof (SerializableClass));
      ConstructorEmitter emitter = SerializationImplementer.ImplementDeserializationConstructorByThrowing (classEmitter);
      Assert.IsNotNull (emitter);

      try
      {
        Activator.CreateInstance (classEmitter.BuildType (), new object[] { null, new StreamingContext () });
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException),
        ExpectedMessage = "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers.")]
    public void ImplementDeserializationConstructorByThrowingIfNotExistsOnBaseWhenBaseHasNoCtor ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowingIfNotExistsOnBase", typeof (object));
      ConstructorEmitter emitter = SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (classEmitter);
      Assert.IsNotNull (emitter);

      try
      {
        Activator.CreateInstance (classEmitter.BuildType (), new object[] { null, new StreamingContext () });
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void ImplementDeserializationConstructorByThrowingIfNotExistsOnBaseWhenBaseHasCtor ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowingIfNotExistsOnBase", typeof (SerializableClass));
      classEmitter.ReplicateBaseTypeConstructors (delegate { }, delegate { });

      ConstructorEmitter emitter = SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (classEmitter);
      Assert.IsNull (emitter);

      var info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter ());
      var context = new StreamingContext ();
      var instance = (SerializableClass) Activator.CreateInstance (classEmitter.BuildType (), new object[] { info, context });

      Assert.AreEqual (info, instance.Info);
      Assert.AreEqual (context, instance.Context);
    }

    [Test]
    public void OnDeserializationWithFormatter ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      instance = Serializer.SerializeAndDeserialize (instance);

      Assert.IsTrue (instance.OnBaseDeserializingCalled);
      Assert.IsTrue (instance.OnBaseDeserializedCalled);
      Assert.IsTrue (instance.OnDeserializingCalled);
      Assert.IsTrue (instance.OnDeserializedCalled);
      Assert.IsTrue (instance.OnDeserializationCalled);
    }

    [Test]
    public void RaiseOnDeserialization ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      SerializationImplementer.RaiseOnDeserialization (instance, null);

      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsTrue (instance.OnDeserializationCalled);
    }

    [Test]
    public void RaiseOnDeserializing ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      SerializationImplementer.RaiseOnDeserializing (instance, new StreamingContext());

      Assert.IsTrue (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsTrue(instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);
    }

    [Test]
    public void RaiseOnDeserialized ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      SerializationImplementer.RaiseOnDeserialized (instance, new StreamingContext ());

      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsTrue (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsTrue (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);
    }
  }
}
