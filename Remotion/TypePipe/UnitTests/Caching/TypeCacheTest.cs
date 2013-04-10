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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.TypePipe.UnitTests.Caching
{
  [TestFixture]
  public class TypeCacheTest
  {
    private ITypeAssembler _typeAssemblerMock;
    private ITypeCacheSynchronizationPoint _typeCacheSynchronizationPoint;
    private IMutableTypeBatchCodeGenerator _batchCodeGeneratorMock;

    private TypeCache _cache;

    private Func<ICacheKeyProvider, Type, object> _fromRequestedTypeFunc;
    private Func<ICacheKeyProvider, Type, object> _fromGeneratedTypeFunc;

    private ConcurrentDictionary<object[], Type> _types;
    private ConcurrentDictionary<object[], Delegate> _constructorCalls;
    private IDictionary<string, object> _participantState;

    private readonly Type _assembledType = typeof (AssembledType);
    private readonly Delegate _generatedCtorCall = new Func<int> (() => 7);
    private readonly Type _requestedType = typeof (RequestedType);
    private Type _delegateType;
    private bool _allowNonPublic;

    [SetUp]
    public void SetUp ()
    {
      _typeAssemblerMock = MockRepository.GenerateStrictMock<ITypeAssembler>();
      _typeCacheSynchronizationPoint = MockRepository.GenerateStrictMock<ITypeCacheSynchronizationPoint>();
      _batchCodeGeneratorMock = MockRepository.GenerateStrictMock<IMutableTypeBatchCodeGenerator>();

      _cache = new TypeCache (_typeAssemblerMock, _typeCacheSynchronizationPoint, _batchCodeGeneratorMock);

      _fromRequestedTypeFunc = (Func<ICacheKeyProvider, Type, object>) PrivateInvoke.GetNonPublicStaticField (typeof (TypeCache), "s_fromRequestedType");
      _fromGeneratedTypeFunc = (Func<ICacheKeyProvider, Type, object>) PrivateInvoke.GetNonPublicStaticField (typeof (TypeCache), "s_fromAssembledType");

      _types = (ConcurrentDictionary<object[], Type>) PrivateInvoke.GetNonPublicField (_cache, "_types");
      _constructorCalls = (ConcurrentDictionary<object[], Delegate>) PrivateInvoke.GetNonPublicField (_cache, "_constructorCalls");
      _participantState = (IDictionary<string, object>) PrivateInvoke.GetNonPublicField (_cache, "_participantState");

      _delegateType = ReflectionObjectMother.GetSomeDelegateType();
      _allowNonPublic = BooleanObjectMother.GetRandomBoolean();
    }

    [Test]
    public void ParticipantConfigurationID ()
    {
      _typeAssemblerMock.Expect (mock => mock.ParticipantConfigurationID).Return ("configId");

      Assert.That (_cache.ParticipantConfigurationID, Is.EqualTo ("configId"));
    }

    [Test]
    public void Participants ()
    {
      var participants = new ReadOnlyCollection<IParticipant> (new IParticipant[0]);
      _typeAssemblerMock.Expect (mock => mock.Participants).Return (participants);

      Assert.That (_cache.Participants, Is.SameAs (participants));
    }

    [Test]
    public void GetOrCreateType_CacheHit ()
    {
      _types.Add (new object[] { _requestedType, "key" }, _assembledType);
      _typeAssemblerMock.Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 1))
                        .Return (new object[] { null, "key" });

      var result = _cache.GetOrCreateType (_requestedType);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_assembledType));
    }

    [Test]
    public void GetOrCreateType_CacheMiss ()
    {
      var key = new object[] { null, "key" };
      var expectedKey = new object[] { _requestedType, "key" };

      _typeAssemblerMock
          .Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 1))
          .Return (key);
      _typeCacheSynchronizationPoint
          .Expect (mock => mock.GetOrGenerateType (_types, expectedKey, _requestedType, _participantState, _batchCodeGeneratorMock))
          .Return (_assembledType);

      var result = _cache.GetOrCreateType (_requestedType);

      _typeAssemblerMock.VerifyAllExpectations();
      _typeCacheSynchronizationPoint.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_assembledType));
    }

    [Test]
    public void GetOrCreateConstructorCall_CacheHit ()
    {
      _constructorCalls.Add (new object[] { _requestedType, _delegateType, _allowNonPublic, "key" }, _generatedCtorCall);
      _typeAssemblerMock.Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 3))
                        .Return (new object[] { null, null, null, "key" });

      var result = _cache.GetOrCreateConstructorCall (_requestedType, _delegateType, _allowNonPublic);

      _typeAssemblerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_generatedCtorCall));
    }

    [Test]
    public void GetOrCreateConstructorCall_CacheMiss ()
    {
      var constructorKey = new object[] { null, null, null, "key" };
      var expectedConstructorKey = new object[] { _requestedType, _delegateType, _allowNonPublic, "key" };
      var expectedTypeKey = new object[] { _requestedType, "key" };

      _typeAssemblerMock.Expect (mock => mock.GetCompoundCacheKey (_fromRequestedTypeFunc, _requestedType, 3)).Return (constructorKey);
      _typeCacheSynchronizationPoint
          .Expect (
              mock => mock.GetOrGenerateConstructorCall (
                  _constructorCalls,
                  expectedConstructorKey,
                  _delegateType,
                  _allowNonPublic,
                  _types,
                  expectedTypeKey, _requestedType, _participantState, _batchCodeGeneratorMock))
          .Return (_generatedCtorCall);

      var result = _cache.GetOrCreateConstructorCall (_requestedType, _delegateType, _allowNonPublic);

      _typeAssemblerMock.VerifyAllExpectations();
      _typeCacheSynchronizationPoint.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_generatedCtorCall));
    }

    [Test]
    public void LoadTypes ()
    {
      var additionalGeneratedType = ReflectionObjectMother.GetSomeOtherType();
      _typeAssemblerMock.Expect (mock => mock.IsAssembledType (_assembledType)).Return (true);
      _typeAssemblerMock.Expect (mock => mock.IsAssembledType (additionalGeneratedType)).Return (false);
      _typeAssemblerMock.Expect (mock => mock.GetRequestedType (_assembledType)).Return (_requestedType);
      _typeAssemblerMock.Expect (mock => mock.GetCompoundCacheKey (_fromGeneratedTypeFunc, _assembledType, 1)).Return (new object[] { null, "key" });
      _typeCacheSynchronizationPoint
          .Expect (
              mock => mock.RebuildParticipantState (
                  Arg.Is (_types),
                  Arg<IEnumerable<KeyValuePair<object[], Type>>>.Is.Anything,
                  Arg<IEnumerable<Type>>.List.Equal (new[] { additionalGeneratedType }),
                  Arg.Is (_participantState)))
          .WhenCalled (
              mi =>
              {
                var keysToAssembledTypes = (IEnumerable<KeyValuePair<object[], Type>>) mi.Arguments[1];
                var pair = keysToAssembledTypes.Single();
                Assert.That (pair.Key, Is.EqualTo (new object[] { _requestedType, "key" }));
                Assert.That (pair.Value, Is.SameAs (_assembledType));
              });

      _cache.LoadTypes (new[] { _assembledType, additionalGeneratedType });

      _typeAssemblerMock.VerifyAllExpectations();
      _typeCacheSynchronizationPoint.VerifyAllExpectations();
    }

    private class RequestedType {}
    private class AssembledType {}
  }
}