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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public class DomainObjectEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private DomainObject _domainObject;
    private bool _cancel;
    private bool _hasChangingEventBeenCalled = false;
    private bool _hasChangedEventBeenCalled = false;
    private PropertyValue _changingPropertyValue;
    private PropertyValue _changedPropertyValue;
    private object _changingOldValue;
    private object _changingNewValue;
    private object _changedOldValue;
    private object _changedNewValue;
    private bool _hasDeletingEventBeenCalled = false;
    private bool _hasDeletedEventBeenCalled = false;

    private bool _hasRelationChangingEventBeenCalled = false;
    private bool _hasRelationChangedEventBeenCalled = false;
    private string _changingRelationPropertyName;
    private string _changedRelationPropertyName;
    private DomainObject _oldRelatedObject;
    private DomainObject _newRelatedObject;

    private bool _hasCommittingEventBeenCalled = false;
    private bool _hasCommittedEventBeenCalled = false;

    // construction and disposing

    public DomainObjectEventReceiver (DomainObject domainObject)
      : this (domainObject, false)
    {
    }

    public DomainObjectEventReceiver (DomainObject domainObject, bool cancel)
    {
      _domainObject = domainObject;
      _cancel = cancel;

      _domainObject.PropertyChanging += new PropertyChangeEventHandler (DomainObject_PropertyChanging);
      _domainObject.PropertyChanged += new PropertyChangeEventHandler (DomainObject_PropertyChanged);
      _domainObject.RelationChanging += new RelationChangingEventHandler (DomainObject_RelationChanging);
      _domainObject.RelationChanged += new RelationChangedEventHandler (DomainObject_RelationChanged);
      _domainObject.Deleting += new EventHandler (domainObject_Deleting);
      _domainObject.Deleted += new EventHandler (domainObject_Deleted);
      _domainObject.Committing += new EventHandler (DomainObject_Committing);
      _domainObject.Committed += new EventHandler (DomainObject_Committed);
    }

    // methods and properties

    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }

    public bool HasChangingEventBeenCalled
    {
      get { return _hasChangingEventBeenCalled; }
    }

    public bool HasChangedEventBeenCalled
    {
      get { return _hasChangedEventBeenCalled; }
    }

    public PropertyValue ChangingPropertyValue
    {
      get { return _changingPropertyValue; }
    }

    public PropertyValue ChangedPropertyValue
    {
      get { return _changedPropertyValue; }
    }

    public object ChangingOldValue
    {
      get { return _changingOldValue; }
    }

    public object ChangingNewValue
    {
      get { return _changingNewValue; }
    }

    public object ChangedOldValue
    {
      get { return _changedOldValue; }
    }

    public object ChangedNewValue
    {
      get { return _changedNewValue; }
    }

    public bool HasRelationChangingEventBeenCalled
    {
      get { return _hasRelationChangingEventBeenCalled; }
    }

    public bool HasRelationChangedEventBeenCalled
    {
      get { return _hasRelationChangedEventBeenCalled; }
    }

    public string ChangingRelationPropertyName
    {
      get { return _changingRelationPropertyName; }
    }

    public string ChangedRelationPropertyName
    {
      get { return _changedRelationPropertyName; }
    }

    public DomainObject OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    public DomainObject NewRelatedObject
    {
      get { return _newRelatedObject; }
    }

    public bool HasDeletingEventBeenCalled
    {
      get { return _hasDeletingEventBeenCalled; }
    }

    public bool HasDeletedEventBeenCalled
    {
      get { return _hasDeletedEventBeenCalled; }
    }

    public bool HasCommittingEventBeenCalled
    {
      get { return _hasCommittingEventBeenCalled; }
    }

    public bool HasCommittedEventBeenCalled
    {
      get { return _hasCommittedEventBeenCalled; }
    }

    private void DomainObject_PropertyChanging (object sender, PropertyChangeEventArgs args)
    {
      _hasChangingEventBeenCalled = true;
      _changingPropertyValue = args.PropertyValue;
      _changingOldValue = args.OldValue;
      _changingNewValue = args.NewValue;

      if (_cancel)
        CancelOperation ();
    }

    private void DomainObject_PropertyChanged (object sender, PropertyChangeEventArgs args)
    {
      _hasChangedEventBeenCalled = true;
      _changedPropertyValue = args.PropertyValue;
      _changedOldValue = args.OldValue;
      _changedNewValue = args.NewValue;
    }

    protected virtual void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      _hasRelationChangingEventBeenCalled = true;
      _changingRelationPropertyName = args.PropertyName;
      _oldRelatedObject = args.OldRelatedObject;
      _newRelatedObject = args.NewRelatedObject;

      if (_cancel)
        CancelOperation ();
    }

    protected virtual void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      _hasRelationChangedEventBeenCalled = true;
      _changedRelationPropertyName = args.PropertyName;
    }

    protected virtual void domainObject_Deleting (object sender, EventArgs args)
    {
      if (_cancel)
        CancelOperation ();

      _hasDeletingEventBeenCalled = true;
    }

    protected virtual void domainObject_Deleted (object sender, EventArgs e)
    {
      _hasDeletedEventBeenCalled = true;
    }

    private void DomainObject_Committing (object sender, EventArgs e)
    {
      if (_hasCommittingEventBeenCalled)
        throw CreateApplicationException ("Committing event on DomainObject '{0}' has already been called.", _domainObject.ID);

      if (_cancel)
        CancelOperation ();

      _hasCommittingEventBeenCalled = true;
    }

    private void DomainObject_Committed (object sender, EventArgs e)
    {
      if (_hasCommittedEventBeenCalled)
        throw CreateApplicationException ("Committed event on DomainObject '{0}' has already been called.", _domainObject.ID);

      _hasCommittedEventBeenCalled = true;
    }

    private ApplicationException CreateApplicationException (string message, params object[] args)
    {
      return new ApplicationException (string.Format (message, args));
    }
  }
}
