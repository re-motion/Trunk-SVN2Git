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
  //TODO: Type property for concrete numeric type
  /// <summary>The <see cref="IBusinessObjectNumericProperty"/> interface provides additional meta data for numeric values.</summary>
  /// <remarks>
  /// <note type="inotes">
  /// The objects returned for this property must implement the <see cref="IFormattable"/> interface in order to be displayed by the 
  /// <see cref="IBusinessObject.GetPropertyString(IBusinessObjectProperty,string)"/> methods.
  /// </note>
  /// </remarks>
  public interface IBusinessObjectNumericProperty : IBusinessObjectProperty
  {
    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    bool AllowNegative { get; }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    Type Type { get; }
  }
}
