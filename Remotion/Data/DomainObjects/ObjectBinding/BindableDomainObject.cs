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
using System.ComponentModel;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Provides a base class for bindable <see cref="DomainObject"/> classes.
  /// </summary>
  /// <remarks>
  /// Deriving from this base class is equivalent to deriving from the <see cref="DomainObject"/> class and applying the
  /// <see cref="BindableDomainObjectAttribute"/> to the derived class.
  /// </remarks>
  [BindableDomainObjectProvider]
  [BindableObjectBaseClass]
  [Serializable]
  public abstract class BindableDomainObject : DomainObject, IBusinessObjectWithIdentity, BindableDomainObjectMixin.IDomainObject
  {
    private IBindableDomainObjectImplementation _implementation;

    protected BindableDomainObject ()
    {
      InitializeImplementation();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    protected BindableDomainObject (IBindableDomainObjectImplementation implementation)
    {
      ArgumentUtility.CheckNotNull ("implementation", implementation);
      _implementation = implementation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableDomainObject"/> class in the process of deserialization.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> coming from the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> coming from the .NET serialization infrastructure.</param>
    /// <remarks>Be sure to call this base constructor from the deserialization constructor of any concrete <see cref="BindableDomainObject"/> type
    /// implementing the <see cref="ISerializable"/> interface.</remarks>
    protected BindableDomainObject (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      if (loadMode == LoadMode.WholeDomainObjectInitialized)
        InitializeImplementation ();
      base.OnLoaded (loadMode);
    }

    private void InitializeImplementation ()
    {
      _implementation = BindableDomainObjectImplementation.Create (this);
    }

    /// <summary>
    /// Provides a possibility to override the display name of the bindable domain object.
    /// </summary>
    /// <value>The display name.</value>
    /// <remarks>Override this property to replace the default display name provided by the <see cref="BindableObjectClass"/> with a custom one.
    /// </remarks>
    [StorageClassNone]
    public virtual string DisplayName
    {
      get { return _implementation.BaseDisplayName; }
    }

    string IBusinessObjectWithIdentity.UniqueIdentifier
    {
      get { return _implementation.BaseUniqueIdentifier; }
    }

    object IBusinessObject.GetProperty (IBusinessObjectProperty property)
    {
      return _implementation.GetProperty (property);
    }

    void IBusinessObject.SetProperty (IBusinessObjectProperty property, object value)
    {
      _implementation.SetProperty (property, value);
    }

    string IBusinessObject.GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return _implementation.GetPropertyString (property, format);
    }

    string IBusinessObject.DisplayNameSafe
    {
      get { return _implementation.DisplayNameSafe; }
    }

    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return _implementation.BusinessObjectClass; }
    }

    PropertyIndexer BindableDomainObjectMixin.IDomainObject.Properties
    {
      get { return Properties; }
    }
  }
}
