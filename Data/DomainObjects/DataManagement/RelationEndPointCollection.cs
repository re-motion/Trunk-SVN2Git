/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  [Serializable]
  public class RelationEndPointCollection : CommonCollection
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;

    // construction and disposing

    public RelationEndPointCollection (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      _clientTransaction = clientTransaction;
    }

    public RelationEndPointCollection (RelationEndPointCollection collection, bool makeCollectionReadOnly)
        : this (collection.ClientTransaction)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      foreach (RelationEndPoint endPoint in collection)
        Add (endPoint);

      SetIsReadOnly (makeCollectionReadOnly);
    }

    // methods and properties

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public void Combine (RelationEndPointCollection endPoints)
    {
      foreach (RelationEndPoint endPoint in endPoints)
      {
        if (!Contains (endPoint))
          Add (endPoint);
      }
    }

    public RelationEndPointCollection GetOppositeRelationEndPoints (DomainObject domainObject)
    {
      RelationEndPointCollection oppositeEndPoints = new RelationEndPointCollection (_clientTransaction);

      foreach (RelationEndPointID endPointID in _clientTransaction.GetDataContainer(domainObject).RelationEndPointIDs)
        oppositeEndPoints.Combine (GetOppositeRelationEndPoints (this[endPointID]));

      return oppositeEndPoints;
    }

    public RelationEndPointCollection GetOppositeRelationEndPoints (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      RelationEndPointCollection oppositeEndPoints = new RelationEndPointCollection (_clientTransaction);

      if (endPoint.OppositeEndPointDefinition.IsNull)
        return oppositeEndPoints;

      if (endPoint.Definition.Cardinality == CardinalityType.One)
      {
        ObjectEndPoint objectEndPoint = (ObjectEndPoint) endPoint;
        if (objectEndPoint.OppositeObjectID != null)
        {
          RelationEndPointID oppositeEndPointID = new RelationEndPointID (
              objectEndPoint.OppositeObjectID, objectEndPoint.OppositeEndPointDefinition);

          oppositeEndPoints.Add (this[oppositeEndPointID]);
        }
      }
      else
      {
        CollectionEndPoint collectionEndPoint = (CollectionEndPoint) endPoint;
        foreach (DomainObject oppositeDomainObject in collectionEndPoint.OppositeDomainObjects)
        {
          RelationEndPointID oppositeEndPointID = new RelationEndPointID (
              oppositeDomainObject.ID, collectionEndPoint.OppositeEndPointDefinition);

          oppositeEndPoints.Add (this[oppositeEndPointID]);
        }
      }

      return oppositeEndPoints;
    }

    #region Standard implementation for collections

    public bool Contains (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return BaseContains (endPoint.ID, endPoint);
    }

    public bool Contains (RelationEndPointID id)
    {
      return BaseContainsKey (id);
    }

    public RelationEndPoint this [int index]
    {
      get { return (RelationEndPoint) BaseGetObject (index); }
    }

    public RelationEndPoint this [RelationEndPointID id]
    {
      get { return (RelationEndPoint) BaseGetObject (id); }
    }

    public int Add (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return BaseAdd (endPoint.ID, endPoint);
    }

    public void Remove (int index)
    {
      Remove (this[index]);
    }

    public void Remove (RelationEndPointID id)
    {
      Remove (this[id]);
    }

    public void Remove (RelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      BaseRemove (endPoint.ID);
    }

    public void Clear ()
    {
      BaseClear();
    }

    #endregion
  }
}
