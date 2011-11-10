// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// This class contains extension methods for the <see cref="IBusinessObject"/> interface.
  /// </summary>
  public static class BusinessObjectExtensions
  {
    /// <summary>
    ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property is declared. Must not be <see langword="null" />.</param>
    /// <param name="propertyIdentifier">
    /// A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// Must not be <see langword="null" /> or empty.
    /// </param>
    /// <returns> The property value for the <paramref name="propertyIdentifier"/> parameter. </returns>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/> is not part of this 
    ///   <paramref name="businessObject"/>'s <see cref="IBusinessObject.BusinessObjectClass"/>.
    /// </exception>
    public static object GetProperty (this IBusinessObject businessObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      var propertyDefinition = GetPropertyDefinition (businessObject, propertyIdentifier);

      return businessObject.GetProperty (propertyDefinition);
    }

    /// <summary>
    ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property is declared. Must not be <see langword="null" />.</param>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <param name="value"> 
    ///   The new value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/> is not part of this 
    ///   <paramref name="businessObject"/>'s <see cref="IBusinessObject.BusinessObjectClass"/>.
    /// </exception>
    public static void SetProperty (this IBusinessObject businessObject, string propertyIdentifier, object value)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      var propertyDefinition = GetPropertyDefinition (businessObject, propertyIdentifier);

      businessObject.SetProperty (propertyDefinition, value);
    }

    /// <summary> 
    ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
    ///   identified by the passed <paramref name="propertyIdentifier"/>.
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property is declared. Must not be <see langword="null" />.</param>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <returns> 
    ///   The string representation of the property value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </returns>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/> is not part of this 
    ///   <paramref name="businessObject"/>'s <see cref="IBusinessObject.BusinessObjectClass"/>.
    /// </exception>
    public static string GetPropertyString (this IBusinessObject businessObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      var propertyDefinition = GetPropertyDefinition (businessObject, propertyIdentifier);

      return businessObject.GetPropertyString (propertyDefinition, null);
    }

    private static IBusinessObjectProperty GetPropertyDefinition (IBusinessObject businessObject, string propertyIdentifier)
    {
      var businessObjectClass = businessObject.BusinessObjectClass;
      Assertion.IsNotNull (businessObjectClass, "The business object's BusinessObjectClass-property evaluated and returned null.");

      var propertyDefinition = businessObjectClass.GetPropertyDefinition (propertyIdentifier);
      if (propertyDefinition == null)
      {
        throw new InvalidOperationException (
            string.Format ("The business object's class ('{0}') does not contain a property named '{1}'.", businessObjectClass.Identifier, propertyIdentifier));
      }

      return propertyDefinition;
    }
  }
}
