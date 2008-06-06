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
