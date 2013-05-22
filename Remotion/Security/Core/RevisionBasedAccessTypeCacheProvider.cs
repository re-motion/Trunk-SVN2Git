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
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Context;
using Remotion.Security.Configuration;

namespace Remotion.Security
{
  using CacheType = LazyLockingCachingAdapter<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>;

  [Serializable]
  public class RevisionBasedAccessTypeCacheProvider : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    private class Revision
    {
      public readonly int Value;
      public Revision (int value)
      {
        Value = value;
      }
    }

    private static readonly string s_revisionKey = SafeContextKeys.SecurityRevisionBasedAccessTypeCacheProviderRevision;

    private readonly object _syncRoot = new object();

    private LazyLockingCachingAdapter<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> _cache;
    private volatile int _revision;

    public RevisionBasedAccessTypeCacheProvider ()
        : this ("Revision", new NameValueCollection())
    {
    }

    public RevisionBasedAccessTypeCacheProvider (string name, NameValueCollection config)
        : base (name, config)
    {
      _revision = 0;
      _cache = CacheFactory.CreateWithLazyLocking<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>();
    }

    protected RevisionBasedAccessTypeCacheProvider (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      throw new InvalidOperationException ("Serialization can only happen via the GlobalAccessTypeCacheProviderObjectReference.");
    }

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      GlobalAccessTypeCacheProviderObjectReference.DoGetObjectDataForWellKnownProvider (this, info, context);
    }

    public ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> GetCache ()
    {
      int currentRevision = GetCurrentRevision();
      if (_revision < currentRevision)
      {
        lock (_syncRoot)
        {
          if (_revision < currentRevision)
          {
            _revision = currentRevision;
            _cache = CacheFactory.CreateWithLazyLocking<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>();
          }
        }
      }

      return _cache;
    }

    private int GetCurrentRevision ()
    {
      var revision = (Revision) SafeContext.Instance.GetData (s_revisionKey);
      if (revision == null)
      {
        revision = new Revision (GetRevisionFromSecurityProvider());
        SafeContext.Instance.SetData (s_revisionKey, revision);
      }

      return revision.Value;
    }

    private int GetRevisionFromSecurityProvider ()
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
