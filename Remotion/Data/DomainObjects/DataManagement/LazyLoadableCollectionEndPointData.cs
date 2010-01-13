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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Implements lazy-loading support for the <see cref="CollectionEndPoint"/> class by wrapping the data kept by a <see cref="CollectionEndPoint"/> 
  /// and allowing that data to be unloaded. When the <see cref="LazyLoadableCollectionEndPointData"/> is accessed and its data is empty, 
  /// it loads the data from a <see cref="ClientTransaction"/>.
  /// </summary>
  public class LazyLoadableCollectionEndPointData : IFlattenedSerializable
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;

    private DomainObjectCollectionData _dataStore;
    private DomainObjectCollection _originalOppositeDomainObjectsContents;

    public LazyLoadableCollectionEndPointData (
        ClientTransaction clientTransaction, 
        RelationEndPointID endPointID, 
        IEnumerable<DomainObject> initialContents)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      _clientTransaction = clientTransaction;
      _endPointID = endPointID;

      if (initialContents != null)
        SetContents (initialContents);
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public bool IsDataAvailable
    {
      get { return _dataStore != null; }
    }

    public DomainObjectCollectionData DataStore
    {
      get
      {
        EnsureDataAvailable();

        Assertion.IsNotNull (_dataStore);
        return _dataStore;
      }
    }

    public DomainObjectCollection OriginalOppositeDomainObjectsContents
    {
      get 
      {
        EnsureDataAvailable ();

        Assertion.IsNotNull (_originalOppositeDomainObjectsContents);
        return _originalOppositeDomainObjectsContents; 
      }
    }

    public void EnsureDataAvailable ()
    {
      if (!IsDataAvailable)
      {
        var contents = _clientTransaction.LoadRelatedObjects (_endPointID);
        SetContents (contents);
      }
    }

    public void Unload ()
    {
      _dataStore = null;
      _originalOppositeDomainObjectsContents = null; // this is an optimization to allow the DomainObjectCollection to be garbage-collected
    }

    private void SetContents (IEnumerable<DomainObject> initialContents)
    {
      _dataStore = new DomainObjectCollectionData (initialContents);
      var collectionType = _endPointID.Definition.PropertyType;
      _originalOppositeDomainObjectsContents = new DomainObjectCollectionFactory ().CreateCollection (collectionType, initialContents).AsReadOnly ();
    }

    #region Serialization

    protected LazyLoadableCollectionEndPointData (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _clientTransaction = info.GetValueForHandle<ClientTransaction> ();
      _endPointID = info.GetValueForHandle<RelationEndPointID> ();
      _dataStore = info.GetValue<DomainObjectCollectionData> ();
      _originalOppositeDomainObjectsContents = info.GetValue<DomainObjectCollection> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_clientTransaction);
      info.AddHandle (_endPointID);

      info.AddValue (_dataStore);
      info.AddValue (_originalOppositeDomainObjectsContents);
    }
    #endregion
  }
}