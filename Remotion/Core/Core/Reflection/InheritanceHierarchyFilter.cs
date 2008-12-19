// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Reflection
{
  /// <summary>
  /// The <see cref="InheritanceHierarchyFilter"/> can be used to get all leaf classes within a deifned set of types passed into the 
  /// constructor.
  /// </summary>
  public class InheritanceHierarchyFilter
  {
    private readonly Type[] _types;

    public InheritanceHierarchyFilter (Type[] types)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("types", types);
      _types = types;
    }

    public Type[] GetLeafTypes ()
    {
      Set<Type> baseTypes = new Set<Type>();
      foreach (Type type in _types)
      {
        baseTypes.Add (type.BaseType);
        if (type.BaseType.IsGenericType)
          baseTypes.Add (type.BaseType.GetGenericTypeDefinition());
      }

      return Array.FindAll (_types, delegate (Type type) { return !baseTypes.Contains (type); });
    }
  }
}
