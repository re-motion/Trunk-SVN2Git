// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class ObjectEndPointSetModification : RelationEndPointModification
  {
    private readonly ObjectEndPoint _modifiedEndPoint;

    public ObjectEndPointSetModification (ObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject)
      : base (modifiedEndPoint, modifiedEndPoint.GetOppositeObject(true), newRelatedObject)
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModification is needed.", "modifiedEndPoint");

      _modifiedEndPoint = modifiedEndPoint;
    }

    public override void Perform ()
    {
      var id = NewRelatedObject == null ? null : NewRelatedObject.ID;
      _modifiedEndPoint.OppositeObjectID = id;
    }

    public override BidirectionalRelationModification CreateBidirectionalModification ()
    {
      // order.Customer = newCustomer (1:n) 
      // => oldCustomer.Orders.Remove (order) (remove)
      // => newCustomer.Orders.Add (order)
      // => order.Customer = newCustomer

      // order.OrderTicket = newTicket (1:1) => SetRelatedObjectForOneToOneRelation
      // => order.OrderTicket = newTicket
      // => oldTicket.Order = null (remove)
      // => newTicket.Order = order
      // => oldOrderOfNewTicket.OrderTicket = null (remove)

      // person.Address = newAddress (1:1 uni) => SetRelatedObjectForUnidirectionalRelation
      // => person.Address = newAddress

      // for equal objects: don't forget to touch the foreign key property; should probably be done in ObjectEndPoint.SetOppositeID...

      throw new NotImplementedException ();
    }
  }
}