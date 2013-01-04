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
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.TestDomain;

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

      var delegationTarget = classEmitter.CreateMethod (
          "DelegationTarget",
          MethodAttributes.Public,
          typeof (void),
          new[] { typeof (SerializationInfo), typeof (StreamingContext) });

      delegationTarget
          .AddStatement (new AssignStatement (delegationTargetCalled, new ConstReference (true).ToExpression()))
          .AddStatement (new ReturnStatement());

      var implementedMethod = SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter,
          delegate (IMethodEmitter getObjectDataMethod, bool baseIsISerializeable)
          {
            Assert.That (getObjectDataMethod, Is.Not.Null);
            Assert.That (baseIsISerializeable, Is.False);

            return
                new MethodInvocationExpression (delegationTarget.MethodBuilder, getObjectDataMethod.ArgumentReferences[0].ToExpression(),
                getObjectDataMethod.ArgumentReferences[1].ToExpression());
          });

      Type t = classEmitter.BuildType ();
      object instance = Activator.CreateInstance (t);

      Assert.That ((bool) PrivateInvoke.GetPublicField (instance, "DelegationTargetCalled"), Is.False);
      PrivateInvoke.InvokePublicMethod (instance, implementedMethod.Name, null, new StreamingContext());
      Assert.That ((bool) PrivateInvoke.GetPublicField (instance, "DelegationTargetCalled"), Is.True);
    }

    [Test]
    public void ImplementGetObjectDataByDelegationBaseSerializable ()
    {
      var classEmitter = new CustomClassEmitter (Scope, "ImplementGetObjectDataByDelegationBaseSerializable", typeof (SerializableClass));
      SerializationImplementer.ImplementGetObjectDataByDelegation (classEmitter,
          delegate (IMethodEmitter getObjectDataMethod, bool baseIsISerializeable)
          {
            Assert.That (getObjectDataMethod, Is.Not.Null);
            Assert.That (baseIsISerializeable, Is.True);

            return new MethodInvocationExpression (typeof (SerializableClass).GetMethod ("GetObjectData"),
              getObjectDataMethod.ArgumentReferences[0].ToExpression(),
              getObjectDataMethod.ArgumentReferences[1].ToExpression());
          });

      var instance = (SerializableClass) Activator.CreateInstance (classEmitter.BuildType ());
      var info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter());
      var context = new StreamingContext();

      instance.GetObjectData (info, context);

      Assert.That (instance.Info, Is.EqualTo (info));
      Assert.That (instance.Context, Is.EqualTo (context));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No public or protected deserialization constructor in type "
        + "Remotion.UnitTests.Reflection.CodeGeneration.TestDomain.SerializableClassWithoutCtor - serialization is not supported.")]
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
        + "Remotion.UnitTests.Reflection.CodeGeneration.TestDomain.SerializableClassWithPrivateGetObjectData - serialization is not supported.")]
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
      Assert.That (emitter, Is.Not.Null);

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
      Assert.That (emitter, Is.Not.Null);

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
      Assert.That (emitter, Is.Not.Null);

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
      Assert.That (emitter, Is.Null);

      var info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter ());
      var context = new StreamingContext ();
      var instance = (SerializableClass) Activator.CreateInstance (classEmitter.BuildType (), new object[] { info, context });

      Assert.That (instance.Info, Is.EqualTo (info));
      Assert.That (instance.Context, Is.EqualTo (context));
    }

    [Test]
    public void OnDeserializationWithFormatter ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.That (instance.OnBaseDeserializingCalled, Is.False);
      Assert.That (instance.OnBaseDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializingCalled, Is.False);
      Assert.That (instance.OnDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializationCalled, Is.False);

      instance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (instance.OnBaseDeserializingCalled, Is.True);
      Assert.That (instance.OnBaseDeserializedCalled, Is.True);
      Assert.That (instance.OnDeserializingCalled, Is.True);
      Assert.That (instance.OnDeserializedCalled, Is.True);
      Assert.That (instance.OnDeserializationCalled, Is.True);
    }

    [Test]
    public void RaiseOnDeserialization ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.That (instance.OnBaseDeserializingCalled, Is.False);
      Assert.That (instance.OnBaseDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializingCalled, Is.False);
      Assert.That (instance.OnDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializationCalled, Is.False);

      SerializationImplementer.RaiseOnDeserialization (instance, null);

      Assert.That (instance.OnBaseDeserializingCalled, Is.False);
      Assert.That (instance.OnBaseDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializingCalled, Is.False);
      Assert.That (instance.OnDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializationCalled, Is.True);
    }

    [Test]
    public void RaiseOnDeserializing ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.That (instance.OnBaseDeserializingCalled, Is.False);
      Assert.That (instance.OnBaseDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializingCalled, Is.False);
      Assert.That (instance.OnDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializationCalled, Is.False);

      SerializationImplementer.RaiseOnDeserializing (instance, new StreamingContext());

      Assert.That (instance.OnBaseDeserializingCalled, Is.True);
      Assert.That (instance.OnBaseDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializingCalled, Is.True);
      Assert.That (instance.OnDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializationCalled, Is.False);
    }

    [Test]
    public void RaiseOnDeserialized ()
    {
      var instance = new ClassWithDeserializationEvents ();
      Assert.That (instance.OnBaseDeserializingCalled, Is.False);
      Assert.That (instance.OnBaseDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializingCalled, Is.False);
      Assert.That (instance.OnDeserializedCalled, Is.False);
      Assert.That (instance.OnDeserializationCalled, Is.False);

      SerializationImplementer.RaiseOnDeserialized (instance, new StreamingContext ());

      Assert.That (instance.OnBaseDeserializingCalled, Is.False);
      Assert.That (instance.OnBaseDeserializedCalled, Is.True);
      Assert.That (instance.OnDeserializingCalled, Is.False);
      Assert.That (instance.OnDeserializedCalled, Is.True);
      Assert.That (instance.OnDeserializationCalled, Is.False);
    }
  }
}
