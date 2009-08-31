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
using System.Reflection;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Categorizes <see cref="Type"/>|s into "valid" and "invalid" types, 
  /// based on whether their assembly is a member of the class's assembly collection.
  /// </summary>
  public class AssemblyLevelTypeFilter : ITypeFilter
  {
    private readonly Dictionary<Assembly,bool> _validAssemblies = new Dictionary<Assembly, bool>();

    public AssemblyLevelTypeFilter (IEnumerable<Assembly> validAssemblies)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("validAssemblies", validAssemblies);
      foreach (var assembly in validAssemblies)
      {
        _validAssemblies.Add (assembly, true);
      }
    }

    /// <summary>
    /// Returns true if the passed <see cref="Type"/> is a member of the <see cref="AssemblyLevelTypeFilter"/>|s assemblies.
    /// </summary>
    public bool IsTypeValid (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _validAssemblies.ContainsKey (type.Assembly);
    }
  }
}