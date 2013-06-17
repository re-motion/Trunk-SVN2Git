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
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  [Serializable]
  public sealed class GlobalAccessTypeCache : IGlobalAccessTypeCache, ISerializable, IObjectReference
  {
    //TODO RM-5521: test
    private class Repository : RepositoryBase<Repository.Data, RevisionKey, Int32RevisionValue>
    {
      private readonly RevisionKey _revisionKey = new RevisionKey();

      public class Data : RevisionBasedData
      {
        public readonly LazyLockingCachingAdapter<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> AccessTypes;

        internal Data (Int32RevisionValue revision)
            : base (revision)
        {
          AccessTypes = CacheFactory.CreateWithLazyLocking<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>();
        }
      }

      public Repository (IRevisionProvider<RevisionKey, Int32RevisionValue> revisionProvider)
          : base (revisionProvider)
      {
      }

      public ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> AccessTypes
      {
        get { return GetCachedData (_revisionKey).AccessTypes; }
      }

      protected override Data LoadData (Int32RevisionValue revision)
      {
        return new Data (revision);
      }
    }

    private readonly Repository _repository;

    public GlobalAccessTypeCache (IDomainRevisionProvider revisionProvider)
    {
      ArgumentUtility.CheckNotNull ("revisionProvider", revisionProvider);
      _repository = new Repository (revisionProvider);
    }

    private GlobalAccessTypeCache (SerializationInfo info, StreamingContext context)
    {
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return (GlobalAccessTypeCache) SafeServiceLocator.Current.GetAllInstances<IGlobalAccessTypeCache>()
          .First (() => new InvalidOperationException ("No instance of IGlobalAccessTypeCache has been registered with the ServiceLocator."));
    }

    public AccessType[] GetOrCreateValue (
        Tuple<ISecurityContext, ISecurityPrincipal> key,
        Func<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      return _repository.AccessTypes.GetOrCreateValue (key, valueFactory);
    }

    public bool TryGetValue (Tuple<ISecurityContext, ISecurityPrincipal> key, out AccessType[] value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _repository.AccessTypes.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      _repository.AccessTypes.Clear();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}