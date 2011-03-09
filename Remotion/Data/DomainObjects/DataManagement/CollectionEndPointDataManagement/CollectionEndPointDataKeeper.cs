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
    private readonly HashSet<DomainObject> _originalItemsWithoutEndPoint;

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
      _originalItemsWithoutEndPoint = new HashSet<DomainObject>();
    }

    // TODO 3774: Interface
    public IComparer<DomainObject> SortExpressionBasedComparer
    {
      get { return _sortExpressionBasedComparer; }
    }

    // TODO 3774: EndPointID property, serialize (AddHandle)

    public IDomainObjectCollectionData CollectionData
    {
      get { return _currentOppositeEndPointTracker; }
    }

    public IObjectEndPoint[] OppositeEndPoints
    {
      get { return _currentOppositeEndPointTracker.GetOppositeEndPoints(); }
    }

    public ReadOnlyCollectionDataDecorator OriginalCollectionData
    {
      get { return _collectionData.OriginalData; }
    }

    public IObjectEndPoint[] OriginalOppositeEndPoints
    {
      get { return _originalOppositeEndPoints.ToArray(); }
    }

    public DomainObject[] OriginalItemsWithoutEndPoints
    {
      get { return _originalItemsWithoutEndPoint.ToArray(); }
    }

    public bool ContainsOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      return _originalOppositeEndPoints.Contains (oppositeEndPoint);
    }

    public void RegisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (ContainsOriginalOppositeEndPoint (oppositeEndPoint))
        throw new InvalidOperationException ("The opposite end-point has already been registered.");
      
      var item = oppositeEndPoint.GetDomainObjectReference();
      _collectionData.RegisterOriginalItem (item);
      _originalOppositeEndPoints.Add (oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!ContainsOriginalOppositeEndPoint (oppositeEndPoint))
        throw new InvalidOperationException ("The opposite end-point has not been registered.");

      var itemID = oppositeEndPoint.ObjectID;
      _collectionData.UnregisterOriginalItem (itemID);
      _originalOppositeEndPoints.Remove (oppositeEndPoint);
    }

    public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _collectionData.RegisterOriginalItem (domainObject);
      _originalItemsWithoutEndPoint.Add (domainObject);
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

    public void Commit ()
    {
      _collectionData.Commit ();
      
      _originalOppositeEndPoints.Clear();
      _originalOppositeEndPoints.UnionWith (_currentOppositeEndPointTracker.GetOppositeEndPoints());
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

      _originalItemsWithoutEndPoint = new HashSet<DomainObject>();
      info.FillCollection (_originalItemsWithoutEndPoint);
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddValue (_sortExpressionBasedComparer);
      info.AddValue (_collectionData);
      info.AddValue (_currentOppositeEndPointTracker);

      info.AddCollection (_originalOppositeEndPoints);
      info.AddCollection (_originalItemsWithoutEndPoint);
    }

    #endregion
  }
}