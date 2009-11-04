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
using System.Collections.Generic;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Loads root assemblies by assembly names.
  /// </summary>
  public class NamedRootAssemblyFinder : IRootAssemblyFinder
  {
    private readonly IEnumerable<AssemblyNameSpecification> _specifications;

    public NamedRootAssemblyFinder (IEnumerable<AssemblyNameSpecification> specifications)
    {
      ArgumentUtility.CheckNotNull ("specifications", specifications);
      _specifications = specifications;
    }

    public IEnumerable<AssemblyNameSpecification> Specifications
    {
      get { return _specifications; }
    }

    public RootAssembly[] FindRootAssemblies (IAssemblyLoader loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      var rootAssemblies = from specification in _specifications
                           let assembly = loader.TryLoadAssembly (specification.AssemblyName, specification.ToString())
                           where assembly != null
                           select new RootAssembly(assembly, specification.FollowReferences);
      return rootAssemblies.Distinct().ToArray ();
    }
  }
}
