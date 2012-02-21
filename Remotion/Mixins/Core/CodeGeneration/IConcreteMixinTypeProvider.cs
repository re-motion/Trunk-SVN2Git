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

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Provides a concrete mixin type for a given <see cref="ConcreteMixinTypeIdentifier"/>.
  /// </summary>
  public interface IConcreteMixinTypeProvider
  {
    /// <summary>
    /// Gets a concrete mixin type for the given mixin configuration.
    /// </summary>
    /// <param name="concreteMixinTypeIdentifier">The <see cref="ConcreteMixinTypeIdentifier"/> defining the mixin type to get.</param>
    /// <returns>A concrete mixin type for the given <paramref name="concreteMixinTypeIdentifier"/>.</returns>
    /// <remarks>This is mostly for internal reasons, users will hardly ever need to use this method.</remarks>
    ConcreteMixinType GetConcreteMixinType (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier);
  }
}