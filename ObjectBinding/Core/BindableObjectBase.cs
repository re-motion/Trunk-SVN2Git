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
using System.ComponentModel;
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Provides a base implementation of <see cref="IBusinessObject"/> associated with the reflection-based <see cref="BindableObjectProvider"/>.
  /// </summary>
  /// <remarks>
  /// If derivation from this class is not possible, the reflection-based implementation can also be added via the 
  /// <see cref="BindableObjectAttribute"/>, which adds the <see cref="BindableObjectMixin"/> to the class.
  /// </remarks>
  [BindableObjectProvider]
  [BindableObjectBaseClass]
  [Serializable]
  public abstract class BindableObjectBase : IBusinessObject
  {
    private readonly IBindableObjectBaseImplementation _implementation;

    protected BindableObjectBase()
    {
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectMixin), typeof (Mixin<,>)), 
                             "we assume the mixin does not have a base object");
      _implementation = BindableObjectBaseImplementation.Create (this);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    protected BindableObjectBase (IBindableObjectBaseImplementation implementation)
    {
      ArgumentUtility.CheckNotNull ("implementation", implementation);
      _implementation = implementation;
    }

    public object GetProperty(IBusinessObjectProperty property)
    {
      return _implementation.GetProperty(property);
    }

    public void SetProperty(IBusinessObjectProperty property, object value)
    {
      _implementation.SetProperty(property, value);
    }

    public string GetPropertyString(IBusinessObjectProperty property, string format)
    {
      return _implementation.GetPropertyString(property, format);
    }

    public virtual string DisplayName
    {
      get { return _implementation.BaseDisplayName; }
    }

    public string DisplayNameSafe
    {
      get { return _implementation.DisplayNameSafe; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _implementation.BusinessObjectClass; }
    }
  }
}