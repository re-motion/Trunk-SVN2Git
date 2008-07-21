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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver
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
