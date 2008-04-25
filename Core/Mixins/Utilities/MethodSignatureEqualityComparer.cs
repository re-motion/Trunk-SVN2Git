using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class MethodSignatureEqualityComparer : IEqualityComparer<MethodInfo>
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public bool Equals (MethodInfo x, MethodInfo y)
    {
      if (x == null && y == null)
        return true;
      else if (x == null || y == null)
        return false;
      else
        return s_signatureChecker.MethodSignaturesMatch (x, y);
    }

    public int GetHashCode (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      ParameterInfo[] parameters = method.GetParameters();
      Type[] signatureItems = new Type[parameters.Length + 1];

      signatureItems[0] = GetSafeType (method.ReturnType);
      for (int i = 0; i < parameters.Length; ++i)
      {
        signatureItems[i + 1] = GetSafeType(parameters[i].ParameterType);
      }

      return EqualityUtility.GetRotatedHashCode (signatureItems);
    }

    // Ensures that no generic parameters are used in hash code calculations.
    // One can't reliably match hash codes of unbound generic parameters, therefore this method returns a constant (typeof(object)) for those
    internal static Type GetSafeType (Type t)
    {
      if (t.IsGenericParameter || t.ContainsGenericParameters)
        return typeof (object);
      else
        return t;
    }
  }
}
