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
using Remotion.Mixins.Context;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Provides a concrete, mixed type for a given <see cref="ClassContext"/>.
  /// </summary>
  public interface IConcreteTypeProvider
  {
    /// <summary>
    /// Gets a concrete mixed type for the given target class configuration.
    /// </summary>
    /// <param name="classContext">The <see cref="ClassContext"/> holding the mixin configuration for the target class.</param>
    /// <returns>A concrete type with all mixins from <paramref name="classContext"/> mixed in.</returns>
    /// <remarks>This is mostly for internal reasons, users should use <see cref="TypeFactory.GetConcreteType(Type)"/> instead.</remarks>
    Type GetConcreteType (ClassContext classContext);
  }
}