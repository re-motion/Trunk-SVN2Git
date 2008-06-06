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
using System.Reflection;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class MixinReflector
  {
    public enum InitializationMode { Construction, Deserialization }

    public static TMixin Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      return (TMixin) Get (typeof (TMixin), mixinTarget);
    }

    public static object Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      IMixinTarget castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        MixinDefinition mixinDefinition = castMixinTarget.Configuration.GetMixinByConfiguredType (mixinType);
        if (mixinDefinition != null)
          return castMixinTarget.Mixins[mixinDefinition.MixinIndex];
      }
      return null;
    }

    public static Type GetMixinBaseType (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type currentType = concreteMixinType;
      Type mixinBaseOne = typeof (Mixin<>);
      Type mixinBaseTwo = typeof (Mixin<,>);

      while (currentType != null && !ReflectionUtility.IsEqualOrInstantiationOf (currentType, mixinBaseOne)
          && !ReflectionUtility.IsEqualOrInstantiationOf (currentType, mixinBaseTwo))
        currentType = currentType.BaseType;
      return currentType;
    }

    public static PropertyInfo GetTargetProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static PropertyInfo GetBaseProperty (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      Type mixinBaseType = GetMixinBaseType (concreteMixinType);
      if (mixinBaseType == null)
        return null;
      else
        return mixinBaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static Type GetBaseCallProxyType (object mixinTargetInstance)
    {
      ArgumentUtility.CheckNotNull ("mixinTargetInstance", mixinTargetInstance);
      IMixinTarget castTarget = mixinTargetInstance as IMixinTarget;
      if (castTarget == null)
      {
        string message = string.Format ("The given object of type {0} is not a mixin target.", mixinTargetInstance.GetType().FullName);
        throw new ArgumentException (message, "mixinTargetInstance");
      }

      Assertion.IsNotNull (castTarget.FirstBaseCallProxy);
      Type baseCallProxyType = castTarget.FirstBaseCallProxy.GetType();
      return baseCallProxyType;
    }

    /// <summary>
    /// Returns the <see cref="ClassContext"/> that was used as the mixin configuration when the given concrete mixed <paramref name="type"/>
    /// was created by the <see cref="TypeFactory"/>.
    /// </summary>
    /// <param name="type">The type whose mixin configuration is to be retrieved.</param>
    /// <returns>The <see cref="ClassContext"/> used when the given concrete mixed <paramref name="type"/> was created, or <see langword="null"/>
    /// if <paramref name="type"/> is no mixed type.</returns>
    public static ClassContext GetClassContextFromConcreteType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ConcreteMixedTypeAttribute attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (type, true);
      if (attribute == null)
        return null;
      else
        return attribute.GetClassContext ();
    }

    /// <summary>
    /// Returns the <see cref="MixinDefinition"/> that was used as the mixin configuration for a specific mixin when its target object was created.
    /// </summary>
    /// <param name="mixin">The mixin whose configuration should be returned.</param>
    /// <param name="mixedInstance">The instance containing <paramref name="mixin"/>.</param>
    /// <returns>The <see cref="MixinDefinition"/> object corresponding to the <paramref name="mixin"/>.</returns>
    public static MixinDefinition GetMixinConfiguration (object mixin, object mixedInstance)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("mixedInstance", mixedInstance);

      IMixinTarget mixinTarget = mixedInstance as IMixinTarget;
      if (mixinTarget == null)
        throw new ArgumentException ("The given instance is not a mixed object.", "mixedInstance");

      int index = Array.IndexOf (mixinTarget.Mixins, mixin);
      if (index == -1)
        throw new ArgumentException ("The given mixin is not a part of the given instance.", "mixin");

      return mixinTarget.Configuration.Mixins[index];
    }
  }
}
