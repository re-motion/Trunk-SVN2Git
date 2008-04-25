using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class PropertyNameAndSignatureEqualityComparer : IEqualityComparer<PropertyInfo>
  {
    private PropertySignatureEqualityComparer _signatureComparer = new PropertySignatureEqualityComparer();

    public bool Equals (PropertyInfo x, PropertyInfo y)
    {
      if (x == null && y == null)
        return true;
      else if (x == null || y == null)
        return false;
      else
        return x.Name == y.Name && _signatureComparer.Equals (x, y);
    }

    public int GetHashCode (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return property.Name.GetHashCode() ^ _signatureComparer.GetHashCode (property);
    }
  }
}
