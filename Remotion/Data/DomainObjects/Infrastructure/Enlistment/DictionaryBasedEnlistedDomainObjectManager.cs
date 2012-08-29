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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Manages the enlisted objects via a <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  [Serializable]
  public class DictionaryBasedEnlistedDomainObjectManager : IEnlistedDomainObjectManager
  {
    private readonly Dictionary<ObjectID, DomainObject> _enlistedObjects = new Dictionary<ObjectID, DomainObject> ();

    public int EnlistedDomainObjectCount
    {
      get { return _enlistedObjects.Count; }
    }

    public IEnumerable<DomainObject> GetEnlistedDomainObjects ()
    {
      return _enlistedObjects.Values;
    }

    public DomainObject GetEnlistedDomainObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _enlistedObjects.GetValueOrDefault (objectID);
    }

    public bool EnlistDomainObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      DomainObject alreadyEnlistedObject = GetEnlistedDomainObject (domainObject.ID);
      if (alreadyEnlistedObject != null && alreadyEnlistedObject != domainObject)
      {
        string message = string.Format ("A domain object instance for object '{0}' already exists in this transaction.", domainObject.ID);
        throw new InvalidOperationException (message);
      }
      else if (alreadyEnlistedObject == null)
      {
        _enlistedObjects.Add (domainObject.ID, domainObject);
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool IsEnlisted (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      return GetEnlistedDomainObject (domainObject.ID) == domainObject;
    }
  }
}