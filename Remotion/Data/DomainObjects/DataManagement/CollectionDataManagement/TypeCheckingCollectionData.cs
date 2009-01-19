// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// This class acts as a type-checking adapter for another <see cref="IDomainObjectTransactionContext"/> object. Every inserting method of the
  /// <see cref="IDomainObjectCollectionData"/> interface will check the type of the new item for a given required item <see cref="Type"/>.
  /// </summary>
  [Serializable]
  public class TypeCheckingCollectionData : IDomainObjectCollectionData
  {
    private readonly IDomainObjectCollectionData _data;
    private readonly Type _requiredItemType;

    public TypeCheckingCollectionData (IDomainObjectCollectionData data, Type requiredItemType)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("requiredItemType", requiredItemType);

      _data = data;
      _requiredItemType = requiredItemType;
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count
    {
      get { return _data.Count; }
    }

    public bool IsReadOnly
    {
      get { return _data.IsReadOnly; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _data.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _data.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _data.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _data.IndexOf (objectID);
    }

    public void Clear ()
    {
      _data.Clear ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      CheckItemType (domainObject, "domainObject");

      _data.Insert (index, domainObject);
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _data.Remove (objectID);
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);
      CheckItemType (newDomainObject, "newDomainObject");

      _data.Replace (oldDomainObjectID, newDomainObject);
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
  }
}