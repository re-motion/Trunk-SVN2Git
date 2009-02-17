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
  // TODO: check for usages 
  public class NullEndPointModification : RelationEndPointModification
  {
    public NullEndPointModification (RelationEndPoint affectedEndPoint, DomainObject oldRelatedObject, DomainObject newRelatedObject)
        : base (affectedEndPoint, oldRelatedObject, newRelatedObject)
    {
    }

    public override void Begin ()
    {
      // do nothing
    }

    public override void Perform ()
    {
      // do nothing
    }

    public override void End ()
    {
      // do nothing
    }

    public override void NotifyClientTransactionOfBegin ()
    {
      // do nothing
    }

    public override void NotifyClientTransactionOfEnd ()
    {
      // do nothing
    }

    public override BidirectionalEndPointsModification CreateBidirectionalModification ()
    {
#warning TODO FS: Throw sensible exception.
      throw new System.NotImplementedException ();
    }
  }
}