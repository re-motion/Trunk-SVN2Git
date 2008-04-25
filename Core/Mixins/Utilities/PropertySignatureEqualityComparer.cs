using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class PropertySignatureEqualityComparer : IEqualityComparer<PropertyInfo>
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public bool Equals (PropertyInfo x, PropertyInfo y)
    {
      if (x == null && y == null)
        return true;
      else if (x == null || y == null)
        return false;
      else
        return s_signatureChecker.PropertySignaturesMatch (x, y);
    }

    public int GetHashCode (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("obj", property);

      ParameterInfo[] parameters = property.GetIndexParameters();
      Type[] signatureItems = new Type[parameters.Length + 1];
      signatureItems[0] = MethodSignatureEqualityComparer.GetSafeType (property.PropertyType);
      for (int i = 0; i < parameters.Length; ++i)
        signatureItems[i + 1] = MethodSignatureEqualityComparer.GetSafeType (parameters[i].ParameterType);

      return EqualityUtility.GetRotatedHashCode (signatureItems);
    }
  }

}
