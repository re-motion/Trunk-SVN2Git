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
namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class ObjectEndPointSetOneManyModification : ObjectEndPointSetModificationBase
  {
    public ObjectEndPointSetOneManyModification (ObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject)
        : base(modifiedEndPoint, newRelatedObject)
    {
    }

    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      var newRelatedEndPoint = (CollectionEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      var oldRelatedEndPoint = (CollectionEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (OldRelatedObject, newRelatedEndPoint.Definition);

      var bidirectionalModification = new NotifyingBidirectionalRelationModification (
        // => order.Customer = newCustomer
          this,
        // => newCustomer.Orders.Add (order)
          newRelatedEndPoint.CreateAddModification (ModifiedEndPoint.GetDomainObject ()),
        // => oldCustomer.Orders.Remove (order) (remove)
          oldRelatedEndPoint.CreateRemoveModification (ModifiedEndPoint.GetDomainObject ()));
      return bidirectionalModification;
    }
  }
}