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
    public RelationEndPointTouchModification (RelationEndPoint endPointBeingModified)
        : base (endPointBeingModified, null, null)
    {
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
      throw new NotSupportedException ("Touch modifications cannot be the starting point for a bidirectional modification.");
    }
  }
}