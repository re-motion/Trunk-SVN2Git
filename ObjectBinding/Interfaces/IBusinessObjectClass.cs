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
  ///   The <b>IBusinessObjectClassWithIdentity</b> interface provides functionality for defining the <b>Class</b> of an 
  ///   <see cref="IBusinessObject"/>. 
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <b>IBusinessObjectClass</b> interface provides the list of <see cref="IBusinessObjectProperty"/> instances
  ///     available by an <see cref="IBusinessObject"/> of this <b>Class</b>'s type. 
  ///   </para><para>
  ///     It also provides services for accessing class specific meta data.
  ///   </para>
  /// </remarks>
  public interface IBusinessObjectClass
  {
    /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this
    ///   business object class.
    /// </param>
    /// <returns> Returns the <see cref="IBusinessObjectProperty"/> or <see langword="null"/>. </returns>
    /// <remarks> 
    ///   It is not specified wheter an exception is thrown or <see langword="null"/> is returned if the 
    ///   <see cref="IBusinessObjectProperty"/> could not be found.
    /// </remarks>
    IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier);

    /// <summary> 
    ///   Returns the <see cref="IBusinessObjectProperty"/> instances defined for this business object class.
    /// </summary>
    /// <returns> An array of <see cref="IBusinessObjectProperty"/> instances. Must not be <see langword="null"/>. </returns>
    IBusinessObjectProperty[] GetPropertyDefinitions ();

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this business object class. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type.
    ///   <note type="inotes">
    ///     Must not return <see langword="null"/>.
    ///   </note>
    /// </value>
    IBusinessObjectProvider BusinessObjectProvider { get; }

    /// <summary>
    ///   Gets a flag that specifies whether a referenced object of this business object class needs to be 
    ///   written back to its container if some of its values have changed.
    /// </summary>
    /// <value> <see langword="true"/> if the <see cref="IBusinessObject"/> must be reassigned to its container. </value>
    /// <example>
    ///   The following pseudo code shows how this value affects the binding behaviour.
    ///   <code><![CDATA[
    ///   Address address = person.Address;
    ///   address.City = "Vienna";
    ///   // the RequiresWriteBack property of the 'Address' business object class specifies 
    ///   // whether the following statement is required:
    ///   person.Address = address;
    ///   ]]></code>
    /// </example>
    bool RequiresWriteBack { get; }

    /// <summary> Gets the identifier (i.e. the type name) for this business object class. </summary>
    /// <value> 
    ///   A string that uniquely identifies the business object class within the business object model. 
    /// </value>
    string Identifier { get; }
  }
}
