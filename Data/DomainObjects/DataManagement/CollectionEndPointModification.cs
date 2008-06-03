/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPointModification : RelationEndPointModification
  {
    public readonly CollectionEndPointChangeAgent ChangeAgent;

    private readonly CollectionEndPoint _affectedEndPoint;

    public CollectionEndPointModification (CollectionEndPoint affectedEndPoint, CollectionEndPointChangeAgent changeAgent)
      : base (affectedEndPoint, changeAgent.OldEndPoint, changeAgent.NewEndPoint)
    {
      _affectedEndPoint = affectedEndPoint;
      ChangeAgent = changeAgent;
    }

    public override void Begin ()
    {
      ChangeAgent.BeginRelationChange();
      base.Begin();
    }

    public override void Perform ()
    {
      _affectedEndPoint.PerformRelationChange (this);
    }

    public override void End ()
    {
      ChangeAgent.EndRelationChange();
      base.End();
    }
  }
}
