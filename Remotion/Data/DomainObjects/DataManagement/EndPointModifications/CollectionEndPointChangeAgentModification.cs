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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  // TODO: Remove this class.
  public class CollectionEndPointChangeAgentModification : RelationEndPointModification
  {
    private readonly CollectionEndPointChangeAgent _changeAgent;
    private readonly CollectionEndPoint _affectedEndPoint;

    public CollectionEndPointChangeAgentModification (CollectionEndPoint affectedEndPoint, CollectionEndPointChangeAgent changeAgent)
        : base (affectedEndPoint, changeAgent.OldEndPoint.GetDomainObject (), changeAgent.NewEndPoint.GetDomainObject ())
    {
      _affectedEndPoint = affectedEndPoint;
      _changeAgent = changeAgent;
    }

    public CollectionEndPointChangeAgent ChangeAgent
    {
      get { return _changeAgent; }
    }

    public override void Begin ()
    {
      _changeAgent.BeginRelationChange();
      base.Begin();
    }

    public override void Perform ()
    {
      _affectedEndPoint.PerformRelationChange (this);
    }

    public override void End ()
    {
      _changeAgent.EndRelationChange();
      base.End();
    }
  }
}