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

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents a modification on a <see cref="RelationEndPoint"/> that does not actually modify the end point, but only touch it.
  /// </summary>
  public class RelationEndPointTouchModification : RelationEndPointModification
  {
    private readonly RelationEndPoint[] _oppositeEndPoints;

    public RelationEndPointTouchModification (RelationEndPoint endPointBeingModified, params RelationEndPoint[] oppositeEndPoints)
        : base (endPointBeingModified, null, null)
    {
      _oppositeEndPoints = oppositeEndPoints;
    }

    public RelationEndPoint[] OppositeEndPoints
    {
      get { return _oppositeEndPoints; }
    }

    public override void Begin ()
    {
      // do not issue any change notifications, a touch is not a change
    }

    public override void Perform ()
    {
      ModifiedEndPoint.Touch ();
    }

    public override void End ()
    {
      // do not issue any change notifications, a touch is not a change
    }

    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var bidirectionalModification = new NonNotifyingBidirectionalRelationModification (this);

      foreach (var oppositeEndPoint in _oppositeEndPoints)
        bidirectionalModification.AddModificationStep (new RelationEndPointTouchModification (oppositeEndPoint));
      
      return bidirectionalModification;
      
      //if (oppositeEndPointDefinition.IsAnonymous)
      //{
      //  return ;
      //}
      //else
      //{
      //  var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      //  var oppositeEndPoint = relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, oppositeEndPointDefinition);
      //  return new BidirectionalRelationModification (this, new RelationEndPointTouchModification (oppositeEndPoint));
      //}
    }
  }
}