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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Provides a base implementation of <see cref="IBusinessObjectWithIdentity"/> associated with the reflection-based <see cref="BindableObjectProvider"/>.
  /// </summary>
  /// <remarks>
  /// If derivation from this class is not possible, the reflection-based implementation can also be added via the 
  /// <see cref="BindableObjectWithIdentityAttribute"/>, which adds the <see cref="BindableObjectWithIdentityMixin"/> to the class.
  /// </remarks>
  [BindableObjectWithIdentityProvider]
  [BindableObjectBaseClass]
  [Serializable]
  public abstract class BindableObjectWithIdentityBase : IBusinessObjectWithIdentity
  {
    private readonly IBusinessObjectWithIdentity _implementation;

    protected BindableObjectWithIdentityBase()
    {
      _implementation = BindableObjectWithIdentityBaseImplementation.Create (this);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    protected BindableObjectWithIdentityBase (IBusinessObjectWithIdentity businessObjectImplementation)
    {
      ArgumentUtility.CheckNotNull ("businessObjectImplementation", businessObjectImplementation);
      _implementation = businessObjectImplementation;
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
      get { return _implementation.DisplayName; }
    }

    public string DisplayNameSafe
    {
      get { return _implementation.DisplayNameSafe; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _implementation.BusinessObjectClass; }
    }

    public abstract string UniqueIdentifier { get; }
  }
}