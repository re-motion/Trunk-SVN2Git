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
    /// Gets a flag that describes whether the serivce returns objects of type <see cref="IBusinessObjectWithIdentity"/> for the current property.
    /// </summary>
    /// <param name="property">The <see cref="IBusinessObjectReferenceProperty"/> to be tested.</param>
    /// <returns><see langword="true" /> if the objects will implement <see cref="IBusinessObjectWithIdentity"/>.</returns>
    bool SupportsIdentity (IBusinessObjectReferenceProperty property);

    /// <summary>
    /// Retrieves the list of <see cref="IBusinessObject"/> intances for the specified <paramref name="referencingObject"/>, 
    /// <paramref name="property"/>, and <paramref name="searchStatement"/>. 
    /// </summary>
    /// <param name="referencingObject">
    /// The object containing the <paramref name="property"/> for which the list of possible values is to be retrieved.
    /// </param>
    /// <param name="property">
    /// The <see cref="IBusinessObjectReferenceProperty"/> that will be assigned with one of the objects from the result. Must not be <see langword="null" />.
    /// </param>
    /// <param name="searchStatement">An optional <see cref="string"/> that can be used to further qualify the search.</param>
    /// <returns>A list of <see cref="IBusinessObject"/> instances. The result may be empty.</returns>
    IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement);
  }
}
