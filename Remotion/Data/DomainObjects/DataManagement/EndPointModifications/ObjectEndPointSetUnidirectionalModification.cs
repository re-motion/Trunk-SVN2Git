// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public class ObjectEndPointSetUnidirectionalModification : ObjectEndPointSetModificationBase
  {
    public ObjectEndPointSetUnidirectionalModification (ObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject)
        : base(modifiedEndPoint, newRelatedObject)
    {
      if (!modifiedEndPoint.OppositeEndPointDefinition.IsAnonymous)
      {
        var message = string.Format ("EndPoint '{0}' is from a bidirectional relation - use a ObjectEndPointSetOneOneModification or ObjectEndPointSetOneManyModification instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "modifiedEndPoint");
      }

      if (newRelatedObject == modifiedEndPoint.GetOppositeObject (true))
      {
        var message = string.Format ("New related object for EndPoint '{0}' is the same as its old value - use a ObjectEndPointSetSameModification instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "newRelatedObject");
      }
    }

    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      return new NotifyingBidirectionalRelationModification (this);
    }
  }
}