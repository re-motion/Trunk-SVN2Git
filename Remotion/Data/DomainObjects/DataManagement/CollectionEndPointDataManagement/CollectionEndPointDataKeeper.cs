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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Keeps the data of a <see cref="ICollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointDataKeeper : ICollectionEndPointDataKeeper
  {
    private readonly IComparer<DomainObject> _sortExpressionBasedComparer;

    private readonly EndPointTrackingCollectionDataDecorator _currentOppositeEndPointTracker;
    private readonly ChangeCachingCollectionDataDecorator _collectionData;

    private readonly HashSet<IObjectEndPoint> _originalOppositeEndPoints;

    public CollectionEndPointDataKeeper (
        ClientTransaction clientTransaction,
        RelationEndPointID endPointID,
        IComparer<DomainObject> sortExpressionBasedComparer, 
        IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _sortExpressionBasedComparer = sortExpressionBasedComparer;

      var wrappedData = new DomainObjectCollectionData ();
      var updateListener = new CollectionDataStateUpdateListener (clientTransaction, endPointID);

      var oppositeEndPointDefinition = endPointID.Definition.GetMandatoryOppositeEndPointDefinition();
      _collectionData = new ChangeCachingCollectionDataDecorator (wrappedData, updateListener);
      _currentOppositeEndPointTracker = new EndPointTrackingCollectionDataDecorator (_collectionData, endPointProvider, oppositeEndPointDefinition);
      
      _originalOppositeEndPoints = new HashSet<IObjectEndPoint>();
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

    public IObjectEndPoint[] OriginalOppositeEndPoints
    {
      get { return _originalOppositeEndPoints.ToArray(); }
    }

    public bool ContainsOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      return _originalOppositeEndPoints.Contains (oppositeEndPoint);
    }

    public void RegisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      // TODO 3771
      //if (ContainsOriginalOppositeEndPoint (oppositeEndPoint))
      //  throw new InvalidOperationException ("The opposite end-point has already been registered.");
      
      var item = oppositeEndPoint.GetDomainObjectReference();
      _collectionData.RegisterOriginalItem (item);
      _originalOppositeEndPoints.Add (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized ();
    }

    public void UnregisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      // TODO 3771
      //if (!ContainsOriginalOppositeEndPoint (oppositeEndPoint))
      //  throw new InvalidOperationException ("The opposite end-point has not been registered.");

      var itemID = oppositeEndPoint.ObjectID;
      _collectionData.UnregisterOriginalItem (itemID);
      _originalOppositeEndPoints.Remove (oppositeEndPoint);
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
      // TODO 3771: _originalOppositeEndPoints = _tracker.GetOppositeEndPoints();
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private CollectionEndPointDataKeeper (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _sortExpressionBasedComparer = info.GetValue<IComparer<DomainObject>>();
      _collectionData = info.GetValue<ChangeCachingCollectionDataDecorator>();
      _currentOppositeEndPointTracker = info.GetValue<EndPointTrackingCollectionDataDecorator>();

      _originalOppositeEndPoints = new HashSet<IObjectEndPoint>();
      info.FillCollection (_originalOppositeEndPoints);
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddValue (_sortExpressionBasedComparer);
      info.AddValue (_collectionData);
      info.AddValue (_currentOppositeEndPointTracker);

      info.AddCollection (_originalOppositeEndPoints);
    }

    #endregion
  }
}