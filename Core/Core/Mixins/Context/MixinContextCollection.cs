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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class MixinContextCollection : ReadOnlyContextCollection<Type, MixinContext>
  {
    internal static bool ContainsOverrideForMixin (IEnumerable<MixinContext> mixinContexts, Type mixinType)
    {
      return GetOverrideForMixin (mixinContexts, mixinType) != null;
    }

    internal static MixinContext GetOverrideForMixin (IEnumerable<MixinContext> mixinContexts, Type mixinType)
    {
      Type typeToSearchFor = mixinType.IsGenericType ? mixinType.GetGenericTypeDefinition () : mixinType;

      foreach (MixinContext mixin in mixinContexts)
      {
        if (ReflectionUtility.CanAscribe (mixin.MixinType, typeToSearchFor))
          return mixin;
      }
      return null;
    }

    public MixinContextCollection (IEnumerable<MixinContext> values)
      : base (delegate (MixinContext context) { return context.MixinType; }, values)
    {
    }

    /// <summary>
    /// Determines whether this <see cref="ClassContext"/> contains a mixin type assignable to the specified type.
    /// </summary>
    /// <param name="baseMixinType">The mixin type to check for.</param>
    /// <returns>
    /// True if the <see cref="ClassContext"/> contains a type assignable to the specified type; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="baseMixinType"/> parameter is <see langword="null"/>.</exception>
    public bool ContainsAssignableMixin (Type baseMixinType)
    {
      ArgumentUtility.CheckNotNull ("baseMixinType", baseMixinType);
      foreach (MixinContext mixin in this)
      {
        if (baseMixinType.IsAssignableFrom (mixin.MixinType))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Determines whether a mixin configured with this <see cref="ClassContext"/> overrides the given <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type which is to be checked for overriders.</param>
    /// <returns>
    /// True if the specified mixin type is overridden in this class context; otherwise, false.
    /// </returns>
    public bool ContainsOverrideForMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      return ContainsOverrideForMixin (this, mixinType);
    }
  }
}
