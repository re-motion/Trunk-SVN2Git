// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class RelationEndPointModifier : ICollectionEndPointChangeDelegate
  {
    private readonly RelationEndPointMap _relationEndPointMap;

    public RelationEndPointModifier (RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);
      _relationEndPointMap = relationEndPointMap;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _relationEndPointMap.ClientTransaction; }
    }

    public void SetRelatedObject (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.Cardinality != CardinalityType.One)
      {
        throw new InvalidOperationException (string.Format ("{0} can only be called for end points with a cardinality of '{1}'.",
                                                            "SetRelatedObject", CardinalityType.One));
      }

      CheckClientTransactionForObjectEndPoint (endPointID, newRelatedObject);
      CheckDeleted (newRelatedObject);
      CheckType (endPointID, newRelatedObject);

      var endPoint = (ObjectEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
      CheckDeleted (endPoint);

      // TODO FS: 1032
      var oldRelatedObject = _relationEndPointMap.GetRelatedObject (endPointID, true);
      if (ReferenceEquals (oldRelatedObject, newRelatedObject))
      {
        var setModification = endPoint.CreateSetModification (newRelatedObject);
        var bidirectionalModification = setModification.CreateBidirectionalModification ();
        bidirectionalModification.ExecuteAllSteps ();
      }
      else
      {
        if (endPoint.OppositeEndPointDefinition.IsAnonymous)
          SetRelatedObjectForUnidirectionalRelation (endPoint, newRelatedObject);
        else
          SetRelatedObjectForBidirectionalRelation (endPoint, newRelatedObject);
      }
    }

    public void PerformInsert (CollectionEndPoint collectionEndPoint, DomainObject insertedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedObject", insertedObject);

      CheckClientTransactionForInsertionIntoCollectionEndPoint (collectionEndPoint, insertedObject, index);
      CheckDeleted (collectionEndPoint);
      CheckDeleted (insertedObject);

      var insertModification = collectionEndPoint.CreateInsertModification (insertedObject, index);
      var bidirectionalModification = insertModification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();
    }

    public void PerformReplace (CollectionEndPoint endPoint, int index, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("newRelatedObject", newRelatedObject);

      CheckClientTransactionForReplacementInCollectionEndPoint (endPoint.ID, newRelatedObject, index);
      CheckDeleted (endPoint);
      CheckDeleted (newRelatedObject);

      var replaceModification = endPoint.CreateReplaceModification (index, newRelatedObject);
      var bidirectionalModification = replaceModification.CreateBidirectionalModification ();

      bidirectionalModification.ExecuteAllSteps ();
    }

    public void PerformSelfReplace (CollectionEndPoint endPoint, DomainObject selfReplacedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("selfReplacedObject", selfReplacedObject);

      // Simulate self-replace by setting opposite object to its current value.
      // TODO 1032: Consider creating a CollectionEndPointSelfReplaceModification instead
      var oppositeEndPointID = new RelationEndPointID (selfReplacedObject.ID, endPoint.OppositeEndPointDefinition);
      SetRelatedObject (oppositeEndPointID, endPoint.GetDomainObject());
    }

    public void PerformRemove (CollectionEndPoint endPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      CheckClientTransactionForRemovalFromCollectionEndPoint (endPoint.ID, removedRelatedObject);
      CheckDeleted (endPoint);
      CheckDeleted (removedRelatedObject);

      var removeModification = endPoint.CreateRemoveModification (removedRelatedObject);
      var bidirectionalModification = removeModification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();
    }

    public NotifyingBidirectionalRelationModification GetOppositeEndPointModificationsForDelete (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointCollection allAffectedRelationEndPoints = _relationEndPointMap.GetAllRelationEndPointsWithLazyLoad (domainObject);
      RelationEndPointCollection allOppositeRelationEndPoints = allAffectedRelationEndPoints.GetOppositeRelationEndPoints (domainObject);

      var modifications = new NotifyingBidirectionalRelationModification ();
      foreach (RelationEndPoint oppositeEndPoint in allOppositeRelationEndPoints)
        modifications.AddModificationStep (oppositeEndPoint.CreateRemoveModification (domainObject));

      return modifications;
    }

    private void CheckClientTransactionForObjectEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      if (newRelatedObject != null && !newRelatedObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        var endPointObject = ClientTransaction.GetObject (endPointID.ObjectID);
        string additionalInfo = GetAdditionalInfoForMismatchingClientTransactions ("to be set into the property", endPointObject,
                                                                                   "owning the property", newRelatedObject);

        throw CreateClientTransactionsDifferException (
            "Property '{0}' of DomainObject '{1}' cannot be set to DomainObject '{2}'."
            + " The objects do not belong to the same ClientTransaction.{3}",
            endPointID.PropertyName,
            endPointID.ObjectID,
            newRelatedObject.ID,
            additionalInfo);
      }
    }

    private void CheckClientTransactionForInsertionIntoCollectionEndPoint (CollectionEndPoint endPoint, DomainObject newRelatedObject, int index)
    {
      if (newRelatedObject != null && !newRelatedObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        var endPointObject = endPoint.GetDomainObject ();
        string additionalInfo = GetAdditionalInfoForMismatchingClientTransactions (
            "to be inserted", endPointObject, "owning the collection", newRelatedObject);

        throw CreateClientTransactionsDifferException (
            "Cannot insert DomainObject '{0}' at position {1} into collection of property '{2}' of DomainObject '{3}'."
            + " The objects do not belong to the same ClientTransaction.{4}",
            newRelatedObject.ID,
            index,
            endPoint.PropertyName,
            endPoint.ObjectID,
            additionalInfo);
      }
    }

    private void CheckClientTransactionForRemovalFromCollectionEndPoint (RelationEndPointID endPointID, DomainObject relatedObject)
    {
      if (relatedObject != null && !relatedObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        throw CreateClientTransactionsDifferException (
            "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}',"
            + " because the objects do not belong to the same ClientTransaction.",
            relatedObject.ID,
            endPointID.PropertyName,
            endPointID.ObjectID);
      }
    }

    private void CheckClientTransactionForReplacementInCollectionEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject, int index)
    {
      if (newRelatedObject != null && !newRelatedObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        throw CreateClientTransactionsDifferException (
            "Cannot replace DomainObject at position {0} with DomainObject '{1}'"
            + " in collection of property '{2}' of DomainObject '{3}',"
            + " because the objects do not belong to the same ClientTransaction.",
            index,
            newRelatedObject.ID,
            endPointID.PropertyName,
            endPointID.ObjectID);
      }
    }

    private string GetAdditionalInfoForMismatchingClientTransactions (
        string endPointObjectRole, DomainObject endPointObject, string newRelatedObjectRole, DomainObject newRelatedObject)
    {
      string additionalInfo = "";

      if (newRelatedObject.BindingTransaction != null)
      {
        additionalInfo += string.Format (
            " The {0} object {1} is bound to a BindingClientTransaction.", newRelatedObject.GetPublicDomainObjectType ().Name, endPointObjectRole);
        if (endPointObject.BindingTransaction != null)
        {
          additionalInfo += string.Format (
              " The {0} object {1} is also bound, but to a different BindingClientTransaction.",
              endPointObject.GetPublicDomainObjectType ().Name, newRelatedObjectRole);
        }
      }
      else if (endPointObject.BindingTransaction != null)
      {
        additionalInfo += string.Format (
            " The {0} object {1} is bound to a BindingClientTransaction.", endPointObject.GetPublicDomainObjectType ().Name, newRelatedObjectRole);
      }
      return additionalInfo;
    }

    private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
    {
      return new ClientTransactionsDifferException (string.Format (message, args));
    }

    private void CheckDeleted (RelationEndPoint endPoint)
    {
      CheckDeleted (endPoint.GetDomainObject ());
    }

    private void CheckDeleted (DomainObject domainObject)
    {
      if (domainObject != null && domainObject.TransactionContext[ClientTransaction].State == StateType.Deleted)
        throw new ObjectDeletedException (domainObject.ID);
    }

    private void CheckType (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      if (newRelatedObject == null)
        return;

      if (!endPointID.OppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (newRelatedObject.ID.ClassDefinition))
      {
        var message = string.Format ("DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}', because it is not compatible "
                                     + "with the type of the property.", 
                                     newRelatedObject.ID,
                                     endPointID.PropertyName,
                                     endPointID.ObjectID);
        throw new DataManagementException (message);
      }
    }


    private void SetRelatedObjectForUnidirectionalRelation (ObjectEndPoint unidirectionalObjectEndPoint, DomainObject newRelatedObject)
    {
      DomainObject oldRelatedObject = _relationEndPointMap.GetRelatedObject (unidirectionalObjectEndPoint.ID, true);

      Assertion.IsFalse (ReferenceEquals (newRelatedObject, oldRelatedObject));
      RelationEndPointModification modification = 
          unidirectionalObjectEndPoint.CreateSetModification (newRelatedObject);
      modification.ExecuteAllSteps ();
    }

    private void SetRelatedObjectForBidirectionalRelation (ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      DomainObject oldRelatedObject = _relationEndPointMap.GetRelatedObject (endPoint.ID, false);

      RelationEndPoint newRelatedEndPoint = _relationEndPointMap.GetRelationEndPointWithLazyLoad (newRelatedObject, endPoint.OppositeEndPointDefinition);
      RelationEndPoint oldRelatedEndPoint = _relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObject, newRelatedEndPoint.Definition);

      Assertion.IsFalse (ReferenceEquals (newRelatedObject, oldRelatedObject));

      if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
        SetRelatedObjectForOneToOneRelation (endPoint, oldRelatedObject, newRelatedObject);
      else
        SetRelatedObjectForOneToManyRelation (endPoint, newRelatedEndPoint, oldRelatedEndPoint);
    }

    // TODO refactor: Unify SetRelatedObject*-methods to one single method => add *RelationChange-methods to IEndPoint
    private void SetRelatedObjectForOneToOneRelation (
        ObjectEndPoint endPoint,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      var newRelatedEndPoint = (ObjectEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (newRelatedObject, endPoint.OppositeEndPointDefinition);
      var oldRelatedEndPoint = (ObjectEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObject, newRelatedEndPoint.Definition);

      var oldRelatedObjectOfNewRelatedObject = newRelatedObject == null ? null : _relationEndPointMap.GetRelatedObject (newRelatedEndPoint.ID, true);
      var oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObjectOfNewRelatedObject, endPoint.Definition);

      Assertion.IsTrue (newRelatedObject != null || oldRelatedEndPointOfNewRelatedEndPoint.IsNull, 
          "a null newRelatedObject will cause oldRelatedEndPointOfNewRelatedEndPoint to be a null end point");

      var modifications = new NotifyingBidirectionalRelationModification (
          endPoint.CreateSetModification (newRelatedObject),
          oldRelatedEndPoint.CreateRemoveModification (endPoint.GetDomainObject ()),
          newRelatedEndPoint.CreateSetModification (endPoint.GetDomainObject ()),
          oldRelatedEndPointOfNewRelatedEndPoint.CreateRemoveModification (newRelatedObject));

      modifications.ExecuteAllSteps ();
    }

    private void SetRelatedObjectForOneToManyRelation (
        ObjectEndPoint endPoint,
        RelationEndPoint newRelatedEndPoint,
        RelationEndPoint oldRelatedEndPoint)
    {
      // TODO 1032: create modifications instead
      if (!newRelatedEndPoint.IsNull)
      {
        DomainObjectCollection collection = _relationEndPointMap.GetRelatedObjects (newRelatedEndPoint.ID);
        collection.Add (endPoint.GetDomainObject ());
      }
      else
      {
        DomainObjectCollection collection = _relationEndPointMap.GetRelatedObjects (oldRelatedEndPoint.ID);
        collection.Remove (endPoint.GetDomainObject ());
      }
    }
  }
}