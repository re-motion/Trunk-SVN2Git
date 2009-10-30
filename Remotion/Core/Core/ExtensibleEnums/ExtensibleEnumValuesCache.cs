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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Caches <see cref="ExtensibleEnumValues{T}"/> instances for non-generic access.
  /// </summary>
  public class ExtensibleEnumValuesCache
  {
    public static readonly ExtensibleEnumValuesCache Instance = new ExtensibleEnumValuesCache();

    private readonly InterlockedCache<Type, IExtensibleEnumValues> _cache = new InterlockedCache<Type, IExtensibleEnumValues>();

    private ExtensibleEnumValuesCache ()
    {
    }

    public IExtensibleEnumValues GetValues (Type extensibleEnumType)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);

      return _cache.GetOrCreateValue (extensibleEnumType, CreateInstance);
    }

    private IExtensibleEnumValues CreateInstance (Type extensibleEnumType)
    {
      Type valuesType;
      try
      {
        valuesType = typeof (ExtensibleEnumValues<>).MakeGenericType (extensibleEnumType);
      }
      catch (ArgumentException ex) // constraint violation
      {
        var message = string.Format ("Type '{0}' is not an extensible enum type directly derived from ExtensibleEnum<T>.", extensibleEnumType);
        throw new ArgumentException (message, "extensibleEnumType", ex);
      }
      return (IExtensibleEnumValues) Activator.CreateInstance (valuesType);
    }
  }
}