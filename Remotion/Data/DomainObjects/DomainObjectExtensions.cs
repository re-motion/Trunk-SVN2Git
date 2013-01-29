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

using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides useful extension methods for working with <see cref="DomainObject"/> instances.
  /// </summary>
  public static class DomainObjectExtensions
  {
    /// <summary>
    /// Returns the <see cref="DomainObject.ID"/> of the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if 
    /// <paramref name="domainObjectOrNull"/> is itself <see langword="null" />.
    /// </summary>
    /// <param name="domainObjectOrNull">The <see cref="DomainObject"/> whose <see cref="DomainObject.ID"/> to get. If this parameter is 
    /// <see langword="null" />, the method returns <see langword="null" />.</param>
    /// <returns>The <paramref name="domainObjectOrNull"/>'s <see cref="DomainObject.ID"/>, or <see langword="null" /> if <paramref name="domainObjectOrNull"/>
    /// is <see langword="null" />.</returns>
    public static ObjectID GetSafeID (this DomainObject domainObjectOrNull)
    {
      return domainObjectOrNull != null ? domainObjectOrNull.ID : null;
    }

    /// <summary>
    /// Returns a typed handle to the given <paramref name="domainObject"/>. The generic type parameter <typeparamref name="T"/> is inferred from the 
    /// static type of the value passed as <paramref name="domainObject"/>.
    /// </summary>
    /// <typeparam name="T">The type to be used for the returned <see cref="IDomainObjectHandle{T}"/>.</typeparam>
    /// <param name="domainObject">The <see cref="DomainObject"/> to get a handle for.</param>
    /// <returns>A typed handle to the given <paramref name="domainObject"/>.</returns>
    public static IDomainObjectHandle<T> GetHandle<T> (this T domainObject) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      return domainObject.ID.GetHandle<T>();
    }
  }
}