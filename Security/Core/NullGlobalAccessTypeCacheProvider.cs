/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Remotion.Collections;
using Remotion.Configuration;
using System.Runtime.Serialization;
using Remotion.Security.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IGlobalAccessTypeCacheProvider"/> according to the "Null Object Pattern".
  /// </summary>
  [Serializable]
  public class NullGlobalAccessTypeCacheProvider : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    private readonly NullCache<Tuple<ISecurityContext, string>, AccessType[]> _cache = new NullCache<Tuple<ISecurityContext, string>, AccessType[]>();

    public NullGlobalAccessTypeCacheProvider()
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

    public ICache<Tuple<ISecurityContext, string>, AccessType[]> GetCache()
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
