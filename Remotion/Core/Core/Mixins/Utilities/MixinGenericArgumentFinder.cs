// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class MixinGenericArgumentFinder
  {
    public static readonly MixinGenericArgumentFinder ThisArgumentFinder = new MixinGenericArgumentFinder (0);
    public static readonly MixinGenericArgumentFinder BaseArgumentFinder = new MixinGenericArgumentFinder (1);

    private readonly int _genericArgumentIndex;

    private MixinGenericArgumentFinder (int genericArgumentIndex)
    {
      _genericArgumentIndex = genericArgumentIndex;
    }

    public Type FindGenericArgument (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      var mixinBase = MixinReflector.GetMixinBaseType (mixinType);
      if (mixinBase == null)
        return null;

      Assertion.IsTrue (mixinBase.IsGenericType);
      
      var genericArguments = mixinBase.GetGenericArguments ();
      return genericArguments.Length > _genericArgumentIndex ? genericArguments[_genericArgumentIndex] : null;
    }
  }
}