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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection.Generics;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class EmittableOperandProviderTest
  {
    private EmittableOperandProvider _provider;

    private MutableType _mutableType;
    private MutableGenericParameter _mutableGenericParameter;
    private MutableFieldInfo _mutableField;
    private MutableConstructorInfo _mutableConstructor;
    private MutableMethodInfo _mutableMethod;

    private Type _listInstantiation;

    [SetUp]
    public void SetUp ()
    {
      _provider = new EmittableOperandProvider();

      _mutableType = MutableTypeObjectMother.Create();
      _mutableGenericParameter = MutableGenericParameterObjectMother.Create();
      _mutableField = MutableFieldInfoObjectMother.Create();
      _mutableConstructor = MutableConstructorInfoObjectMother.Create();
      _mutableMethod = MutableMethodInfoObjectMother.Create();

      _listInstantiation = typeof (List<>).MakeTypePipeGenericType (_mutableType);
    }

    [Test]
    public void AddMapping ()
    {
      CheckAddMapping<MutableType, Type> (_provider.AddMapping, _provider.GetEmittableType, _mutableType);
      CheckAddMapping<MutableGenericParameter, Type> (_provider.AddMapping, _provider.GetEmittableType, _mutableGenericParameter);
      CheckAddMapping<MutableFieldInfo, FieldInfo> (_provider.AddMapping, _provider.GetEmittableField, _mutableField);
      CheckAddMapping<MutableConstructorInfo, ConstructorInfo> (_provider.AddMapping, _provider.GetEmittableConstructor, _mutableConstructor);
      CheckAddMapping<MutableMethodInfo, MethodInfo> (_provider.AddMapping, _provider.GetEmittableMethod, _mutableMethod);
    }

    [Test]
    public void AddMapping_Twice ()
    {
      CheckAddMappingTwiceThrows<MutableType, Type> (
          _provider.AddMapping, _mutableType, "Type '{memberName}' is already mapped.\r\nParameter name: mappedType");
      CheckAddMappingTwiceThrows<MutableGenericParameter, Type> (
          _provider.AddMapping, _mutableGenericParameter, "Type '{memberName}' is already mapped.\r\nParameter name: mappedType");
      CheckAddMappingTwiceThrows<MutableFieldInfo, FieldInfo> (
          _provider.AddMapping, _mutableField, "FieldInfo '{memberName}' is already mapped.\r\nParameter name: mappedField");
      CheckAddMappingTwiceThrows<MutableConstructorInfo, ConstructorInfo> (
          _provider.AddMapping, _mutableConstructor, "ConstructorInfo '{memberName}' is already mapped.\r\nParameter name: mappedConstructor");
      CheckAddMappingTwiceThrows<MutableMethodInfo, MethodInfo> (
          _provider.AddMapping, _mutableMethod, "MethodInfo '{memberName}' is already mapped.\r\nParameter name: mappedMethod");
    }

    [Test]
    public void GetEmittableXXX_Mutable ()
    {
      var typeBuilder = ReflectionEmitObjectMother.GetSomeTypeBuilder();
      var genericParameterBuilder = ReflectionEmitObjectMother.GetSomeGenericTypeParameterBuilder();
      var fieldBuilder = ReflectionEmitObjectMother.GetSomeFieldBuilder();
      var constructorBuilder = ReflectionEmitObjectMother.GetSomeConstructorBuilder();
      var methodBuilder = ReflectionEmitObjectMother.GetSomeMethodBuilder();

      _provider.AddMapping (_mutableType, typeBuilder);
      _provider.AddMapping (_mutableGenericParameter, genericParameterBuilder);
      _provider.AddMapping (_mutableField, fieldBuilder);
      _provider.AddMapping (_mutableConstructor, constructorBuilder);
      _provider.AddMapping (_mutableMethod, methodBuilder);

      Assert.That (_provider.GetEmittableType (_mutableType), Is.SameAs (typeBuilder));
      Assert.That (_provider.GetEmittableType (_mutableGenericParameter), Is.SameAs (genericParameterBuilder));
      Assert.That (_provider.GetEmittableField (_mutableField), Is.SameAs (fieldBuilder));
      Assert.That (_provider.GetEmittableConstructor (_mutableConstructor), Is.SameAs (constructorBuilder));
      Assert.That (_provider.GetEmittableMethod (_mutableMethod), Is.SameAs (methodBuilder));
    }

    [Test]
    public void GetEmittableXXX_RuntimeInfos ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      var genericParameter = ReflectionObjectMother.GetSomeGenericParameter();
      var field = ReflectionObjectMother.GetSomeField();
      var ctor = ReflectionObjectMother.GetSomeConstructor();
      var method = ReflectionObjectMother.GetSomeMethod();

      Assert.That (_provider.GetEmittableType (type), Is.SameAs (type));
      Assert.That (_provider.GetEmittableType (genericParameter), Is.SameAs (genericParameter));
      Assert.That (_provider.GetEmittableField (field), Is.SameAs (field));
      Assert.That (_provider.GetEmittableConstructor (ctor), Is.SameAs (ctor));
      Assert.That (_provider.GetEmittableMethod (method), Is.SameAs (method));
    }
    
    [Test]
    public void GetEmittableXXX_NoMapping ()
    {
      Assert.That (() => _provider.GetEmittableType (_mutableType), Throws.Exception);
      Assert.That (() => _provider.GetEmittableType (_mutableGenericParameter), Throws.Exception);
      Assert.That (() => _provider.GetEmittableField (_mutableField), Throws.Exception);
      Assert.That (() => _provider.GetEmittableConstructor (_mutableConstructor), Throws.Exception);
      Assert.That (() => _provider.GetEmittableMethod (_mutableMethod), Throws.Exception);
    }

    [Test]
    public void GetEmittableType ()
    {
      var proxyType = MutableTypeObjectMother.Create();
      _provider.AddMapping (proxyType, ReflectionObjectMother.GetSomeType());

      var constructedType = typeof (List<>).MakeTypePipeGenericType (proxyType);
      Assert.That (constructedType.GetGenericArguments(), Is.EqualTo (new[] { proxyType }));

      _provider.GetEmittableType (constructedType);

      Assert.That (constructedType.GetGenericArguments(), Is.EqualTo (new[] { proxyType }));
    }

    [Test]
    public void GetEmittableType_TypeInstantiation ()
    {
      var emittableType = ReflectionObjectMother.GetSomeType();
      _provider.AddMapping (_mutableType, emittableType);

      var instantiation = typeof (List<>).MakeTypePipeGenericType (typeof (Func<>).MakeTypePipeGenericType (_mutableType));
      Assert.That (instantiation, Is.TypeOf<TypeInstantiation>());

      var result = _provider.GetEmittableType (instantiation);

      Assert.That (result, Is.Not.InstanceOf<CustomType>());
      var emittableGenericArgument = result.GetGenericArguments().Single().GetGenericArguments().Single();
      Assert.That (emittableGenericArgument, Is.SameAs (emittableType));
    }

    [Test]
    public void GetEmittableMethod_MethodInstantiation_OnRuntimeType ()
    {
      var emittableType = ReflectionObjectMother.GetSomeType();
      _provider.AddMapping (_mutableType, emittableType);

      var genericMethodDefinition = typeof (Enumerable).GetMethod ("Empty");
      var instantiation = genericMethodDefinition.MakeTypePipeGenericMethod (_mutableType);
      Assert.That (instantiation, Is.TypeOf<MethodInstantiation>());

      var result = _provider.GetEmittableMethod (instantiation);

      Assert.That (result, Is.Not.InstanceOf<CustomMethodInfo>());
      var emittableGenericArgument = result.GetGenericArguments().Single();
      Assert.That (emittableGenericArgument, Is.SameAs (emittableType));
    }

    [Test]
    public void GetEmittableMethod_MethodInstantiation_OnCustomType ()
    {
      var genericMethodDefinition = MutableMethodInfoObjectMother.Create (genericParameters: new[] { MutableGenericParameterObjectMother.Create() });
      var emittableGenericMethodDefinition = typeof (Enumerable).GetMethod ("Empty");
      _provider.AddMapping (genericMethodDefinition, emittableGenericMethodDefinition);

      var emittableType = ReflectionObjectMother.GetSomeType();
      var instantiation = genericMethodDefinition.MakeTypePipeGenericMethod (emittableType);
      Assert.That (instantiation, Is.TypeOf<MethodInstantiation>());

      var result = _provider.GetEmittableMethod (instantiation);

      Assert.That (result, Is.Not.InstanceOf<CustomMethodInfo>());
      var emittableGenericArgument = result.GetGenericArguments().Single();
      Assert.That (emittableGenericArgument, Is.SameAs (emittableType));
    }

    [Test]
    public void GetEmittableXXX_MembersFromTypeInstantiation ()
    {
      var field = _listInstantiation.GetField ("_size", BindingFlags.NonPublic | BindingFlags.Instance);
      var ctor = _listInstantiation.GetConstructor (Type.EmptyTypes);
      var method = _listInstantiation.GetMethod ("Add");

      var emittableType = ReflectionEmitObjectMother.GetSomeTypeBuilder();
      _provider.AddMapping (_mutableType, emittableType);

      var emittableField = _provider.GetEmittableField (field);
      var emittableCtor = _provider.GetEmittableConstructor (ctor);
      var emittableMethod = _provider.GetEmittableMethod (method);

      CheckEmittableMemberInstantiation (emittableField, emittableType);
      CheckEmittableMemberInstantiation (emittableCtor, emittableType);
      CheckEmittableMemberInstantiation (emittableMethod, emittableType);
    }

    private static void CheckEmittableMemberInstantiation (MemberInfo emittableMember, Type emittableTypeArgument)
    {
      Assertion.IsNotNull (emittableMember.DeclaringType);
      var proxyTypeGenericArgument = emittableMember.DeclaringType.GetGenericArguments().Single();
      Assert.That (proxyTypeGenericArgument, Is.SameAs (emittableTypeArgument));
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
  }
}