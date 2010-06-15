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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents a modification performed on a <see cref="RelationEndPoint"/>. Provides default behavior for triggering the required
  /// events and notifying the <see cref="ClientTransaction"/> about the modification. The actual modification has to be specified by subclasses
  /// by implementing <see cref="Perform"/>. In addition, <see cref="ExpandToAllRelatedObjects"/> has to be overridden to return a 
  /// composite object containing all commands needed to be performed when this modification starts a bidirectional relation change.
  /// </summary>
  public abstract class RelationEndPointModificationCommand : IDataManagementCommand
  {
    private readonly IEndPoint _modifiedEndPoint;
    private readonly DomainObject _domainObject;

    private readonly DomainObject _oldRelatedObject;
    private readonly DomainObject _newRelatedObject;

    protected RelationEndPointModificationCommand (IEndPoint endPointBeingModified, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPointBeingModified", endPointBeingModified);

      _modifiedEndPoint = endPointBeingModified;
      _domainObject = endPointBeingModified.GetDomainObject ();

      _oldRelatedObject = oldRelatedObject;
      _newRelatedObject = newRelatedObject;
    }

    public IEndPoint ModifiedEndPoint
    {
      get { return _modifiedEndPoint; }
    }

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public DomainObject OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    public DomainObject NewRelatedObject
    {
      get { return _newRelatedObject; }
    }

    /// <summary>
    /// Performs this command without raising any events and without performing any bidirectional modifications.
    /// </summary>
    public abstract void Perform ();

    /// <summary>
    /// Returns a new <see cref="IDataManagementCommand"/> instance that involves changes to all objects affected by this
    /// <see cref="RelationEndPointModificationCommand"/>. If no other objects are involved by the change, this method returns just this
    /// <see cref="IDataManagementCommand"/>.
    /// </summary>
    /// <returns>A new <see cref="IDataManagementCommand"/> instance that involves changes to all objects affected by this
    /// <see cref="RelationEndPointModificationCommand"/>.</returns>
    public abstract ExpandedCommand ExpandToAllRelatedObjects ();

    public void Begin ()
    {
      _modifiedEndPoint.ClientTransaction.Execute (ScopedBegin);
    }

    public void End ()
    {
      _modifiedEndPoint.ClientTransaction.Execute (ScopedEnd);
    }

    public void NotifyClientTransactionOfBegin ()
    {
      _modifiedEndPoint.ClientTransaction.Execute (ScopedNotifyClientTransactionOfBegin);
    }

    public void NotifyClientTransactionOfEnd ()
    {
      _modifiedEndPoint.ClientTransaction.Execute (ScopedNotifyClientTransactionOfEnd);
    }

    protected virtual void ScopedBegin ()
    {
      DomainObject domainObject = _modifiedEndPoint.GetDomainObject ();
      domainObject.OnRelationChanging (new RelationChangingEventArgs (_modifiedEndPoint.Definition.PropertyName, _oldRelatedObject, _newRelatedObject));
    }

    protected virtual void ScopedEnd ()
    {
      DomainObject domainObject = _modifiedEndPoint.GetDomainObject ();
      domainObject.OnRelationChanged (new RelationChangedEventArgs (_modifiedEndPoint.Definition.PropertyName));
    }

    protected virtual void ScopedNotifyClientTransactionOfBegin ()
    {
      RaiseClientTransactionBeginNotification (_oldRelatedObject, _newRelatedObject);
    }

    protected virtual void ScopedNotifyClientTransactionOfEnd ()
    {
      RaiseClientTransactionEndNotification (_oldRelatedObject, _newRelatedObject);
    }

    protected void RaiseClientTransactionBeginNotification (DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      var eventSink = _modifiedEndPoint.ClientTransaction.TransactionEventSink;
      eventSink.RelationChanging (
          _modifiedEndPoint.ClientTransaction, _domainObject, _modifiedEndPoint.Definition, oldRelatedObject, newRelatedObject);
    }

    protected void RaiseClientTransactionEndNotification (DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      var eventSink = _modifiedEndPoint.ClientTransaction.TransactionEventSink;
      eventSink.RelationChanged (
          _modifiedEndPoint.ClientTransaction, 
          _domainObject, 
          _modifiedEndPoint.Definition.PropertyName);
    }
  }
}
