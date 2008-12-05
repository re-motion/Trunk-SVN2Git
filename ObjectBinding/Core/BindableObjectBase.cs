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
