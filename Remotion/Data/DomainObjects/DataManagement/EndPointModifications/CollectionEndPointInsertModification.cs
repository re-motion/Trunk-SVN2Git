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
  public class CollectionEndPointInsertModification : RelationEndPointModification
  {
    private readonly int _index;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointInsertModification (
        CollectionEndPoint modifiedEndPoint, DomainObject insertedObject, int index, IDomainObjectCollectionData collectionData)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            null,
            ArgumentUtility.CheckNotNull ("insertedObject", insertedObject))
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModification is needed.", "modifiedEndPoint");

      _index = index;
      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.OppositeDomainObjects;
    }

    public int Index
    {
      get { return _index; }
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
      ModifiedCollection.BeginAdd (NewRelatedObject);
      base.Begin();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Insert (Index, NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      ModifiedCollection.EndAdd (NewRelatedObject);
      base.End();
    }

    /// <summary>
    /// Creates all modifications needed to perform a bidirectional insert operation into this collection end point.
    /// </summary>
    /// <remarks>
    /// An insert operation of the form "customer.Orders.Insert (insertedOrder, index)" needs three steps:
    /// <list type="bullet">
    ///   <item>insertedOrder.Customer = customer,</item>
    ///   <item>customer.Orders.Insert (insertedOrder, index), and</item>
    ///   <item>oldCustomer.Orders.Remove (insertedOrder) - with oldCustomer being the old customer of the inserted order (if non-null).</item>
    /// </list>
    /// </remarks>
    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;

      // the end point that will be linked to the collection end point after the operation
      var insertedObjectEndPoint =
          (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfInsertedObject = relationEndPointMap.GetRelatedObject (insertedObjectEndPoint.ID, false);
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfInsertedObject =
          (CollectionEndPoint)
          relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObjectOfInsertedObject, insertedObjectEndPoint.OppositeEndPointDefinition);

      return new NotifyingBidirectionalRelationModification (
          // insertedOrder.Customer = customer (previously oldCustomer)
          insertedObjectEndPoint.CreateSetModification (ModifiedEndPoint.GetDomainObject()),
          // customer.Orders.Insert (insertedOrder, index)
          this,
          // oldCustomer.Orders.Remove (insertedOrder)
          oldRelatedEndPointOfInsertedObject.CreateRemoveModification (NewRelatedObject));
    }
  }
}