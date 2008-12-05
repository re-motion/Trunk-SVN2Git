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
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  [Serializable]
  public class PropertyChangeState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    private PropertyValue _propertyValue;
    private object _oldValue;
    private object _newValue;

    // construction and disposing

    public PropertyChangeState (
        object sender,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
      : this (sender, propertyValue, oldValue, newValue, null)
    {
    }

    public PropertyChangeState (
        object sender,
        PropertyValue propertyValue,
        object oldValue,
        object newValue,
        string message)
      : base (sender, message)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      _propertyValue = propertyValue;
      _oldValue = oldValue;
      _newValue = newValue;
    }

    // methods and properties

    public PropertyValue PropertyValue
    {
      get { return _propertyValue; }
    }

    public object OldValue
    {
      get { return _oldValue; }
    }

    public object NewValue
    {
      get { return _newValue; }
    }

    public override void Check (ChangeState expectedState)
    {
      base.Check (expectedState);

      PropertyChangeState propertyChangeState = (PropertyChangeState) expectedState;

      if (_propertyValue.Name != propertyChangeState.PropertyValue.Name)
      {
        throw CreateApplicationException (
            "Actual PropertyName '{0}' and expected PropertyName '{1}' do not match.",
            _propertyValue.Name,
            propertyChangeState.PropertyValue.Name);
      }

      if (!Equals (_oldValue, propertyChangeState.OldValue))
      {
        throw CreateApplicationException (
            "Actual old value '{0}' and expected old value '{1}' do not match.",
            _oldValue,
            propertyChangeState.OldValue);
      }

      if (!Equals (_newValue, propertyChangeState.NewValue))
      {
        throw CreateApplicationException (
            "Actual new value '{0}' and expected new value '{1}' do not match.",
            _newValue,
            propertyChangeState.NewValue);
      }
    }
  }
}
