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
using System.ComponentModel;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides a covariant, typed interface for handles to <see cref="DomainObject"/> instances.
  /// </summary>
  /// <typeparam name="T">The class of the <see cref="DomainObject"/> identified by this <see cref="IDomainObjectHandle{T}"/>.</typeparam>
  /// <remarks>
  /// Use this interface when you need a typed representation of a certain <see cref="DomainObject"/> instance that is not bound to a specific
  /// <see cref="ClientTransaction"/>. Get a handle for a <see cref="DomainObject"/> or an <see cref="DomainObjects.ObjectID"/> by calling 
  /// <see cref="DomainObjectExtensions.GetHandle{T}"/> or <see cref="DomainObjects.ObjectID.GetHandle{T}"/>.
  /// </remarks>
  [TypeConverter (typeof (DomainObjectHandleConverter))]
  [DomainObjectHandle]
  public interface IDomainObjectHandle<out T> : IEquatable<IDomainObjectHandle<DomainObject>>
      where T : DomainObject
  {
    /// <summary>
    /// Returns the <see cref="DomainObjects.ObjectID"/> of the object represented by this <see cref="IDomainObjectHandle{T}"/>.
    /// </summary>
    ObjectID ObjectID { get; }

    /// <summary>
    /// Returns this <see cref="IDomainObjectHandle{T}"/> as an <see cref="IDomainObjectHandle{T}"/> of another type.
    /// </summary>
    /// <returns>The same <see cref="IDomainObjectHandle{T}"/> as this object, but typed to another type <typeparamref name="TOther"/>>.</returns>
    /// <exception cref="InvalidCastException">This <see cref="IDomainObjectHandle{T}"/> is not compatible with <typeparamref name="TOther"/>.</exception>
    IDomainObjectHandle<TOther> Cast<TOther> () where TOther : DomainObject;
  }
}