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
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationBaseNonSerializable", typeof (object),
          new Type[] {typeof (ISerializable)}, TypeAttributes.Public, false );

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
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationBaseSerializable", typeof (SerializableClass));
      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter,
          delegate (CustomMethodEmitter getObjectDataMethod, bool baseIsISerializeable)
          {
            Assert.IsNotNull (getObjectDataMethod);
            Assert.IsTrue (baseIsISerializeable);

            return new MethodInvocationExpression (typeof (SerializableClass).GetMethod ("GetObjectData"),
              getObjectDataMethod.ArgumentReferences[0].ToExpression(),
              getObjectDataMethod.ArgumentReferences[1].ToExpression());
          });

      SerializableClass instance = (SerializableClass) Activator.CreateInstance (classEmitter.BuildType ());
      SerializationInfo info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter());
      StreamingContext context = new StreamingContext();

      instance.GetObjectData (info, context);

      Assert.AreEqual (info, instance.Info);
      Assert.AreEqual (context, instance.Context);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No public or protected deserialization constructor in type "
        + "Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes.SerializableClassWithoutCtor - serialization is not supported.")]
    public void ImplementGetObjectDataByDelegationThrowsIfBaseHasNoDeserializationCtor ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationThrowsIfBaseHasNoDeserializationCtor", typeof (SerializableClassWithoutCtor),
          new Type[] {typeof (ISerializable)}, TypeAttributes.Public | TypeAttributes.Serializable, false);
      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter, delegate { return null; });
      SerializableClassWithoutCtor instance = (SerializableClassWithoutCtor) Activator.CreateInstance (classEmitter.BuildType ());

      Serializer.Serialize (instance);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No public or protected GetObjectData in type "
        + "Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes.SerializableClassWithPrivateGetObjectData - serialization is not supported.")]
    public void ImplementGetObjectDataByDelegationThrowsIfBaseHasPrivateGetObjectData ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationThrowsIfBaseHasPrivateGetObjectData", typeof (SerializableClassWithPrivateGetObjectData),
          new Type[] { typeof (ISerializable) }, TypeAttributes.Public | TypeAttributes.Serializable, false);

      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter, delegate { return null; });
      SerializableClassWithPrivateGetObjectData instance =
          (SerializableClassWithPrivateGetObjectData) Activator.CreateInstance (classEmitter.BuildType ());

      Serializer.Serialize (instance);
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException),
        ExpectedMessage = "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers.")]
    public void ImplementDeserializationConstructorByThrowingWhenBaseHasNoCtor ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowingWhenBaseHasNoCtor", typeof (object));
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
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowing", typeof (SerializableClass));
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
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowingIfNotExistsOnBase", typeof (object));
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
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "ImplementDeserializationConstructorByThrowingIfNotExistsOnBase", typeof (SerializableClass));
      classEmitter.ReplicateBaseTypeConstructors ();

      ConstructorEmitter emitter = SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (classEmitter);
      Assert.IsNull (emitter);

      SerializationInfo info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter ());
      StreamingContext context = new StreamingContext ();
      SerializableClass instance = (SerializableClass) Activator.CreateInstance (classEmitter.BuildType (), new object[] { info, context });

      Assert.AreEqual (info, instance.Info);
      Assert.AreEqual (context, instance.Context);
    }

    [Test]
    public void OnDeserializationWithFormatter ()
    {
      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
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
      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
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
      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
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
      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
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
