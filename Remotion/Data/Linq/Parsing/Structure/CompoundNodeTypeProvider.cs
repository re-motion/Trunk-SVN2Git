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
using System.Reflection;
using Remotion.Data.Linq.Utilities;
using System.Linq;

namespace Remotion.Data.Linq.Parsing.Structure
{
  /// <summary>
  /// Implements <see cref="INodeTypeProvider"/> by storing a list of inner <see cref="INodeTypeProvider"/> instances.
  /// The <see cref="IsRegistered"/> and <see cref="GetNodeType"/> methods delegate to these inner instances.
  /// </summary>
  public class CompoundNodeTypeProvider : INodeTypeProvider
  {
    public static CompoundNodeTypeProvider CreateDefault ()
    {
      var innerProviders = new INodeTypeProvider[]
                           {
                               MethodInfoBasedNodeTypeRegistry.CreateDefault(),
                               MethodNameBasedNodeTypeRegistry.CreateDefault()
                           };
      return new CompoundNodeTypeProvider (innerProviders);
    }

    private readonly List<INodeTypeProvider> _innerProviders;

    public CompoundNodeTypeProvider (IEnumerable<INodeTypeProvider> innerProviders)
    {
      ArgumentUtility.CheckNotNull ("innerProviders", innerProviders);
      _innerProviders = new List<INodeTypeProvider> (innerProviders);
    }

    public IList<INodeTypeProvider> InnerProviders
    {
      get { return _innerProviders; }
    }

    public bool IsRegistered (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      return _innerProviders.Any (p => p.IsRegistered (method));
    }

    public Type GetNodeType (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      return InnerProviders.Select (p => p.GetNodeType (method)).FirstOrDefault (t => t != null);
    }
  }
}