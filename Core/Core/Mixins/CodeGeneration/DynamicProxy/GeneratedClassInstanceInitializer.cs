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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class GeneratedClassInstanceInitializer
  {
    public static void InitializeMixinTarget (IInitializableMixinTarget mixinTarget, bool deserialization)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      object[] mixinInstances = MixedObjectInstantiationScope.Current.SuppliedMixinInstances;
      TargetClassDefinition configuration = mixinTarget.Configuration;

      InitializeFirstProxy (mixinTarget);

      object[] extensions = PrepareExtensionsWithGivenMixinInstances (configuration, mixinInstances);
      FillUpExtensionsWithNewMixinInstances (extensions, configuration);

      mixinTarget.SetExtensions (extensions);
      InitializeMixinInstances (extensions, mixinTarget, deserialization);
    }

    private static void InitializeFirstProxy (IInitializableMixinTarget mixinTarget)
    {
      object baseCallProxy = mixinTarget.CreateBaseCallProxy (0);
      mixinTarget.SetFirstBaseCallProxy (baseCallProxy);
    }

    private static object[] PrepareExtensionsWithGivenMixinInstances (TargetClassDefinition configuration, object[] mixinInstances)
    {
      object[] extensions = new object[configuration.Mixins.Count];

      if (mixinInstances != null)
      {
        foreach (object mixinInstance in mixinInstances)
        {
          ArgumentUtility.CheckNotNull ("mixinInstances", mixinInstance);
          MixinDefinition mixinDefinition = GetMixinDefinitionFromMixinInstance(mixinInstance, configuration);
          if (mixinDefinition == null)
          {
            string message = string.Format ("The supplied mixin of type {0} is not valid in the current configuration.", mixinInstance.GetType());
            throw new ArgumentException (message, "mixinInstances");
          }
          else if (mixinDefinition.NeedsDerivedMixinType () && !ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType.IsInstanceOfType (mixinInstance))
          {
            string message = string.Format ("The mixin {0} applied to base type {1} needs to have a subclass generated at runtime. It is therefore "
                + "not possible to use the given object of type {2} as a mixin instance.", mixinDefinition.FullName, configuration.FullName,
                mixinInstance.GetType().Name);
            throw new ArgumentException (message, "mixinInstances");
          }
          else
            extensions[mixinDefinition.MixinIndex] = mixinInstance;
        }
      }
      return extensions;
    }

    private static MixinDefinition GetMixinDefinitionFromMixinInstance (object mixinInstance, TargetClassDefinition targetClassDefinition)
    {
      Type mixinType = mixinInstance.GetType();
      while (MixinTypeUtility.IsGeneratedByMixinEngine (mixinType))
        mixinType = mixinType.BaseType;

      return targetClassDefinition.Mixins[mixinType];
    }

    private static void FillUpExtensionsWithNewMixinInstances (object[] extensions, TargetClassDefinition configuration)
    {
      foreach (MixinDefinition mixinDefinition in configuration.Mixins)
      {
        if (extensions[mixinDefinition.MixinIndex] == null)
          extensions[mixinDefinition.MixinIndex] = InstantiateMixin (mixinDefinition);
      }
    }

    private static object InstantiateMixin (MixinDefinition mixinDefinition)
    {
      Type mixinType = mixinDefinition.Type;
      Assertion.IsFalse (mixinType.ContainsGenericParameters);

      if (mixinDefinition.NeedsDerivedMixinType ())
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;

      object mixinInstance;
      if (mixinType.IsValueType)
        mixinInstance = Activator.CreateInstance (mixinType); // there's always a public constructor for value types
      else
      {
        try
        {
          mixinInstance = ObjectFactory.Create (mixinType).With();
        }
        catch (MissingMethodException ex)
        {
          string message = string.Format (
              "Cannot instantiate mixin {0}, there is no visible default constructor.",
              mixinDefinition.Type);
          throw new MissingMethodException (message, ex);
        }
      }

      return mixinInstance;
    }

    private static void InitializeMixinInstances (object[] mixins, IInitializableMixinTarget mixinTargetInstance, bool deserialization)
    {
      for (int i = 0; i < mixins.Length; ++i)
      {
        var initializableMixin = mixins[i] as IInitializableMixin;
        if (initializableMixin != null)
        {
          object baseCallProxyInstance = mixinTargetInstance.CreateBaseCallProxy (i + 1);
          initializableMixin.Initialize (mixinTargetInstance, baseCallProxyInstance, deserialization);
        }
      }
    }
  }
}
