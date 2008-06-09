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

namespace Remotion.ObjectBinding
{
  /// <summary> 
  ///   The <see cref="IBusinessObject"/> interface provides functionality to get and set the state of a business object.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     An <see cref="IBusinessObject"/> knows its <see cref="IBusinessObjectClass"/> through the 
  ///     <see cref="BusinessObjectClass"/> property.
  ///   </para><para>
  ///     Its state can be accessed through a number of get and set methods as well as indexers.
  ///   </para>
  ///   <note type="inotes">
  ///     Unless you must extend an existing class with the business object functionality, you can use the 
  ///     <see cref="T:Remotion.ObjectBinding.BusinessObject"/> class as a base implementation.
  ///   </note>
  /// </remarks>
  public interface IBusinessObject
  {
    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
    /// <returns> The property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="Exception">
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    object GetProperty (IBusinessObjectProperty property);

    // TODO: Replace with extension method in 2.0
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
    object GetProperty (string propertyIdentifier);

    /// <overloads> Sets the value accessed through the specified property. </overloads>
    /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="value"> The new value for the <paramref name="property"/> parameter. </param>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    void SetProperty (IBusinessObjectProperty property, object value);

    // TODO: Replace with extension method in 2.0
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
    void SetProperty (string propertyIdentifier, object value);

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
    string GetPropertyString (IBusinessObjectProperty property, string format);

    // TODO: Replace with extension method in 2.0
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
    string GetPropertyString (string propertyIdentifier);

    /// <summary> Gets the human readable representation of this <see cref="IBusinessObject"/>. </summary>
    /// <value> A <see cref="string"/> identifying this object to the user. </value>
    string DisplayName { get; }

    /// <summary> Gets the human readable representation of this <see cref="IBusinessObjectWithIdentity"/> in an exception-safe manner. </summary>
    /// <remarks> Accessing this property must not fail during normal operations. </remarks>
    string DisplayNameSafe { get; }

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="IBusinessObjectClass"/> instance acting as the business object's type. </value>
    IBusinessObjectClass BusinessObjectClass { get; }
  }
}
