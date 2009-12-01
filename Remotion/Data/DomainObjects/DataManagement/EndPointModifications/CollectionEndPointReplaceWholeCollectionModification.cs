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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents the replacement of the whole <see cref="CollectionEndPoint.OppositeDomainObjects"/> collection, including the transformation
  /// of the involved <see cref="DomainObjectCollection"/> instances into stand-alone resp. associated collections.
  /// </summary>
  public class CollectionEndPointReplaceWholeCollectionModification : RelationEndPointModification
  {
    public CollectionEndPointReplaceWholeCollectionModification (
        CollectionEndPoint modifiedEndPoint, 
        DomainObjectCollection newOppositeCollection,
        IDomainObjectCollectionTransformer oldOppositeCollectionTransformer,
        IDomainObjectCollectionTransformer newOppositeCollectionTransformer,
        IDomainObjectCollectionData modifiedEndPointDataStore)
      : base (ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint), null, null)
    {
      ArgumentUtility.CheckNotNull ("newOppositeCollection", newOppositeCollection);
      ArgumentUtility.CheckNotNull ("oldOppositeCollectionTransformer", oldOppositeCollectionTransformer);
      ArgumentUtility.CheckNotNull ("newOppositeCollectionTransformer", newOppositeCollectionTransformer);
      ArgumentUtility.CheckNotNull ("modifiedEndPointDataStore", modifiedEndPointDataStore);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModification is needed.", "modifiedEndPoint");

      ModifiedEndPoint = modifiedEndPoint;
      NewOppositeCollection = newOppositeCollection;
      OldOppositeCollectionTransformer = oldOppositeCollectionTransformer;
      NewOppositeCollectionTransformer = newOppositeCollectionTransformer;
      ModifiedEndPointDataStore = modifiedEndPointDataStore;
    }

    public new CollectionEndPoint ModifiedEndPoint { get; private set; }
    public IDomainObjectCollectionData ModifiedEndPointDataStore { get; private set; }

    public DomainObjectCollection NewOppositeCollection { get; private set; }

    public IDomainObjectCollectionTransformer OldOppositeCollectionTransformer { get; private set; }
    public IDomainObjectCollectionTransformer NewOppositeCollectionTransformer { get; private set; }
    

    public override void Perform ()
    {
      // only transform the old collection to stand-alone if it is still associated with this end point
      // rationale: during rollback, the old relation might have already been associated with another end-point, we must not overwrite this!
      if (OldOppositeCollectionTransformer.Collection.AssociatedEndPoint == ModifiedEndPoint)
        OldOppositeCollectionTransformer.TransformToStandAlone();

      // copy over the data
      ModifiedEndPointDataStore.ReplaceContents (NewOppositeCollection.Cast<DomainObject> ());

      // we must always associate the new collection with the end point, however - even during rollback phase
      NewOppositeCollectionTransformer.TransformToAssociated (ModifiedEndPoint);

      // now make end point refer to the new collection by reference, too
      ModifiedEndPoint.OppositeDomainObjects = NewOppositeCollection; // this also touches the end point
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
    public override CompositeRelationModification CreateRelationModification ()
    {
      var relationEndPointMap = base.ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      var domainObjectOfCollectionEndPoint = base.ModifiedEndPoint.GetDomainObject ();
      
      var modificationsOfOldNotInNew = from oldObject in ModifiedEndPoint.OppositeDomainObjects.Cast<DomainObject> ()
                                       where !NewOppositeCollection.ContainsObject (oldObject)
                                       let endPoint = GetOppositeEndPoint (oldObject)
                                       select endPoint.CreateRemoveModification (domainObjectOfCollectionEndPoint); // oldOrder.Customer = null
      
      var modificationsOfNewNotInOld = from newObject in NewOppositeCollection.Cast<DomainObject> ()
                                       where !ModifiedEndPoint.OppositeDomainObjects.ContainsObject (newObject)
                                       let endPointOfNewObject = GetOppositeEndPoint (newObject) // newOrder.Customer
                                       let oldRelatedOfNewObject = relationEndPointMap.GetRelatedObject (endPointOfNewObject.ID, false) // newOrder.Customer
                                       let endPointOfOldRelatedOfNewObject = GetEquivalentEndPoint (oldRelatedOfNewObject) // newOrder.Customer.Orders
                                       let removeModification = endPointOfOldRelatedOfNewObject.CreateRemoveModification (newObject) // newOrder.Customer.Orders.Remove (newOrder)
                                       let setModification = endPointOfNewObject.CreateSetModification (domainObjectOfCollectionEndPoint) // newOrder.Customer = customer
                                       select Tuple.NewTuple (removeModification, setModification);

      var allModificationSteps =
          modificationsOfOldNotInNew
          .Concat (Unzip (modificationsOfNewNotInOld))
          .Concat (new IRelationEndPointModification[] { this }); // customer.Orders = newOrders
      return new CompositeRelationModificationWithEvents (allModificationSteps);
    }

    private IEnumerable<IRelationEndPointModification> Unzip (IEnumerable<Tuple<IRelationEndPointModification, IRelationEndPointModification>> tuples)
    {
      foreach (var tuple in tuples)
      {
        yield return tuple.A;
        yield return tuple.B;
      }
    }
  }
}