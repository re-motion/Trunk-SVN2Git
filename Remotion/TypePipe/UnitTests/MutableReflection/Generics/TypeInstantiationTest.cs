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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Generics
{
  [TestFixture]
  public class TypeInstantiationTest
  {
    private const BindingFlags c_allBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private IMemberSelector _memberSelectorMock;
    private ITypeInstantiator _typeInstantiatorMock;

    [SetUp]
    public void SetUp ()
    {
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();
      _typeInstantiatorMock = MockRepository.GenerateStrictMock<ITypeInstantiator>();
    }

    [Test]
    public void Initialization_AdjustsBaseType_AndNames ()
    {
      var typeArgument = ReflectionObjectMother.GetSomeType();
      var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock);
      var fakeBaseType = ReflectionObjectMother.GetSomeType();
      SetupExpectationsOnMemberSelector (genericTypeDefinition);
      _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (genericTypeDefinition.BaseType)).Return (fakeBaseType);
      _typeInstantiatorMock.Expect (mock => mock.GetSimpleName (genericTypeDefinition)).Return ("name");
      _typeInstantiatorMock.Expect (mock => mock.GetFullName (genericTypeDefinition)).Return ("full name");
      _typeInstantiatorMock.Expect (mock => mock.TypeArguments).Return (new[] { typeArgument });

      var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

      _typeInstantiatorMock.VerifyAllExpectations();
      Assert.That (instantiation.BaseType, Is.SameAs (fakeBaseType));
      Assert.That (instantiation.Name, Is.EqualTo ("name"));
      Assert.That (instantiation.Namespace, Is.EqualTo (genericTypeDefinition.Namespace));
      Assert.That (instantiation.FullName, Is.EqualTo ("full name"));
      Assert.That (instantiation.Attributes, Is.EqualTo (genericTypeDefinition.Attributes));
      Assert.That (instantiation.IsGenericType, Is.True);
      Assert.That (instantiation.IsGenericTypeDefinition, Is.True);
      Assert.That (instantiation.GetGenericArguments(), Is.EqualTo (new[] { typeArgument }));
    }

    [Test]
    public void Initialization_AdjustsInterfaces ()
    {
      var iface = ReflectionObjectMother.GetSomeInterfaceType();
      var fakeInterface = ReflectionObjectMother.GetSomeDifferentInterfaceType();
      var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, interfaces: new[] { iface });
      SetupExpectationsOnMemberSelector (genericTypeDefinition);
      StubBaseTypeAdjustment (genericTypeDefinition);
      _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (iface)).Return (fakeInterface);

      var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

      _typeInstantiatorMock.VerifyAllExpectations();
      Assert.That (instantiation.GetInterfaces(), Is.EqualTo (new[] { fakeInterface }));
    }

    [Test]
    public void Initialization_AdjustsFields ()
    {
      var fields = new FieldInfo[] { CustomFieldInfoObjectMother.Create() };
      var fakeField1 = ReflectionObjectMother.GetSomeField();
      var fakeField2 = ReflectionObjectMother.GetSomeOtherField();
      var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, fields: fields);
      SetupExpectationsOnMemberSelector (genericTypeDefinition, inputFields: fields, outputFields: new[] { fakeField1 });
      StubBaseTypeAdjustment (genericTypeDefinition);
      _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (fakeField1)).Return (fakeField2);

      var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

      _typeInstantiatorMock.VerifyAllExpectations();
      Assert.That (instantiation.GetFields (c_allBindingFlags), Is.EqualTo (new[] { fakeField2 }));
    }

    [Test]
    public void Initialization_AdjustsConstructors ()
    {
      var constructors = new ConstructorInfo[] { CustomConstructorInfoObjectMother.Create() };
      var fakeConstructor1 = ReflectionObjectMother.GetSomeConstructor();
      var fakeConstructor2 = ReflectionObjectMother.GetSomeOtherConstructor();
      var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, constructors: constructors);
      SetupExpectationsOnMemberSelector (genericTypeDefinition, inputConstructors: constructors, outputConstructors: new[] { fakeConstructor1 });
      StubBaseTypeAdjustment (genericTypeDefinition);
      _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (fakeConstructor1)).Return (fakeConstructor2);

      var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

      _typeInstantiatorMock.VerifyAllExpectations();
      Assert.That (instantiation.GetConstructors (c_allBindingFlags), Is.EqualTo (new[] { fakeConstructor2 }));
    }

    [Test]
    public void Initialization_AdjustsMethods ()
    {
      var methods = new MethodInfo[] { CustomMethodInfoObjectMother.Create() };
      var fakeMethod1 = ReflectionObjectMother.GetSomeMethod();
      var fakeMethod2 = ReflectionObjectMother.GetSomeOtherMethod();
      var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, methods: methods);
      SetupExpectationsOnMemberSelector (genericTypeDefinition, inputMethods: methods, outputMethods: new[] { fakeMethod1 });
      StubBaseTypeAdjustment (genericTypeDefinition);
      _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (fakeMethod1)).Return (fakeMethod2);

      var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

      _typeInstantiatorMock.VerifyAllExpectations();
      Assert.That (instantiation.GetMethods (c_allBindingFlags), Is.EqualTo (new[] { fakeMethod2 }));
    }

    private void SetupExpectationsOnMemberSelector (
        Type declaringType,
        FieldInfo[] inputFields = null,
        FieldInfo[] outputFields = null,
        ConstructorInfo[] inputConstructors = null,
        ConstructorInfo[] outputConstructors = null,
        MethodInfo[] inputMethods = null,
        MethodInfo[] outputMethods = null)
    {
      inputFields = inputFields ?? new FieldInfo[0];
      outputFields = outputFields ?? new FieldInfo[0];
      inputConstructors = inputConstructors ?? new ConstructorInfo[0];
      outputConstructors = outputConstructors ?? new ConstructorInfo[0];
      inputMethods = inputMethods ?? new MethodInfo[0];
      outputMethods = outputMethods ?? new MethodInfo[0];

      _memberSelectorMock.Expect (mock => mock.SelectFields (inputFields, c_allBindingFlags)).Return (outputFields);
      _memberSelectorMock.Expect (mock => mock.SelectMethods (inputConstructors, c_allBindingFlags, declaringType)).Return (outputConstructors);
      _memberSelectorMock.Expect (mock => mock.SelectMethods (inputMethods, c_allBindingFlags, declaringType)).Return (outputMethods);
    }

    private Type CreateGenericTypeDefinition (
        IMemberSelector memberSelector,
        IEnumerable<Type> interfaces = null,
        IEnumerable<FieldInfo> fields = null,
        IEnumerable<ConstructorInfo> constructors = null,
        IEnumerable<MethodInfo> methods = null)
    {
      return CustomTypeObjectMother.Create (
          memberSelector,
          isGenericTypeDefinition: true,
          isGenericType: true,
          typeArguments: new[] { ReflectionObjectMother.GetSomeType () },
          interfaces: interfaces,
          fields: fields,
          constructors: constructors,
          methods: methods);
    }

    private TypeInstantiation CreateTypeInstantion (ITypeInstantiator typeInstantiator, Type genericTypeDefinition)
    {
      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
      var underlyingTypeFactory = new ThrowingUnderlyingTypeFactory();
      return new TypeInstantiation (memberSelector, underlyingTypeFactory, typeInstantiator, genericTypeDefinition);
    }

    private void StubBaseTypeAdjustment (Type genericTypeDefinition)
    {
      var fakeBaseType = ReflectionObjectMother.GetSomeType();
      _typeInstantiatorMock.Stub (stub => stub.SubstituteGenericParameters (genericTypeDefinition.BaseType)).Return (fakeBaseType);
      _typeInstantiatorMock.Stub (stub => stub.GetSimpleName (genericTypeDefinition)).Return ("name");
      _typeInstantiatorMock.Stub (stub => stub.GetFullName (genericTypeDefinition)).Return ("full name");
      _typeInstantiatorMock.Stub (stub => stub.TypeArguments).Return (new[] { ReflectionObjectMother.GetSomeType() });
    }
  }
}