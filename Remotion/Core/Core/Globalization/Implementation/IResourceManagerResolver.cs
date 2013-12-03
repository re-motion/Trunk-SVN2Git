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
using Remotion.ServiceLocation;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Defines the interface to retrieve an <see cref="IResourceManager"/> to make a resource-lookup.
  /// </summary>
  [ConcreteImplementation (typeof (ResourceManagerResolver), Lifetime = LifetimeKind.Singleton)]
  public interface IResourceManagerResolver
  {
    /// <summary>
    /// Returns the <see cref="IResourceManager"/> for a specified <see cref="Type"/>.
    /// </summary>
    /// <param name="objectType">The <see cref="Type"/> for that a <see cref="IResourceManager"/> should be returned.</param>
    /// <returns>
    /// Returns the <see cref="IResourceManager"/> for the specified type. If no resource manager could be found,
    /// a <see cref="NullResourceManager"/> is returned.
    /// </returns>
    IResourceManager GetResourceManager (Type objectType);
  }
}