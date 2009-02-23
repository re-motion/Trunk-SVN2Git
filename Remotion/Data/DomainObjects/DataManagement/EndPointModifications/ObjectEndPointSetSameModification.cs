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
  public class ObjectEndPointSetSameModification : ObjectEndPointSetModificationBase
  {
    public ObjectEndPointSetSameModification (ObjectEndPoint modifiedEndPoint)
        : base (modifiedEndPoint, modifiedEndPoint.GetOppositeObject (true))
    {
    }

    public override void Begin ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void End ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    /// <summary>
    /// Creates all modification steps needed to perform a bidirectional set-same operation on this <see cref="ObjectEndPoint"/>. One of the steps is 
    /// this modification, the other steps are the opposite modifications on the new/old related objects.
    /// </summary>
    /// <remarks>
    /// A same-set operation of the form "order.OrderTicket = order.OrderTicket" needs two steps:
    /// <list type="bullet">
    ///   <item>order.Touch()1 and</item>
    ///   <item>order.OrderTicket.Touch.</item>
    /// </list>
    /// No change notifications are sent for this operation.
    /// </remarks>
    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var bidirectionalModification = new NonNotifyingBidirectionalRelationModification (this);
      if (!ModifiedEndPoint.OppositeEndPointDefinition.IsAnonymous)
      {
        var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
        var oppositeEndPoint = relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
        bidirectionalModification.AddModificationStep (new RelationEndPointTouchModification (oppositeEndPoint));
      }
      return bidirectionalModification;
    }
  }
}