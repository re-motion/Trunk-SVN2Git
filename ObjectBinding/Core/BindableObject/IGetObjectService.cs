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
  /// <see cref="IGetObjectService"/> defines the interface required for retrieving an <see cref="IBusinessObjectWithIdentity"/> by its unique identifier.
  /// </summary>
  /// <remarks>
  /// You can register a service-instance with the <see cref="BusinessObjectProvider"/>'s <see cref="IBusinessObjectProvider.AddService"/> method 
  /// using either the <see cref="IGetObjectService"/> type or a derived type as the key to identify this service. If you register a service using 
  /// a derived type, you also have to apply the <see cref="GetObjectServiceTypeAttribute"/> to the bindable object for which the service is intended.
  /// </remarks>
  public interface IGetObjectService : IBusinessObjectService
  {
    /// <summary>
    /// Retrieves the <see cref="IBusinessObjectWithIdentity"/> identified by the <paramref name="uniqueIdentifier"/>.
    /// </summary>
    /// <param name="classWithIdentity">
    /// The <see cref="BindableObjectClassWithIdentity"/> containing the metadata for the object's type. Must not be <see langword="null" />.
    /// </param>
    /// <param name="uniqueIdentifier">The unique identifier of the object. Must not be <see langword="null" /> or empty.</param>
    /// <returns>The object specified by <paramref name="uniqueIdentifier"/>.</returns>
    /// <remarks>The behavior for missing objects is not defined and depends on the specific implementation of the <see cref="IGetObjectService"/>.</remarks>
    IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier);
  }
}
