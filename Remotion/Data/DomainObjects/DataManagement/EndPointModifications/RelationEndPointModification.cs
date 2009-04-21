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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public abstract class RelationEndPointModification
  {
    private readonly RelationEndPoint _modifiedEndPoint;
    private readonly DomainObject _oldRelatedObject;
    private readonly DomainObject _newRelatedObject;

    protected RelationEndPointModification (RelationEndPoint endPointBeingModified, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPointBeingModified", endPointBeingModified);

      _modifiedEndPoint = endPointBeingModified;
      _oldRelatedObject = oldRelatedObject;
      _newRelatedObject = newRelatedObject;
    }

    public RelationEndPoint ModifiedEndPoint
    {
      get { return _modifiedEndPoint; }
    }

    public DomainObject OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    public DomainObject NewRelatedObject
    {
      get { return _newRelatedObject; }
    }

    public abstract void Perform ();

    public virtual void Begin ()
    {
      DomainObject domainObject = _modifiedEndPoint.GetDomainObject();
      domainObject.OnRelationChanging (new RelationChangingEventArgs (_modifiedEndPoint.PropertyName, _oldRelatedObject, _newRelatedObject));
    }

    public virtual void End ()
    {
      DomainObject domainObject = _modifiedEndPoint.GetDomainObject ();
      domainObject.OnRelationChanged (new RelationChangedEventArgs (_modifiedEndPoint.PropertyName));
    }

    public virtual void NotifyClientTransactionOfBegin ()
    {
      _modifiedEndPoint.NotifyClientTransactionOfBeginRelationChange (_oldRelatedObject, _newRelatedObject);
    }

    public virtual void NotifyClientTransactionOfEnd ()
    {
      _modifiedEndPoint.NotifyClientTransactionOfEndRelationChange ();
    }

    public void ExecuteAllSteps ()
    {
      NotifyClientTransactionOfBegin();
      Begin ();
      Perform();
      NotifyClientTransactionOfEnd ();
      End();
    }

    /// <summary>
    /// Creates all modification steps needed to perform a bidirectional operation. One of the steps is this modification, the other 
    /// steps are the opposite modifications on the new/old related objects.
    /// </summary>
    public abstract BidirectionalRelationModificationBase CreateBidirectionalModification ();
  }
}