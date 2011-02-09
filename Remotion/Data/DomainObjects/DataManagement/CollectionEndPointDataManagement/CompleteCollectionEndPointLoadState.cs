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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  [Serializable]
  public class CompleteCollectionEndPointLoadState : ICollectionEndPointLoadState
  {
    [NonSerialized] // Workaround for flattened serialization, see CollectionEndPoint.FixupLoadState
    private readonly ICollectionEndPoint _collectionEndPoint;

    private readonly ICollectionEndPointDataKeeper _dataKeeper;

    public CompleteCollectionEndPointLoadState (ICollectionEndPoint collectionEndPoint, ICollectionEndPointDataKeeper dataKeeper)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);

      _collectionEndPoint = collectionEndPoint;
      _dataKeeper = dataKeeper;
    }

    public ICollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public void EnsureDataComplete ()
    {
      // Data is already complete
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      var collectionType = _collectionEndPoint.Definition.PropertyType;
      return DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, _dataKeeper.OriginalCollectionData);
    }

    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var oppositeEndPointDefinition = _collectionEndPoint.Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in _dataKeeper.CollectionData
             let oppositeEndPointID = new RelationEndPointID (oppositeDomainObject.ID, oppositeEndPointDefinition)
             select dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    public IDataManagementCommand CreateSetOppositeCollectionCommand (IAssociatableDomainObjectCollection newOppositeCollection)
    {
      ArgumentUtility.CheckNotNull ("newOppositeCollection", newOppositeCollection);

      return newOppositeCollection.CreateAssociationCommand (_collectionEndPoint);
    }

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      return new CollectionEndPointRemoveCommand (_collectionEndPoint, removedRelatedObject, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      return new CollectionEndPointDeleteCommand (_collectionEndPoint, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);

      return new CollectionEndPointInsertCommand (_collectionEndPoint, index, insertedRelatedObject, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      return CreateInsertCommand (addedRelatedObject, _dataKeeper.CollectionData.Count);
    }

    public IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      var replacedObject = _dataKeeper.CollectionData.GetObject(index);
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameCommand (_collectionEndPoint, replacedObject, _dataKeeper.CollectionData);
      else
        return new CollectionEndPointReplaceCommand (_collectionEndPoint, replacedObject, index, replacementObject, _dataKeeper.CollectionData);
    }

    public void SetValueFrom (ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      _dataKeeper.CollectionData.ReplaceContents (sourceEndPoint.Collection.Cast<DomainObject> ());

      if (sourceEndPoint.HasBeenTouched || _collectionEndPoint.HasChanged)
        _collectionEndPoint.Touch ();
    }

    public void CheckMandatory ()
    {
      if (_dataKeeper.CollectionData.Count == 0)
      {
        var objectReference = _collectionEndPoint.GetDomainObjectReference ();
        var message = string.Format (
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            _collectionEndPoint.Definition.PropertyName,
            objectReference.ID);
        throw new MandatoryRelationNotSetException (objectReference, _collectionEndPoint.Definition.PropertyName, message);
      }
    }
  }
}