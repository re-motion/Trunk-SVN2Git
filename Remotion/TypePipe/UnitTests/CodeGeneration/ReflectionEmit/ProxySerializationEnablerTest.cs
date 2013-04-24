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
using System.Runtime.Serialization;
using Remotion.TypePipe.Dlr.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.Serialization.Implementation;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class ProxySerializationEnablerTest
  {
    private ISerializableFieldFinder _serializableFieldFinderMock;
    
    private ProxySerializationEnabler _enabler;

    private MutableType _someProxy;
    private MutableType _serializableInterfaceProxy;
    private MutableType _serializableProxy;
    private MutableType _deserializationCallbackProxy;
    private MutableType _serializableInterfaceWithDeserializationCallbackProxy;

    private MethodInfo _someInitializationMethod;

    [SetUp]
    public void SetUp ()
    {
      _serializableFieldFinderMock = MockRepository.GenerateStrictMock<ISerializableFieldFinder>();

      _enabler = new ProxySerializationEnabler (_serializableFieldFinderMock);

      _someProxy = MutableTypeObjectMother.Create (typeof (SomeType));
      _serializableProxy = MutableTypeObjectMother.Create (typeof (SomeType), attributes: TypeAttributes.Serializable);
      _serializableInterfaceProxy = MutableTypeObjectMother.Create (typeof (SerializableInterfaceType), copyCtorsFromBase: true);
      _deserializationCallbackProxy = MutableTypeObjectMother.Create (typeof (DeserializationCallbackType));
      _serializableInterfaceWithDeserializationCallbackProxy =
          MutableTypeObjectMother.Create (baseType: typeof (SerializableWithDeserializationCallbackType), attributes: TypeAttributes.Serializable);

      _someInitializationMethod = ReflectionObjectMother.GetSomeInstanceMethod();
    }

    [Test]
    public void MakeSerializable_SerializableInterfaceType ()
    {
      StubFilterWithNoSerializedFields();

      _enabler.MakeSerializable (_serializableInterfaceProxy, _someInitializationMethod);

      Assert.That (_serializableInterfaceProxy.AddedInterfaces, Is.Empty);
      Assert.That (_serializableInterfaceProxy.AddedMethods, Is.Empty);
    }

    [Test]
    public void MakeSerializable_SerializableInterfaceType_SerializedFields ()
    {
      var dummyField = _serializableInterfaceProxy.AddField ("input field", FieldAttributes.Private, typeof (int));
      var fakeFieldType = ReflectionObjectMother.GetSomeType();
      FieldInfo fakeField = MutableFieldInfoObjectMother.Create (_serializableInterfaceProxy, type: fakeFieldType);
      var fakeMapping = new[] { Tuple.Create ("fake key", fakeField) };
      _serializableFieldFinderMock
          .Expect (mock => mock.GetSerializableFieldMapping (Arg<IEnumerable<FieldInfo>>.List.Equal (new[] { dummyField })))
          .Return (fakeMapping);

      _enabler.MakeSerializable (_serializableInterfaceProxy, _someInitializationMethod);

      _serializableFieldFinderMock.VerifyAllExpectations();
      Assert.That (_serializableInterfaceProxy.AddedInterfaces, Is.Empty);
      Assert.That (_serializableInterfaceProxy.AddedMethods, Has.Count.EqualTo (1));

      var baseMethod =
          NormalizingMemberInfoFromExpressionUtility.GetMethod ((SerializableInterfaceType obj) => obj.GetObjectData (null, new StreamingContext()));
      var method = _serializableInterfaceProxy.AddedMethods.Single();
      var expectedMethodBody = Expression.Block (
          typeof (void),
          Expression.Call (
              new ThisExpression (_serializableInterfaceProxy),
              NonVirtualCallMethodInfoAdapter.Adapt (baseMethod),
              method.ParameterExpressions.Cast<Expression>()),
          Expression.Call (
              method.ParameterExpressions[0],
              "AddValue",
              Type.EmptyTypes,
              Expression.Constant ("fake key"),
              Expression.Field (new ThisExpression (_serializableInterfaceProxy), fakeField)));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedMethodBody, method.Body);

      var baseCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new SerializableInterfaceType (null, new StreamingContext()));
      var ctor = _serializableInterfaceProxy.AddedConstructors.Single();
      var getValueMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((SerializationInfo obj) => obj.GetValue ("", null));
      var expectedCtorBody = Expression.Block (
          typeof (void),
          Expression.Call (
              new ThisExpression (_serializableInterfaceProxy),
              NonVirtualCallMethodInfoAdapter.Adapt (baseCtor),
              ctor.ParameterExpressions.Cast<Expression>()),
          Expression.Assign (
              Expression.Field (new ThisExpression (_serializableInterfaceProxy), fakeField),
              Expression.Convert (
                  Expression.Call (ctor.ParameterExpressions[0], getValueMethod, Expression.Constant ("fake key"), Expression.Constant (fakeFieldType)),
                  fakeFieldType)));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedCtorBody, ctor.Body);
    }

    [Test]
    public void MakeSerializable_SomeType_SerializedFields ()
    {
      StubFilterWithSerializedFields (_someProxy);

      _enabler.MakeSerializable (_someProxy, _someInitializationMethod);

      Assert.That (_someProxy.AddedInterfaces, Is.Empty);
      Assert.That (_someProxy.AddedMethods, Is.Empty);
    }

    [Test]
    public void MakeSerializable_SomeType ()
    {
      StubFilterWithNoSerializedFields();

      _enabler.MakeSerializable (_someProxy, initializationMethod: null);

      Assert.That (_someProxy.AddedInterfaces, Is.Empty);
      Assert.That (_someProxy.AddedMethods, Is.Empty);
    }

    [Test]
    public void MakeSerializable_SerializableType_WithInitializations ()
    {
      StubFilterWithNoSerializedFields();
      var initMethod = MutableMethodInfoObjectMother.Create (
          _serializableProxy, returnType: typeof (void), parameters: ParameterDeclaration.None);

      _enabler.MakeSerializable (_serializableProxy, initMethod);

      Assert.That (_serializableProxy.AddedInterfaces, Is.EqualTo (new[] { typeof (IDeserializationCallback) }));
      Assert.That (_serializableProxy.AddedMethods, Has.Count.EqualTo (1));

      var method = _serializableProxy.AddedMethods.Single();
      Assert.That (method.Name, Is.EqualTo ("System.Runtime.Serialization.IDeserializationCallback.OnDeserialization"));
      Assert.That (method.GetParameters ().Select (p => p.ParameterType), Is.EqualTo (new[] { typeof (object) }));
      var expectedBody = MethodCallExpression.Call (new ThisExpression (_serializableProxy), initMethod);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);
    }

    [Test]
    public void MakeSerializable_DeserializationCallbackType_WithInitializations ()
    {
      StubFilterWithNoSerializedFields();
      var initMethod = MutableMethodInfoObjectMother.Create (
          _deserializationCallbackProxy, returnType: typeof (void), parameters: ParameterDeclaration.None);
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DeserializationCallbackType obj) => obj.OnDeserialization (null));

      _enabler.MakeSerializable (_deserializationCallbackProxy, initMethod);

      Assert.That (_deserializationCallbackProxy.AddedMethods, Has.Count.EqualTo (1));
      var method = _deserializationCallbackProxy.AddedMethods.Single();
      var expectedBody = Expression.Block (
          typeof (void),
          Expression.Call (
              new ThisExpression (_deserializationCallbackProxy),
              new NonVirtualCallMethodInfoAdapter (baseMethod),
              method.ParameterExpressions.Cast<Expression>()),
          MethodCallExpression.Call (new ThisExpression (_deserializationCallbackProxy), initMethod));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);
    }

    [Test]
    public void MakeSerializable_SerializableWithDeserializationCallbackType ()
    {
      StubFilterWithNoSerializedFields();

      _enabler.MakeSerializable (_serializableInterfaceWithDeserializationCallbackProxy, initializationMethod: null);

      Assert.That (_serializableInterfaceWithDeserializationCallbackProxy.AddedInterfaces, Is.Empty);
      Assert.That (_serializableInterfaceWithDeserializationCallbackProxy.AddedMethods, Is.Empty);
    }

    [Test]
    public void MakeSerializable_SerializableWithDeserializationCallbackType_WithInitializations ()
    {
      StubFilterWithNoSerializedFields();
      var initMethod = MutableMethodInfoObjectMother.Create (
          _serializableInterfaceWithDeserializationCallbackProxy, returnType: typeof (void), parameters: ParameterDeclaration.None);
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((SerializableWithDeserializationCallbackType obj) => obj.OnDeserialization (null));

      _enabler.MakeSerializable (_serializableInterfaceWithDeserializationCallbackProxy, initMethod);

      Assert.That (_serializableInterfaceWithDeserializationCallbackProxy.AddedMethods, Has.Count.EqualTo (1));
      var method = _serializableInterfaceWithDeserializationCallbackProxy.AddedMethods.Single();
      var expectedBody = Expression.Block (
          typeof (void),
          Expression.Call (
              new ThisExpression (_serializableInterfaceWithDeserializationCallbackProxy),
              new NonVirtualCallMethodInfoAdapter (baseMethod),
              method.ParameterExpressions.Cast<Expression>()),
          MethodCallExpression.Call (new ThisExpression (_serializableInterfaceWithDeserializationCallbackProxy), initMethod));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, method.Body);
    }

    [Test]
    public void MakeSerializable_ISerializable_SerializedFields_MissingCtor ()
    {
      var proxyType = MutableTypeObjectMother.Create (typeof (SerializableInterfaceType), copyCtorsFromBase: false);
      StubFilterWithSerializedFields (proxyType);

      _enabler.MakeSerializable (proxyType, _someInitializationMethod);

      Assert.That (proxyType.AddedConstructors, Is.Empty);
      Assert.That (proxyType.AddedMethods, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The proxy type implements ISerializable but GetObjectData cannot be overridden. "
        + "Make sure that GetObjectData is implemented implicitly (not explicitly) and virtual.")]
    public void MakeSerializable_ISerializable_SerializedFields_CannotOverrideGetObjectData ()
    {
      var proxyType = MutableTypeObjectMother.Create (typeof (ExplicitSerializableInterfaceType), copyCtorsFromBase: true);
      StubFilterWithSerializedFields (proxyType);

      _enabler.MakeSerializable (proxyType, _someInitializationMethod);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The proxy type implements IDeserializationCallback but OnDeserialization cannot be overridden. "
        + "Make sure that OnDeserialization is implemented implicitly (not explicitly) and virtual.")]
    public void MakeSerializable_IDeserializationCallback_CannotOverrideGetObjectData ()
    {
      var proxyType = MutableTypeObjectMother.Create (typeof (ExplicitDeserializationCallbackType));
      StubFilterWithNoSerializedFields ();

      _enabler.MakeSerializable (proxyType, _someInitializationMethod);
    }

    [Test]
    public void IsDeserializationConstructor ()
    {
      var ctor1 = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new SerializableInterfaceType (null, new StreamingContext()));
      var ctor2 = ReflectionObjectMother.GetSomeConstructor();

      Assert.That (_enabler.IsDeserializationConstructor (ctor1), Is.True);
      Assert.That (_enabler.IsDeserializationConstructor (ctor2), Is.False);
    }

    private void StubFilterWithNoSerializedFields ()
    {
      _serializableFieldFinderMock
          .Stub (stub => stub.GetSerializableFieldMapping (Arg<IEnumerable<FieldInfo>>.Is.Anything))
          .Return (new Tuple<string, FieldInfo>[0]);
    }

    private void StubFilterWithSerializedFields (MutableType declaringType)
    {
      _serializableFieldFinderMock
          .Stub (stub => stub.GetSerializableFieldMapping (Arg<IEnumerable<FieldInfo>>.Is.Anything))
          .Return (new[] { Tuple.Create<string, FieldInfo> ("someField", MutableFieldInfoObjectMother.Create (declaringType)) });
    }

    public class SomeType { }

    public class SerializableInterfaceType : ISerializable
    {
      public SerializableInterfaceType (SerializationInfo info, StreamingContext context) { Dev.Null = info; Dev.Null = context; }
      public virtual void GetObjectData (SerializationInfo info, StreamingContext context) { }
    }

    public class DeserializationCallbackType : IDeserializationCallback
    {
      public virtual void OnDeserialization (object sender) { }
    }

    public class SerializableWithDeserializationCallbackType : IDeserializationCallback
    {
      public virtual void OnDeserialization (object sender) { }
    }

    public class ExplicitSerializableInterfaceType : ISerializable
    {
      public ExplicitSerializableInterfaceType (SerializationInfo info, StreamingContext context) { Dev.Null = info; Dev.Null = context; }
      void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context) { }
    }

    public class ExplicitDeserializationCallbackType : IDeserializationCallback
    {
      void IDeserializationCallback.OnDeserialization (object sender) { }
    }
  }
}