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
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Keeps the data of a <see cref="ICollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointDataKeeper : ICollectionEndPointDataKeeper
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;
    private readonly IComparer<DomainObject> _sortExpressionBasedComparer;

    private readonly ChangeCachingCollectionDataDecorator _collectionData;

    private readonly HashSet<IObjectEndPoint> _oppositeEndPoints;

    public CollectionEndPointDataKeeper (
        ClientTransaction clientTransaction,
        RelationEndPointID endPointID,
        IComparer<DomainObject> sortExpressionBasedComparer)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      _clientTransaction = clientTransaction;
      _endPointID = endPointID;
      _sortExpressionBasedComparer = sortExpressionBasedComparer;

      var wrappedData = new DomainObjectCollectionData ();
      var updateListener = new CollectionDataStateUpdateListener (_clientTransaction, _endPointID);
      _collectionData = new ChangeCachingCollectionDataDecorator (wrappedData, updateListener);
      
      _oppositeEndPoints = new HashSet<IObjectEndPoint>();
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

    public IDomainObjectCollectionData CollectionData
    {
      get { return _collectionData; }
    }

    public ReadOnlyCollectionDataDecorator OriginalCollectionData
    {
      get { return _collectionData.OriginalData; }
    }

    public IObjectEndPoint[] OppositeEndPoints
    {
      get { return _oppositeEndPoints.ToArray(); }
    }

    public void RegisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);
      
      var item = oppositeEndPoint.GetDomainObjectReference();
      _collectionData.RegisterOriginalItem (item);
      _oppositeEndPoints.Add (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized ();
    }

    public void UnregisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      var itemID = oppositeEndPoint.ObjectID;
      _collectionData.UnregisterOriginalItem (itemID);
      _oppositeEndPoints.Remove (oppositeEndPoint);
      oppositeEndPoint.MarkUnsynchronized ();
    }

    public bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      return _collectionData.HasChanged (changeDetectionStrategy);
    }

    public void SortCurrentAndOriginalData()
    {
      if (_sortExpressionBasedComparer != null)
        _collectionData.SortOriginalAndCurrent (_sortExpressionBasedComparer);
    }

    public void CommitOriginalContents ()
    {
      _collectionData.Commit ();
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private CollectionEndPointDataKeeper (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      _endPointID = info.GetValue<RelationEndPointID>();
      _sortExpressionBasedComparer = info.GetValue<IComparer<DomainObject>>();
      
      _collectionData = info.GetValue<ChangeCachingCollectionDataDecorator>();

      _oppositeEndPoints = new HashSet<IObjectEndPoint>();
      info.FillCollection (_oppositeEndPoints);
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_clientTransaction);
      info.AddValue (_endPointID);
      info.AddValue (_sortExpressionBasedComparer);
      info.AddValue (_collectionData);
      info.AddCollection (_oppositeEndPoints);
    }

    #endregion
  }
}