// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Text;

namespace Remotion.Mixins.Utilities
{
  public class MixedTypeConstructorLookupInfo : ConstructorLookupInfo
  {
    private const BindingFlags c_allowNonPublicBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly Type _targetType;
    private readonly bool _allowNonPublic;

    public MixedTypeConstructorLookupInfo (Type concreteType, Type targetType, bool allowNonPublic)
      : base (concreteType, c_allowNonPublicBindingFlags)
    {
      _targetType = targetType;
      _allowNonPublic = allowNonPublic;
    }

    protected override ConstructorInfo GetConstructor (Type[] parameterTypes)
    {
      ConstructorInfo targetTypeCtor = _targetType.GetConstructor (BindingFlags, null, CallingConventions.Any, parameterTypes, null);

      if (targetTypeCtor == null)
      {
        string message = string.Format ("Type {0} does not contain a constructor with the following signature: ({1}).",
            _targetType.FullName, SeparatedStringBuilder.Build (",", parameterTypes, t => t.FullName));
        throw new MissingMethodException (message);
      }
      else if (!targetTypeCtor.IsPublic && !_allowNonPublic)
      {
        string message = string.Format (
            "Type {0} contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is not set).",
            _targetType.FullName);
        throw new MissingMethodException (message);
      }
      else
        return base.GetConstructor (parameterTypes);
    }

    protected override object GetCacheKey (Type delegateType)
    {
      return Tuple.Create (_allowNonPublic, _targetType, base.GetCacheKey (delegateType));
    }
  }
}
