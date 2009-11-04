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

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// <see cref="ISearchAvailableObjectsService"/> defines the interface required for retrieving a list of <see cref="IBusinessObject"/> instances 
  /// that may be assigned to the specified <see cref="IBusinessObjectReferenceProperty"/>.
  /// </summary>
  /// <remarks>
  /// You can register a service-instance with the <see cref="BusinessObjectProvider"/>'s <see cref="IBusinessObjectProvider.AddService"/> method 
  /// using either the <see cref="ISearchAvailableObjectsService"/> type or a derived type as the key to identify this service. If you register a service using 
  /// a derived type, you also have to apply the <see cref="SearchAvailableObjectsServiceTypeAttribute"/> to the bindable object for which the service is intended.
  /// </remarks>
  public interface ISearchAvailableObjectsService : IBusinessObjectService
  {
    /// <summary>
    /// Gets a flag that describes whether the serivce can be used to retrieve the available objects for this property.
    /// </summary>
    /// <param name="property">The <see cref="IBusinessObjectReferenceProperty"/> to be tested.</param>
    /// <returns><see langword="true" /> if the search service is compatible with this <paramref name="property"/>.</returns>
    bool SupportsProperty (IBusinessObjectReferenceProperty property);

    /// <summary>
    /// Retrieves the list of <see cref="IBusinessObject"/> intances for the specified <paramref name="referencingObject"/>, 
    /// <paramref name="property"/>, and <paramref name="searchArguments"/>. 
    /// </summary>
    /// <param name="referencingObject">
    /// The object containing the <paramref name="property"/> for which the list of possible values is to be retrieved.
    /// </param>
    /// <param name="property">
    /// The <see cref="IBusinessObjectReferenceProperty"/> that will be assigned with one of the objects from the result. Must not be <see langword="null" />.
    /// </param>
    /// <param name="searchArguments">A parameter-object containing additional information for executing the search. Can be <see langword="null"/>.</param>
    /// <returns>A list of <see cref="IBusinessObject"/> instances. The result may be empty.</returns>
    IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments);
  }
}
