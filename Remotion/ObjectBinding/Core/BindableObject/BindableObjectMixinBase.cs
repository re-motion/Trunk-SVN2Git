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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Provides a base class for mixins introducing <see cref="IBusinessObject"/> to arbitrary target classes.
  /// </summary>
  /// <typeparam name="TBindableObject">The bindable object type to be used as the target class of this mixin.</typeparam>
  /// <remarks>
  /// The default mixin derived from this class is <see cref="BindableObjectMixin"/>, but a custom implementation exists for OPF's domain objects.
  /// </remarks>
  [Serializable]
  public abstract class BindableObjectMixinBase<TBindableObject> : Mixin<TBindableObject>, IBusinessObject
      where TBindableObject: class
  {
    [NonSerialized]
    private MixinConfiguration _mixinConfigurationAtInstantiationTime;
    [NonSerialized]
    private BindableObjectProvider _bindableObjectProvider;
    [NonSerialized]
    private DoubleCheckedLockingContainer<BindableObjectClass> _bindableObjectClass;

    protected abstract Type GetTypeForBindableObjectClass ();

    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
    /// <returns> The property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="Exception">
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public object GetProperty (IBusinessObjectProperty property)
    {
      var propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase> ("property", property);
      
      if (propertyBase.ValueGetter == null)
        throw new InvalidOperationException (string.Format ("Property has no getter."));

      object nativeValue = propertyBase.ValueGetter (This);
      
      if (!propertyBase.IsList && propertyBase.IsDefaultValue(((IBusinessObject) This)))
        return null;
      else
        return propertyBase.ConvertFromNativePropertyType (nativeValue);
    }
    
    /// <overloads> Sets the value accessed through the specified property. </overloads>
    /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="value"> The new value for the <paramref name="property"/> parameter. </param>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public void SetProperty (IBusinessObjectProperty property, object value)
    {
      var propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase> ("property", property);

      if (propertyBase.ValueSetter == null)
        throw new InvalidOperationException (string.Format ("Property has no setter."));

      object nativeValue = propertyBase.ConvertToNativePropertyType (value);

      propertyBase.ValueSetter (This, nativeValue);
    }

    /// <summary> 
    ///   Gets the formatted string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="format"> The format string applied by the <b>ToString</b> method. </param>
    /// <returns> The string representation of the property value for the <paramref name="property"/> parameter.  </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public string GetPropertyString (IBusinessObjectProperty property, string format)
    {
      var stringFormatterService =
          (IBusinessObjectStringFormatterService)
          BusinessObjectClass.BusinessObjectProvider.GetService (typeof (IBusinessObjectStringFormatterService));
      return stringFormatterService.GetPropertyString ((IBusinessObject) This, property, format);
    }

    /// <summary> Gets the <see cref="BindableObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="BindableObjectClass"/> instance acting as the business object's type. </value>
    public BindableObjectClass BusinessObjectClass
    {
      get { return _bindableObjectClass.Value; }
    }

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="IBusinessObjectClass"/> instance acting as the business object's type. </value>
    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return BusinessObjectClass; }
    }

    /// <summary> Gets the human readable representation of this <see cref="IBusinessObject"/>. </summary>
    /// <value> The default implementation returns the <see cref="BusinessObjectClass"/>'s <see cref="IBusinessObjectClass.Identifier"/>. </value>
    public virtual string DisplayName
    {
      get { return BusinessObjectClass.Identifier; }
    }

    /// <summary>
    ///   Gets the value of <see cref="DisplayName"/> if it is accessible and otherwise falls back to the <see cref="string"/> returned by
    ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
    /// </summary>
    public string DisplayNameSafe
    {
      get
      {
        if (!BusinessObjectClass.HasPropertyDefinition ("DisplayName"))
          return DisplayName;

        IBusinessObjectProperty displayNameProperty = BusinessObjectClass.GetPropertyDefinition ("DisplayName");
        if (displayNameProperty.IsAccessible (BusinessObjectClass, (IBusinessObject) This))
          return DisplayName;

        return BusinessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();
      }
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized();

      var typeForBindableObjectClass = GetTypeForBindableObjectClass ();
      _mixinConfigurationAtInstantiationTime = MixinConfiguration.ActiveConfiguration;
      _bindableObjectProvider = BindableObjectProvider.GetProviderForBindableObjectType (typeForBindableObjectClass);
      _bindableObjectClass = new DoubleCheckedLockingContainer<BindableObjectClass> (InitializeBindableObjectClass);
    }

    protected override void OnDeserialized ()
    {
      base.OnDeserialized();

      var typeForBindableObjectClass = GetTypeForBindableObjectClass ();
      _mixinConfigurationAtInstantiationTime = MixinConfiguration.ActiveConfiguration;
      _bindableObjectProvider = BindableObjectProvider.GetProviderForBindableObjectType (typeForBindableObjectClass);
      _bindableObjectClass = new DoubleCheckedLockingContainer<BindableObjectClass> (InitializeBindableObjectClass);
    }

    private BindableObjectClass InitializeBindableObjectClass ()
    {
      // reactivate the mixin configuration to get the bindable object class originally expected
      using (_mixinConfigurationAtInstantiationTime.EnterScope ())
      {
        var targetType = GetTypeForBindableObjectClass();
        return _bindableObjectProvider.GetBindableObjectClass (targetType);
      }
    }
  }
}