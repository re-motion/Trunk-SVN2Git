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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public abstract class RelationEndPointModification
  {
    public readonly RelationEndPoint AffectedEndPoint;
    public readonly IEndPoint OldEndPoint;
    public readonly IEndPoint NewEndPoint;

    public RelationEndPointModification (RelationEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      ArgumentUtility.CheckNotNull ("affectedEndPoint", affectedEndPoint);
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

      AffectedEndPoint = affectedEndPoint;
      OldEndPoint = oldEndPoint;
      NewEndPoint = newEndPoint;
    }

    public abstract void Perform ();

    public virtual void Begin ()
    {
      AffectedEndPoint.GetDomainObject().BeginRelationChange (
          AffectedEndPoint.PropertyName, OldEndPoint.GetDomainObject(), NewEndPoint.GetDomainObject());
    }

    public virtual void End ()
    {
      DomainObject domainObject = AffectedEndPoint.GetDomainObject ();
      domainObject.EndRelationChange (AffectedEndPoint.PropertyName);
    }

    public virtual void NotifyClientTransactionOfBegin ()
    {
      AffectedEndPoint.NotifyClientTransactionOfBeginRelationChange (OldEndPoint, NewEndPoint);
    }

    public virtual void NotifyClientTransactionOfEnd ()
    {
      AffectedEndPoint.NotifyClientTransactionOfEndRelationChange ();
    }

    public void ExecuteAllSteps ()
    {
      NotifyClientTransactionOfBegin();
      Begin ();
      Perform();
      NotifyClientTransactionOfEnd ();
      End();
    }
  }
}
