// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Implements a decorator for <see cref="IDomainObjectCollectionData"/> that performs semantic checks on the arguments passed to 
  /// <see cref="Insert"/> or <see cref="Replace"/>. Use this to avoid <see cref="Insert"/> or <see cref="Replace"/> being called on the wrapped
  /// <see cref="IDomainObjectCollectionData"/> instance if the operation cannot be executed because of the arguments.
  /// </summary>
  [Serializable]
  public class ArgumentCheckingCollectionDataDecorator : IDomainObjectCollectionData
  {
    private readonly IDomainObjectCollectionData _wrappedData;

    public ArgumentCheckingCollectionDataDecorator (IDomainObjectCollectionData wrappedData)
    {
      ArgumentUtility.CheckNotNull ("wrappedData", wrappedData);
      _wrappedData = wrappedData;
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _wrappedData.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public int Count
    {
      get { return _wrappedData.Count; }
    }

    public bool IsReadOnly
    {
      get { return _wrappedData.IsReadOnly; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.ContainsObjectID(objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _wrappedData.GetObject(index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.GetObject(objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.IndexOf(objectID);
    }

    public void Clear ()
    {
      _wrappedData.Clear();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      if (ContainsObjectID (domainObject.ID))
        throw new InvalidOperationException (string.Format ("The collection already contains an object with ID '{0}'.", domainObject.ID));

      _wrappedData.Insert(index, domainObject);
    }

    public void Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _wrappedData.Remove (domainObject);
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      int index = IndexOf (oldDomainObjectID);
      if (index == -1)
        throw new KeyNotFoundException (string.Format ("The collection does not contain a DomainObject with ID '{0}'.", oldDomainObjectID));

      if (ContainsObjectID (newDomainObject.ID) && oldDomainObjectID != newDomainObject.ID)
        throw new InvalidOperationException (string.Format ("The collection already contains an object with ID '{0}'.", newDomainObject.ID));

      _wrappedData.Replace (oldDomainObjectID, newDomainObject);
    }
  }
}
