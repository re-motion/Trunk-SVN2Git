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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.StrongNaming
{
  public class ControllableStrongNameTypeVerifier : IControllableStrongNameTypeVerifier
  {
    private readonly IStrongNameTypeVerifier _strongNameTypeVerifier;
    private readonly Dictionary<Type, bool> _cache = new Dictionary<Type, bool>();

    public ControllableStrongNameTypeVerifier (IStrongNameTypeVerifier strongNameTypeVerifier)
    {
      ArgumentUtility.CheckNotNull ("strongNameTypeVerifier", strongNameTypeVerifier);

      _strongNameTypeVerifier = strongNameTypeVerifier;
    }

    public bool IsStrongNamed (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      bool strongNamed;

      if (!_cache.TryGetValue(type, out strongNamed))
      {
        strongNamed = _strongNameTypeVerifier.IsStrongNamed (type);
        _cache.Add (type, strongNamed);
      }

      return strongNamed;
    }

    public void SetIsStrongNamed (MutableType mutableType, bool strongNamed)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      if (strongNamed)
        _cache[mutableType] = true;
      else
        _cache.Remove (mutableType);
    }
  }
}