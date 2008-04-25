using System;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class BaseCallProxyInitializer
  {
    public static void InitializeFirstProxy (IMixinTarget mixinTarget)
    {
      Type type = mixinTarget.GetType ();
      Type baseCallProxyType = FindBaseCallProxyType (type);
      Assertion.IsNotNull (baseCallProxyType);

      object firstBaseCallProxy = InstantiateBaseCallProxy (baseCallProxyType, mixinTarget, 0);
      type.GetField ("__first").SetValue (mixinTarget, firstBaseCallProxy);
    }

    public static object InstantiateBaseCallProxy (Type baseCallProxyType, IMixinTarget targetInstance, int depth)
    {
      Assertion.IsNotNull (baseCallProxyType);
      return Activator.CreateInstance (baseCallProxyType, new object[] { targetInstance, depth });
    }

    private static Type FindBaseCallProxyType (Type type)
    {
      Assertion.IsNotNull (type);
      Type baseCallProxyType;
      do
      {
        baseCallProxyType = type.GetNestedType ("BaseCallProxy");
        type = type.BaseType;
      } while (baseCallProxyType == null && type != null);
      return baseCallProxyType;
    }
  }
}