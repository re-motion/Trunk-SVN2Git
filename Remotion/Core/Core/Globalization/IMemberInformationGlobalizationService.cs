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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines an interface for retrieving the human-readable localized representation of the reflection object.
  /// </summary>
  [ConcreteImplementation (typeof (MemberInformationGlobalizationService), Lifetime = LifetimeKind.Singleton)]
  public interface IMemberInformationGlobalizationService
  {
    /// <summary>
    /// Returns the human-readable property name of the spefified reflection object.
    /// </summary>
    /// <param name="propertyInformation">The <see cref="IPropertyInformation"/> that defines the property name for the resource lookup.</param>
    /// <param name="typeInformationForResourceResolution">The <see cref="ITypeInformation"/> that should be used for the resource resolution.</param>
    /// <returns>The human-readable localized representation of the property.</returns>
    string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformationForResourceResolution);

    /// <summary>
    /// Returns the human-readable type name of the specified reflection object.
    /// </summary>
    /// <param name="typeInformation">The <see cref="ITypeInformation"/> that defines the type name for the resource lookup.</param>
    /// <param name="typeInformationForResourceResolution">The <see cref="ITypeInformation"/> that should be used for the resource resolution.</param>
    /// <returns>The human-readable localized representation of the type.</returns>
    string GetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution);
  }
}