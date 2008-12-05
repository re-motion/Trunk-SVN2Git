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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  [Serializable]
  public class PropertyValueContainerEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private bool _cancel;
    [NonSerialized]
    private PropertyValue _changingPropertyValue;
    [NonSerialized]
    private PropertyValue _changedPropertyValue;
    private object _changingOldValue;
    private object _changingNewValue;
    private object _changedOldValue;
    private object _changedNewValue;

    // construction and disposing

    public PropertyValueContainerEventReceiver (PropertyValueCollection propertyValueCollection, bool cancel)
    {
      _cancel = cancel;

      propertyValueCollection.PropertyChanging += new PropertyChangeEventHandler (
          PropertyValueContainer_PropertyChanging);

      propertyValueCollection.PropertyChanged += new PropertyChangeEventHandler (
          PropertyValueContainer_PropertyChanged);
    }

    public PropertyValueContainerEventReceiver (DataContainer dataContainer, bool cancel)
    {
      _cancel = cancel;

      dataContainer.PropertyChanging += new PropertyChangeEventHandler (PropertyValueContainer_PropertyChanging);
      dataContainer.PropertyChanged += new PropertyChangeEventHandler (PropertyValueContainer_PropertyChanged);
    }

    // methods and properties

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

    private void PropertyValueContainer_PropertyChanging (object sender, PropertyChangeEventArgs args)
    {
      _changingPropertyValue = args.PropertyValue;
      _changingOldValue = args.OldValue;
      _changingNewValue = args.NewValue;

      if (_cancel)
        CancelOperation ();
    }

    private void PropertyValueContainer_PropertyChanged (object sender, PropertyChangeEventArgs args)
    {
      _changedPropertyValue = args.PropertyValue;
      _changedOldValue = args.OldValue;
      _changedNewValue = args.NewValue;
    }
  }
}
