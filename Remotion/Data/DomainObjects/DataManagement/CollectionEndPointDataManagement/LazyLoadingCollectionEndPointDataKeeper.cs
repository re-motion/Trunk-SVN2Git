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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Implements lazy-loading support for the <see cref="CollectionEndPoint"/> class by wrapping the data kept by a <see cref="CollectionEndPoint"/> 
  /// and allowing that data to be unloaded. When the <see cref="LazyLoadingCollectionEndPointDataKeeper"/> is accessed and its data is empty, 
  /// it loads the data from a <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class LazyLoadingCollectionEndPointDataKeeper : ICollectionEndPointDataKeeper, ICollectionDataStateUpdateListener
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;
    private readonly IComparer<DomainObject> _sortExpressionBasedComparer;
    
    private readonly ChangeCachingCollectionDataDecorator _collectionData;
    private bool _isDataAvailable;
    
    public LazyLoadingCollectionEndPointDataKeeper (
        ClientTransaction clientTransaction,
        RelationEndPointID endPointID,
        IComparer<DomainObject> sortExpressionBasedComparer,
        IEnumerable<DomainObject> initialContents)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      _clientTransaction = clientTransaction;
      _endPointID = endPointID;
      _sortExpressionBasedComparer = sortExpressionBasedComparer;

      var wrappedData = new DomainObjectCollectionData (initialContents ?? Enumerable.Empty<DomainObject> ());
      _collectionData = new ChangeCachingCollectionDataDecorator (wrappedData, this);
      _isDataAvailable = initialContents != null;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public IComparer<DomainObject> SortExpressionBasedComparer
    {
      get { return _sortExpressionBasedComparer; }
    }

    public bool IsDataAvailable
    {
      get { return _isDataAvailable; }
    }

    public IDomainObjectCollectionData CollectionData
    {
      get { return _collectionData; }
    }

    public IDomainObjectCollectionData OriginalCollectionData
    {
      get { return _collectionData.OriginalData; }
    }

    public void RegisterOriginalObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _collectionData.RegisterOriginalItem (domainObject);
    }

    public void UnregisterOriginalObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      _collectionData.UnregisterOriginalItem (objectID);
    }

    public bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      return _collectionData.HasChanged (changeDetectionStrategy);
    }

    public void EnsureDataAvailable ()
    {
      if (!IsDataAvailable)
      {
        _clientTransaction.LoadRelatedObjects (_endPointID);

        MarkDataAvailable ();
      }
    }

    public void MarkDataAvailable ()
    {
      if (_sortExpressionBasedComparer != null)
        _collectionData.SortOriginalAndCurrent (_sortExpressionBasedComparer);

      _isDataAvailable = true;
    }

    public void Unload ()
    {
      _isDataAvailable = false;
    }

    public void CommitOriginalContents ()
    {
      _collectionData.Commit ();
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