/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
