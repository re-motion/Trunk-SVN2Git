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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class CollectionEndPointReplaceWholeCollectionModification : RelationEndPointModification
  {
    private readonly CollectionEndPoint _endPointBeingModified;
    private readonly DomainObjectCollection _newOppositeCollection;

    public CollectionEndPointReplaceWholeCollectionModification (
        CollectionEndPoint endPointBeingModified, 
        DomainObjectCollection newOppositeCollection)
      : base (ArgumentUtility.CheckNotNull ("endPointBeingModified", endPointBeingModified), null, null)
    {
      ArgumentUtility.CheckNotNull ("newOppositeCollection", newOppositeCollection);

      _endPointBeingModified = endPointBeingModified;
      _newOppositeCollection = newOppositeCollection;
    }

    public override void Perform ()
    {
      var oldOpposites = _endPointBeingModified.OppositeDomainObjects;
      _newOppositeCollection.AssociateWithEndPoint (_endPointBeingModified);

      oldOpposites.ChangeDelegate = null;
      _newOppositeCollection.ChangeDelegate = _endPointBeingModified;
    }

    /// <summary>
    /// Creates all modifications needed to perform a bidirectional collection replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A replace operation of the form "customer.Orders = newOrders" involves the following steps:
    /// <list type="bullet">
    ///   <item>for each oldOrder the old collection (Orders) that's not in the new one: oldOrder.Customer = <see langword="null" />,</item>
    ///   <item>for each newOrder in the new collection (newOrders) that's not in the old one: newOrder.Customer.Orders.Remove (newOrder),</item>
    ///   <item>for each newOrder in the new collection (newOrders) that's not in the old one: newOrder.Customer = customer,</item>
    ///   <item>customer.Orders = newOrders.</item>
    /// </list>
    /// </remarks>
    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      var domainObjectOfCollectionEndPoint = ModifiedEndPoint.GetDomainObject ();
      
      var modificationsOfOldNotInNew = from oldObject in _endPointBeingModified.OppositeDomainObjects.Cast<DomainObject> ()
                                       where !_newOppositeCollection.ContainsObject (oldObject)
                                       let endPoint = GetOppositeEndPoint (oldObject)
                                       select endPoint.CreateRemoveModification (domainObjectOfCollectionEndPoint); // oldOrder.Customer = null
      
      var modificationsOfNewNotInOld = from newObject in _newOppositeCollection.Cast<DomainObject> ()
                                       where !_endPointBeingModified.OppositeDomainObjects.ContainsObject (newObject)
                                       let endPointOfNewObject = GetOppositeEndPoint (newObject) // newOrder.Customer
                                       let oldRelatedOfNewObject = relationEndPointMap.GetRelatedObject (endPointOfNewObject.ID, false) // newOrder.Customer
                                       let endPointOfOldRelatedOfNewObject = GetSameEndPoint (oldRelatedOfNewObject) // newOrder.Customer.Orders
                                       let removeModification = endPointOfOldRelatedOfNewObject.CreateRemoveModification (newObject) // newOrder.Customer.Orders.Remove (newOrder)
                                       let setModification = endPointOfNewObject.CreateSetModification (domainObjectOfCollectionEndPoint) // newOrder.Customer = customer
                                       select Tuple.NewTuple (setModification, removeModification);

      var modifications = 
          new IRelationEndPointModification[] { this } // customer.Orders = newOrders
          .Concat (modificationsOfOldNotInNew)
          .Concat (Unzip (modificationsOfNewNotInOld))
          .ToArray ();
      
      return new NotifyingBidirectionalRelationModification (modifications);
    }

    private IEnumerable<IRelationEndPointModification> Unzip (IEnumerable<Tuple<IRelationEndPointModification, IRelationEndPointModification>> tuples)
    {
      foreach (var tuple in tuples)
      {
        yield return tuple.A;
        yield return tuple.B;
      }
    }

    private ObjectEndPoint GetOppositeEndPoint (DomainObject domainObject)
    {
      return (ObjectEndPoint) GetEndPoint (domainObject, ModifiedEndPoint.OppositeEndPointDefinition);
    }

    private CollectionEndPoint GetSameEndPoint (DomainObject domainObject)
    {
      return (CollectionEndPoint) GetEndPoint (domainObject, ModifiedEndPoint.Definition);
    }

    private RelationEndPoint GetEndPoint (DomainObject domainObject, IRelationEndPointDefinition endPointDefinition)
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      return relationEndPointMap.GetRelationEndPointWithLazyLoad (domainObject, endPointDefinition);
    }
  }
}