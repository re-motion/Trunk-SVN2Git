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
  /// <see cref="Insert"/>, <see cref="Replace"/>, and <see cref="Remove(Remotion.Data.DomainObjects.DomainObject)"/>.
  /// </summary>
  [Serializable]
  public class ArgumentCheckingCollectionDataDecorator : IDomainObjectCollectionData
  {
    private readonly IDomainObjectCollectionData _wrappedData;
    private readonly Type _requiredItemType;

    public ArgumentCheckingCollectionDataDecorator (IDomainObjectCollectionData wrappedData, Type requiredItemType)
    {
      ArgumentUtility.CheckNotNull ("wrappedData", wrappedData);

      _wrappedData = wrappedData;
      _requiredItemType = requiredItemType;
    }

    public Type RequiredItemType
    {
      get { return _requiredItemType; }
    }

    public int Count
    {
      get { return _wrappedData.Count; }
    }

    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _wrappedData.AssociatedEndPoint; }
    }

    public IDomainObjectCollectionData GetUndecoratedDataStore ()
    {
      return _wrappedData.GetUndecoratedDataStore ();
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
      return _wrappedData.GetObject (objectID);
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

      if (index < 0 || index > Count)
      {
        throw new ArgumentOutOfRangeException (
            "index",
            index,
            "Index is out of range. Must be non-negative and less than or equal to the size of the collection.");
      }

      if (ContainsObjectID (domainObject.ID))
        throw new InvalidOperationException (string.Format ("The collection already contains an object with ID '{0}'.", domainObject.ID));

      CheckItemType (domainObject, "domainObject");

      _wrappedData.Insert (index, domainObject);
    }

    public void Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      var existingObject = GetObject (domainObject.ID);
      if (existingObject != null && existingObject != domainObject)
      {
        var message = "The object to be removed has the same ID as an object in this collection, but is a different object reference.";
        throw new ArgumentException (message, "domainObject");
      }

      _wrappedData.Remove (domainObject);
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _wrappedData.Remove (objectID);
    }

    public void Replace (int index, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      if (index < 0 || index >= Count)
      {
        throw new ArgumentOutOfRangeException (
            "index",
            index,
            "Index is out of range. Must be non-negative and less than the size of the collection.");
      }

      if (ContainsObjectID (newDomainObject.ID) && !ReferenceEquals (GetObject (index), newDomainObject))
      {
        var message = string.Format ("The collection already contains an object with ID '{0}'.", newDomainObject.ID);
        throw new InvalidOperationException (message);
      }

      CheckItemType (newDomainObject, "newDomainObject");

      _wrappedData.Replace (index, newDomainObject);
    }

    private void CheckItemType (DomainObject domainObject, string argumentName)
    {
      if (_requiredItemType != null && !_requiredItemType.IsInstanceOfType (domainObject))
      {
        string message = string.Format ("Values of type '{0}' cannot be added to this collection. Values must be of type '{1}' or derived from '{1}'.",
            domainObject.GetPublicDomainObjectType (), _requiredItemType);
        throw new ArgumentTypeException (message, argumentName, _requiredItemType, domainObject.GetPublicDomainObjectType ());
      }
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _wrappedData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}
