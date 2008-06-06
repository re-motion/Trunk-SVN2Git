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
