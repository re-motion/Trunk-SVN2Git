// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.BindableObject
{
  [ConcreteImplementation (typeof (BindableObjectGlobalizationService), Lifetime = LifetimeKind.Singleton)]
  public interface IBindableObjectGlobalizationService : IBusinessObjectService
  {
    /// <summary>
    /// Gets the localized display name of an enumeration value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    string GetEnumerationValueDisplayName (Enum value);

    /// <summary>
    /// Gets the localized display name of the extensible enumeration value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    string GetExtensibleEnumerationValueDisplayName (IExtensibleEnum value) //move to member info globalization service
        ;

    /// <summary>
    /// Gets the localized display name of a boolean value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    string GetBooleanValueDisplayName (bool value);

    /// <summary>
    /// Gets the localized display name of a property.
    /// </summary>
    /// <param name="propertyInformation"></param>
    /// <param name="typeInformationForResourceResolution"></param>
    /// <returns>The localized display name.</returns>
    string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformationForResourceResolution);
  }
}