// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins
{
  /// <summary>
  /// This interface is implicitly implemented by all mixed types and objects returned by <see cref="TypeFactory"/> and <see cref="ObjectFactory"/>.
  /// </summary>
  public interface IMixinTarget
  {
    /// <summary>
    /// Gets the mixin target's configuration data.
    /// </summary>
    /// <value>A <see cref="TargetClassDefinition"/> object holding the configuration data used to create the mixin target.</value>
    TargetClassDefinition Configuration { get; }

    /// <summary>
    /// Gets the mixins associated with the mixed object.
    /// </summary>
    /// <value>The mixin instances associated with the mixed object.</value>
    object[] Mixins { get; }

    /// <summary>
    /// Gets the first base call proxy.
    /// </summary>
    /// <value>An object the mixin type uses to call overridden methods. This is an instance of a generated type with no defined public API, so
    /// it is only useful for internal purposes.</value>
    object FirstBaseCallProxy { get; }
  }
}
