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
  /// Implements the <see cref="IDomainObjectCollectionData"/> interface for lazy loading. This class decorates an inner 
  /// <see cref="IDomainObjectCollectionData"/> object, but allows that inner object to be unloaded. When the 
  /// <see cref="LazyLoadingDomainObjectCollectionData"/> is accessed and its inner object is <see langword="null" />, it loads the data from a
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  public class LazyLoadingDomainObjectCollectionData : IDomainObjectCollectionData
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;

    private DomainObjectCollectionData _actualData;

    public LazyLoadingDomainObjectCollectionData (
        ClientTransaction clientTransaction, 
        RelationEndPointID endPointID, 
        IEnumerable<DomainObject> initialContents)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      _clientTransaction = clientTransaction;
      _endPointID = endPointID;

      _actualData = initialContents != null ? new DomainObjectCollectionData (initialContents) : null;
    }

    public bool IsDataAvailable
    {
      get { return _actualData != null; }
    }

    public DomainObjectCollectionData ActualData
    {
      get
      {
        if (!IsDataAvailable)
        {
          var contents = _clientTransaction.LoadRelatedObjects (_endPointID);
          _actualData = new DomainObjectCollectionData (contents);
        }

        Assertion.IsNotNull (_actualData);
        return _actualData;
      }
    }

    ICollectionEndPoint IDomainObjectCollectionData.AssociatedEndPoint
    {
      get { return null; }
    }

    public int Count
    {
      get { return ActualData.Count; }
    }

    Type IDomainObjectCollectionData.RequiredItemType
    {
      get { return null; }
    }

    public void Unload ()
    {
      _actualData = null;
    }

    IDomainObjectCollectionData IDomainObjectCollectionData.GetUndecoratedDataStore ()
    {
      return this;
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return ActualData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return ((IEnumerable) ActualData).GetEnumerator ();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      return ActualData.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return ActualData.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      return ActualData.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      return ActualData.IndexOf (objectID);
    }

    public void Clear ()
    {
      ActualData.Clear();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ActualData.Insert (index, domainObject);
    }

    public bool Remove (DomainObject domainObject)
    {
      return ActualData.Remove (domainObject);
    }

    public bool Remove (ObjectID objectID)
    {
      return ActualData.Remove (objectID);
    }

    public void Replace (int index, DomainObject value)
    {
      ActualData.Replace (index, value);
    }
  }
}