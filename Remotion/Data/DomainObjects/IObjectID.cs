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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides a covariant, typed interface for instances of <see cref="ObjectID"/>.
  /// </summary>
  /// <typeparam name="T">The class of the object identified by this <see cref="IObjectID{T}"/>.</typeparam>
  public interface IObjectID<out T> : IComparable
      where T : DomainObject
  {
    /// <summary>
    /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> of the <see cref="Persistence.StorageProvider"/> which stores the object.
    /// </summary>
    StorageProviderDefinition StorageProviderDefinition { get; }

    /// <summary>
    /// Gets the ID value used to identify the object in the storage provider.
    /// </summary>
    /// <remarks>
    /// <b>Value</b> can be of type <see cref="System.Guid"/>, <see cref="System.Int32"/> or <see cref="System.String"/>.
    /// </remarks>
    object Value { get; }

    /// <summary>
    /// The class ID of the object class.
    /// </summary>
    string ClassID { get; }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> associated with this <see cref="IObjectID{T}"/>.
    /// </summary>
    ClassDefinition ClassDefinition { get; }

    /// <summary>
    /// Returns this <see cref="IObjectID{T}"/> as an untyped <see cref="ObjectID"/>.
    /// </summary>
    /// <returns>An <see cref="ObjectID"/> instance identifying the same <see cref="DomainObject"/> as this <see cref="IObjectID{T}"/>.</returns>
    ObjectID AsObjectID ();

    /// <summary>
    /// Returns the string representation of the current <see cref="IObjectID{T}"/>.
    /// </summary>
    /// <returns>A <see cref="String"/> that represents the current <see cref="IObjectID{T}"/>. The string can be parsed via 
    /// <see cref="ObjectID.Parse"/>.</returns>
    string ToString ();

    /// <summary>
    /// Determines whether the specified <see cref="IObjectID{T}"/> is equal to the current <see cref="IObjectID{T}"/>.
    /// </summary>
    /// <param name="obj">The <see cref="ObjectID"/> to compare with the current <see cref="ObjectID"/>. </param>
    /// <returns><see langword="true"/> if the specified <see cref="ObjectID"/> is equal to the current <see cref="ObjectID"/>; otherwise, 
    /// <see langword="false"/>.</returns>
    bool Equals (object obj);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    int GetHashCode ();
  }
}