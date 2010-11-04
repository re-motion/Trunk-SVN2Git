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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Implements lazy-loading support for the <see cref="CollectionEndPoint"/> class by wrapping the data kept by a <see cref="CollectionEndPoint"/> 
  /// and allowing that data to be unloaded. When the <see cref="LazyLoadableCollectionEndPointData"/> is accessed and its data is empty, 
  /// it loads the data from a <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class LazyLoadableCollectionEndPointData : ICollectionEndPointData, ICollectionDataStateUpdateListener
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;

    private ChangeCachingCollectionDataDecorator _collectionData;

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
      get { return _collectionData != null; }
    }

    public IDomainObjectCollectionData CollectionData
    {
      get
      {
        EnsureDataAvailable ();

        Assertion.IsNotNull (_collectionData);
        return _collectionData;
      }
    }

    public IDomainObjectCollectionData OriginalCollectionData
    {
      get
      {
        EnsureDataAvailable ();

        return _collectionData.OriginalData;
      }
    }

    public bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      if (!IsDataAvailable)
        return false; // if we are unloaded, we're definitely unchanged
      else
        return _collectionData.HasChanged (changeDetectionStrategy);
    }

    public void EnsureDataAvailable ()
    {
      if (!IsDataAvailable)
      {
        var contents = _clientTransaction.LoadRelatedObjects (_endPointID);
        SetContents (contents);

        RaiseChangeStateNotification (false);
      }
    }

    public void Unload ()
    {
      _collectionData = null;

      RaiseChangeStateNotification (false);
    }

    public void CommitOriginalContents ()
    {
      if (IsDataAvailable)
        _collectionData.Commit ();
    }

    private void SetContents (IEnumerable<DomainObject> initialContents)
    {
      var dataStore = new DomainObjectCollectionData (initialContents);
      _collectionData = new ChangeCachingCollectionDataDecorator (dataStore, this);
    }

    private void RaiseChangeStateNotification (bool? newChangedState)
    {
      ClientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (ClientTransaction, EndPointID, newChangedState);
    }

    void ICollectionDataStateUpdateListener.StateUpdated (bool? newChangedState)
    {
      RaiseChangeStateNotification (newChangedState);
    }
  }
}