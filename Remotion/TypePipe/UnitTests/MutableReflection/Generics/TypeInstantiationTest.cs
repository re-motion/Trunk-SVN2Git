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
using System.Reflection;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Remotion.Utilities;
using System.Linq;
using Remotion.Development.UnitTesting;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Generics
{
  [TestFixture]
  public class TypeInstantiationTest
  {
    private const BindingFlags c_allMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private Type _genericTypeDefinition;
    private CustomType _customType;
    private Type[] _typeArguments;

    private TypeInstantiation _instantiation;

    [SetUp]
    public void SetUp ()
    {
      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
      var underlyingTypeFactory = new ThrowingUnderlyingTypeFactory();

      _genericTypeDefinition = typeof (GenericType<>);
      _customType = CustomTypeObjectMother.Create (fullName: "MyNs.Blub");
      _typeArguments = new Type[] { _customType };

      _instantiation = TypeInstantiation.Create (_genericTypeDefinition, _typeArguments, memberSelector, underlyingTypeFactory);
    }

    [Test]
    public void Create_Initialization ()
    {
      Assert.That (_instantiation.Name, Is.EqualTo (_genericTypeDefinition.Name));
      Assert.That (_instantiation.Namespace, Is.EqualTo (_genericTypeDefinition.Namespace));
      Assert.That (_instantiation.Attributes, Is.EqualTo (_genericTypeDefinition.Attributes));
      Assert.That (_instantiation.IsGenericType, Is.True);
      Assert.That (_instantiation.IsGenericTypeDefinition, Is.False);
      Assert.That (_instantiation.GetGenericArguments(), Is.EqualTo (_typeArguments));
    }

    [Test]
    public void Create_BaseType ()
    {
      var baseType = _instantiation.BaseType;
      Assertion.IsNotNull (baseType);
      Assert.That (baseType, Is.TypeOf<TypeInstantiation>());
      Assert.That (baseType.GetGenericTypeDefinition(), Is.SameAs (typeof (BaseType<>)));
      Assert.That (_instantiation.GetGenericArguments(), Is.EqualTo (new[] { _customType }));
    }

    [Test]
    public void Create_NameAndFullName ()
    {
      Assert.That (_instantiation.Name, Is.EqualTo ("GenericType`1"));
      Assert.That (
          _instantiation.FullName,
          Is.EqualTo (
              "Remotion.TypePipe.UnitTests.MutableReflection.Generics.TypeInstantiationTest+GenericType`1[[MyNs.Blub, TypePipe_GeneratedAssembly]]"));
    }

    [Test]
    public void Create_Interfaces ()
    {
      var iface = _instantiation.GetInterfaces().Single();

      Assert.That (iface, Is.TypeOf<TypeInstantiation>());
      Assert.That (iface.GetGenericTypeDefinition(), Is.SameAs (typeof (IMyInterface<>)));
      Assert.That (_instantiation.GetGenericArguments(), Is.EqualTo (new[] { _customType }));
    }

    [Test]
    public void Create_Fields ()
    {
      var field = _instantiation.GetFields (c_allMembers).Single ();

      var fieldOnGenericType = _genericTypeDefinition.GetField ("Field");
      Assert.That (field, Is.TypeOf<FieldOnTypeInstantiation> ());
      Assert.That (field.DeclaringType, Is.SameAs (_instantiation));
      Assert.That (field.As<FieldOnTypeInstantiation> ().FieldOnGenericType, Is.EqualTo (fieldOnGenericType));
    }

    interface IMyInterface<T> {}
    class BaseType<T> { }
    class GenericType<T> : BaseType<T>, IMyInterface<T>
    {
      public T Field;
    }

    //[Test]
    //public void Initialization_AdjustsBaseType_AndNames ()
    //{
    //  var typeArgument = ReflectionObjectMother.GetSomeType();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock);
    //  var fakeBaseType = ReflectionObjectMother.GetSomeType();
    //  SetupExpectationsOnMemberSelector (_memberSelectorMock, genericTypeDefinition);
    //  _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (genericTypeDefinition.BaseType)).Return (fakeBaseType);
    //  _typeInstantiatorMock.Expect (mock => mock.GetFullName (genericTypeDefinition)).Return ("full name");
    //  _typeInstantiatorMock.Expect (mock => mock.TypeArguments).Return (new[] { typeArgument });

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.BaseType, Is.SameAs (fakeBaseType));
    //  Assert.That (instantiation.Name, Is.EqualTo (genericTypeDefinition.Name));
    //  Assert.That (instantiation.Namespace, Is.EqualTo (genericTypeDefinition.Namespace));
    //  Assert.That (instantiation.FullName, Is.EqualTo ("full name"));
    //  Assert.That (instantiation.Attributes, Is.EqualTo (genericTypeDefinition.Attributes));
    //  Assert.That (instantiation.IsGenericType, Is.True);
    //  Assert.That (instantiation.IsGenericTypeDefinition, Is.False);
    //  Assert.That (instantiation.GetGenericArguments(), Is.EqualTo (new[] { typeArgument }));
    //}

    //[Test]
    //public void Initialization_AdjustsInterfaces ()
    //{
    //  var iface = ReflectionObjectMother.GetSomeInterfaceType();
    //  var fakeInterface = ReflectionObjectMother.GetSomeOtherInterfaceType();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, interfaces: new[] { iface });
    //  SetupExpectationsOnMemberSelector (_memberSelectorMock, genericTypeDefinition);
    //  StubBaseTypeAdjustment (genericTypeDefinition);
    //  _typeInstantiatorMock.Expect (mock => mock.SubstituteGenericParameters (iface)).Return (fakeInterface);

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.GetInterfaces(), Is.EqualTo (new[] { fakeInterface }));
    //}

    //[Test]
    //public void Initialization_AdjustsFields ()
    //{
    //  var fields = new FieldInfo[] { CustomFieldInfoObjectMother.Create() };
    //  var fakeField1 = ReflectionObjectMother.GetSomeField();
    //  var fakeField2 = ReflectionObjectMother.GetSomeOtherField();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, fields: fields);
    //  SetupExpectationsOnMemberSelector (_memberSelectorMock, genericTypeDefinition, inputFields: fields, outputFields: new[] { fakeField1 });
    //  StubBaseTypeAdjustment (genericTypeDefinition);
    //  object backReference = null;
    //  _typeInstantiatorMock
    //      .Expect (mock => mock.SubstituteGenericParameters (Arg<TypeInstantiation>.Is.Anything, Arg.Is (fakeField1)))
    //      .Return (fakeField2)
    //      .WhenCalled (mi => backReference = mi.Arguments[0]);

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.GetFields (c_allMembers), Is.EqualTo (new[] { fakeField2 }));
    //  Assert.That (backReference, Is.SameAs (instantiation));
    //}

    //[Test]
    //public void Initialization_AdjustsConstructors ()
    //{
    //  var constructors = new ConstructorInfo[] { CustomConstructorInfoObjectMother.Create() };
    //  var fakeConstructor1 = ReflectionObjectMother.GetSomeConstructor();
    //  var fakeConstructor2 = ReflectionObjectMother.GetSomeOtherConstructor();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, constructors: constructors);
    //  SetupExpectationsOnMemberSelector (
    //      _memberSelectorMock, genericTypeDefinition, inputConstructors: constructors, outputConstructors: new[] { fakeConstructor1 });
    //  StubBaseTypeAdjustment (genericTypeDefinition);
    //  object backReference = null;
    //  _typeInstantiatorMock
    //      .Expect (mock => mock.SubstituteGenericParameters (Arg<TypeInstantiation>.Is.Anything, Arg.Is (fakeConstructor1)))
    //      .Return (fakeConstructor2)
    //      .WhenCalled (mi => backReference = mi.Arguments[0]);

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.GetConstructors (c_allMembers), Is.EqualTo (new[] { fakeConstructor2 }));
    //  Assert.That (backReference, Is.SameAs (instantiation));
    //}

    //[Test]
    //public void Initialization_AdjustsMethods ()
    //{
    //  var methods = new MethodInfo[] { CustomMethodInfoObjectMother.Create() };
    //  var fakeMethod1 = ReflectionObjectMother.GetSomeMethod();
    //  var fakeMethod2 = ReflectionObjectMother.GetSomeOtherMethod();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, methods: methods);
    //  SetupExpectationsOnMemberSelector (_memberSelectorMock, genericTypeDefinition, inputMethods: methods, outputMethods: new[] { fakeMethod1 });
    //  StubBaseTypeAdjustment (genericTypeDefinition);
    //  object backReference = null;
    //  _typeInstantiatorMock
    //      .Expect (mock => mock.SubstituteGenericParameters (Arg<TypeInstantiation>.Is.Anything, Arg.Is (fakeMethod1)))
    //      .Return (fakeMethod2)
    //      .WhenCalled (mi => backReference = mi.Arguments[0]);

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.GetMethods (c_allMembers), Is.EqualTo (new[] { fakeMethod2 }));
    //  Assert.That (backReference, Is.SameAs (instantiation));
    //}

    //[Test]
    //public void Initialization_AdjustsProperties ()
    //{
    //  var properties = new PropertyInfo[] { CustomPropertyInfoObjectMother.Create() };
    //  var fakeProperty1 = ReflectionObjectMother.GetSomeProperty();
    //  var fakeProperty2 = ReflectionObjectMother.GetSomeOtherProperty();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, properties: properties);
    //  SetupExpectationsOnMemberSelector (_memberSelectorMock, genericTypeDefinition, inputProperties: properties, outputProperties: new[] { fakeProperty1 });
    //  StubBaseTypeAdjustment (genericTypeDefinition);
    //  object backReference = null;
    //  _typeInstantiatorMock
    //      .Expect (mock => mock.SubstituteGenericParameters (Arg<TypeInstantiation>.Is.Anything, Arg.Is (fakeProperty1)))
    //      .Return (fakeProperty2)
    //      .WhenCalled (mi => backReference = mi.Arguments[0]);

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.GetProperties (c_allMembers), Is.EqualTo (new[] { fakeProperty2 }));
    //  Assert.That (backReference, Is.SameAs (instantiation));
    //}

    //[Test]
    //public void Initialization_AdjustsEvents ()
    //{
    //  var events = new EventInfo[] { CustomEventInfoObjectMother.Create() };
    //  var fakeEvent1 = ReflectionObjectMother.GetSomeEvent();
    //  var fakeEvent2 = ReflectionObjectMother.GetSomeOtherEvent();
    //  var genericTypeDefinition = CreateGenericTypeDefinition (_memberSelectorMock, events: events);
    //  SetupExpectationsOnMemberSelector (_memberSelectorMock, genericTypeDefinition, inputEvents: events, outputEvents: new[] { fakeEvent1 });
    //  StubBaseTypeAdjustment (genericTypeDefinition);
    //  object backReference = null;
    //  _typeInstantiatorMock
    //      .Expect (mock => mock.SubstituteGenericParameters (Arg<TypeInstantiation>.Is.Anything, Arg.Is(fakeEvent1)))
    //      .Return (fakeEvent2)
    //      .WhenCalled (mi => backReference = mi.Arguments[0]);

    //  var instantiation = CreateTypeInstantion (_typeInstantiatorMock, genericTypeDefinition);

    //  _typeInstantiatorMock.VerifyAllExpectations();
    //  Assert.That (instantiation.GetEvents (c_allMembers), Is.EqualTo (new[] { fakeEvent2 }));
    //  Assert.That (backReference, Is.SameAs (instantiation));
    //}

    //private void SetupExpectationsOnMemberSelector (
    //    IMemberSelector memberSelectorMock,
    //    Type declaringType,
    //    IEnumerable<FieldInfo> inputFields = null,
    //    IEnumerable<FieldInfo> outputFields = null,
    //    IEnumerable<ConstructorInfo> inputConstructors = null,
    //    IEnumerable<ConstructorInfo> outputConstructors = null,
    //    IEnumerable<MethodInfo> inputMethods = null,
    //    IEnumerable<MethodInfo> outputMethods = null,
    //    IEnumerable<PropertyInfo> inputProperties = null,
    //    IEnumerable<PropertyInfo> outputProperties = null,
    //    IEnumerable<EventInfo> inputEvents = null,
    //    IEnumerable<EventInfo> outputEvents = null)
    //{
    //  inputFields = inputFields ?? new FieldInfo[0];
    //  outputFields = outputFields ?? new FieldInfo[0];
    //  inputConstructors = inputConstructors ?? new ConstructorInfo[0];
    //  outputConstructors = outputConstructors ?? new ConstructorInfo[0];
    //  inputMethods = inputMethods ?? new MethodInfo[0];
    //  outputMethods = outputMethods ?? new MethodInfo[0];
    //  inputProperties = inputProperties ?? new PropertyInfo[0];
    //  outputProperties = outputProperties ?? new PropertyInfo[0];
    //  inputEvents = inputEvents ?? new EventInfo[0];
    //  outputEvents = outputEvents ?? new EventInfo[0];

    //  memberSelectorMock.Expect (mock => mock.SelectFields (inputFields, c_allMembers, declaringType)).Return (outputFields);
    //  memberSelectorMock.Expect (mock => mock.SelectMethods (inputConstructors, c_allMembers, declaringType)).Return (outputConstructors);
    //  memberSelectorMock.Expect (mock => mock.SelectMethods (inputMethods, c_allMembers, declaringType)).Return (outputMethods);
    //  memberSelectorMock.Expect (mock => mock.SelectProperties (inputProperties, c_allMembers, declaringType)).Return (outputProperties);
    //  memberSelectorMock.Expect (mock => mock.SelectEvents (inputEvents, c_allMembers, declaringType)).Return (outputEvents);
    //}

    //private Type CreateGenericTypeDefinition (
    //    IMemberSelector memberSelector,
    //    IEnumerable<Type> interfaces = null,
    //    IEnumerable<FieldInfo> fields = null,
    //    IEnumerable<ConstructorInfo> constructors = null,
    //    IEnumerable<MethodInfo> methods = null,
    //    IEnumerable<PropertyInfo> properties = null,
    //    IEnumerable<EventInfo> events = null)
    //{
    //  return CustomTypeObjectMother.Create (
    //      memberSelector,
    //      isGenericTypeDefinition: true,
    //      isGenericType: true,
    //      typeArguments: new[] { ReflectionObjectMother.GetSomeType() },
    //      interfaces: interfaces,
    //      fields: fields,
    //      constructors: constructors,
    //      methods: methods,
    //      properties: properties,
    //      events: events);
    //}

    //private TypeInstantiation CreateTypeInstantion (ITypeInstantiator typeInstantiator, Type genericTypeDefinition)
    //{
    //  var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
    //  var underlyingTypeFactory = new ThrowingUnderlyingTypeFactory();
    //  return new TypeInstantiation (memberSelector, underlyingTypeFactory, typeInstantiator, genericTypeDefinition);
    //}

    //private void StubBaseTypeAdjustment (Type genericTypeDefinition)
    //{
    //  var fakeBaseType = ReflectionObjectMother.GetSomeType();
    //  _typeInstantiatorMock.Stub (stub => stub.SubstituteGenericParameters (genericTypeDefinition.BaseType)).Return (fakeBaseType);
    //  _typeInstantiatorMock.Stub (stub => stub.GetFullName (genericTypeDefinition)).Return ("full name");
    //  _typeInstantiatorMock.Stub (stub => stub.TypeArguments).Return (new[] { ReflectionObjectMother.GetSomeType() });
    //}
  }
}