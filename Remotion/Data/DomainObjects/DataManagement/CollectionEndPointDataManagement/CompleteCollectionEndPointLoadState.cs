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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteCollectionEndPointLoadState : ICollectionEndPointLoadState
  {
    private readonly ICollectionEndPointDataKeeper _dataKeeper;
    private readonly ClientTransaction _clientTransaction;

    public CompleteCollectionEndPointLoadState (ICollectionEndPointDataKeeper dataKeeper, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _dataKeeper = dataKeeper;
      _clientTransaction = clientTransaction;
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public bool IsDataComplete ()
    {
      return true;
    }

    public void EnsureDataComplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      // Data is already complete
    }

    public IDomainObjectCollectionData GetCollectionData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return _dataKeeper.CollectionData;
    }

    public DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      var collectionType = collectionEndPoint.Definition.PropertyType;
      return DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, _dataKeeper.OriginalCollectionData);
    }

    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (ICollectionEndPoint collectionEndPoint, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var oppositeEndPointDefinition = collectionEndPoint.Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in _dataKeeper.CollectionData
             let oppositeEndPointID = new RelationEndPointID (oppositeDomainObject.ID, oppositeEndPointDefinition)
             select dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    public void RegisterOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.RegisterOriginalObject (oppositeEndPoint.GetDomainObjectReference());
    }

    public void UnregisterOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      collectionEndPoint.MarkDataIncomplete ();
      collectionEndPoint.UnregisterOppositeEndPoint (oppositeEndPoint);
    }

    public IDataManagementCommand CreateSetOppositeCollectionCommand (
        ICollectionEndPoint collectionEndPoint, 
        DomainObjectCollection newCollection, 
        Action<DomainObjectCollection> collectionSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);
      ArgumentUtility.CheckNotNull ("collectionSetter", collectionSetter);

      return new CollectionEndPointReplaceWholeCollectionCommand (
          collectionEndPoint,
          newCollection,
          collectionSetter,
          collectionEndPoint.Collection,
          newCollection);
    }

    public IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      
      return new CollectionEndPointRemoveCommand (collectionEndPoint, removedRelatedObject, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return new CollectionEndPointDeleteCommand (collectionEndPoint, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);

      return new CollectionEndPointInsertCommand (collectionEndPoint, index, insertedRelatedObject, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return CreateInsertCommand (collectionEndPoint, addedRelatedObject, _dataKeeper.CollectionData.Count);
    }

    public IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      var replacedObject = _dataKeeper.CollectionData.GetObject(index);
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameCommand (collectionEndPoint, replacedObject, _dataKeeper.CollectionData);
      else
        return new CollectionEndPointReplaceCommand (collectionEndPoint, replacedObject, index, replacementObject, _dataKeeper.CollectionData);
    }

    public void SetValueFrom (ICollectionEndPoint collectionEndPoint, ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      _dataKeeper.CollectionData.ReplaceContents (sourceEndPoint.Collection.Cast<DomainObject> ());

      if (sourceEndPoint.HasBeenTouched || collectionEndPoint.HasChanged)
        collectionEndPoint.Touch ();
    }

    public void CheckMandatory (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (_dataKeeper.CollectionData.Count == 0)
      {
        var objectReference = collectionEndPoint.GetDomainObjectReference ();
        var message = String.Format (
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            collectionEndPoint.Definition.PropertyName,
            objectReference.ID);
        throw new MandatoryRelationNotSetException (objectReference, collectionEndPoint.Definition.PropertyName, message);
      }
    }

    public void OnDataMarkedComplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      // ignore, data is already complete
    }

    public void OnDataMarkedIncomplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      _clientTransaction.TransactionEventSink.RelationEndPointUnloading (_clientTransaction, collectionEndPoint);
    }

    #region Serialization

    public CompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper>();
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_dataKeeper);
      info.AddHandle (_clientTransaction);
    }

    #endregion
  }
}