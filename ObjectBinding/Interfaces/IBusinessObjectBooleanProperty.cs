/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding
{
  /// <summary> The <b>IBusinessObjectBooleanProperty</b> interface is used for accessing <see cref="bool"/> values. </summary>
  public interface IBusinessObjectBooleanProperty: IBusinessObjectProperty
  {
    /// <summary> Returns the human readable value of the boolean property. </summary>
    /// <param name="value"> The <see cref="bool"/> value to be formatted. </param>
    /// <returns> The human readable string value of the boolean property. </returns>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    string GetDisplayName (bool value);

    /// <summary> Returns the default value to be assumed if the boolean property returns <see langword="null"/>. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> for which to get the property's default value. </param>
    /// <remarks> 
    ///   If <see langword="null"/> is returned, the object model does not define a default value. In case the 
    ///   caller requires a default value, the selection of the appropriate value is left to the caller.
    /// </remarks>
    bool? GetDefaultValue (IBusinessObjectClass objectClass);
  }
}
