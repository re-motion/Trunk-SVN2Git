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

namespace Remotion.Mixins
{
  /// <summary>
  /// Defines how the <see cref="TypeFactory"/> and <see cref="ObjectFactory"/> behave when asked to generate a concrete type for a target
  /// type without any mixin configuration information.
  /// </summary>
  public enum GenerationPolicy
  {
    /// <summary>
    /// Specifies that <see cref="TypeFactory"/> and <see cref="ObjectFactory"/> should always generate concrete types, no matter whether
    /// mixin configuration information exists for the given target type or not.
    /// </summary>
    ForceGeneration,
    /// <summary>
    /// Specifies that <see cref="TypeFactory"/> and <see cref="ObjectFactory"/> should only generate concrete types if
    /// mixin configuration information exists for the given target type.
    /// </summary>
    GenerateOnlyIfConfigured
  }
}
