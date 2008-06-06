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
      where TBindableObject : class
  {
    [NonSerialized]
    private BindableObjectClass _bindableObjectClass;

    public BindableObjectMixinBase ()
    {
    }

    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
    /// <returns> The property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="Exception">
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public object GetProperty (IBusinessObjectProperty property)
    {
      PropertyBase propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase> ("property", property);

      object nativeValue;

      //TODO: catch and unwrap the TargetException
      nativeValue = propertyBase.PropertyInfo.GetValue (This, new object[0]);
      
      if (!propertyBase.IsList && IsDefaultValue (propertyBase, nativeValue))
        return null;
      else
        return propertyBase.ConvertFromNativePropertyType (nativeValue);
    }

    /// <summary>
    /// Determines whether the given <paramref name="property"/>, whose current value is <paramref name="nativeValue"/>, still has its default value.
    /// </summary>
    /// <param name="property">The property to be checked.</param>
    /// <param name="nativeValue">The property's current value in its native form.</param>
    /// <returns>
    /// True if the <paramref name="property"/> still has its default value; otherwise, false.
    /// </returns>
    /// <remarks>The default implementation always returns false.</remarks>
    protected virtual bool IsDefaultValue (PropertyBase property, object nativeValue)
    {
      return false;
    }

    /// <summary>
    ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
    ///   <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <returns> The property value for the <paramref name="propertyIdentifier"/> parameter. </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/>
    ///   is not part of this business object's class. 
    /// </exception>
    public object GetProperty (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);
      return GetProperty (_bindableObjectClass.GetPropertyDefinition (propertyIdentifier));
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
      PropertyBase propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase> ("property", property);

      object nativeValue = propertyBase.ConvertToNativePropertyType (value);

      //TODO: catch and unwrap the TargetException
      propertyBase.PropertyInfo.SetValue (This, nativeValue, new object[0]);
    }

    /// <summary>
    ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
    ///   <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <param name="value"> 
    ///   The new value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </param>
    /// <exception cref="Exception"> 
    ///   Thrown if the <see cref="IBusinessObjectProperty"/> identified by the <paramref name="propertyIdentifier"/>
    ///   is not part of this business object's class. 
    /// </exception>
    public void SetProperty (string propertyIdentifier, object value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);
      SetProperty (_bindableObjectClass.GetPropertyDefinition (propertyIdentifier), value);
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
      IBusinessObjectStringFormatterService stringFormatterService =
          (IBusinessObjectStringFormatterService)
          BusinessObjectClass.BusinessObjectProvider.GetService (typeof (IBusinessObjectStringFormatterService));
      return stringFormatterService.GetPropertyString ((IBusinessObject) This, property, format);
    }

    /// <summary> 
    ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
    ///   identified by the passed <paramref name="propertyIdentifier"/>.
    /// </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <returns> 
    ///   The string representation of the property value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="propertyIdentifier"/> is not part of this business object's class. 
    /// </exception>
    public string GetPropertyString (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);
      return GetPropertyString (_bindableObjectClass.GetPropertyDefinition (propertyIdentifier), null);
    }

    /// <summary> Gets the <see cref="BindableObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="BindableObjectClass"/> instance acting as the business object's type. </value>
    public BindableObjectClass BusinessObjectClass
    {
      get { return _bindableObjectClass; }
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
      get { return _bindableObjectClass.Identifier; }
    }

    /// <summary>
    ///   Gets the value of <see cref="DisplayName"/> if it is accessible and otherwise falls back to the <see cref="string"/> returned by
    ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
    /// </summary>
    public string DisplayNameSafe
    {
      get
      {
        if (!_bindableObjectClass.HasPropertyDefinition ("DisplayName"))
          return DisplayName;

        IBusinessObjectProperty displayNameProperty = _bindableObjectClass.GetPropertyDefinition ("DisplayName");
        if (displayNameProperty.IsAccessible (_bindableObjectClass, (IBusinessObject) This))
          return DisplayName;

        return _bindableObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ();
      }
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      _bindableObjectClass = InitializeBindableObjectClass ();
    }

    protected override void OnDeserialized ()
    {
      base.OnDeserialized ();

      _bindableObjectClass = InitializeBindableObjectClass ();
    }

    protected abstract BindableObjectClass InitializeBindableObjectClass ();
  }
}
