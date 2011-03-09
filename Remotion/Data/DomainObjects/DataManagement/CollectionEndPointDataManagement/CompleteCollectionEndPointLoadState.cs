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
using System.Collections.ObjectModel;
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
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ClientTransaction _clientTransaction;

    private readonly List<IObjectEndPoint> _unsynchronizedOppositeEndPoints;

    public CompleteCollectionEndPointLoadState (
        ICollectionEndPointDataKeeper dataKeeper, 
        IRelationEndPointProvider endPointProvider, 
        ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _dataKeeper = dataKeeper;
      _endPointProvider = endPointProvider;
      _clientTransaction = clientTransaction;

      _unsynchronizedOppositeEndPoints = new List<IObjectEndPoint> ();
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
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

    public void MarkDataComplete (ICollectionEndPoint collectionEndPoint, DomainObject[] items, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already complete.");
    }

    public void MarkDataIncomplete (ICollectionEndPoint collectionEndPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      _clientTransaction.TransactionEventSink.RelationEndPointUnloading (_clientTransaction, collectionEndPoint);

      stateSetter();
      
      foreach (var oppositeEndPoint in _unsynchronizedOppositeEndPoints)
        collectionEndPoint.RegisterOppositeEndPoint (oppositeEndPoint);
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

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      var oppositeEndPointDefinition = collectionEndPoint.Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in _dataKeeper.CollectionData
             select RelationEndPointID.Create (oppositeDomainObject.ID, oppositeEndPointDefinition);
    }

    public void RegisterOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _unsynchronizedOppositeEndPoints.Add (oppositeEndPoint);
      oppositeEndPoint.MarkUnsynchronized();
    }

    public void UnregisterOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      collectionEndPoint.MarkDataIncomplete ();
      collectionEndPoint.UnregisterOppositeEndPoint (oppositeEndPoint);
    }

    public ReadOnlyCollection<IObjectEndPoint> GetUnsynchronizedOppositeEndPoints ()
    {
      return _unsynchronizedOppositeEndPoints.AsReadOnly();
    }

    public void SynchronizeOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!_unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint))
      {
        var message = string.Format (
            "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
            oppositeEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      _dataKeeper.RegisterOppositeEndPoint (oppositeEndPoint);
    }

    public IDataManagementCommand CreateSetCollectionCommand (
        ICollectionEndPoint collectionEndPoint, 
        DomainObjectCollection newCollection, 
        Action<DomainObjectCollection> collectionSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);
      ArgumentUtility.CheckNotNull ("collectionSetter", collectionSetter);

      return new CollectionEndPointSetCollectionCommand (
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
      
      return new CollectionEndPointRemoveCommand (collectionEndPoint, removedRelatedObject, _dataKeeper.CollectionData, _endPointProvider);
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

      return new CollectionEndPointInsertCommand (collectionEndPoint, index, insertedRelatedObject, _dataKeeper.CollectionData, _endPointProvider);
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
        return new CollectionEndPointReplaceSameCommand (collectionEndPoint, replacedObject);
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

    #region Serialization

    public CompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper> ();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      _unsynchronizedOppositeEndPoints = new List<IObjectEndPoint>();
       info.FillCollection (_unsynchronizedOppositeEndPoints);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_dataKeeper);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_clientTransaction);
      info.AddCollection (_unsynchronizedOppositeEndPoints);
    }

    #endregion
  }
}