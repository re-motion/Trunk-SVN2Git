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
  /// Decorates another implementation of <see cref="IDomainObjectCollectionData"/> with functionality for tracking the <see cref="IObjectEndPoint"/>
  /// instances associated with the items being added to or removed from the collection.
  /// </summary>
  public class EndPointTrackingCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase, IFlattenedSerializable
  {
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IRelationEndPointDefinition _objectEndPointDefinition;
    private readonly HashSet<IObjectEndPoint> _oppositeEndPoints;

    public EndPointTrackingCollectionDataDecorator (
        IDomainObjectCollectionData wrappedData, 
        IRelationEndPointProvider endPointProvider, 
        IRelationEndPointDefinition objectEndPointDefinition)
      : base (ArgumentUtility.CheckNotNull ("wrappedData", wrappedData))
    {
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("objectEndPointDefinition", objectEndPointDefinition);

      _endPointProvider = endPointProvider;
      _objectEndPointDefinition = objectEndPointDefinition;

      _oppositeEndPoints = new HashSet<IObjectEndPoint> (wrappedData.Select (d => GetEndPoint (d.ID)));
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IRelationEndPointDefinition ObjectEndPointDefinition
    {
      get { return _objectEndPointDefinition; }
    }

    public HashSet<IObjectEndPoint> OppositeEndPoints
    {
      get { return _oppositeEndPoints; }
    }

    public IObjectEndPoint[] GetOppositeEndPoints()
    {
      return _oppositeEndPoints.ToArray();
    }

    public override void Clear ()
    {
      base.Clear ();

      _oppositeEndPoints.Clear();
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      base.Insert (index, domainObject);

      var endPoint = GetEndPoint (domainObject.ID);
      _oppositeEndPoints.Add (endPoint);
    }

    public override bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      base.Remove (domainObject);

      var endPoint = GetEndPoint (domainObject.ID);
      return _oppositeEndPoints.Remove (endPoint);
    }

    public override bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      base.Remove (objectID);

      var endPoint = GetEndPoint (objectID);
      return _oppositeEndPoints.Remove (endPoint);
    }

    public override void Replace (int index, DomainObject value)
    {
      base.Replace (index, value);
      // TODO 3771: Remove end-point of old value, add end-point of new value
    }
    
    private IObjectEndPoint GetEndPoint (ObjectID domainObjectID)
    {
      var endPointID = RelationEndPointID.Create (domainObjectID, _objectEndPointDefinition);
      // TODO 3771: Use GetRelationEndPointWithoutLoading, assert that an end-point is always returned
      return (IObjectEndPoint) _endPointProvider.GetRelationEndPointWithLazyLoad (endPointID);
    }

    #region Serialization
    
    public EndPointTrackingCollectionDataDecorator (FlattenedDeserializationInfo info)
      : base (info.GetValue<IDomainObjectCollectionData> ())
    {
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _objectEndPointDefinition = info.GetValueForHandle<IRelationEndPointDefinition> ();
      _oppositeEndPoints = new HashSet<IObjectEndPoint>();
      info.FillCollection (_oppositeEndPoints);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (WrappedData);
      info.AddHandle(_endPointProvider);
      info.AddHandle(_objectEndPointDefinition);
      info.AddCollection (_oppositeEndPoints);
    }
    
    #endregion Serialization
  }
}