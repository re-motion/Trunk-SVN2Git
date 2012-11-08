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
using Remotion.Utilities;

namespace Remotion.TypePipe
{
  /// <summary>
  /// A compound cache key consisting of the requested type and the individual cache keys provided by the <see cref="IParticipant"/> via the
  /// <see cref="ICacheKeyProvider"/> interface.
  /// This class is an implementation detail of the pipeline caching facilities.
  /// </summary>
  /// <remarks>
  /// Note that this class makes the following assumptions.
  /// <list type="bullet">
  ///   <item>The length of the <see cref="_cacheKeys"/> array is assumed to be equal when comparing instances.</item>
  ///   <item>The hash code is pre-computed, therefore the individual cache keys must be immutable.</item>
  /// </list>
  /// </remarks>
  public class CompoundCacheKey : IEquatable<CompoundCacheKey>
  {
    private readonly Type _requestedType;
    private readonly CacheKey[] _cacheKeys;
    private readonly int _preComputedHashCode;

    public CompoundCacheKey (Type requestedType, CacheKey[] cacheKeys)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);
      ArgumentUtility.CheckNotNullOrEmpty ("cacheKeys", cacheKeys);

      _requestedType = requestedType;
      _cacheKeys = cacheKeys;

      // ReSharper disable CoVariantArrayConversion
      int hashCodePart = EqualityUtility.GetRotatedHashCode (_cacheKeys);
      // ReSharper restore CoVariantArrayConversion
      _preComputedHashCode = EqualityUtility.GetRotatedHashCode (_requestedType, hashCodePart);
    }

    // ReSharper disable LoopCanBeConvertedToQuery (reason: performance)
    public bool Equals (CompoundCacheKey other)
    {
      ArgumentUtility.CheckNotNull ("other", other);
      Assertion.IsTrue (_cacheKeys.Length == other._cacheKeys.Length);

      if (_requestedType != other._requestedType)
        return false;

      for (int i = 0; i < _cacheKeys.Length; ++i)
      {
        if (!_cacheKeys[i].Equals (other._cacheKeys[i]))
          return false;
      }

      return true;
    }

    public override bool Equals (object obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      var compoundCacheKey = obj as CompoundCacheKey;
      if (compoundCacheKey == null)
        return false;

      return Equals (compoundCacheKey);
    }

    public override int GetHashCode ()
    {
      return _preComputedHashCode;
    }
  }
}