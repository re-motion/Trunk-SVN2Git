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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class RelationEndPointModifier
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
      CheckNewRelatedObjectType (endPointID, newRelatedObject);

      var endPoint = (ObjectEndPoint) _relationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
      CheckDeleted (endPoint.GetDomainObject ());

      var setModification = endPoint.CreateSetModification (newRelatedObject);
      var bidirectionalModification = setModification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();
    }

    private void CheckDeleted (DomainObject domainObject)
    {
      if (domainObject != null && domainObject.TransactionContext[ClientTransaction].State == StateType.Deleted)
        throw new ObjectDeletedException (domainObject.ID);
    }

    private void CheckNewRelatedObjectType (RelationEndPointID endPointID, DomainObject newRelatedObject)
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

    private void CheckClientTransactionForObjectEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject)
    {
      if (newRelatedObject != null && !newRelatedObject.TransactionContext[ClientTransaction].CanBeUsedInTransaction)
      {
        var endPointObject = ClientTransaction.GetObject (endPointID.ObjectID);
        string additionalInfo = GetAdditionalInfoForMismatchingClientTransactions ("owning the property", endPointObject,
                                                                                   "to be set into the property", newRelatedObject);

        throw CreateClientTransactionsDifferException (
            "Property '{0}' of DomainObject '{1}' cannot be set to DomainObject '{2}'."
            + " The objects do not belong to the same ClientTransaction.{3}",
            endPointID.PropertyName,
            endPointID.ObjectID,
            newRelatedObject.ID,
            additionalInfo);
      }
    }

    private string GetAdditionalInfoForMismatchingClientTransactions (
        string endPointObjectRole, DomainObject endPointObject, string newRelatedObjectRole, DomainObject newRelatedObject)
    {
      string additionalInfo = "";

      if (newRelatedObject.HasBindingTransaction)
      {
        additionalInfo += string.Format (
            " The {0} object {1} is bound to a BindingClientTransaction.", newRelatedObject.GetPublicDomainObjectType ().Name, newRelatedObjectRole);
        if (endPointObject.HasBindingTransaction)
        {
          additionalInfo += string.Format (
              " The {0} object {1} is also bound, but to a different BindingClientTransaction.",
              endPointObject.GetPublicDomainObjectType ().Name, endPointObjectRole);
        }
      }
      else if (endPointObject.HasBindingTransaction)
      {
        additionalInfo += string.Format (
            " The {0} object {1} is bound to a BindingClientTransaction.", endPointObject.GetPublicDomainObjectType ().Name, endPointObjectRole);
      }
      return additionalInfo;
    }

    private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
    {
      return new ClientTransactionsDifferException (string.Format (message, args));
    }
  }
}
