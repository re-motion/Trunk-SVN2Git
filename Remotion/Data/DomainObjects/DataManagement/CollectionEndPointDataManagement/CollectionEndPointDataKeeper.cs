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
    private readonly RelationEndPointID _endPointID;
    private readonly IComparer<DomainObject> _sortExpressionBasedComparer;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;

    private readonly ChangeCachingCollectionDataDecorator _changeCachingCollectionData;

    private readonly HashSet<IObjectEndPoint> _originalOppositeEndPoints;
    private readonly HashSet<DomainObject> _originalItemsWithoutEndPoint;

    public CollectionEndPointDataKeeper (
        ClientTransaction clientTransaction,
        RelationEndPointID endPointID,
        IComparer<DomainObject> sortExpressionBasedComparer, 
        IRelationEndPointProvider endPointProvider,
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);

      _endPointID = endPointID;
      _sortExpressionBasedComparer = sortExpressionBasedComparer;
      _endPointProvider = endPointProvider;
      _changeDetectionStrategy = changeDetectionStrategy;

      var wrappedData = new DomainObjectCollectionData ();
      var updateListener = new CollectionDataStateUpdateListener (clientTransaction, endPointID);
      _changeCachingCollectionData = new ChangeCachingCollectionDataDecorator (wrappedData, updateListener);
     
      _originalOppositeEndPoints = new HashSet<IObjectEndPoint>();
      _originalItemsWithoutEndPoint = new HashSet<DomainObject>();
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public IComparer<DomainObject> SortExpressionBasedComparer
    {
      get { return _sortExpressionBasedComparer; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ICollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _changeDetectionStrategy; }
    }

    public IDomainObjectCollectionData CollectionData
    {
      get { return _changeCachingCollectionData; }
    }

    public ReadOnlyCollectionDataDecorator OriginalCollectionData
    {
      get { return _changeCachingCollectionData.OriginalData; }
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

      if (_originalItemsWithoutEndPoint.Contains (item))
        _originalItemsWithoutEndPoint.Remove (item);
      else
        _changeCachingCollectionData.RegisterOriginalItem (item);

      _originalOppositeEndPoints.Add (oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!ContainsOriginalOppositeEndPoint (oppositeEndPoint))
        throw new InvalidOperationException ("The opposite end-point has not been registered.");

      var itemID = oppositeEndPoint.ObjectID;
      _changeCachingCollectionData.UnregisterOriginalItem (itemID);
      _originalOppositeEndPoints.Remove (oppositeEndPoint);
    }

    public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _changeCachingCollectionData.RegisterOriginalItem (domainObject);
      _originalItemsWithoutEndPoint.Add (domainObject);
    }

    public bool HasDataChanged ()
    {
      return _changeCachingCollectionData.HasChanged (_changeDetectionStrategy);
    }

    public void SortCurrentAndOriginalData()
    {
      if (_sortExpressionBasedComparer != null)
        _changeCachingCollectionData.SortOriginalAndCurrent (_sortExpressionBasedComparer);
    }

    public void Commit ()
    {
      _changeCachingCollectionData.Commit ();
      
      _originalOppositeEndPoints.Clear();
      _originalItemsWithoutEndPoint.Clear();

      var oppositeEndPointDefinition = _endPointID.Definition.GetMandatoryOppositeEndPointDefinition ();
      foreach (var item in OriginalCollectionData)
      {
        var endPoint = GetOppositeEndPoint (item.ID, oppositeEndPointDefinition);
        if (endPoint != null)
          _originalOppositeEndPoints.Add (endPoint);
        else
          _originalItemsWithoutEndPoint.Add (item);
      }

      Assertion.IsTrue (OriginalCollectionData.Count == _originalOppositeEndPoints.Count + _originalItemsWithoutEndPoint.Count);
    }

    private IObjectEndPoint GetOppositeEndPoint (ObjectID domainObjectID, IRelationEndPointDefinition oppositeEndPointDefinition)
    {
      var endPointID = RelationEndPointID.Create (domainObjectID, oppositeEndPointDefinition);
      return (IObjectEndPoint) _endPointProvider.GetRelationEndPointWithoutLoading (endPointID);
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private CollectionEndPointDataKeeper (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _endPointID = info.GetValueForHandle<RelationEndPointID> ();
      _sortExpressionBasedComparer = info.GetValue<IComparer<DomainObject>> ();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _changeDetectionStrategy = info.GetValueForHandle<ICollectionEndPointChangeDetectionStrategy> ();

      _changeCachingCollectionData = info.GetValue<ChangeCachingCollectionDataDecorator> ();

      _originalOppositeEndPoints = new HashSet<IObjectEndPoint>();
      info.FillCollection (_originalOppositeEndPoints);

      _originalItemsWithoutEndPoint = new HashSet<DomainObject>();
      info.FillCollection (_originalItemsWithoutEndPoint);
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_endPointID);
      info.AddValue (_sortExpressionBasedComparer);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_changeDetectionStrategy);
      info.AddValue (_changeCachingCollectionData);

      info.AddCollection (_originalOppositeEndPoints);
      info.AddCollection (_originalItemsWithoutEndPoint);
    }

    #endregion
  }
}