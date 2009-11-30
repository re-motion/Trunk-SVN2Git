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

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Represents a modification that touches, but does not change the modified end point.
  /// </summary>
  public class RelationEndPointTouchModification : RelationEndPointModification
  {
    public RelationEndPointTouchModification (RelationEndPoint endPointBeingModified)
        : base (endPointBeingModified, null, null)
    {
    }

    public override void Begin ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void Perform ()
    {
      ModifiedEndPoint.Touch ();
    }

    public override void End ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override CompositeRelationModification CreateRelationModification ()
    {
      throw new NotSupportedException ("Cannot create a bidirectional operation from a touch modification.");
    }
  }
}