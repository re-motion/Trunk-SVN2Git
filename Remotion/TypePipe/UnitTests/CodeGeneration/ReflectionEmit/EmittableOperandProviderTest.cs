// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class EmittableOperandProviderTest
  {
    private EmittableOperandProvider _provider;

    private ProxyType _proxyType;
    private MutableFieldInfo _mutableField;
    private MutableConstructorInfo _mutableConstructor;
    private MutableMethodInfo _mutableMethod;

    [SetUp]
    public void SetUp ()
    {
      _provider = new EmittableOperandProvider();

      _proxyType = ProxyTypeObjectMother.Create();
      _mutableField = MutableFieldInfoObjectMother.Create();
      _mutableConstructor = MutableConstructorInfoObjectMother.Create();
      _mutableMethod = MutableMethodInfoObjectMother.Create();
    }

    [Test]
    public void AddMapping ()
    {
      CheckAddMapping<ProxyType, Type> (_provider.AddMapping, _provider.GetEmittableType, _proxyType);
      CheckAddMapping<MutableFieldInfo, FieldInfo> (_provider.AddMapping, _provider.GetEmittableField, _mutableField);
      CheckAddMapping<MutableConstructorInfo, ConstructorInfo> (_provider.AddMapping, _provider.GetEmittableConstructor, _mutableConstructor);
      CheckAddMapping<MutableMethodInfo, MethodInfo> (_provider.AddMapping, _provider.GetEmittableMethod, _mutableMethod);
    }

    [Test]
    public void AddMapping_Twice ()
    {
      CheckAddMappingTwiceThrows<ProxyType, Type> (
          _provider.AddMapping, _proxyType, "ProxyType '{memberName}' is already mapped.\r\nParameter name: mappedType");
      CheckAddMappingTwiceThrows<MutableFieldInfo, FieldInfo> (
          _provider.AddMapping, _mutableField, "MutableFieldInfo '{memberName}' is already mapped.\r\nParameter name: mappedField");
      CheckAddMappingTwiceThrows<MutableConstructorInfo, ConstructorInfo> (
          _provider.AddMapping, _mutableConstructor, "MutableConstructorInfo '{memberName}' is already mapped.\r\nParameter name: mappedConstructor");
      CheckAddMappingTwiceThrows<MutableMethodInfo, MethodInfo> (
          _provider.AddMapping, _mutableMethod, "MutableMethodInfo '{memberName}' is already mapped.\r\nParameter name: mappedMethod");
    }

    [Test]
    public void GetEmittableXXX_Mutable ()
    {
      var typeBuilder = ReflectionEmitObjectMother.GetSomeTypeBuilder();
      var fieldBuilder = ReflectionEmitObjectMother.GetSomeFieldBuilder();
      var constructorBuilder = ReflectionEmitObjectMother.GetSomeConstructorBuilder();
      var methodBuilder = ReflectionEmitObjectMother.GetSomeMethodBuilder();

      _provider.AddMapping (_proxyType, typeBuilder);
      _provider.AddMapping (_mutableField, fieldBuilder);
      _provider.AddMapping (_mutableConstructor, constructorBuilder);
      _provider.AddMapping (_mutableMethod, methodBuilder);

      Assert.That (_provider.GetEmittableType (_proxyType), Is.SameAs (typeBuilder));
      Assert.That (_provider.GetEmittableField (_mutableField), Is.SameAs (fieldBuilder));
      Assert.That (_provider.GetEmittableConstructor (_mutableConstructor), Is.SameAs (constructorBuilder));
      Assert.That (_provider.GetEmittableMethod (_mutableMethod), Is.SameAs (methodBuilder));
    }

    [Test]
    public void GetEmittableXXX_RuntimeInfos ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      var field = ReflectionObjectMother.GetSomeField();
      var ctor = ReflectionObjectMother.GetSomeConstructor();
      var method = ReflectionObjectMother.GetSomeMethod();

      Assert.That (_provider.GetEmittableType (type), Is.SameAs (type));
      Assert.That (_provider.GetEmittableField (field), Is.SameAs (field));
      Assert.That (_provider.GetEmittableConstructor (ctor), Is.SameAs (ctor));
      Assert.That (_provider.GetEmittableMethod (method), Is.SameAs (method));
    }
    
    [Test]
    public void GetEmittableXXX_NoMapping ()
    {
      CheckGetEmitableOperandWithNoMappingThrows (_provider.GetEmittableType, _proxyType);
      CheckGetEmitableOperandWithNoMappingThrows (_provider.GetEmittableField, _mutableField);
      CheckGetEmitableOperandWithNoMappingThrows (_provider.GetEmittableConstructor, _mutableConstructor);
      CheckGetEmitableOperandWithNoMappingThrows (_provider.GetEmittableMethod, _mutableMethod);
    }

    [Test]
    public void GetEmittableType ()
    {
      var proxyType = ProxyTypeObjectMother.Create();
      _provider.AddMapping (proxyType, ReflectionObjectMother.GetSomeType());

      var constructedType = typeof (List<>).MakeTypePipeGenericType (proxyType);
      Assert.That (constructedType.GetGenericArguments(), Is.EqualTo (new[] { proxyType }));

      _provider.GetEmittableType (constructedType);

      Assert.That (constructedType.GetGenericArguments(), Is.EqualTo (new[] { proxyType }));
    }

    [Test]
    public void GetEmittableType_GenericType_ProxyTypeGenericParameter ()
    {
      var emittableType = ReflectionObjectMother.GetSomeType();
      _provider.AddMapping (_proxyType, emittableType);

      // Test recursion: List<Func<pt xxx>> as TypeBuilderInstantiation
      var constructedType = typeof (List<>).MakeTypePipeGenericType (typeof (Func<>).MakeTypePipeGenericType (_proxyType));
      Assert.That (constructedType.IsRuntimeType(), Is.False);

      var result = _provider.GetEmittableType (constructedType);

      var proxyTypeGenericArgument = result.GetGenericArguments().Single().GetGenericArguments().Single();
      Assert.That (proxyTypeGenericArgument, Is.SameAs (emittableType));
    }

    [Test]
    public void GetEmittableField_GenericTypeDeclaringType_ProxyTypeGenericParameter ()
    {
      var constructedType = (TypeInstantiation) typeof (StrongBox<>).MakeTypePipeGenericType (_proxyType);
      FieldInfo field = new FieldOnTypeInstantiation (constructedType, typeof (StrongBox<>).GetField ("Value"));

      CheckGetEmittableMemberOnTypeBuilderInstantiationWithOneGenericArgument (_provider, (p, f) => p.GetEmittableField (f), _proxyType, field);
    }

    [Test]
    public void GetEmittableConstructor_GenericTypeDeclaringType_ProxyTypeGenericParameter ()
    {
      var constructedType = (TypeInstantiation) typeof (List<>).MakeTypePipeGenericType (_proxyType);
      ConstructorInfo ctor = new ConstructorOnTypeInstantiation (constructedType, typeof (List<>).GetConstructor (Type.EmptyTypes));

      CheckGetEmittableMemberOnTypeBuilderInstantiationWithOneGenericArgument (_provider, (p, c) => p.GetEmittableConstructor (c), _proxyType, ctor);
    }

    [Test]
    public void GetEmittableMethod_GenericTypeDeclaringType_ProxyTypeGenericParameter ()
    {
      var constructedType = (TypeInstantiation) typeof (List<>).MakeTypePipeGenericType (_proxyType);
      MethodInfo method = new MethodOnTypeInstantiation (constructedType, typeof (List<>).GetMethod ("Add"));

      CheckGetEmittableMemberOnTypeBuilderInstantiationWithOneGenericArgument (_provider, (p, m) => p.GetEmittableMethod (m), _proxyType, method);
    }

    private void CheckAddMapping<TMutable, T> (
        Action<TMutable, T> addMappingMethod, Func<T, T> getEmittableOperandMethod, TMutable operandToBeEmitted)
        where TMutable : T
        where T : class
    {
      var fakeOperand = MockRepository.GenerateStub<T>();

      addMappingMethod (operandToBeEmitted, fakeOperand);

      var result = getEmittableOperandMethod (operandToBeEmitted);
      Assert.That (result, Is.SameAs (fakeOperand));
    }

    private void CheckAddMappingTwiceThrows<TMutable, TBuilder> (
        Action<TMutable, TBuilder> addMappingMethod, TMutable operandToBeEmitted, string expectedMessageTemplate)
        where TMutable : MemberInfo
        where TBuilder : class
    {
      addMappingMethod (operandToBeEmitted, MockRepository.GenerateStub<TBuilder>());

      var expectedMessage = expectedMessageTemplate.Replace ("{memberName}", operandToBeEmitted.Name);
      Assert.That (
          () => addMappingMethod (operandToBeEmitted, MockRepository.GenerateStub<TBuilder>()),
          Throws.ArgumentException.With.Message.EqualTo (expectedMessage));
    }

    private void CheckGetEmitableOperandWithNoMappingThrows<TMutable, TBase> (
        Func<TMutable, TBase> getEmittableOperandMethod, TMutable operandToBeEmitted)
        where TMutable : TBase
    {
      var message = string.Format ("No emittable operand found for '{0}' of type '{1}'.", operandToBeEmitted, operandToBeEmitted.GetType().Name);
      Assert.That (
          () => getEmittableOperandMethod (operandToBeEmitted),
          Throws.InvalidOperationException.With.Message.EqualTo (message));
    }

    private static void CheckGetEmittableMemberOnTypeBuilderInstantiationWithOneGenericArgument<T> (
        EmittableOperandProvider provider, Func<EmittableOperandProvider, T, T> getEmittableFunc, ProxyType proxyTypeWithMapping, T member)
        where T : MemberInfo
    {
      var emittableType = ReflectionEmitObjectMother.GetSomeTypeBuilderInstantiation();
      provider.AddMapping (proxyTypeWithMapping, emittableType);

      var result = getEmittableFunc (provider, member);

      var proxyTypeGenericArgument = result.DeclaringType.GetGenericArguments().Single();
      Assert.That (proxyTypeGenericArgument, Is.SameAs (emittableType));
    }
  }
}