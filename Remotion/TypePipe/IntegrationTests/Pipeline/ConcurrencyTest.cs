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
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Caching;

namespace Remotion.TypePipe.IntegrationTests.Pipeline
{
  [TestFixture]
  [Timeout (1000)] // Set timeout for all tests.
  public class ConcurrencyTest : IntegrationTestBase
  {
    private Mutex _blockingMutex;

    private IPipeline _pipeline;

    public override void SetUp ()
    {
      base.SetUp ();

      _blockingMutex = new Mutex (initiallyOwned: true);

      var blockingParticipant = CreateParticipant (
          (id, ctx) =>
          {
            if (ctx.RequestedType == typeof (DomainTypeCausingParticipantToBlock))
              _blockingMutex.WaitOne();
          },
          additionalTypeFunc: (additionalTypeID, ctx) => typeof (OtherDomainType));

      _pipeline = CreatePipeline (blockingParticipant);
    }

    [Test]
    public void CachedTypesCanBeRetrievedAndInstantiated_DuringCodeGenerationOfAnotherType ()
    {
      // Generate type without blocking.
      _pipeline.Create<DomainType>();

      var t = StartAndWaitUntilBlocked (() => _pipeline.Create<DomainTypeCausingParticipantToBlock>());

      // Although code is generated in [t], which is blocked by the mutex, we can create instances of and retrieve already generated types.
      _pipeline.Create<DomainType>();
      _pipeline.ReflectionService.GetAssembledType (typeof (DomainType));

      _blockingMutex.ReleaseMutex();
      WaitUntilCompleted (t);
    }

    [Test]
    public void CodeGenerationIsSerialized ()
    {
      var t1 = StartAndWaitUntilBlocked (() => _pipeline.Create<DomainTypeCausingParticipantToBlock>());
      var t2 = StartAndWaitUntilBlocked (() => _pipeline.Create<DomainType>());

      // Both threads are now blocked. [t1] is blocked by the mutex, [t2] is blocked by the code generation in [t1].
      _blockingMutex.ReleaseMutex();

      // Now both threads run to completion (code generation is serialized).
      WaitUntilCompleted (t1, t2);
    }

    [Test]
    public void CodeManagerAPIs_CannotRunWhileCodeIsGenerated ()
    {
      var t1 = StartAndWaitUntilBlocked (() => _pipeline.Create<DomainTypeCausingParticipantToBlock>());
      var t2 = StartAndWaitUntilBlocked (() => Dev.Null = _pipeline.CodeManager.AssemblyDirectory);
      var t3 = StartAndWaitUntilBlocked (() => Dev.Null = _pipeline.CodeManager.AssemblyNamePattern);
      var t4 = StartAndWaitUntilBlocked (() => Flush());

      // All threads are now blocked. [t1] is blocked by the mutex, [t2, ...] are blocked by the code generation in [t1].
      _blockingMutex.ReleaseMutex();

      // Now all threads run to completion (user APIs do not interfere with code generation).
      WaitUntilCompleted (t1, t2, t3, t4);
    }

    [Test]
    public void ReflectionServiceAPIs_CannotRunWhileCodeIsGenerated ()
    {
      // Populate cache.
      var cachedAssembledType = _pipeline.ReflectionService.GetAssembledType (typeof (DomainType));
      var cachedAssembledTypeID = _pipeline.ReflectionService.GetTypeID (cachedAssembledType);
      var otherCachedAssembledType = _pipeline.ReflectionService.GetAssembledType (typeof (OtherDomainType));
      _pipeline.ReflectionService.InstantiateAssembledType (otherCachedAssembledType);

      var newRequestedType = typeof (object);
      var newAssembledTypeID = new AssembledTypeID (newRequestedType, new object[0]);

      var t1 = StartAndWaitUntilBlocked (() => _pipeline.Create<DomainTypeCausingParticipantToBlock>());
      var t2 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.IsAssembledType (cachedAssembledType));
      var t3 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.GetRequestedType (cachedAssembledType));
      var t4 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.GetTypeID (cachedAssembledType));
      var t5 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.GetAssembledType (newRequestedType));
      var t6 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.GetAssembledType (newAssembledTypeID));
      var t7 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.GetAdditionalType ("additional type id"));
      var t8 = StartAndWaitUntilBlocked (() => _pipeline.ReflectionService.InstantiateAssembledType (cachedAssembledType));

      // Operations that only return cached results are not blocked.
      Assert.That (_pipeline.ReflectionService.GetAssembledType (typeof (DomainType)), Is.SameAs (cachedAssembledType));
      Assert.That (_pipeline.ReflectionService.GetAssembledType (cachedAssembledTypeID), Is.SameAs (cachedAssembledType));
      Assert.That (_pipeline.ReflectionService.InstantiateAssembledType (otherCachedAssembledType), Is.Not.Null);

      // All threads that cannot be served purely by the cache are now blocked.
      // [t1] is blocked by the mutex, [t2, ...] are blocked by the code generation in [t1].
      _blockingMutex.ReleaseMutex();

      // Now all threads run to completion (user APIs do not interfere with code generation).
      WaitUntilCompleted (t1, t2, t3, t4, t5, t6, t7, t8);
    }

    private Thread StartAndWaitUntilBlocked (ThreadStart action)
    {
      var thread = new Thread (action);
      thread.Start();

      while (thread.ThreadState != ThreadState.WaitSleepJoin)
        Thread.Sleep (10); // TODO 5057: Use Thread.Yield instead.

      Assert.That (thread.ThreadState, Is.EqualTo (ThreadState.WaitSleepJoin));

      return thread;
    }

    private void WaitUntilCompleted (params Thread[] threads)
    {
      foreach (var t in threads)
        t.Join();
    }

    public class DomainTypeCausingParticipantToBlock {}
    public class DomainType {}
    public class OtherDomainType {}
  }
}