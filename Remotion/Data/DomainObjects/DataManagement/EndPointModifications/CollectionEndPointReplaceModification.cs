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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class CollectionEndPointReplaceModification : RelationEndPointModification
  {
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointReplaceModification (
        CollectionEndPoint modifiedEndPoint, DomainObject replacedObject, DomainObject replacementObject, IDomainObjectCollectionData collectionData)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("replacedObject", replacedObject),
            ArgumentUtility.CheckNotNull ("replacementObject", replacementObject))
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModification is needed.", "modifiedEndPoint");

      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.OppositeDomainObjects;
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public override void Begin ()
    {
      ModifiedCollection.BeginRemove (OldRelatedObject);
      ModifiedCollection.BeginAdd (NewRelatedObject);
      base.Begin();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Replace (OldRelatedObject.ID, NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      ModifiedCollection.EndRemove (OldRelatedObject);
      ModifiedCollection.EndAdd (NewRelatedObject);
      base.End();
    }

    /// <summary>
    /// Creates all modifications needed to perform a bidirectional replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A replace operation of the form "customer.Orders[index] = newOrder" needs four steps:
    /// <list type="bullet">
    ///   <item>customer.Order[index].Customer = null,</item>
    ///   <item>newOrder.Customer = customer,</item>
    ///   <item>customer.Orders[index] = newOrder,</item>
    ///   <item>oldCustomer.Orders.Remove (insertedOrder) - with oldCustomer being the old customer of the new order (if non-null).</item>
    /// </list>
    /// </remarks>
    public override BidirectionalEndPointsModification CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;

      // the end point that will be linked to the collection end point after the operation
      var endPointOfNewObject =
          (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      // the end point that was linked to the collection end point before the operation
      var endPointOfOldObject =
          (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (OldRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfNewObject = relationEndPointMap.GetRelatedObject (endPointOfNewObject.ID, false);
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfNewObject =
          (CollectionEndPoint)
          relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObjectOfNewObject, endPointOfNewObject.OppositeEndPointDefinition);

      return new BidirectionalEndPointsModification (
          // customer.Order[index].Customer = null
          endPointOfOldObject.CreateRemoveModification (ModifiedEndPoint.GetDomainObject()),
          // newOrder.Customer = customer
          endPointOfNewObject.CreateSetModification (oldRelatedObjectOfNewObject, ModifiedEndPoint.GetDomainObject()),
          // customer.Orders[index] = newOrder
          this,
          // oldCustomer.Orders.Remove (insertedOrder)
          oldRelatedEndPointOfNewObject.CreateRemoveModification (NewRelatedObject));
    }
  }
}