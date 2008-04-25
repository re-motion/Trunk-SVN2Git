using System;
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal static class MixinInstanceInitializer
  {
    public static void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget mixinTargetInstance,
        MixinReflector.InitializationMode mode)
    {
      Type baseCallProxyType = MixinReflector.GetBaseCallProxyType (mixinTargetInstance);
      object baseCallProxyInstance = BaseCallProxyInitializer.InstantiateBaseCallProxy (baseCallProxyType, mixinTargetInstance, mixinDefinition.MixinIndex + 1);
      InvokeMixinInitializationMethod (mixinInstance, mixinTargetInstance, baseCallProxyInstance, mode);
    }

    internal static void InvokeMixinInitializationMethod (object mixinInstance, object mixinTargetInstance, object baseCallProxyInstance, 
        MixinReflector.InitializationMode mode)
    {
      MethodInfo initializationMethod = MixinReflector.GetInitializationMethod (mixinInstance.GetType (), mode);
      if (initializationMethod != null)
      {
        Assertion.IsFalse (initializationMethod.ContainsGenericParameters);

        ParameterInfo[] methodArguments = initializationMethod.GetParameters ();
        object[] argumentValues = new object[methodArguments.Length];
        for (int i = 0; i < argumentValues.Length; ++i)
          argumentValues[i] = GetMixinInitializationArgument (methodArguments[i], mixinTargetInstance, baseCallProxyInstance);

        initializationMethod.Invoke (mixinInstance, argumentValues);
      }
    }

    private static object GetMixinInitializationArgument (ParameterInfo parameter, object mixinTargetInstance, object baseCallProxyInstance)
    {
      if (parameter.IsDefined (typeof (ThisAttribute), false))
        return mixinTargetInstance;
      else if (parameter.IsDefined (typeof (BaseAttribute), false))
        return baseCallProxyInstance;
      else
        throw new NotSupportedException ("Initialization methods can only contain this or base arguments.");
    }
  }
}
