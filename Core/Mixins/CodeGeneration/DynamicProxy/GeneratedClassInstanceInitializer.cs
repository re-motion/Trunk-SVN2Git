using System;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class GeneratedClassInstanceInitializer
  {
    public static void InitializeMixinTarget (IInitializableMixinTarget mixinTarget, MixinReflector.InitializationMode initializationMode)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);

      object[] mixinInstances = MixedObjectInstantiationScope.Current.SuppliedMixinInstances;
      TargetClassDefinition configuration = mixinTarget.Configuration;

      InitializeFirstProxy (mixinTarget);
        
      object[] extensions = PrepareExtensionsWithGivenMixinInstances (configuration, mixinInstances);
      FillUpExtensionsWithNewMixinInstances (extensions, configuration);

      SetExtensionsField (mixinTarget, extensions);
      InitializeMixinInstances (extensions, configuration, mixinTarget, initializationMode);
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
          else if (TypeGenerator.NeedsDerivedMixinType (mixinDefinition) && !ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).IsInstanceOfType (mixinInstance))
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
      ConcreteMixinTypeAttribute[] mixinTypeAttributes =
          (ConcreteMixinTypeAttribute[]) mixinType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
          
      if (mixinTypeAttributes.Length > 0)
      {
        Assertion.IsTrue (mixinTypeAttributes.Length == 1, "AllowMultiple == false");
        return mixinTypeAttributes[0].GetMixinDefinition();
      }
      else
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

      if (TypeGenerator.NeedsDerivedMixinType (mixinDefinition))
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);

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

    private static void SetExtensionsField (IMixinTarget mixinTarget, object[] extensions)
    {
      Type type = mixinTarget.GetType ();
      type.GetField ("__extensions").SetValue (mixinTarget, extensions);
    }

    private static void InitializeMixinInstances (object[] mixins, TargetClassDefinition configuration, IInitializableMixinTarget mixinTargetInstance,
        MixinReflector.InitializationMode mode)
    {
      Assertion.IsTrue (mixins.Length == configuration.Mixins.Count);
      for (int i = 0; i < mixins.Length; ++i)
        MixinInstanceInitializer.InitializeMixinInstance (configuration.Mixins[i], mixins[i], mixinTargetInstance, mode);
    }
  }
}
