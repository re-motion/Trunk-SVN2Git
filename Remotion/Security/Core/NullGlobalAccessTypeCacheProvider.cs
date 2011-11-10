// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Security.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IGlobalAccessTypeCacheProvider"/> according to the "Null Object Pattern".
  /// </summary>
  [Serializable]
  public class NullGlobalAccessTypeCacheProvider : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    private readonly NullCache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> _cache =
        new NullCache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>();

    public NullGlobalAccessTypeCacheProvider ()
        : this ("Null", new NameValueCollection())
    {
    }


    public NullGlobalAccessTypeCacheProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    protected NullGlobalAccessTypeCacheProvider (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    public ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> GetCache ()
    {
      return _cache;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      if (this == SecurityConfiguration.Current.GlobalAccessTypeCacheProviders[Name])
        GlobalAccessTypeCacheProviderObjectReference.DoGetObjectDataForWellKnownProvider (this, info, context);
      else
        base.GetObjectData (info, context);
    }
  }
}
