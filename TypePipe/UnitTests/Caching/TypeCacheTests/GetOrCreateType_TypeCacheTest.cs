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
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.Development.UnitTesting.ObjectMothers.Caching;
using Remotion.TypePipe.TypeAssembly.Implementation;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Caching.TypeCacheTests
{
  [TestFixture]
  public class GetOrCreateType_TypeCacheTest
  {
    private ITypeAssembler _typeAssemblerMock;
    private IAssemblyContextPool _assemblyContextPoolMock;

    private TypeCache _cache;

    private IDictionary<AssembledTypeID, Lazy<Type>> _assembledTypes;
    private IDictionary<object, Lazy<Type>> _additionalTypes;

    private readonly Type _requestedType = typeof (RequestedType);
    private readonly Type _assembledType = typeof (AssembledType);

    [SetUp]
    public void SetUp ()
    {
      _typeAssemblerMock = MockRepository.GenerateStrictMock<ITypeAssembler>();
      _assemblyContextPoolMock = MockRepository.GenerateStrictMock<IAssemblyContextPool>();

      _cache = new TypeCache (_typeAssemblerMock, _assemblyContextPoolMock);

      _assembledTypes = (IDictionary<AssembledTypeID, Lazy<Type>>) PrivateInvoke.GetNonPublicField (_cache, "_assembledTypes");
      _additionalTypes = (IDictionary<object, Lazy<Type>>) PrivateInvoke.GetNonPublicField (_cache, "_additionalTypes");
    }

    [Test]
    public void CacheHit ()
    {
      var typeID = AssembledTypeIDObjectMother.Create (_requestedType);
      _assembledTypes.Add (typeID, new Lazy<Type> (() => _assembledType, LazyThreadSafetyMode.None));

      var result = _cache.GetOrCreateType (typeID);

      _typeAssemblerMock.VerifyAllExpectations();
      _assemblyContextPoolMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_assembledType));
    }

    [Test]
    public void CacheMiss_UsesAssemblyContextFromPool ()
    {
      var typeID = AssembledTypeIDObjectMother.Create();
      var assemblyContext = CreateAssemblyContext();

      bool isDequeued = false;
      _assemblyContextPoolMock
          .Expect (mock => mock.Dequeue())
          .Return (assemblyContext)
          .WhenCalled (mi => { isDequeued = true; });

      _typeAssemblerMock
          .Expect (
              mock => mock.AssembleType (
                  // Use strongly typed Equals overload.
                  Arg<AssembledTypeID>.Matches (id => id.Equals (typeID)),
                  Arg.Is (assemblyContext.ParticipantState),
                  Arg.Is (assemblyContext.MutableTypeBatchCodeGenerator)))
          .Return (new TypeAssemblyResult (_assembledType))
          .WhenCalled (mi => Assert.That (isDequeued, Is.True));

      _assemblyContextPoolMock
          .Expect (mock => mock.Enqueue (assemblyContext))
          .WhenCalled (mi => Assert.That (isDequeued, Is.True));

      var result = _cache.GetOrCreateType (typeID);

      _typeAssemblerMock.VerifyAllExpectations();
      _assemblyContextPoolMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_assembledType));

      Assert.That (_assembledTypes[typeID].IsValueCreated, Is.True);
      Assert.That (_assembledTypes[typeID].Value, Is.SameAs (_assembledType));
    }

    [Test]
    public void CacheMiss_AndExceptionDuringAssembleType_DoesNotCacheException ()
    {
      var expectedException = new Exception();
      var typeID = AssembledTypeIDObjectMother.Create();
      var assemblyContext = CreateAssemblyContext();

      _assemblyContextPoolMock.Expect (mock => mock.Dequeue()).Return (assemblyContext);
      _typeAssemblerMock.Expect (mock => mock.AssembleType (new AssembledTypeID(), null, null)).IgnoreArguments().Throw (expectedException);
      _assemblyContextPoolMock.Expect (mock => mock.Enqueue (assemblyContext));

      _assemblyContextPoolMock.Expect (mock => mock.Dequeue()).Return (assemblyContext);
      _typeAssemblerMock
          .Expect (mock => mock.AssembleType (new AssembledTypeID(), null, null))
          .IgnoreArguments()
          .Return (new TypeAssemblyResult (_assembledType));
      _assemblyContextPoolMock.Expect (mock => mock.Enqueue (assemblyContext));

      Assert.That (() => _cache.GetOrCreateType (typeID), Throws.Exception.SameAs (expectedException));
      Assert.That (_cache.GetOrCreateType (typeID), Is.SameAs (_assembledType));

      _typeAssemblerMock.VerifyAllExpectations();
      _assemblyContextPoolMock.VerifyAllExpectations();
    }

    [Test]
    public void CacheMiss_AndExceptionDuringAssembleType_ReturnsAssemblyContextToPool ()
    {
      var expectedException = new Exception();
      var typeID = AssembledTypeIDObjectMother.Create();
      var assemblyContext = CreateAssemblyContext();

      bool isDequeued = false;
      _assemblyContextPoolMock
          .Expect (mock => mock.Dequeue())
          .Return (assemblyContext)
          .WhenCalled (mi => { isDequeued = true; });

      _typeAssemblerMock
          .Expect (mock => mock.AssembleType (new AssembledTypeID(), null, null))
          .IgnoreArguments()
          .Throw (expectedException)
          .WhenCalled (mi => Assert.That (isDequeued, Is.True));

      _assemblyContextPoolMock
          .Expect (mock => mock.Enqueue (assemblyContext))
          .WhenCalled (mi => Assert.That (isDequeued, Is.True));

      Assert.That (() => _cache.GetOrCreateType (typeID), Throws.Exception.SameAs (expectedException));

      _typeAssemblerMock.VerifyAllExpectations();
      _assemblyContextPoolMock.VerifyAllExpectations();
    }

    [Test]
    public void CacheMiss_AddsAdditionalTypesToCacheBeforeReturningAssemblyContextToPool ()
    {
      var assemblyContext = CreateAssemblyContext();
      var typeID = AssembledTypeIDObjectMother.Create();
      var additionalTypeID = new object();
      var additionalType = ReflectionObjectMother.GetSomeType();

      _assemblyContextPoolMock.Expect (mock => mock.Dequeue()).Return (assemblyContext);

      _typeAssemblerMock
          .Expect (mock => mock.AssembleType (new AssembledTypeID(), null, null))
          .IgnoreArguments()
          .Return (new TypeAssemblyResult (_assembledType, new Dictionary<object, Type> { { additionalTypeID, additionalType } }));

      _assemblyContextPoolMock
          .Expect (mock => mock.Enqueue (assemblyContext))
          .WhenCalled (mi => Assert.That (_additionalTypes[additionalTypeID].Value, Is.SameAs (additionalType)));

      var result = _cache.GetOrCreateType (typeID);

      _assemblyContextPoolMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_assembledType));
    }

    [Test]
    public void CacheMiss_AddsAdditionalTypesToCache_OverridesPreviouslyCachedValue ()
    {
      var assemblyContext = CreateAssemblyContext();
      var typeID = AssembledTypeIDObjectMother.Create();
      var additionalTypeID = new object();
      var additionalType = ReflectionObjectMother.GetSomeType();
      _additionalTypes.Add (additionalTypeID, new Lazy<Type> (() => null, LazyThreadSafetyMode.None));

      _assemblyContextPoolMock.Expect (mock => mock.Dequeue()).Return (assemblyContext);

      _typeAssemblerMock
          .Expect (mock => mock.AssembleType (new AssembledTypeID(), null, null))
          .IgnoreArguments()
          .Return (new TypeAssemblyResult (_assembledType, new Dictionary<object, Type> { { additionalTypeID, additionalType } }));

      _assemblyContextPoolMock
          .Expect (mock => mock.Enqueue (assemblyContext))
          .WhenCalled (mi => Assert.That (_additionalTypes[additionalTypeID].Value, Is.SameAs (additionalType)));

      var result = _cache.GetOrCreateType (typeID);

      _assemblyContextPoolMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_assembledType));
    }

    private AssemblyContext CreateAssemblyContext ()
    {
      return new AssemblyContext (
          MockRepository.GenerateStrictMock<IMutableTypeBatchCodeGenerator>(),
          MockRepository.GenerateStrictMock<IGeneratedCodeFlusher>());
    }

    private class RequestedType {}
    private class AssembledType {}
  }
}