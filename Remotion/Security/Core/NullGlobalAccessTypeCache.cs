// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security
{
  [Serializable]
  [ImplementationFor (typeof (IGlobalAccessTypeCache), Position = 1, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class NullGlobalAccessTypeCache : IGlobalAccessTypeCache
  {
    public NullGlobalAccessTypeCache ()
    {
    }

    public AccessType[] GetOrCreateValue (
        GlobalAccessTypeCacheKey key,
        Func<GlobalAccessTypeCacheKey, AccessType[]> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      return valueFactory (key);
    }

    public bool TryGetValue (GlobalAccessTypeCacheKey key, out AccessType[] value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      value = null;
      return false;
    }

    public void Clear ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }
  }
}