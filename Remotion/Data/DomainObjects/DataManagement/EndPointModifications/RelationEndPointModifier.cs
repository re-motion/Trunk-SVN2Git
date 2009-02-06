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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
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

      RelationEndPoint endPoint = _relationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
      CheckDeleted (endPoint);

      if (endPoint.OppositeEndPointDefinition.IsNull)
        SetRelatedObjectForUnidirectionalRelation (endPoint, newRelatedObject);
      else
        SetRelatedObjectForBidirectionalRelation (endPoint, newRelatedObject);
    }

    public void PerformInsert (CollectionEndPoint endPoint, DomainObject domainObject, int index)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      CheckClientTransactionForInsertionIntoCollectionEndPoint (endPoint, domainObject, index);
      CheckDeleted (endPoint);
      CheckDeleted (domainObject);

      var addingEndPoint = (ObjectEndPoint) GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);
      var oldRelatedOfAddingEndPoint =
          (CollectionEndPoint) GetRelationEndPoint (_relationEndPointMap.GetRelatedObject (addingEndPoint.ID, false), endPoint.Definition);

      RelationEndPoint oldRelatedNullEndPoint = RelationEndPoint.CreateNullRelationEndPoint (addingEndPoint.Definition);

      var modifications = new RelationEndPointModificationCollection (
          addingEndPoint.CreateModification (oldRelatedOfAddingEndPoint, endPoint),
          endPoint.CreateInsertModification (oldRelatedNullEndPoint, addingEndPoint, index),
          oldRelatedOfAddingEndPoint.CreateDeleteModification (addingEndPoint));

      modifications.ExecuteAllSteps ();
    }

    public void PerformReplace (CollectionEndPoint endPoint, DomainObject domainObject, int index)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      CheckClientTransactionForReplacementInCollectionEndPoint (endPoint.ID, domainObject, index);
      CheckDeleted (endPoint);
      CheckDeleted (domainObject);

      var newEndPoint = (ObjectEndPoint) GetRelationEndPoint (
                                             domainObject, endPoint.OppositeEndPointDefinition);

      var oldEndPoint = (ObjectEndPoint) GetRelationEndPoint (
                                             endPoint.OppositeDomainObjects[index], endPoint.OppositeEndPointDefinition);

      var oldEndPointOfNewEndPoint = (CollectionEndPoint) GetRelationEndPoint (
                                                              _relationEndPointMap.GetRelatedObject (newEndPoint.ID, false),
                                                              newEndPoint.OppositeEndPointDefinition);

      var modifications = new RelationEndPointModificationCollection (
          oldEndPoint.CreateDeleteModification (endPoint),
          newEndPoint.CreateModification (oldEndPointOfNewEndPoint, endPoint),
          endPoint.CreateReplaceModification (oldEndPoint, newEndPoint),
          oldEndPointOfNewEndPoint.CreateDeleteModification (newEndPoint));

      modifications.ExecuteAllSteps ();
    }

    public void PerformSelfReplace (CollectionEndPoint endPoint, DomainObject domainObject, int index)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      SetRelatedObjectForEqualObjects (endPoint, GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition));
    }

    public void PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      CheckClientTransactionForRemovalFromCollectionEndPoint (endPoint.ID, domainObject);
      CheckDeleted (endPoint);
      CheckDeleted (domainObject);

      var removedEndPoint = (ObjectEndPoint) GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

      var modifications = new RelationEndPointModificationCollection (
          removedEndPoint.CreateDeleteModification (endPoint),
          endPoint.CreateDeleteModification (removedEndPoint));

      modifications.ExecuteAllSteps ();
    }

    // TODO: Check this after 997
    public RelationEndPointModificationCollection GetOppositeEndPointModificationsForDelete (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RelationEndPointCollection allAffectedRelationEndPoints = _relationEndPointMap.GetAllRelationEndPointsWithLazyLoad (domainObject);
      RelationEndPointCollection allOppositeRelationEndPoints = allAffectedRelationEndPoints.GetOppositeRelationEndPoints (domainObject);

      var modifications = new RelationEndPointModificationCollection ();
      foreach (RelationEndPoint oppositeEndPoint in allOppositeRelationEndPoints)
      {
        IRelationEndPointDefinition endPointDefinition = oppositeEndPoint.OppositeEndPointDefinition;
        RelationEndPoint oldEndPoint = allAffectedRelationEndPoints[new RelationEndPointID (domainObject.ID, endPointDefinition)];

        modifications.Add (oppositeEndPoint.CreateDeleteModification (oldEndPoint));
      }
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

    private RelationEndPoint GetRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      if (domainObject != null)
        return _relationEndPointMap.GetRelationEndPointWithLazyLoad (new RelationEndPointID (domainObject.ID, definition));
      else
        return RelationEndPoint.CreateNullRelationEndPoint (definition);
    }


    private void SetRelatedObjectForUnidirectionalRelation (RelationEndPoint endPoint, DomainObject newRelatedObject)
    {
      DomainObject oldRelatedObject = _relationEndPointMap.GetRelatedObject (endPoint.ID, true);

      AnonymousEndPoint newRelatedEndPoint = GetAnonymousEndPoint (newRelatedObject, endPoint.RelationDefinition);
      AnonymousEndPoint oldRelatedEndPoint = GetAnonymousEndPoint (oldRelatedObject, endPoint.RelationDefinition);

      if (ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
        SetRelatedObjectForEqualObjects (endPoint, null);
      else
      {
        RelationEndPointModification modification = endPoint.CreateModification (oldRelatedEndPoint, newRelatedEndPoint);
        modification.ExecuteAllSteps ();
      }
    }

    private void SetRelatedObjectForEqualObjects (RelationEndPoint endPoint, RelationEndPoint oppositeEndPoint)
    {
      Assertion.IsNotNull (endPoint);

      RelationEndPoint realEndPoint = endPoint.Definition.IsVirtual ? oppositeEndPoint : endPoint;
      RelationEndPoint virtualEndPoint = endPoint.Definition.IsVirtual ? endPoint : oppositeEndPoint;

      Assertion.IsNotNull (realEndPoint);
      realEndPoint.Touch ();

      if (virtualEndPoint != null) // bidirectional?
        virtualEndPoint.Touch ();

      // touch foreign key property
      if (!realEndPoint.IsNull) // null end points have no data container, so no foreign key needs to be touched
        realEndPoint.GetDataContainer ().PropertyValues[realEndPoint.PropertyName].Touch ();
    }

    private void SetRelatedObjectForBidirectionalRelation (RelationEndPoint endPoint, DomainObject newRelatedObject)
    {
      DomainObject oldRelatedObject = _relationEndPointMap.GetRelatedObject (endPoint.ID, false);

      RelationEndPoint newRelatedEndPoint = GetRelationEndPoint (newRelatedObject, endPoint.OppositeEndPointDefinition);
      RelationEndPoint oldRelatedEndPoint = GetRelationEndPoint (oldRelatedObject, newRelatedEndPoint.Definition);

      if (ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
        SetRelatedObjectForEqualObjects (endPoint, newRelatedEndPoint);
      else if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
        SetRelatedObjectForOneToOneRelation ((ObjectEndPoint) endPoint, (ObjectEndPoint) newRelatedEndPoint, (ObjectEndPoint) oldRelatedEndPoint);
      else
        SetRelatedObjectForOneToManyRelation ((ObjectEndPoint) endPoint, newRelatedEndPoint, oldRelatedEndPoint);
    }

    // TODO refactor: Unify SetRelatedObject*-methods to one single method => add *RelationChange-methods to IEndPoint
    private void SetRelatedObjectForOneToOneRelation (
        ObjectEndPoint endPoint,
        ObjectEndPoint newRelatedEndPoint,
        ObjectEndPoint oldRelatedEndPoint)
    {
      var oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint)
                                                   RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition);

      if (!newRelatedEndPoint.IsNull)
      {
        DomainObject oldRelatedObject = _relationEndPointMap.GetRelatedObject (newRelatedEndPoint.ID, false);
        oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) GetRelationEndPoint (oldRelatedObject, endPoint.Definition);
      }

      var modifications = new RelationEndPointModificationCollection (
          endPoint.CreateModification (oldRelatedEndPoint, newRelatedEndPoint),
          oldRelatedEndPoint.CreateDeleteModification (endPoint),
          newRelatedEndPoint.CreateModification (oldRelatedEndPointOfNewRelatedEndPoint, endPoint),
          oldRelatedEndPointOfNewRelatedEndPoint.CreateDeleteModification (newRelatedEndPoint));

      modifications.ExecuteAllSteps ();
    }

    private void SetRelatedObjectForOneToManyRelation (
        ObjectEndPoint endPoint,
        RelationEndPoint newRelatedEndPoint,
        RelationEndPoint oldRelatedEndPoint)
    {
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

    private AnonymousEndPoint GetAnonymousEndPoint (DomainObject domainObject, RelationDefinition relationDefinition)
    {
      if (domainObject != null)
        return new AnonymousEndPoint (ClientTransaction, domainObject, relationDefinition);
      else
        return new NullAnonymousEndPoint (relationDefinition);
    }
  }
}