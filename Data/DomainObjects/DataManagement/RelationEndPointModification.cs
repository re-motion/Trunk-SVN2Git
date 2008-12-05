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
using Remotion.Data.DomainObjects.Infrastructure;
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
      DomainObject domainObject = _affectedEndPoint.GetDomainObject();
      domainObject.EventManager.BeginRelationChange (_affectedEndPoint.PropertyName, _oldEndPoint.GetDomainObject(), _newEndPoint.GetDomainObject());
    }

    public virtual void End ()
    {
      DomainObject domainObject = _affectedEndPoint.GetDomainObject ();
      domainObject.EventManager.EndRelationChange (_affectedEndPoint.PropertyName);
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
