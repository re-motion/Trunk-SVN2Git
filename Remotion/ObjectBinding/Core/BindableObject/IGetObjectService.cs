// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
