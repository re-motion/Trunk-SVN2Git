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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.Implementation;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.TypePipe.UnitTests.Caching
{
  [TestFixture]
  public class TypeCacheTest
  {
    private readonly Type _generatedType1 = typeof (GeneratedType1);
    private readonly Type _generatedType2 = typeof (GeneratedType2);
    private readonly Delegate _delegate1 = new Func<int> (() => 7);
    private readonly Delegate _delegate2 = new Func<string> (() => "");
    private Type _requestedType;
    private Type _delegateType;
    private bool _allowNonPublic;
    private Tuple<Type[], Type> _fakeSignature;
    private ConstructorInfo _fakeConstructor;

    private ITypeAssembler _typeAssemblerMock;
    private ITypeAssemblyContextCodeGenerator _typeAssemblyContextCodeGeneratorMock;
    private IConstructorFinder _constructorFinderMock;
    private IDelegateFactory _delegateFactoryMock;
    private ICodeGenerator _fakeCodeGenerator;

    private TypeCache _cache;

    private Func<ICacheKeyProvider, Type, object> _fromRequestedTypeFunc;
    private Func<ICacheKeyProvider, Type, object> _fromGeneratedTypeFunc;

    private object _lockObject;
    private Dictionary<object[], Type> _types;
    private Dictionary<object[], Delegate> _constructorCalls;
    private IDictionary<string, object> _participantState;

    [SetUp]
    public void SetUp ()
    {
      _typeAssemblerMock = MockRepository.GenerateStrictMock<ITypeAssembler>();
      _typeAssemblyContextCodeGeneratorMock = MockRepository.GenerateStrictMock<ITypeAssemblyContextCodeGenerator>();
      _constructorFinderMock = MockRepository.GenerateStrictMock<IConstructorFinder>();
      _delegateFactoryMock = MockRepository.GenerateStrictMock<IDelegateFactory>();

      _fakeCodeGenerator = MockRepository.GenerateStub<ICodeGenerator>();
      _typeAssemblyContextCodeGeneratorMock.Expect (mock => mock.CodeGenerator).Return (_fakeCodeGenerator);

      _cache = new TypeCache (_typeAssemblerMock, _typeAssemblyContextCodeGeneratorMock, _constructorFinderMock, _delegateFactoryMock);

      _fromRequestedTypeFunc = (Func<ICacheKeyProvider, Type, object>) PrivateInvoke.GetNonPublicStaticField (typeof (TypeCache), "s_fromRequestedType");
      _fromGeneratedTypeFunc = (Func<ICacheKeyProvider, Type, object>) PrivateInvoke.GetNonPublicStaticField (typeof (TypeCache), "s_fromGeneratedType");

      _lockObject = PrivateInvoke.GetNonPublicField (_cache, "_lock");
      _types = (Dictionary<object[], Type>) PrivateInvoke.GetNonPublicField (_cache, "_types");
      _constructorCalls = (Dictionary<object[], Delegate>) PrivateInvoke.GetNonPublicField (_cache, "_constructorCalls");
      _participantState = (IDictionary<string, object>) PrivateInvoke.GetNonPublicField (_cache, "_participantState");

      _requestedType = ReflectionObjectMother.GetSomeType();
      _delegateType = ReflectionObjectMother.GetSomeDelegateType();
      _allowNonPublic = BooleanObjectMother.GetRandomBoolean();
      var parameterTypes = new[] { ReflectionObjectMother.GetSomeType(), ReflectionObjectMother.GetSomeOtherType() };
      var returnType = ReflectionObjectMother.GetSomeType();
      _fakeSignature = Tuple.Create (parameterTypes, returnType);
      _fakeConstructor = ReflectionObjectMother.GetSomeConstructor();
    }

    [Test]
    public void ParticipantConfigurationID ()
    {
      _typeAssemblerMock.Expect (mock => mock.ParticipantConfigurationID).Return ("configId");

      var result = _cache.ParticipantConfigurationID;

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("configId"));
    }

    [Test]
    public void CodeGenerator ()
    {
      var result = _cache.CodeGenerator;

      Assert.That (result, Is.TypeOf<LockingCodeGeneratorDecorator>());

      var lockObject = PrivateInvoke.GetNonPublicField (result, "_lockObject");
      var innerCodeGenerator = PrivateInvoke.GetNonPublicField (result, "_innerCodeGenerator");

      Assert.That (lockObject, Is.SameAs (_lockObject));
      Assert.That (innerCodeGenerator, Is.SameAs (_fakeCodeGenerator));

      Assert.That (_cache.CodeGenerator, Is.SameAs (result));
    }

    [Test]
    public void GetOrCreateType_CacheHit ()
    {
      _types.Add (new object[] { _requestedType, "key" }, _generatedType1);
      _typeAssemblerMock.Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 1))
                        .Return (new object[] { null, "key" });

      var result = _cache.GetOrCreateType (_requestedType);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_generatedType1));
    }

    [Test]
    public void GetOrCreateType_CacheMiss ()
    {
      _types.Add (new object[] { _requestedType, "key" }, _generatedType1);
      _typeAssemblerMock
          .Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 1))
          .WhenCalled (x => LockTestHelper.CheckLockIsNotHeld (_lockObject))
          .Return (new object[] { null, "other key" });
      _typeAssemblerMock
          .Expect (mock => mock.AssembleType (_requestedType, _participantState, _typeAssemblyContextCodeGeneratorMock))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_generatedType2);

      var result = _cache.GetOrCreateType (_requestedType);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_generatedType2));
      Assert.That (_types[new object[] { _requestedType, "other key" }], Is.SameAs (_generatedType2));
    }

    [Test]
    public void GetOrCreateConstructorCall_CacheHit ()
    {
      _constructorCalls.Add (new object[] { _requestedType, _delegateType, _allowNonPublic, "key" }, _delegate1);
      _typeAssemblerMock.Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 3))
                        .Return (new object[] { null, null, null, "key" });

      var result = _cache.GetOrCreateConstructorCall (_requestedType, _delegateType, _allowNonPublic);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_delegate1));
    }

    [Test]
    public void GetOrCreateConstructorCall_CacheMissDelegates_CacheHitTypes ()
    {
      _constructorCalls.Add (new object[] { _requestedType, _delegateType, _allowNonPublic, "ctor key" }, _delegate1);
      _types.Add (new object[] { _requestedType, "type key" }, _generatedType1);
      _typeAssemblerMock
          .Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 3))
          .WhenCalled (mi => LockTestHelper.CheckLockIsNotHeld (_lockObject))
          .Return (new object[] { null, null, null, "type key" });
      _delegateFactoryMock
          .Expect (mock => mock.GetSignature (_delegateType))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_fakeSignature);
      _constructorFinderMock
          .Expect (mock => mock.GetConstructor (_generatedType1, _fakeSignature.Item1, _allowNonPublic, _requestedType, _fakeSignature.Item1))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_fakeConstructor);
      _delegateFactoryMock
          .Expect (mock => mock.CreateConstructorCall (_fakeConstructor, _delegateType))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_delegate2);

      var result = _cache.GetOrCreateConstructorCall (_requestedType, _delegateType, _allowNonPublic);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_delegate2));
      Assert.That (_constructorCalls[new object[] { _requestedType, _delegateType, _allowNonPublic, "type key" }], Is.SameAs (_delegate2));
    }

    [Test]
    public void GetOrCreateConstructorCall_CacheMissDelegates_CacheMissTypes ()
    {
      _constructorCalls.Add (new object[] { _requestedType, _delegateType, _allowNonPublic, "ctor key" }, _delegate1);
      _types.Add (new object[] { _requestedType, "type key" }, _generatedType1);
      _typeAssemblerMock
          .Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 3))
          .WhenCalled (mi => LockTestHelper.CheckLockIsNotHeld (_lockObject))
          .Return (new object[] { null, null, null, "other type key" });
      _delegateFactoryMock
          .Expect (mock => mock.GetSignature (_delegateType))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_fakeSignature);
      _typeAssemblerMock
          .Expect (mock => mock.AssembleType (_requestedType, _participantState, _typeAssemblyContextCodeGeneratorMock))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_generatedType2);
      _constructorFinderMock
          .Expect (mock => mock.GetConstructor (_generatedType2, _fakeSignature.Item1, _allowNonPublic, _requestedType, _fakeSignature.Item1))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_fakeConstructor);
      _delegateFactoryMock
          .Expect (mock => mock.CreateConstructorCall (_fakeConstructor, _delegateType))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (_delegate2);

      var result = _cache.GetOrCreateConstructorCall (_requestedType, _delegateType, _allowNonPublic);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_delegate2));
      Assert.That (_types[new object[] { _requestedType, "other type key" }], Is.SameAs (_generatedType2));
      Assert.That (_constructorCalls[new object[] { _requestedType, _delegateType, _allowNonPublic, "other type key" }], Is.SameAs (_delegate2));
    }

    [Test]
    public void LoadTypes ()
    {
      _typeAssemblerMock
          .Expect (mock => mock.GetCompoundCacheKey (_fromGeneratedTypeFunc, _generatedType1, 1))
          .WhenCalled (x => LockTestHelper.CheckLockIsHeld (_lockObject))
          .Return (new object[] { null, "type key" });

      _cache.LoadTypes (new[] { _generatedType1, _generatedType2 }.AsOneTime());

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (_types[new object[] { _generatedType1.BaseType, "type key" }], Is.SameAs (_generatedType1));
    }

    [Test]
    public void LoadTypes_SameKey_Nop ()
    {
      _typeAssemblerMock
          .Expect (mock => mock.GetCompoundCacheKey (Arg.Is (_fromGeneratedTypeFunc), Arg<Type>.Is.Anything, Arg.Is (1)))
          .Return (new object[] { null, "type key" })
          .Repeat.Twice();

      var otherGeneratedType = typeof (OtherGeneratedType1);
      Assert.That (otherGeneratedType.BaseType, Is.SameAs (_generatedType1.BaseType));
      _cache.LoadTypes (new[] { _generatedType1 });

      // Does not throw exception or overwrite previously cached type.
      _cache.LoadTypes (new[] { otherGeneratedType });

      Assert.That (_types[new object[] { _generatedType1.BaseType, "type key" }], Is.SameAs (_generatedType1));
    }

    [ProxyType] private class GeneratedType1 { }
    private class GeneratedType2 : ProxyBaseType { }
    [ProxyType] private class ProxyBaseType {}
    [ProxyType] private class OtherGeneratedType1 { }
  }
}