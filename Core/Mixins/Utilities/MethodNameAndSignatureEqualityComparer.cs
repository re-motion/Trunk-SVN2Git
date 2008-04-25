using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class MethodNameAndSignatureEqualityComparer : IEqualityComparer<MethodInfo>
  {
    private MethodSignatureEqualityComparer _signatureComparer = new MethodSignatureEqualityComparer();

    public bool Equals (MethodInfo x, MethodInfo y)
    {
      if (x == null && y == null)
        return true;
      else if (x == null || y == null)
        return false;
      else
        return x.Name == y.Name && _signatureComparer.Equals (x, y);
    }

    public int GetHashCode (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      return method.Name.GetHashCode() ^ _signatureComparer.GetHashCode (method);
    }
  }
}
