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
  public class CollectionEndPointRemoveModification : RelationEndPointModification
  {
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointRemoveModification (CollectionEndPoint modifiedEndPoint, DomainObject removedObject, IDomainObjectCollectionData collectionData)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("removedObject", removedObject),
            null)
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
      base.Begin ();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Remove (OldRelatedObject.ID);
      ModifiedEndPoint.Touch ();
    }

    public override void End ()
    {
      ModifiedCollection.EndRemove (OldRelatedObject);
      base.End ();
    }

    /// <summary>
    /// Creates all modifications needed to perform a bidirectional remove operation from this collection end point.
    /// </summary>
    /// <remarks>
    /// A remove operation of the form "customer.Orders.Remove (RemovedOrder)" needs two steps:
    /// <list type="bullet">
    ///   <item>RemovedOrder.Customer = null and</item>
    ///   <item>customer.Orders.Remove (removedOrder).</item>
    /// </list>
    /// </remarks>
    public override BidirectionalRelationModification CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      var removedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (OldRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      return new BidirectionalRelationModification (removedEndPoint.CreateRemoveModification (ModifiedEndPoint.GetDomainObject ()), this);
    }
  }
}