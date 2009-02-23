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
  public class ObjectEndPointSetOneOneModification : ObjectEndPointSetModificationBase
  {
    public ObjectEndPointSetOneOneModification (ObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject)
        : base(modifiedEndPoint, newRelatedObject)
    {
    }

    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      var newRelatedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      var oldRelatedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (OldRelatedObject, newRelatedEndPoint.Definition);

      var oldRelatedObjectOfNewRelatedObject = NewRelatedObject == null ? null : relationEndPointMap.GetRelatedObject (newRelatedEndPoint.ID, true);
      var oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObjectOfNewRelatedObject, ModifiedEndPoint.Definition);

      var bidirectionalModification = new NotifyingBidirectionalRelationModification (
        // => order.OrderTicket = newTicket
        this,
        // => oldTicket.Order = null (remove)
        oldRelatedEndPoint.CreateRemoveModification (ModifiedEndPoint.GetDomainObject ()),
        // => newTicket.Order = order
        newRelatedEndPoint.CreateSetModification (ModifiedEndPoint.GetDomainObject ()),
        // => oldOrderOfNewTicket.OrderTicket = null (remove)
        oldRelatedEndPointOfNewRelatedEndPoint.CreateRemoveModification (NewRelatedObject));

      return bidirectionalModification;
    }
  }
}