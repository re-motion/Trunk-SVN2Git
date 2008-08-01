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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Context;
using Remotion.Security.Configuration;

namespace Remotion.Security
{
  using CacheType = InterlockedCache<Tuple<ISecurityContext, string>, AccessType[]>;

  [Serializable]
  public class RevisionBasedAccessTypeCacheProvider : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    // constants

    // types

    // static members

    private static readonly string s_revisionKey = typeof (RevisionBasedAccessTypeCacheProvider).AssemblyQualifiedName + "_Revision";

    // member fields

    private readonly object _syncRoot = new object();

    private CacheType _cache;
    private int _revision;

    // construction and disposing

    public RevisionBasedAccessTypeCacheProvider ()
        : this ("Revision", new NameValueCollection())
    {
    }

    public RevisionBasedAccessTypeCacheProvider (string name, NameValueCollection config)
        : base (name, config)
    {
      _revision = 0;
      _cache = new CacheType();
    }

    protected RevisionBasedAccessTypeCacheProvider (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _revision = info.GetInt32 ("_revision");
      _cache = (CacheType) info.GetValue ("_cache", typeof (CacheType));
    }

    // methods and properties

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      if (this == SecurityConfiguration.Current.GlobalAccessTypeCacheProviders[Name])
        GlobalAccessTypeCacheProviderObjectReference.DoGetObjectDataForWellKnownProvider (this, info, context);
      else
      {
        base.GetObjectData (info, context);
        info.AddValue ("_revision", _revision);
        info.AddValue ("_cache", _cache);
      }
    }

    public ICache<Tuple<ISecurityContext, string>, AccessType[]> GetCache ()
    {
      int currentRevision = GetCurrentRevision();
      if (_revision < currentRevision)
      {
        lock (_syncRoot)
        {
          if (_revision < currentRevision)
          {
            _revision = currentRevision;
            _cache = new InterlockedCache<Tuple<ISecurityContext, string>, AccessType[]>();
          }
        }
      }

      return _cache;
    }

    private int GetCurrentRevision ()
    {
      int? revision = (int?) SafeContext.Instance.GetData (s_revisionKey);
      if (!revision.HasValue)
      {
        revision = GetRevisionFromSecurityProvider();
        SafeContext.Instance.SetData (s_revisionKey, revision);
      }

      return revision.Value;
    }

    private int? GetRevisionFromSecurityProvider ()
    {
      var securityProvider = SecurityConfiguration.Current.SecurityProvider as IRevisionBasedSecurityProvider;
      if (securityProvider == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' requires a security provider implementing the '{1}' interface, but the '{2}' only implements the '{3}' interface. "
                + "This exception might be caused if the security provider is set to 'None' but the global accesstype-cache provider "
                + "is still configured for revision based caching.",
                typeof (RevisionBasedAccessTypeCacheProvider).FullName,
                typeof (IRevisionBasedSecurityProvider).FullName,
                SecurityConfiguration.Current.SecurityProvider.GetType().FullName,
                typeof (IServiceProvider).FullName));
      }

      return securityProvider.GetRevision();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}