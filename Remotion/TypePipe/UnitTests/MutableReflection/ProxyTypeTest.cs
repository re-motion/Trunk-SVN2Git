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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.MutableReflection.Implementation.MemberFactory;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class ProxyTypeTest
  {
    private const BindingFlags c_all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private IMemberSelector _memberSelectorMock;
    private IInterfaceMappingComputer _interfaceMappingComputerMock;
    private IMutableMemberFactory _mutableMemberFactoryMock;

    private ProxyType _proxyType;
    private ProxyType _proxyTypeWithoutMocks;

    [SetUp]
    public void SetUp ()
    {
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();
      _interfaceMappingComputerMock = MockRepository.GenerateStrictMock<IInterfaceMappingComputer>();
      _mutableMemberFactoryMock = MockRepository.GenerateStrictMock<IMutableMemberFactory>();

      _proxyType = ProxyTypeObjectMother.Create (
          baseType: typeof (DomainType),
          memberSelector: _memberSelectorMock,
          interfaceMappingComputer: _interfaceMappingComputerMock,
          mutableMemberFactory: _mutableMemberFactoryMock);

      _proxyTypeWithoutMocks = ProxyTypeObjectMother.Create (baseType: typeof (DomainType));
    }

    [Test]
    public void Initialization ()
    {
      var baseType = ReflectionObjectMother.GetSomeSubclassableType();
      var name = "abc";
      var @namespace = "def";
      var fullname = "hij";
      var attributes = (TypeAttributes) 7;

      var proxyType = new ProxyType (
          _memberSelectorMock, baseType, name, @namespace, fullname, attributes, _interfaceMappingComputerMock, _mutableMemberFactoryMock);

      Assert.That (proxyType.DeclaringType, Is.Null);
      Assert.That (proxyType.MutableDeclaringType, Is.Null);
      Assert.That (proxyType.BaseType, Is.SameAs (baseType));
      Assert.That (proxyType.Name, Is.EqualTo (name));
      Assert.That (proxyType.Namespace, Is.EqualTo (@namespace));
      Assert.That (proxyType.FullName, Is.EqualTo (fullname));
      _memberSelectorMock.Stub (mock => mock.SelectMethods<MethodInfo> (null, 0, null)).IgnoreArguments().Return (new MethodInfo[0]);
      Assert.That (proxyType.Attributes, Is.EqualTo (attributes));
      Assert.That (proxyType.IsGenericType, Is.False);
      Assert.That (proxyType.IsGenericTypeDefinition, Is.False);
      Assert.That (proxyType.GetGenericArguments(), Is.Empty);

      Assert.That (proxyType.AddedCustomAttributes, Is.Empty);
      Assert.That (proxyType.Initializations, Is.Empty);
      Assert.That (proxyType.AddedInterfaces, Is.Empty);
      Assert.That (proxyType.AddedFields, Is.Empty);
      Assert.That (proxyType.AddedConstructors, Is.Empty);
      Assert.That (proxyType.AddedMethods, Is.Empty);
      Assert.That (proxyType.AddedProperties, Is.Empty);
      Assert.That (proxyType.AddedEvents, Is.Empty);
    }

    [Test]
    public void Initialization_ThrowsIfUnderlyingTypeCannotBeSubclassed ()
    {
      var msg = "Proxied type must not be sealed, an interface, a value type, an enum, a delegate, an array, a byref type, a pointer, "
                + "a generic parameter, contain generic parameters and must have an accessible constructor.\r\nParameter name: baseType";
      // sealed
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (string)), Throws.ArgumentException.With.Message.EqualTo (msg));
      // interface
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (IDisposable)), Throws.ArgumentException.With.Message.EqualTo (msg));
      // value type
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (int)), Throws.ArgumentException.With.Message.EqualTo (msg));
      // enum
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (ExpressionType)), Throws.ArgumentException.With.Message.EqualTo (msg));
      // delegate
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (Delegate)), Throws.ArgumentException.With.Message.EqualTo (msg));
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (MulticastDelegate)), Throws.ArgumentException.With.Message.EqualTo (msg));
      // open generics
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (List<>)), Throws.ArgumentException.With.Message.EqualTo (msg));
      // closed generics
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (List<int>)), Throws.Nothing);
      // generic parameter
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (List<>).GetGenericArguments ().Single ()), Throws.ArgumentException.With.Message.EqualTo (msg));
      // array
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (int).MakeArrayType ()), Throws.ArgumentException.With.Message.EqualTo (msg));
      // by ref
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (int).MakeByRefType ()), Throws.ArgumentException.With.Message.EqualTo (msg));
      // pointer
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (int).MakePointerType ()), Throws.ArgumentException.With.Message.EqualTo (msg));
      // no accessible ctor
      Assert.That (() => ProxyTypeObjectMother.Create (baseType: typeof (TypeWithoutAccessibleConstructor)), Throws.ArgumentException.With.Message.EqualTo (msg));
    }

    [Test]
    public void CustomAttributeMethods ()
    {
      var declaration = CustomAttributeDeclarationObjectMother.Create (typeof (ObsoleteAttribute));
      _proxyType.AddCustomAttribute (declaration);

      Assert.That (_proxyType.AddedCustomAttributes, Is.EqualTo (new[] { declaration }));
      Assert.That (_proxyType.GetCustomAttributeData().Select (a => a.Type), Is.EquivalentTo (new[] { typeof (ObsoleteAttribute) }));
    }

    [Test]
    public void AddTypeInitializer ()
    {
      Assert.That (_proxyType.MutableTypeInitializer, Is.Null);
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = ctx => null;
      var typeInitializerFake = MutableConstructorInfoObjectMother.Create (attributes: MethodAttributes.Static);
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.CreateConstructor (
                  _proxyType, MethodAttributes.Private | MethodAttributes.Static, ParameterDeclaration.None, bodyProvider))
          .Return (typeInitializerFake);

      var result = _proxyType.AddTypeInitializer (bodyProvider);

      Assert.That (result, Is.SameAs (typeInitializerFake));
      Assert.That (_proxyType.AddedConstructors, Is.Empty);
      Assert.That (_proxyType.MutableTypeInitializer, Is.SameAs (typeInitializerFake));
    }

    [Test]
    public void AddInitialization ()
    {
      Func<InitializationBodyContext, Expression> initializationProvider = ctx => null;

      var fakeExpression = ExpressionTreeObjectMother.GetSomeExpression();
      _mutableMemberFactoryMock.Expect (mock => mock.CreateInitialization (_proxyType, initializationProvider)).Return (fakeExpression);

      _proxyType.AddInitialization (initializationProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (_proxyType.Initializations, Is.EqualTo (new[] { fakeExpression }));
    }

    [Test]
    public void AddInterface ()
    {
      var baseInterface = typeof (DomainType).GetInterfaces().Single();
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();

      _proxyType.AddInterface (addedInterface);
      Assert.That (_proxyType.AddedInterfaces, Is.EqualTo (new[] { addedInterface }));
      Assert.That (_proxyType.GetInterfaces(), Is.EqualTo (new[] { addedInterface, baseInterface }));

      _proxyType.AddInterface (baseInterface); // Base interface can be re-implemented.
      Assert.That (_proxyType.AddedInterfaces, Is.EqualTo (new[] { addedInterface, baseInterface }));
      Assert.That (_proxyType.GetInterfaces(), Is.EqualTo (new[] { addedInterface, baseInterface }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be an interface.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfNotAnInterface ()
    {
      _proxyType.AddInterface (typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Interface 'IDisposable' is already implemented.\r\nParameter name: interfaceType")]
    public void AddInterface_ThrowsIfAlreadyImplemented ()
    {
      _proxyType.AddInterface (typeof (IDisposable));
      _proxyType.AddInterface (typeof (IDisposable));
    }

    [Test]
    public void AddField ()
    {
      var name = "_newField";
      var type = ReflectionObjectMother.GetSomeType();
      var attributes = (FieldAttributes) 7;
      var fakeField = MutableFieldInfoObjectMother.Create (_proxyType);
      _mutableMemberFactoryMock.Expect (mock => mock.CreateField (_proxyType, name, type, attributes)).Return (fakeField);

      var result = _proxyType.AddField (name, attributes, type);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeField));
      Assert.That (_proxyType.AddedFields, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddConstructor ()
    {
      var attributes = (MethodAttributes) 7;
      var parameters = ParameterDeclarationObjectMother.CreateMultiple (2);
      Func<ConstructorBodyCreationContext, Expression> bodyProvider = ctx => null;
      var fakeConstructor = MutableConstructorInfoObjectMother.Create();
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateConstructor (_proxyType, attributes, parameters, bodyProvider))
          .Return (fakeConstructor);

      var result = _proxyType.AddConstructor (attributes, parameters, bodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeConstructor));
      Assert.That (_proxyType.AddedConstructors, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddConstructor_Static ()
    {
      Assert.That (_proxyType.MutableTypeInitializer, Is.Null);
      var typeInitializerFake = MutableConstructorInfoObjectMother.Create (attributes: MethodAttributes.Static);
      _mutableMemberFactoryMock.Stub (stub => stub.CreateConstructor (null, 0, null, null)).IgnoreArguments().Return (typeInitializerFake);

      var result = _proxyType.AddConstructor (0, ParameterDeclaration.None, ctx => Expression.Empty());

      Assert.That (result, Is.SameAs (typeInitializerFake));
      Assert.That (_proxyType.AddedConstructors, Is.Empty);
      Assert.That (_proxyType.MutableTypeInitializer, Is.SameAs (typeInitializerFake));
    }

    [Test]
    public void AddMethod ()
    {
      var name = "Method";
      var attributes = MethodAttributes.Public;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameters = ParameterDeclarationObjectMother.CreateMultiple (2);
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx => null;
      var fakeMethod = MutableMethodInfoObjectMother.Create();
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateMethod (_proxyType, name, attributes, returnType, parameters, bodyProvider))
          .Return (fakeMethod);

      var result = _proxyType.AddMethod (name, attributes, returnType, parameters, bodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeMethod));
      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddGenericMethod ()
    {
      var name = "GenericMethod";
      var attributes = (MethodAttributes) 7;
      var genericParameterDeclarations = new[] { GenericParameterDeclarationObjectMother.Create() };
      Func<GenericParameterContext, Type> returnTypeProvider = ctx => null;
      Func<GenericParameterContext, IEnumerable<ParameterDeclaration>> parameterProvider = ctx => null;
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx => null;
      var fakeMethod = MutableMethodInfoObjectMother.Create();
      _mutableMemberFactoryMock
          .Expect (
              mock =>
              mock.CreateMethod (_proxyType, name, attributes, genericParameterDeclarations, returnTypeProvider, parameterProvider, bodyProvider))
          .Return (fakeMethod);

      var result = _proxyType.AddGenericMethod (name, attributes, genericParameterDeclarations, returnTypeProvider, parameterProvider, bodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeMethod));
      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddExplicitOverride ()
    {
      var method = ReflectionObjectMother.GetSomeMethod();
      Func<MethodBodyCreationContext, Expression> bodyProvider = ctx => null;
      var fakeMethod = MutableMethodInfoObjectMother.Create();
      _mutableMemberFactoryMock.Expect (mock => mock.CreateExplicitOverride (_proxyType, method, bodyProvider)).Return (fakeMethod);

      var result = _proxyType.AddExplicitOverride (method, bodyProvider);

      Assert.That (result, Is.SameAs (fakeMethod));
      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void GetOrAddOverride_CreatesNewOverride ()
    {
      var baseMethod = typeof (DomainType).GetMethod ("ToString");
      var fakeOverride = MutableMethodInfoObjectMother.Create();
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.GetOrCreateOverride (
                  Arg.Is (_proxyType),
                  Arg.Is (baseMethod),
                  out Arg<bool>.Out (true).Dummy))
          .Return (fakeOverride);

      var result = _proxyType.GetOrAddOverride (baseMethod);

      Assert.That (result, Is.SameAs (fakeOverride));
      Assert.That (_proxyType.AddedMethods, Has.Member (result));
    }

    [Test]
    public void GetOrAddOverride_RetrievesExistingOverride ()
    {
      var baseMethod = typeof (DomainType).GetMethod ("ToString");
      var fakeOverride = MutableMethodInfoObjectMother.Create();
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.GetOrCreateOverride (
                  Arg.Is (_proxyType),
                  Arg.Is (baseMethod),
                  out Arg<bool>.Out (false).Dummy))
          .Return (fakeOverride);

      var result = _proxyType.GetOrAddOverride (baseMethod);

      Assert.That (result, Is.SameAs (fakeOverride));
      Assert.That (_proxyType.AddedMethods, Has.No.Member (result));
    }

    [Test]
    public void AddProperty_Simple ()
    {
      var name = "Property";
      var type = ReflectionObjectMother.GetSomeType();
      var indexParameters = ParameterDeclarationObjectMother.CreateMultiple (2);
      var accessorAttributes = (MethodAttributes) 7;
      Func<MethodBodyCreationContext, Expression> getBodyProvider = ctx => null;
      Func<MethodBodyCreationContext, Expression> setBodyProvider = ctx => null;
      var fakeProperty = MutablePropertyInfoObjectMother.CreateReadWrite();
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateProperty (_proxyType, name, type, indexParameters, accessorAttributes, getBodyProvider, setBodyProvider))
          .Return (fakeProperty);

      var result = _proxyType.AddProperty (name, type, indexParameters, accessorAttributes, getBodyProvider, setBodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeProperty));
      Assert.That (_proxyType.AddedProperties, Is.EqualTo (new[] { result }));
      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { result.MutableGetMethod, result.MutableSetMethod }));
      Assert.That (result.MutableGetMethod.Attributes, Is.EqualTo (accessorAttributes));
      Assert.That (result.MutableSetMethod.Attributes, Is.EqualTo (accessorAttributes));
    }

    [Test]
    public void AddProperty_Simple_ReadOnlyProperty ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      Func<MethodBodyCreationContext, Expression> getBodyProvider = ctx => null;
      var fakeGetMethod = MutableMethodInfoObjectMother.Create (returnType: type);
      var fakeProperty = MutablePropertyInfoObjectMother.Create (getMethod: fakeGetMethod);
      Assert.That (fakeProperty.MutableSetMethod, Is.Null);
      _mutableMemberFactoryMock
          .Stub (stub => stub.CreateProperty (_proxyType, "Property", type, ParameterDeclaration.None, MethodAttributes.Public, getBodyProvider, null))
          .Return (fakeProperty);

      _proxyType.AddProperty ("Property", type, getBodyProvider: getBodyProvider);

      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { fakeGetMethod }));
    }

    [Test]
    public void AddProperty_Simple_WriteOnlyProperty ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      Func<MethodBodyCreationContext, Expression> setBodyProvider = ctx => null;
      var fakeSetMethod = MutableMethodInfoObjectMother.Create (parameters: new[] { ParameterDeclarationObjectMother.Create (type) });
      var fakeProperty = MutablePropertyInfoObjectMother.Create (setMethod: fakeSetMethod);
      Assert.That (fakeProperty.MutableGetMethod, Is.Null);
      _mutableMemberFactoryMock
        .Stub (stub => stub.CreateProperty (_proxyType, "Property", type, ParameterDeclaration.None, MethodAttributes.Public, null, setBodyProvider))
        .Return (fakeProperty);

      _proxyType.AddProperty ("Property", type, setBodyProvider: setBodyProvider);

      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { fakeSetMethod }));
    }

    [Test]
    public void AddProperty_Complex ()
    {
      var name = "Property";
      var attributes = (PropertyAttributes) 7;
      var getMethod = MutableMethodInfoObjectMother.Create (attributes: MethodAttributes.Static);
      var setMethod = MutableMethodInfoObjectMother.Create (attributes: MethodAttributes.Static);
      var fakeProperty = MutablePropertyInfoObjectMother.Create();
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateProperty (_proxyType, name, attributes, getMethod, setMethod))
          .Return (fakeProperty);

      var result = _proxyType.AddProperty (name, attributes, getMethod, setMethod);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeProperty));
      Assert.That (_proxyType.AddedProperties, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddEvent_Simple ()
    {
      var name = "Event";
      var handlerType = ReflectionObjectMother.GetSomeType();
      var accessorAttributes = (MethodAttributes) 7;
      Func<MethodBodyCreationContext, Expression> addBodyProvider = ctx => null;
      Func<MethodBodyCreationContext, Expression> removeBodyProvider = ctx => null;
      Func<MethodBodyCreationContext, Expression> raiseBodyProvider = ctx => null;

      var fakeEvent = MutableEventInfoObjectMother.CreateWithAccessors (createRaiseMethod: true);
      _mutableMemberFactoryMock
          .Expect (
              mock => mock.CreateEvent (_proxyType, name, handlerType, accessorAttributes, addBodyProvider, removeBodyProvider, raiseBodyProvider))
          .Return (fakeEvent);

      var result = _proxyType.AddEvent (name, handlerType, accessorAttributes, addBodyProvider, removeBodyProvider, raiseBodyProvider);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeEvent));
      Assert.That (_proxyType.AddedEvents, Is.EqualTo (new[] { result }));
      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { result.MutableAddMethod, result.MutableRemoveMethod, result.MutableRaiseMethod }));
    }

    [Test]
    public void AddEvent_Simple_NoRaiseBodyProvider ()
    {
      var handlerType = ReflectionObjectMother.GetSomeType();
      Func<MethodBodyCreationContext, Expression> addBodyProvider = ctx => null;
      Func<MethodBodyCreationContext, Expression> removeBodyProvider = ctx => null;

      var addRemoveParameters = new[] { new ParameterDeclaration (typeof (Func<int, string>), "handler") };
      var addMethod = MutableMethodInfoObjectMother.Create (returnType: typeof (void), parameters: addRemoveParameters);
      var removeMethod = MutableMethodInfoObjectMother.Create (returnType: typeof (void), parameters: addRemoveParameters);
      var fakeEvent = MutableEventInfoObjectMother.Create (addMethod: addMethod, removeMethod: removeMethod);
      Assert.That (fakeEvent.MutableRaiseMethod, Is.Null);
      _mutableMemberFactoryMock
        .Stub (stub => stub.CreateEvent (null, null, null, 0, null, null, null))
        .IgnoreArguments()
        .Return (fakeEvent);

      var result = _proxyType.AddEvent ("Event", handlerType, addBodyProvider: addBodyProvider, removeBodyProvider: removeBodyProvider);

      Assert.That (_proxyType.AddedMethods, Is.EqualTo (new[] { result.MutableAddMethod, result.MutableRemoveMethod }));
    }

    [Test]
    public void AddEvent_Complex ()
    {
      var eventAttributes = EventAttributes.SpecialName;
      var addMethod = MutableMethodInfoObjectMother.Create();
      var removeMethod = MutableMethodInfoObjectMother.Create();
      var raiseMethod = MutableMethodInfoObjectMother.Create();
      var fakeEvent = MutableEventInfoObjectMother.CreateWithAccessors();
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateEvent (_proxyType, "Event", eventAttributes, addMethod, removeMethod, raiseMethod))
          .Return (fakeEvent);

      var result = _proxyType.AddEvent ("Event", eventAttributes, addMethod, removeMethod, raiseMethod);

      _mutableMemberFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeEvent));
      Assert.That (_proxyType.AddedEvents, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void GetInterfaceMap ()
    {
      var interfaceType = typeof (IDomainInterface);
      var fakeResult = new InterfaceMapping { InterfaceType = ReflectionObjectMother.GetSomeType() };
      _interfaceMappingComputerMock
          .Expect (mock => mock.ComputeMapping (_proxyType, typeof (DomainType).GetInterfaceMap, interfaceType, false))
          .Return (fakeResult);

      var result = _proxyType.GetInterfaceMap (interfaceType);

      _interfaceMappingComputerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult), "Interface mapping is a struct, therefore we must use EqualTo and a non-empty struct.");
    }

    [Test]
    public void GetInterfaceMap_AllowPartialInterfaceMapping ()
    {
      var interfaceType = typeof (IDomainInterface);
      var allowPartial = BooleanObjectMother.GetRandomBoolean();
      var fakeResult = new InterfaceMapping { InterfaceType = ReflectionObjectMother.GetSomeType() };
      _interfaceMappingComputerMock
          .Expect (mock => mock.ComputeMapping (_proxyType, typeof (DomainType).GetInterfaceMap, interfaceType, allowPartial))
          .Return (fakeResult);

      var result = _proxyType.GetInterfaceMap (interfaceType, allowPartial);

      _interfaceMappingComputerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult), "Interface mapping is a struct, therefore we must use EqualTo and a non-empty struct.");
    }

    [Test]
    public void GetAttributeFlagsImpl_Serializable ()
    {
      var proxyType = ProxyTypeObjectMother.Create (memberSelector: _memberSelectorMock);
      _memberSelectorMock.Stub (stub => stub.SelectMethods<MethodInfo> (null, 0, null)).IgnoreArguments().Return (new MethodInfo[0]);
      Assert.That (proxyType.IsTypePipeSerializable(), Is.False);

      proxyType.AddCustomAttribute (CustomAttributeDeclarationObjectMother.Create (typeof (SerializableAttribute)));

      Assert.That (proxyType.IsTypePipeSerializable(), Is.True);
    }

    [Test]
    public void GetAttributeFlagsImpl_Abstract ()
    {
      var allMethods = GetAllMethods (_proxyType);
      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var fakeMethods = new[] { ReflectionObjectMother.GetSomeAbstractMethod() };
      _memberSelectorMock
          .Expect (mock => mock.SelectMethods (Arg<IEnumerable<MethodInfo>>.List.Equal (allMethods), Arg.Is (bindingFlags), Arg.Is ((_proxyType))))
          .Return (fakeMethods).Repeat.Times (2);

      Assert.That (_proxyType.IsAbstract, Is.True);
      Assert.That (_proxyType.Attributes, Is.EqualTo (TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Abstract));
      _memberSelectorMock.VerifyAllExpectations();
    }

    [Test]
    public void GetAttributeFlagsImpl_NonAbstract ()
    {
      var proxyType = ProxyTypeObjectMother.Create (baseType: typeof (AbstractType));
      Assert.That (proxyType.IsAbstract, Is.True);

      var abstractMethodBaseDefinition = NormalizingMemberInfoFromExpressionUtility.GetMethod ((AbstractTypeBase obj) => obj.AbstractMethod1());
      var abstractMethod1 = proxyType.GetMethod ("AbstractMethod1");
      var abstractMethod2 = proxyType.GetMethod ("AbstractMethod2");
      Assert.That (abstractMethod1, Is.Not.EqualTo (abstractMethodBaseDefinition));
      Assert.That (abstractMethod1.GetBaseDefinition(), Is.EqualTo (abstractMethodBaseDefinition));

      proxyType.AddExplicitOverride (abstractMethodBaseDefinition, ctx => Expression.Empty());
      proxyType.AddMethod (attributes: MethodAttributes.Virtual).AddExplicitBaseDefinition (abstractMethod2);

      Assert.That (proxyType.IsAbstract, Is.False);
      Assertion.IsNotNull (proxyType.BaseType);
      Assert.That (proxyType.BaseType.IsAbstract, Is.True);
      Assert.That (proxyType.Attributes & TypeAttributes.Abstract, Is.Not.EqualTo (TypeAttributes.Abstract));
    }

    [Test]
    public void GetAllInterfaces ()
    {
      var baseInterfaces = typeof (DomainType).GetInterfaces();
      Assert.That (baseInterfaces, Is.Not.Empty);
      var addedInterface = ReflectionObjectMother.GetSomeInterfaceType();
      _proxyType.AddInterface (addedInterface);

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyType, "GetAllInterfaces");

      Assert.That (result, Is.EquivalentTo (new[] { addedInterface }.Concat (baseInterfaces)));
    }

    [Test]
    public void GetAllInterfaces_Distinct ()
    {
      var baseInterface = typeof (DomainType).GetInterfaces().Single();
      _proxyType.AddInterface (baseInterface);

      Assert.That (_proxyType.GetInterfaces().Count (ifc => ifc == baseInterface), Is.EqualTo (1));
    }

    [Test]
    public void GetAllFields ()
    {
      var baseFields = typeof (DomainType).GetFields (c_all);
      Assert.That (baseFields, Is.Not.Empty);
      var addedField = _proxyTypeWithoutMocks.AddField();

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyTypeWithoutMocks, "GetAllFields");

      Assert.That (result, Is.EquivalentTo (new[] { addedField }.Concat (baseFields)));
    }

    [Test]
    public void GetAllConstructors ()
    {
      var baseCtors = typeof (DomainType).GetConstructors (c_all);
      Assert.That (baseCtors, Is.Not.Empty);
      var addedCtor = _proxyTypeWithoutMocks.AddConstructor();

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyTypeWithoutMocks, "GetAllConstructors");

      Assert.That (result, Is.EqualTo (new[] { addedCtor }));
    }

    [Test]
    public void GetAllConstructors_TypeInitializer ()
    {
      var addedTypeInitializer = _proxyTypeWithoutMocks.AddConstructor (MethodAttributes.Static);
      var addedCtor = _proxyTypeWithoutMocks.AddConstructor();

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyTypeWithoutMocks, "GetAllConstructors");

      Assert.That (result, Is.EqualTo (new[] { addedCtor, addedTypeInitializer }));
    }

    [Test]
    public void GetAllMethods ()
    {
      var baseMethods = typeof (DomainType).GetMethods (c_all);
      var addedMethod = _proxyTypeWithoutMocks.AddMethod();

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyTypeWithoutMocks, "GetAllMethods");

      Assert.That (result, Is.EquivalentTo (new[] { addedMethod }.Concat (baseMethods)));
    }

    [Test]
    public void GetAllMethods_FiltersOverriddenMethods ()
    {
      var baseMethod = typeof (DomainType).GetMethod ("ToString");
      var fakeOverride = MutableMethodInfoObjectMother.Create (
          declaringType: _proxyType,
          name: baseMethod.Name,
          attributes: baseMethod.Attributes,
          parameters: ParameterDeclaration.None,
          baseMethod: baseMethod);
      _mutableMemberFactoryMock
          .Expect (mock => mock.CreateMethod (null, null, 0, null, null, null))
          .IgnoreArguments()
          .Return (fakeOverride);
      _proxyType.AddMethod ("in", 0, typeof (int), ParameterDeclaration.None, ctx => null);

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyType, "GetAllMethods");

      _memberSelectorMock.VerifyAllExpectations();
      Assert.That (result, Has.Member (fakeOverride));
      Assert.That (result, Has.No.Member (baseMethod));
    }

    [Test]
    public void GetAllProperties ()
    {
      var baseProperties = typeof (DomainType).GetProperties (c_all);
      Assert.That (baseProperties, Is.Not.Empty);
      var addedProperty = _proxyTypeWithoutMocks.AddProperty();

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyTypeWithoutMocks, "GetAllProperties");

      Assert.That (result, Is.EquivalentTo (new[] { addedProperty }.Concat (baseProperties)));
    }

    [Test]
    public void GetAllEvents ()
    {
      var baseEvents = typeof (DomainType).GetEvents (c_all);
      Assert.That (baseEvents, Is.Not.Empty);
      var addedEvent = _proxyTypeWithoutMocks.AddEvent();

      var result = PrivateInvoke.InvokeNonPublicMethod (_proxyTypeWithoutMocks, "GetAllEvents");

      Assert.That (result, Is.EquivalentTo (new[] { addedEvent }.Concat (baseEvents)));
    }

    [Test]
    public void ToDebugString ()
    {
      // Note: ToDebugString() is implemented in CustomType base class.
      Assert.That (_proxyType.ToDebugString(), Is.EqualTo ("ProxyType = \"Proxy\""));
    }

    private IEnumerable<MethodInfo> GetAllMethods (ProxyType proxyType)
    {
      return (IEnumerable<MethodInfo>) PrivateInvoke.InvokeNonPublicMethod (proxyType, "GetAllMethods");
    }

    public class DomainTypeBase
    {
      public int BaseField;

      public string ExplicitOverrideTarget (double d) { return "" + d; }
    }
    public interface IDomainInterface { }
    public class DomainType : DomainTypeBase, IDomainInterface
    {
      public int Field;

      public int Property { get; set; }

      public event EventHandler Event;

      public virtual string VirtualMethod () { return ""; }
      public void NonVirtualMethod () { }
    }

    public abstract class AbstractTypeBase
    {
      public abstract void AbstractMethod1 ();
    }
    public abstract class AbstractType : AbstractTypeBase
    {
      public override abstract void AbstractMethod1 ();
      public abstract void AbstractMethod2 ();
    }

    public class TypeWithoutAccessibleConstructor
    {
      internal TypeWithoutAccessibleConstructor () { }
    }

    class TypeWithMyInterface : IMyInterface { }
    interface IMyInterface { }
  }
}