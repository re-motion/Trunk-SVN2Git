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
    private readonly RelationEndPoint _affectedEndPoint;
    private readonly IEndPoint _oldEndPoint;
    private readonly IEndPoint _newEndPoint;

    public RelationEndPointModification (RelationEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      ArgumentUtility.CheckNotNull ("affectedEndPoint", affectedEndPoint);
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

      _affectedEndPoint = affectedEndPoint;
      _oldEndPoint = oldEndPoint;
      _newEndPoint = newEndPoint;
    }

    public RelationEndPoint AffectedEndPoint
    {
      get { return _affectedEndPoint; }
    }

    public IEndPoint OldEndPoint
    {
      get { return _oldEndPoint; }
    }

    public IEndPoint NewEndPoint
    {
      get { return _newEndPoint; }
    }

    public abstract void Perform ();

    public virtual void Begin ()
    {
      _affectedEndPoint.GetDomainObject().BeginRelationChange (
          _affectedEndPoint.PropertyName, _oldEndPoint.GetDomainObject(), _newEndPoint.GetDomainObject());
    }

    public virtual void End ()
    {
      DomainObject domainObject = _affectedEndPoint.GetDomainObject ();
      domainObject.EndRelationChange (_affectedEndPoint.PropertyName);
    }

    public virtual void NotifyClientTransactionOfBegin ()
    {
      _affectedEndPoint.NotifyClientTransactionOfBeginRelationChange (_oldEndPoint, _newEndPoint);
    }

    public virtual void NotifyClientTransactionOfEnd ()
    {
      _affectedEndPoint.NotifyClientTransactionOfEndRelationChange ();
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
