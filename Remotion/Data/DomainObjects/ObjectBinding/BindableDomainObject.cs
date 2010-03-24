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
using System.ComponentModel;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="BindableDomainObject"/> class.
    /// </summary>
    /// <remarks>
    /// 	<para>
    /// Any constructors implemented on concrete domain objects should delegate to this base constructor. As domain objects generally should
    /// not be constructed via the
    /// <c>new</c> operator, these constructors must remain protected, and the concrete domain objects should have a static "NewObject" method,
    /// which delegates to <see cref="NewObject"/>, passing it the required constructor arguments.
    /// </para>
    /// 	<para>
    /// It is safe to access virtual properties that are automatically implemented by the framework from constructors because this base constructor
    /// prepares everything necessary for them to work.
    /// </para>
    /// </remarks>
    protected BindableDomainObject ()
    {
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

    protected override void OnReferenceInitialized ()
    {
      base.OnReferenceInitialized ();

      if (_implementation == null) // may have been set by ctor
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
