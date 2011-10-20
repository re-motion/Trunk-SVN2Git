﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Allows an <see cref="IPersistenceStrategy"/> to get <see cref="ILoadedObject"/> instances for objects whose data is already known by the target 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  public interface ILoadedObjectProvider
  {
    /// <summary>
    /// Gets an <see cref="ILoadedObject"/> identified by <paramref name="objectID"/>, returning <see langword="null" /> if the 
    /// <paramref name="objectID"/> does not identify a known loaded object.
    /// </summary>
    /// <param name="objectID">The object ID. Must not be <see langword="null" />.</param>
    /// <returns>An <see cref="ILoadedObject"/> for <paramref name="objectID"/>, or <see langword="null" /> if no such object is known.</returns>
    ILoadedObject GetLoadedObject (ObjectID objectID);
  }
}