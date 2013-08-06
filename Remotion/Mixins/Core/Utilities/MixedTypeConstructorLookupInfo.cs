// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Reflection;
using Remotion.Text;

namespace Remotion.Mixins.Utilities
{
  // TODO 5370: Try to remove this class.
  public class MixedTypeConstructorLookupInfo : ConstructorLookupInfo
  {
    private sealed class CacheKey
    {
      private readonly bool _allowNonPublic;
      private readonly Type _concreteType;
      private readonly Type _delegateType;

      public CacheKey (bool allowNonPublic, Type concreteType, Type delegateType)
      {
        _allowNonPublic = allowNonPublic;
        _concreteType = concreteType;
        _delegateType = delegateType;
      }

      public override bool Equals (object obj)
      {
        if (obj.GetType () != typeof (CacheKey))
          return false;

        var castOther = (CacheKey) obj;
        return _allowNonPublic == castOther._allowNonPublic && _concreteType == castOther._concreteType && _delegateType == castOther._delegateType;
      }

      public override int GetHashCode ()
      {
        return _allowNonPublic.GetHashCode () ^ _concreteType.GetHashCode () ^ _delegateType.GetHashCode ();
      }
    }

    private const BindingFlags c_allowNonPublicBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly Type _targetType;
    private readonly bool _allowNonPublic;

    public MixedTypeConstructorLookupInfo (Type concreteType, Type targetType, bool allowNonPublic)
      : base (concreteType, c_allowNonPublicBindingFlags)
    {
      _targetType = targetType;
      _allowNonPublic = allowNonPublic;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public bool AllowNonPublic
    {
      get { return _allowNonPublic; }
    }

    protected override ConstructorInfo GetConstructor (Type[] parameterTypes)
    {
      ConstructorInfo targetTypeCtor = _targetType.GetConstructor (BindingFlags, null, CallingConventions.Any, parameterTypes, null);

      if (targetTypeCtor == null)
      {
        string message = string.Format ("Type '{0}' does not contain a constructor with the following signature: ({1}).",
            _targetType.FullName, SeparatedStringBuilder.Build (",", parameterTypes, t => t.FullName));
        throw new MissingMethodException (message);
      }
      else if (!targetTypeCtor.IsPublic && !_allowNonPublic)
      {
        string message = string.Format (
            "Type '{0}' contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is not set).",
            _targetType.FullName);
        throw new MissingMethodException (message);
      }
      else
        return base.GetConstructor (parameterTypes);
    }

    protected override object GetCacheKey (Type delegateType)
    {
      return new CacheKey (_allowNonPublic, DefiningType, delegateType);
    }
  }
}
