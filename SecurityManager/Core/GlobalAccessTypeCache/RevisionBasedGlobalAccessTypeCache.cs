// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.GlobalAccessTypeCache
{
  [Serializable]
  public sealed class RevisionBasedGlobalAccessTypeCache : IGlobalAccessTypeCache, ISerializable, IObjectReference
  {
    //TODO RM-5521: test
    private class SecurityContextCache : RepositoryBase<SecurityContextCache.Data, RevisionKey, Int32RevisionValue>
    {
      private readonly RevisionKey _revisionKey = new RevisionKey();

      public class Data : RevisionBasedData
      {
        public readonly ICache<ISecurityPrincipal, AccessTypeCache> Items;

        internal Data (Int32RevisionValue revision)
            : base (revision)
        {
          Items = CacheFactory.CreateWithLocking<ISecurityPrincipal, AccessTypeCache>();
        }
      }

      public SecurityContextCache (IRevisionProvider<RevisionKey, Int32RevisionValue> revisionProvider)
          : base (revisionProvider)
      {
      }

      public ICache<ISecurityPrincipal, AccessTypeCache> Items
      {
        get { return GetCachedData (_revisionKey).Items; }
      }

      protected override Data LoadData (Int32RevisionValue revision)
      {
        return new Data (revision);
      }
    }

    private class AccessTypeCache : RepositoryBase<AccessTypeCache.Data, UserRevisionKey, Int32RevisionValue>
    {
      private readonly UserRevisionKey _revisionKey;

      public class Data : RevisionBasedData
      {
        public readonly ICache<ISecurityContext, AccessType[]> Items;

        internal Data (Int32RevisionValue revision)
            : base (revision)
        {
          Items = CacheFactory.CreateWithLazyLocking<ISecurityContext, AccessType[]>();
        }
      }

      public AccessTypeCache (IRevisionProvider<UserRevisionKey, Int32RevisionValue> revisionProvider, string userName)
          : base (revisionProvider)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);
        
        _revisionKey = new UserRevisionKey (userName);
      }

      public ICache<ISecurityContext, AccessType[]> Items
      {
        get { return GetCachedData (_revisionKey).Items; }
      }

      protected override Data LoadData (Int32RevisionValue revision)
      {
        return new Data (revision);
      }
    }

    private readonly IUserRevisionProvider _userRevisionProvider;
    private readonly SecurityContextCache _securityContextCache;

    public RevisionBasedGlobalAccessTypeCache (IDomainRevisionProvider revisionProvider, IUserRevisionProvider userRevisionProvider)
    {
      ArgumentUtility.CheckNotNull ("revisionProvider", revisionProvider);
      ArgumentUtility.CheckNotNull ("userRevisionProvider", userRevisionProvider);
      
      _securityContextCache = new SecurityContextCache (revisionProvider);
      _userRevisionProvider = userRevisionProvider;
    }

    private RevisionBasedGlobalAccessTypeCache (SerializationInfo info, StreamingContext context)
    {
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return (RevisionBasedGlobalAccessTypeCache) SafeServiceLocator.Current.GetAllInstances<IGlobalAccessTypeCache>()
          .First (() => new InvalidOperationException ("No instance of IGlobalAccessTypeCache has been registered with the ServiceLocator."));
    }

    public AccessType[] GetOrCreateValue (
        GlobalAccessTypeCacheKey globalKey,
        Func<GlobalAccessTypeCacheKey, AccessType[]> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("globalKey", globalKey);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      var accessTypeCache = _securityContextCache.Items.GetOrCreateValue (
          globalKey.SecurityPrincipal,
          key => new AccessTypeCache (_userRevisionProvider, globalKey.SecurityPrincipal.User));

      return accessTypeCache.Items.GetOrCreateValue (
          globalKey.SecurityContext,
          key => valueFactory (globalKey));
    }

    public bool TryGetValue (GlobalAccessTypeCacheKey globalKey, out AccessType[] value)
    {
      ArgumentUtility.CheckNotNull ("globalKey", globalKey);

      AccessTypeCache accessTypeCache;
      if (_securityContextCache.Items.TryGetValue (globalKey.SecurityPrincipal, out accessTypeCache))
        return accessTypeCache.Items.TryGetValue (globalKey.SecurityContext, out value);

      value = null;
      return false;
    }

    public void Clear ()
    {
      _securityContextCache.Items.Clear();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}