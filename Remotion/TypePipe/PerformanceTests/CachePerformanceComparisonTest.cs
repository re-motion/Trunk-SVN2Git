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
using System.Globalization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace TypePipe.PerformanceTests
{
  [Explicit ("Performance measurement for caching")]
  [TestFixture]
  public class CachePerformanceComparisonTest
  {
    [Test]
    public void TypePipe ()
    {
      // Pipeline participant configuration is set similar to the current Remotion functionality
      var restoreParticipantStub = MockRepository.GenerateStub<IParticipant>();
      var remixParticipantStub = MockRepository.GenerateStub<IParticipant>();
      restoreParticipantStub.Stub (stub => stub.PartialCacheKeyProvider).Return (new RestoreCacheKeyProvider());
      remixParticipantStub.Stub (stub => stub.PartialCacheKeyProvider).Return (new RemixCacheKeyProvider());

      var typeModifierStub = MockRepository.GenerateStub<ITypeModifier>();
      typeModifierStub.Stub (stub => stub.ApplyModifications (Arg<MutableType>.Is.Anything)).Return (typeof (DomainType));
      var constructorFinder = new ConstructorFinder();
      var typeAssembler = new TypeAssembler (new[] { restoreParticipantStub, remixParticipantStub }, typeModifierStub);
      var constructorProvider = new DelegateFactory();

      ITypeCache typeCacheAsInterface = new TypeCache (typeAssembler, constructorFinder, constructorProvider);

      Func<Type> typeCache = () => typeCacheAsInterface.GetOrCreateType (typeof (DomainType));
      Func<Delegate> constructorDelegateCache =
          () => typeCacheAsInterface.GetOrCreateConstructorCall (typeof (DomainType), typeof (Func<object>), true);

      TimeThis ("TypePipe_Types", typeCache);
      TimeThis ("TypePipe_ConstructorDelegates", constructorDelegateCache);
    }

    [Test]
    public void Remotion ()
    {
      Func<Type> typeCache = () => InterceptedDomainObjectCreator.Instance.Factory.GetConcreteDomainObjectType (typeof (DomainType));
      Func<Delegate> constructorDelegateCache =
          () => InterceptedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (DomainType)).GetDelegate (typeof (Func<object>));

      TimeThis ("Remotion_Types", typeCache);
      TimeThis ("Remotion_ConstructorDelegates", constructorDelegateCache);
    }

    private static void TimeThis<T> (string testName, Func<T> func)
    {
      // Warmup and cache population.
      func();

      const int startPow = 3;
      const int maxPow = 6;
      int hc = 0;

      Console.WriteLine (testName);
      for (int i = startPow; i <= maxPow; ++i)
      {
        GC.Collect (2, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        GC.Collect (2, GCCollectionMode.Forced);

        long requestedInstanceCount = (long) Math.Pow (10, i);

        StopwatchScope.MeasurementAction measurementAction =
            (c, s) => Console.WriteLine (
                "{0}: {1}ms, per call: {2}",
                requestedInstanceCount.ToString (CultureInfo.InvariantCulture).PadLeft(8),
                s.ElapsedTotal.TotalMilliseconds.ToString("0.0000").PadLeft(10),
                (s.ElapsedTotal.TotalMilliseconds / requestedInstanceCount).ToString("0.000000"));

        using (StopwatchScope.CreateScope (measurementAction))
        {
          for (int j = 0; j < requestedInstanceCount; ++j)
          {
            var obj = func();
            hc = hc >> 1;
            hc ^= obj.GetHashCode();
          }
        }
      }
      Console.WriteLine();
    }

    public class RestoreCacheKeyProvider : ICacheKeyProvider
    {
      public object GetCacheKey (Type requestedType)
      {
        var mappingConfiguration = MappingConfiguration.Current;
        return mappingConfiguration.ContainsTypeDefinition (requestedType) ? mappingConfiguration.GetTypeDefinition (requestedType) : null;
      }
    }

    public class RemixCacheKeyProvider : ICacheKeyProvider
    {
      public object GetCacheKey (Type requestedType)
      {
        return MixinConfiguration.ActiveConfiguration.GetContext (requestedType); // may be null
      }
    }

    [DBTable]
    [Uses (typeof (object))]
    public class DomainType : DomainObject
    {
    }
  }
}